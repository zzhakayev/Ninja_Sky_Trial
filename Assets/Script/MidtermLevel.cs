using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class MidtermLevel : MonoBehaviour
{
    public GameObject ninjaPrefab;
    public RuntimeAnimatorController ninjaController;

    private int stage;
    private int coinCount;
    private int coinGoal;
    private bool bossDefeated;
    private float messageUntil;
    private string message = "";
    private BossEnemy boss;
    private GameObject portal;
    private Health health;
    private AudioSource music;
    private AudioSource playerAudio;
    private GUIStyle titleStyle;
    private GUIStyle smallStyle;
    private GUIStyle centeredStyle;
    private TextMeshProUGUI canvasStats;
    private static readonly Color cyan = new Color(0.15f, 0.9f, 0.96f);
    private static readonly Color purple = new Color(0.37f, 0.29f, 0.83f);
    private static readonly Color magenta = new Color(0.92f, 0.26f, 0.60f);
    private static readonly Color orange = new Color(1f, 0.65f, 0.27f);
    private static readonly Color dark = new Color(0.10f, 0.16f, 0.27f);

    void Awake()
    {
        string name = SceneManager.GetActiveScene().name;
        stage = name == "Level1" ? 1 : name == "Level2" ? 2 : name == "Level3" ? 3 : 0;
        if (stage == 0) { enabled = false; return; }

        DisableLegacyArena();
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidbody.linearDamping = 3.1f;
            rigidbody.angularDamping = 1f;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }
        Movement movement = GetComponent<Movement>();
        if (movement != null) { movement.force = 15f; movement.jumpForce = 7.2f; }
        health = GetComponent<Health>();
        if (health != null) health.maxHP = 3;
        playerAudio = GetComponent<AudioSource>();
        AddNinja();
        PrepareCamera();
        SetupMusic();
        CreateEnvironment();
        CreateCanvasHUD();
        if (stage == 1) BuildLevelOne();
        if (stage == 2) BuildLevelTwo();
        if (stage == 3) BuildBossLevel();
        transform.position = stage == 3 ? new Vector3(0f, 0.7f, -9f) : new Vector3(0f, 0.7f, -11f);
        Physics.SyncTransforms();
        ShowMessage(stage == 1 ? "Reach the portal. Collect 2 coins!" : stage == 2 ? "Moving platforms ahead. Collect 3 coins!" : "BOSS BATTLE! Use FIRE to defeat the Guardian.", 6f);
    }

    void DisableLegacyArena()
    {
        string[] names = { "Plane", "Wall_Left", "Wall_Right", "Wall_Back", "Wall_Front", "Obstacle", "Ramp", "Enemy", "Gate", "Exit", "MovingPlatform", "GameManager", "Spawner" };
        foreach (string name in names)
        {
            GameObject existing = GameObject.Find(name);
            if (existing != null) existing.SetActive(false);
        }
        foreach (GameObject coin in GameObject.FindGameObjectsWithTag("Coin")) coin.SetActive(false);
    }

    void PrepareCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        Vector3 start = stage == 3 ? new Vector3(0f, 0.7f, -9f) : new Vector3(0f, 0.7f, -11f);
        cam.transform.position = start + new Vector3(0, 8.1f, -11.5f);
        cam.transform.rotation = Quaternion.Euler(34f, 0f, 0f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.05f, 0.09f, 0.19f);
        CameraFollow follow = cam.GetComponent<CameraFollow>();
        if (follow != null) { follow.target = transform; follow.arenaCenter = null; follow.portraitZoomStrength = 0.12f; }
        cam.fieldOfView = 66f;
    }

    void AddNinja()
    {
        Renderer original = GetComponent<Renderer>();
        Transform existing = transform.Find("Ninja Character");
        GameObject model = existing != null ? existing.gameObject : null;
        if (model == null && ninjaPrefab != null)
        {
            model = Instantiate(ninjaPrefab, transform);
            model.name = "Ninja Character";
        }
        if (model == null)
        {
            if (original != null) original.material.color = purple;
            Debug.LogWarning("Ninja model missing. Use Tools > Ninja Midterm > Fix All 3 Levels.");
            return;
        }
        model.transform.localPosition = new Vector3(0f, -0.65f, 0f);
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one * 1.45f;
        foreach (Collider col in model.GetComponentsInChildren<Collider>()) col.enabled = false;
        Animator animator = model.GetComponent<Animator>();
        if (animator == null) animator = model.AddComponent<Animator>();
        if (ninjaController != null) animator.runtimeAnimatorController = ninjaController;
        animator.applyRootMotion = false;
        Movement movement = GetComponent<Movement>();
        if (movement != null) movement.ConnectAnimator(animator, model.transform);
        if (original != null) original.enabled = false;
        Color[] fabric = { new Color(0.13f, 0.17f, 0.31f), new Color(0.22f, 0.38f, 0.56f), new Color(0.33f, 0.24f, 0.59f), cyan };
        foreach (Renderer part in model.GetComponentsInChildren<Renderer>())
        {
            Material[] originals = part.sharedMaterials;
            if (originals.Length == 0) continue;
            Material[] replacements = new Material[originals.Length];
            for (int i = 0; i < originals.Length; i++)
            {
                if (originals[i] != null && originals[i].mainTexture != null) replacements[i] = originals[i];
                else replacements[i] = NewMaterial(fabric[i % fabric.Length]);
            }
            part.materials = replacements;
        }
    }

    void SetupMusic()
    {
        music = gameObject.AddComponent<AudioSource>();
        music.playOnAwake = false;
        music.loop = true;
        music.spatialBlend = 0;
        music.volume = 0.17f * PlayerPrefs.GetFloat("Volume", 1f);
        music.clip = Resources.Load<AudioClip>(stage == 3 ? "Audio/BossMusic" : "Audio/LevelMusic");
        if (music.clip != null && PlayerPrefs.GetInt("Music", 1) != 0) music.Play();
    }

    Material NewMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        Material material = new Material(shader);
        material.color = color;
        return material;
    }

    GameObject Primitive(string name, PrimitiveType shape, Vector3 position, Vector3 scale, Color color, bool collider = true)
    {
        GameObject go = GameObject.CreatePrimitive(shape);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = scale;
        Renderer render = go.GetComponent<Renderer>();
        if (render != null) render.material = NewMaterial(color);
        if (!collider)
        {
            Collider c = go.GetComponent<Collider>();
            if (c != null) Destroy(c);
        }
        return go;
    }

    void Platform(float x, float z, float width, float length, Color color, bool moving = false, float elevation = 0f)
    {
        GameObject platform = Primitive(moving ? "Moving Platform" : "Floating Platform", PrimitiveType.Cube,
            new Vector3(x, -0.55f + elevation, z), new Vector3(width, 1.1f, length), color);
        Primitive("Platform Glow Trim", PrimitiveType.Cube, new Vector3(x, 0.035f + elevation, z),
            new Vector3(width + 0.12f, 0.08f, length + 0.12f), new Color(color.r * 0.65f, color.g * 0.75f, color.b * 0.8f), false)
            .transform.SetParent(platform.transform, true);
        for (int side = -1; side <= 1; side += 2)
        {
            Primitive("Rune Edge", PrimitiveType.Cube,
                new Vector3(x + side * (width * 0.42f), 0.105f + elevation, z),
                new Vector3(0.09f, 0.025f, length * 0.68f), cyan, false)
                .transform.SetParent(platform.transform, true);
        }
        if (moving)
        {
            Rigidbody rb = platform.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            PlatformClipMotion motion = platform.AddComponent<PlatformClipMotion>();
            motion.distance = 1.5f;
            motion.duration = 4f;
        }
    }

    void Medkit(Vector3 position)
    {
        GameObject item = Primitive("Healing Crystal", PrimitiveType.Sphere, position,
            new Vector3(0.55f, 0.55f, 0.55f), new Color(0.22f, 1f, 0.62f));
        item.GetComponent<Collider>().isTrigger = true;
        item.AddComponent<HealingPickup>();
        item.AddComponent<SpinPickup>();
        Primitive("Healing Cross Vertical", PrimitiveType.Cube,
            position + new Vector3(0f, 0.02f, -0.29f), new Vector3(0.10f, 0.32f, 0.07f), Color.white, false)
            .transform.SetParent(item.transform, true);
        Primitive("Healing Cross Horizontal", PrimitiveType.Cube,
            position + new Vector3(0f, 0.02f, -0.30f), new Vector3(0.30f, 0.10f, 0.07f), Color.white, false)
            .transform.SetParent(item.transform, true);
    }

    void Coin(Vector3 position)
    {
        GameObject coin = Primitive("Energy Coin", PrimitiveType.Cylinder, position, new Vector3(0.46f, 0.065f, 0.46f), orange);
        coin.tag = "Coin";
        coin.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        coin.GetComponent<Collider>().isTrigger = true;
        coin.AddComponent<SpinPickup>();
    }

    void Hazard(Vector3 position, float size = 0.8f)
    {
        GameObject hazard = Primitive("Danger Spike", PrimitiveType.Cylinder, position,
            new Vector3(size, 0.18f, size), magenta);
        hazard.GetComponent<Collider>().isTrigger = true;
        hazard.AddComponent<DamageZone>();
        Primitive("Spike Tip", PrimitiveType.Capsule, position + Vector3.up * 0.36f,
            new Vector3(0.32f, 0.47f, 0.32f), orange, false).transform.SetParent(hazard.transform, true);
    }

    void Enemy(Vector3 position, float distance)
    {
        GameObject enemy = Primitive("Patrol Drone", PrimitiveType.Capsule, position,
            new Vector3(0.7f, 0.7f, 0.7f), magenta);
        enemy.GetComponent<Collider>().isTrigger = true;
        Rigidbody rb = enemy.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        PatrolEnemy patrol = enemy.AddComponent<PatrolEnemy>();
        patrol.offset = new Vector3(distance, 0f, 0f);
        Primitive("Drone Eye", PrimitiveType.Sphere, position + new Vector3(0, 0.08f, -0.36f),
            new Vector3(0.22f, 0.22f, 0.22f), cyan, false).transform.SetParent(enemy.transform, true);
    }

    void CreatePortal(float z, bool locked, float elevation = 0f)
    {
        Primitive("Portal Left", PrimitiveType.Cube, new Vector3(-1.15f, 1.2f + elevation, z),
            new Vector3(0.36f, 2.4f, 0.48f), cyan);
        Primitive("Portal Right", PrimitiveType.Cube, new Vector3(1.15f, 1.2f + elevation, z),
            new Vector3(0.36f, 2.4f, 0.48f), cyan);
        Primitive("Portal Crown", PrimitiveType.Cube, new Vector3(0f, 2.48f + elevation, z),
            new Vector3(2.65f, 0.36f, 0.48f), orange);
        portal = Primitive("Exit Portal", PrimitiveType.Cube, new Vector3(0f, 1.28f + elevation, z),
            new Vector3(1.95f, 2.05f, 0.20f), locked ? magenta : cyan);
        portal.tag = "Exit";
        portal.GetComponent<Collider>().isTrigger = true;
        if (locked) portal.SetActive(false);
    }

    void CreateEnvironment()
    {
        RenderSettings.ambientLight = new Color(0.63f, 0.68f, 0.83f);
        Camera camera = Camera.main;
        if (camera != null) camera.backgroundColor = stage == 3 ? new Color(0.12f, 0.03f, 0.15f) : dark;
        for (int i = 0; i < 15; i++)
        {
            float x = (i % 2 == 0 ? -1f : 1f) * (9f + (i % 3) * 4f);
            float z = -17f + i * 3.7f;
            Primitive("Sky Ruin", PrimitiveType.Cube,
                new Vector3(x, -2.8f + (i % 3) * 0.45f, z),
                new Vector3(1.5f + i % 3, 2.4f + i % 4, 1.7f),
                stage == 3 ? new Color(0.25f, 0.12f, 0.38f) : new Color(0.11f, 0.28f, 0.39f), false);
        }
    }

    void BuildLevelOne()
    {
        coinGoal = 2;
        float[] heights = { 0f, 0.25f, 0.55f, 0.35f, 0.65f };
        float[] z = { -11f, -5f, 1f, 7f, 13f };
        for (int i = 0; i < z.Length; i++)
            Platform(0f, z[i], 5.7f, 4.85f, i % 2 == 0 ? new Color(0.22f, 0.37f, 0.68f) : purple, false, heights[i]);
        Coin(new Vector3(0f, 1f, -5f));
        Coin(new Vector3(-1.4f, 1.3f, 1f));
        Coin(new Vector3(1.5f, 1.1f, 7f));
        Coin(new Vector3(0f, 1.4f, 13f));
        Hazard(new Vector3(1.8f, 0.43f, -3.9f));
        Hazard(new Vector3(-1.8f, 0.53f, 6f));
        Enemy(new Vector3(0f, 1.4f, 1f), 1.4f);
        Medkit(new Vector3(1.1f, 1.28f, 7.9f));
        CreatePortal(13.4f, false, 0.65f);
    }

    void BuildLevelTwo()
    {
        coinGoal = 3;
        Platform(0f, -11f, 5.8f, 4.8f, purple);
        Platform(0f, -5f, 4.9f, 4.7f, new Color(0.2f, 0.42f, 0.53f), false, 0.35f);
        Platform(-1f, 1f, 4.4f, 4.4f, cyan, true, 0.55f);
        Platform(0f, 7f, 5.2f, 4.8f, purple, false, 0.8f);
        Platform(0f, 13f, 5.8f, 4.8f, new Color(0.2f, 0.42f, 0.53f), false, 0.45f);
        Coin(new Vector3(0f, 1.08f, -5f));
        Coin(new Vector3(0f, 1.35f, 1f));
        Coin(new Vector3(-1f, 1.55f, 7f));
        Coin(new Vector3(1f, 1.55f, 7f));
        Coin(new Vector3(0f, 1.2f, 12f));
        Hazard(new Vector3(-1.5f, 0.5f, -4f));
        Hazard(new Vector3(1.6f, 0.5f, -5f));
        Hazard(new Vector3(0f, 0.95f, 8f));
        Enemy(new Vector3(0f, 1.2f, -4f), 1.35f);
        Enemy(new Vector3(0f, 1.65f, 7f), 1.9f);
        Medkit(new Vector3(1.5f, 1.55f, 6.5f));
        CreatePortal(13.4f, false, 0.45f);
    }

    void BuildBossLevel()
    {
        coinGoal = 0;
        Platform(0, -9f, 5.6f, 6f, purple);
        Platform(0, 7f, 13.5f, 21f, new Color(0.35f, 0.18f, 0.46f));
        for (int i = -2; i <= 2; i++)
            Primitive("Boss Arena Marker", PrimitiveType.Cube,
                new Vector3(i * 2.3f, 0.08f, 4.5f), new Vector3(1.1f, 0.12f, 0.35f), cyan, false);
        Hazard(new Vector3(-5.3f, 0.2f, 5.5f), 1.1f);
        Hazard(new Vector3(5.3f, 0.2f, 5.5f), 1.1f);
        GameObject guardian = Primitive("Guardian Boss", PrimitiveType.Capsule,
            new Vector3(0f, 1.7f, 8f), Vector3.one * 1.55f, magenta);
        boss = guardian.AddComponent<BossEnemy>();
        boss.maxHP = 8;
        boss.target = transform;
        boss.level = this;
        guardian.AddComponent<AudioSource>().spatialBlend = 0f;
        Primitive("Guardian Crown", PrimitiveType.Sphere, new Vector3(0, 3.25f, 8),
            new Vector3(0.9f, 0.3f, 0.9f), orange, false).transform.SetParent(guardian.transform, true);
        Primitive("Guardian Eyes", PrimitiveType.Cube, new Vector3(0f, 2.0f, 6.82f),
            new Vector3(0.85f, 0.14f, 0.11f), cyan, false).transform.SetParent(guardian.transform, true);
        Primitive("Guardian Left Horn", PrimitiveType.Capsule, new Vector3(-0.85f, 3.3f, 8f),
            new Vector3(0.32f, 1.3f, 0.32f), cyan, false).transform.SetParent(guardian.transform, true);
        Primitive("Guardian Right Horn", PrimitiveType.Capsule, new Vector3(0.85f, 3.3f, 8f),
            new Vector3(0.32f, 1.3f, 0.32f), cyan, false).transform.SetParent(guardian.transform, true);
        CreatePortal(15f, true);
    }

    void CreateCanvasHUD()
    {
        GameObject root = new GameObject("Ninja HUD Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 8;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panel = new GameObject("Stats Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(root.transform, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(12f, -12f);
        rect.sizeDelta = new Vector2(325f, 84f);
        Image background = panel.GetComponent<Image>();
        background.color = new Color(0.03f, 0.06f, 0.16f, 0.82f);
        background.raycastTarget = false;

        GameObject textObject = new GameObject("HP and Coins", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(panel.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(12f, 8f);
        textRect.offsetMax = new Vector2(-6f, -7f);
        canvasStats = textObject.GetComponent<TextMeshProUGUI>();
        canvasStats.fontSize = 25f;
        canvasStats.fontStyle = FontStyles.Bold;
        canvasStats.color = Color.white;
        canvasStats.raycastTarget = false;
        canvasStats.enableWordWrapping = false;
        canvasStats.text = "NINJA SKY TRIAL";
    }

    public void CollectCoin()
    {
        coinCount++;
        ShowMessage(coinCount >= coinGoal ? "Portal unlocked!" : "Energy collected: " + coinCount + "/" + coinGoal, 2.2f);
    }

    public void TryExit()
    {
        if (stage == 3 && !bossDefeated)
        {
            ShowMessage("Defeat the boss first!", 2.4f);
            return;
        }
        if (coinCount < coinGoal)
        {
            ShowMessage("Collect " + (coinGoal - coinCount) + " more coins!", 2.3f);
            return;
        }
        AudioClip win = Resources.Load<AudioClip>("Audio/Finish");
        if (playerAudio != null && win != null) playerAudio.PlayOneShot(win);
        SceneManager.LoadScene(stage == 1 ? "Level2" : stage == 2 ? "Level3" : "Win");
    }

    public void OnBossDefeated()
    {
        bossDefeated = true;
        if (portal != null) portal.SetActive(true);
        ShowMessage("GUARDIAN DEFEATED! Enter the portal!", 8f);
    }

    void ShowMessage(string text, float duration)
    {
        message = text;
        messageUntil = Time.time + duration;
    }

    void Update()
    {
        if (canvasStats != null)
        {
            int hp = health != null ? health.CurrentHP : 3;
            canvasStats.text = "NINJA SKY TRIAL  /  " + stage + "\nHP " + hp + "/3     COINS " + coinCount + "/" + coinGoal;
        }
    }

    void OnGUI()
    {
        if (stage == 0) return;
        float scale = Mathf.Clamp(Screen.width / 760f, 0.7f, 1.5f);
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 24;
            titleStyle.normal.textColor = Color.white;
            smallStyle = new GUIStyle(titleStyle);
            smallStyle.fontSize = 17;
            centeredStyle = new GUIStyle(titleStyle);
            centeredStyle.alignment = TextAnchor.MiddleCenter;
        }
        if (canvasStats == null)
        {
            GUI.Box(new Rect(12 * scale, 10 * scale, 220 * scale, 85 * scale), "");
            GUI.Label(new Rect(22 * scale, 17 * scale, 220 * scale, 30 * scale), "NINJA SKY TRIAL  /  " + stage, titleStyle);
            int hp = health != null ? health.CurrentHP : 3;
            GUI.Label(new Rect(22 * scale, 50 * scale, 210 * scale, 25 * scale), "HP " + hp + "/3   |   COINS " + coinCount + "/" + coinGoal, smallStyle);
        }
        if (stage == 3 && boss != null)
        {
            GUI.Box(new Rect(Screen.width * 0.29f, 91 * scale, Screen.width * 0.42f, 63 * scale), "");
            GUI.Label(new Rect(Screen.width * 0.30f, 96 * scale, Screen.width * 0.40f, 26 * scale), "GUARDIAN  " + boss.CurrentHP + "/" + boss.maxHP, centeredStyle);
            float width = Screen.width * 0.35f;
            GUI.color = new Color(0.35f, 0.2f, 0.3f);
            GUI.DrawTexture(new Rect(Screen.width * 0.325f, 126 * scale, width, 10 * scale), Texture2D.whiteTexture);
            GUI.color = magenta;
            GUI.DrawTexture(new Rect(Screen.width * 0.325f, 126 * scale, width * (boss.CurrentHP / (float)boss.maxHP), 10 * scale), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
        if (GUI.Button(new Rect(Screen.width - 112 * scale, 12 * scale, 98 * scale, 32 * scale), "RESTART"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (Time.time < messageUntil)
        {
            float width = Mathf.Min(Screen.width - 20f, 520f * scale);
            GUI.Box(new Rect((Screen.width - width) * 0.5f, Screen.height * 0.16f, width, 50 * scale), message);
        }
    }
}
