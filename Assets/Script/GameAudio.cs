using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// One persistent audio service. Music and effects use independent mixer buses.
public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }
    private AudioMixer mixer;
    private AudioMixerGroup tracks, sfx;
    private AudioMixerSnapshot normal, paused;
    private AudioSource menuSource, levelSource;
    private AudioSource[] voices;
    private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    private Coroutine fade;
    private AudioSource target;
    private bool ready;
    private const float MusicGain = 0.32f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance == null) new GameObject("GameAudio").AddComponent<GameAudio>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        var config = Resources.Load<GameAudioConfig>("Week12AudioConfig");
        if (config == null || config.mixer == null)
        {
            Debug.LogError("GameAudio: Week12AudioConfig or mixer missing.");
            return;
        }
        mixer = config.mixer;
        tracks = FindGroup("Tracks");
        sfx = FindGroup("SFX");
        normal = mixer.FindSnapshot("Normal");
        paused = mixer.FindSnapshot("Paused");
        mixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
        menuSource = MakeSource("Menu music", tracks, true);
        levelSource = MakeSource("Level music", tracks, true);
        menuSource.clip = Load("MenuLoop");
        levelSource.clip = Load("LevelLoop");
        voices = new AudioSource[16];
        for (int i = 0; i < voices.Length; i++) voices[i] = MakeSource("SFX " + i, sfx, false);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (mixer == null) return;
        ready = true;
        ApplyVolumes();
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }

    private AudioMixerGroup FindGroup(string name)
    {
        foreach (var group in mixer.FindMatchingGroups(name)) if (group.name == name) return group;
        Debug.LogError("Missing mixer group: " + name);
        return null;
    }

    private AudioSource MakeSource(string name, AudioMixerGroup group, bool loop)
    {
        var child = new GameObject(name);
        child.transform.SetParent(transform, false);
        var source = child.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = group;
        source.playOnAwake = false;
        source.loop = loop;
        source.spatialBlend = 0f;
        source.volume = loop ? 0f : 1f;
        return source;
    }

    private AudioClip Load(string key)
    {
        if (!clips.TryGetValue(key, out var clip))
        {
            clip = Resources.Load<AudioClip>("Audio/Week12/" + key);
            clips[key] = clip;
            if (clip == null) Debug.LogWarning("Missing audio: " + key);
        }
        return clip;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!ready) return;
        SetPaused(false);
        // Route legacy scene sources, including inactive ones; music is now owned here.
        foreach (var root in scene.GetRootGameObjects())
            foreach (var source in root.GetComponentsInChildren<AudioSource>(true))
                if (source.outputAudioMixerGroup == null)
                    source.outputAudioMixerGroup = source.loop ? tracks : sfx;
        bool isLevel = scene.name == "Level1" || scene.name == "Level2" || scene.name == "Level3";
        PlayMusic(isLevel ? levelSource : menuSource);
    }

    private void PlayMusic(AudioSource next)
    {
        if (next == null || next.clip == null || (target == next && next.isPlaying)) return;
        if (fade != null) StopCoroutine(fade);
        target = next;
        if (!next.isPlaying) { next.volume = 0f; next.Play(); }
        fade = StartCoroutine(Crossfade(next));
    }

    private IEnumerator Crossfade(AudioSource next)
    {
        AudioSource previous = next == menuSource ? levelSource : menuSource;
        float from = previous.volume, to = next.volume;
        for (float elapsed = 0f; elapsed < 1f; elapsed += Time.unscaledDeltaTime)
        {
            previous.volume = Mathf.Lerp(from, 0f, elapsed);
            next.volume = Mathf.Lerp(to, MusicGain, elapsed);
            yield return null;
        }
        previous.Stop(); previous.volume = 0f;
        next.volume = MusicGain;
        fade = null;
    }

    public static void RefreshVolumes()
    {
        if (Instance != null) Instance.ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (!ready) return; // AudioMixer.SetFloat must run in Start or later.
        var data = SaveManager.Instance != null ? SaveManager.Instance.Data : null;
        float music = data != null ? data.musicVolume : 0.8f;
        float effects = data != null ? data.sfxVolume : 0.8f;
        bool enabled = data == null || data.musicEnabled;
        mixer.SetFloat("MusicVol", ToDb(enabled ? music : 0f));
        mixer.SetFloat("SFXVol", ToDb(effects));
    }

    private static float ToDb(float volume) => 20f * Mathf.Log10(Mathf.Max(0.0001f, Mathf.Clamp01(volume)));

    public static void Pause(bool value)
    {
        if (Instance != null) Instance.SetPaused(value);
    }
    private void SetPaused(bool value)
    {
        var snapshot = value ? paused : normal;
        if (snapshot != null) snapshot.TransitionTo(0.3f);
    }

    // Pool persists through scene changes, so win/death/UI sounds are not cut off.
    public static void Play(string key, float volume = 1f)
    {
        if (Instance != null) Instance.PlayEffect(key, volume, false, Vector3.zero);
    }
    public static void PlayFailure()
    {
        if (Instance != null) Instance.StartCoroutine(Instance.FailureSequence());
    }
    private IEnumerator FailureSequence()
    {
        PlayEffect("Death", 1f, false, Vector3.zero);
        yield return new WaitForSecondsRealtime(0.52f);
        PlayEffect("Lose", 0.8f, false, Vector3.zero);
    }
    public static void EnemySound(string key, Vector3 position, float volume = 1f)
    {
        if (Instance != null) Instance.PlayEffect(key, volume, true, position);
    }
    private void PlayEffect(string key, float volume, bool spatial, Vector3 position)
    {
        if (voices == null) return;
        var clip = Load(key);
        if (clip == null) return;
        AudioSource voice = null;
        foreach (var source in voices) if (!source.isPlaying) { voice = source; break; }
        if (voice == null) return; // Do not interrupt a warning already playing.
        voice.transform.position = spatial ? position : transform.position;
        voice.spatialBlend = spatial ? 1f : 0f;
        voice.rolloffMode = AudioRolloffMode.Linear;
        voice.minDistance = 6f;
        voice.maxDistance = 35f;
        voice.pitch = Random.Range(0.92f, 1.08f);
        voice.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
