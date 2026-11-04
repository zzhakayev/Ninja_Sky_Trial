using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NinjaModernMenu : MonoBehaviour
{
    private static readonly Color BackgroundColor = new Color32(8, 13, 31, 255);
    private static readonly Color CardColor = new Color32(20, 31, 59, 248);
    private static readonly Color Highlight = new Color32(44, 232, 217, 255);
    private static readonly Color LightText = new Color32(242, 248, 255, 255);
    private static readonly Color SoftText = new Color32(171, 188, 213, 255);
    private static readonly Color ButtonColor = new Color32(39, 58, 92, 255);
    private static readonly Color Orange = new Color32(255, 180, 88, 255);

    private RectTransform root;
    private GameObject homePanel;
    private GameObject helpPanel;
    private GameObject settingsPanel;
    private TMP_FontAsset font;
    private AudioSource menuMusic;
    private TextMeshProUGUI musicButtonText;
    private TextMeshProUGUI volumeText;
    private Slider volumeSlider;
    private bool portrait;

    private void Awake()
    {
        portrait = Screen.height > Screen.width;
        font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font == null) font = TMP_Settings.defaultFontAsset;
        BuildInterface();
        ConfigureMusic();
        Show(homePanel);
    }

    private void BuildInterface()
    {
        GameObject canvasObject = new GameObject("Ninja Modern Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 150;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = portrait ? new Vector2(720, 1280) : new Vector2(1280, 720);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        root = canvasObject.GetComponent<RectTransform>();

        RectTransform background = Image(root, "Deep navy background", BackgroundColor, Vector2.zero, Vector2.zero);
        background.anchorMin = Vector2.zero;
        background.anchorMax = Vector2.one;
        background.offsetMin = Vector2.zero;
        background.offsetMax = Vector2.zero;
        Image(root, "Left glow", new Color32(25, 60, 100, 128), new Vector2(portrait ? 12 : -510, 0), new Vector2(portrait ? 35 : 140, portrait ? 800 : 700));
        Image(root, "Right accent", new Color32(45, 113, 121, 100), new Vector2(portrait ? 335 : 545, 0), new Vector2(12, portrait ? 1150 : 700));

        float cardWidth = portrait ? 650 : 785;
        float cardHeight = portrait ? 980 : 625;
        RectTransform card = Image(root, "Menu card", CardColor, Vector2.zero, new Vector2(cardWidth, cardHeight));
        Image(card, "Top glow", Highlight, new Vector2(0, cardHeight / 2f - 4), new Vector2(cardWidth, 6));

        homePanel = Group(card, "Main menu");
        helpPanel = Group(card, "How to play");
        settingsPanel = Group(card, "Settings");
        BuildHome(homePanel.transform);
        BuildHelp(helpPanel.transform);
        BuildSettings(settingsPanel.transform);
    }

    private void BuildHome(Transform parent)
    {
        float titleY = portrait ? 330 : 204;
        Label(parent, "NINJA", new Vector2(0, titleY + (portrait ? 58 : 24)), new Vector2(620, 100), portrait ? 79 : 69, Highlight, FontStyles.Bold, TextAlignmentOptions.Center);
        Label(parent, "SKY TRIAL", new Vector2(0, titleY - (portrait ? 26 : 45)), new Vector2(650, 91), portrait ? 72 : 62, LightText, FontStyles.Bold, TextAlignmentOptions.Center);
        Label(parent, "3 LEVELS   /   PLATFORMER   /   GUARDIAN", new Vector2(0, portrait ? 182 : 100), new Vector2(650, 40), portrait ? 19 : 18, SoftText, FontStyles.Normal, TextAlignmentOptions.Center);
        Image(parent, "Accent line", Highlight, new Vector2(0, portrait ? 135 : 72), new Vector2(150, 4));

        float firstY = portrait ? 44 : 10;
        float gap = portrait ? 113 : 76;
        float width = portrait ? 500 : 430;
        float height = portrait ? 86 : 62;
        MenuButton(parent, "START GAME", new Vector2(0, firstY), new Vector2(width, height), Highlight, BackgroundColor, Play);
        MenuButton(parent, "HOW TO PLAY", new Vector2(0, firstY - gap), new Vector2(width, height), ButtonColor, LightText, () => Show(helpPanel));
        MenuButton(parent, "SETTINGS", new Vector2(0, firstY - 2 * gap), new Vector2(width, height), ButtonColor, LightText, () => Show(settingsPanel));
        MenuButton(parent, "QUIT", new Vector2(0, firstY - 3 * gap), new Vector2(width, height), ButtonColor, LightText, Quit);
        Label(parent, "COLLECT COINS. CROSS GAPS. DEFEAT THE GUARDIAN.", new Vector2(0, portrait ? -405 : -288), new Vector2(cardTextWidth(), 39), portrait ? 17 : 16, SoftText, FontStyles.Normal, TextAlignmentOptions.Center);
    }

    private void BuildHelp(Transform parent)
    {
        PageTitle(parent, "HOW TO PLAY", "CONTROLS AND OBJECTIVES");
        float top = portrait ? 220 : 119;
        float spacing = portrait ? 93 : 60;
        HelpLine(parent, top, "MOVE", "WASD / ARROW KEYS  OR  ON-SCREEN JOYSTICK");
        HelpLine(parent, top - spacing, "JUMP", "SPACE  OR  GREEN JUMP BUTTON");
        HelpLine(parent, top - spacing * 2, "FIRE", "X / LEFT MOUSE  OR  RED FIRE BUTTON");
        HelpLine(parent, top - spacing * 3, "LEVELS", "COLLECT THE REQUIRED COINS AND FIND THE PORTAL");
        HelpLine(parent, top - spacing * 4, "HEALTH", "AVOID HAZARDS AND PICK UP HEALING CRYSTALS");
        HelpLine(parent, top - spacing * 5, "BOSS", "DODGE GUARDIAN ATTACKS AND SHOOT UNTIL 0 HP");
        MenuButton(parent, "BACK TO MENU", new Vector2(0, portrait ? -390 : -267), new Vector2(portrait ? 480 : 390, portrait ? 79 : 58), Highlight, BackgroundColor, () => Show(homePanel));
    }

    private void BuildSettings(Transform parent)
    {
        PageTitle(parent, "SETTINGS", "YOUR PLAYER AND AUDIO");
        float width = portrait ? 500 : 490;
        Label(parent, "PLAYER NAME", new Vector2(0, portrait ? 206 : 109), new Vector2(width, 36), 24, SoftText, FontStyles.Bold, TextAlignmentOptions.Center);
        CreateNameInput(parent, new Vector2(0, portrait ? 145 : 62), new Vector2(width, portrait ? 75 : 52));
        Label(parent, "MUSIC VOLUME", new Vector2(0, portrait ? 30 : -20), new Vector2(width, 38), 23, SoftText, FontStyles.Bold, TextAlignmentOptions.Center);
        volumeSlider = CreateSlider(parent, new Vector2(0, portrait ? -29 : -70), new Vector2(width - 80, 46));
        volumeText = Label(parent, "", new Vector2(0, portrait ? -75 : -107), new Vector2(width, 35), 20, Highlight, FontStyles.Bold, TextAlignmentOptions.Center);
        MenuButton(parent, "", new Vector2(0, portrait ? -175 : -164), new Vector2(width, portrait ? 77 : 53), ButtonColor, LightText, ToggleMusic, out musicButtonText);
        RefreshVolume();
        RefreshMusic();
        MenuButton(parent, "BACK TO MENU", new Vector2(0, portrait ? -389 : -265), new Vector2(portrait ? 480 : 390, portrait ? 79 : 58), Highlight, BackgroundColor, () => Show(homePanel));
    }

    private float cardTextWidth()
    {
        return portrait ? 620 : 750;
    }

    private void PageTitle(Transform parent, string title, string caption)
    {
        Label(parent, title, new Vector2(0, portrait ? 350 : 231), new Vector2(cardTextWidth(), 92), portrait ? 57 : 46, Highlight, FontStyles.Bold, TextAlignmentOptions.Center);
        Label(parent, caption, new Vector2(0, portrait ? 287 : 183), new Vector2(cardTextWidth(), 42), portrait ? 20 : 17, SoftText, FontStyles.Normal, TextAlignmentOptions.Center);
        Image(parent, "Title rule", new Color32(57, 92, 126, 255), new Vector2(0, portrait ? 248 : 153), new Vector2(portrait ? 470 : 580, 2));
    }

    private void HelpLine(Transform parent, float y, string title, string description)
    {
        float width = portrait ? 560 : 600;
        Label(parent, title, new Vector2(0, y + (portrait ? 19 : 13)), new Vector2(width, 35), portrait ? 23 : 20, Orange, FontStyles.Bold, TextAlignmentOptions.Center);
        Label(parent, description, new Vector2(0, y - (portrait ? 20 : 17)), new Vector2(width, portrait ? 43 : 37), portrait ? 18 : 17, LightText, FontStyles.Normal, TextAlignmentOptions.Center);
    }

    private void CreateNameInput(Transform parent, Vector2 position, Vector2 size)
    {
        RectTransform holder = Image(parent, "Player name input", ButtonColor, position, size);
        TMP_InputField input = holder.gameObject.AddComponent<TMP_InputField>();
        holder.GetComponent<Image>().raycastTarget = true;
        input.targetGraphic = holder.GetComponent<Image>();
        RectTransform viewport = Group(holder, "Input viewport").GetComponent<RectTransform>();
        viewport.anchorMin = new Vector2(0, 0);
        viewport.anchorMax = new Vector2(1, 1);
        viewport.offsetMin = new Vector2(18, 4);
        viewport.offsetMax = new Vector2(-18, -4);
        TextMeshProUGUI entry = Label(viewport, "", Vector2.zero, size - new Vector2(38, 10), portrait ? 31 : 25, LightText, FontStyles.Normal, TextAlignmentOptions.Center);
        TextMeshProUGUI placeholder = Label(viewport, "ENTER PLAYER NAME", Vector2.zero, size - new Vector2(38, 10), portrait ? 25 : 21, SoftText, FontStyles.Italic, TextAlignmentOptions.Center);
        input.textViewport = viewport;
        input.textComponent = entry;
        input.placeholder = placeholder;
        input.characterLimit = 20;
        input.text = PlayerPrefs.GetString("PlayerName", "Player");
        input.onEndEdit.AddListener(value =>
        {
            PlayerPrefs.SetString("PlayerName", string.IsNullOrWhiteSpace(value) ? "Player" : value.Trim());
            PlayerPrefs.Save();
        });
    }

    private Slider CreateSlider(Transform parent, Vector2 position, Vector2 size)
    {
        GameObject sliderObject = new GameObject("Music volume slider", typeof(RectTransform), typeof(Slider));
        sliderObject.transform.SetParent(parent, false);
        RectTransform sliderRT = sliderObject.GetComponent<RectTransform>();
        Place(sliderRT, position, size);
        RectTransform track = Image(sliderRT, "Track", ButtonColor, Vector2.zero, new Vector2(size.x, 11));
        RectTransform fill = Image(sliderRT, "Fill", Highlight, Vector2.zero, new Vector2(size.x, 11));
        fill.anchorMin = new Vector2(0, 0.5f);
        fill.anchorMax = new Vector2(1, 0.5f);
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
        RectTransform handle = Image(sliderRT, "Handle", LightText, Vector2.zero, new Vector2(31, 31));
        Slider slider = sliderObject.GetComponent<Slider>();
        slider.fillRect = fill;
        slider.handleRect = handle;
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = PlayerPrefs.GetFloat("Volume", 1f);
        slider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat("Volume", value);
            PlayerPrefs.Save();
            RefreshVolume();
            UpdateMusicAudio();
        });
        track.GetComponent<Image>().raycastTarget = true;
        handle.GetComponent<Image>().raycastTarget = true;
        fill.GetComponent<Image>().raycastTarget = false;
        return slider;
    }

    private void ConfigureMusic()
    {
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in sources)
        {
            if (source != null && source.isPlaying && source.loop)
            {
                menuMusic = source;
                break;
            }
        }
        if (menuMusic == null)
        {
            AudioClip clip = Resources.Load<AudioClip>("Audio/LevelMusic");
            if (clip != null)
            {
                menuMusic = gameObject.AddComponent<AudioSource>();
                menuMusic.playOnAwake = false;
                menuMusic.loop = true;
                menuMusic.clip = clip;
                menuMusic.spatialBlend = 0f;
                menuMusic.Play();
            }
        }
        UpdateMusicAudio();
    }

    private void UpdateMusicAudio()
    {
        if (menuMusic == null) return;
        menuMusic.volume = 0.17f * PlayerPrefs.GetFloat("Volume", 1f);
        menuMusic.mute = PlayerPrefs.GetInt("Music", 1) == 0;
    }

    private void ToggleMusic()
    {
        int enabled = PlayerPrefs.GetInt("Music", 1) == 0 ? 1 : 0;
        PlayerPrefs.SetInt("Music", enabled);
        PlayerPrefs.Save();
        RefreshMusic();
        UpdateMusicAudio();
    }

    private void RefreshMusic()
    {
        if (musicButtonText != null)
            musicButtonText.text = PlayerPrefs.GetInt("Music", 1) == 0 ? "MUSIC: OFF  (TAP TO ENABLE)" : "MUSIC: ON  (TAP TO MUTE)";
    }

    private void RefreshVolume()
    {
        if (volumeText != null)
            volumeText.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("Volume", 1f) * 100f) + "%";
    }

    private void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void Show(GameObject active)
    {
        if (homePanel != null) homePanel.SetActive(active == homePanel);
        if (helpPanel != null) helpPanel.SetActive(active == helpPanel);
        if (settingsPanel != null) settingsPanel.SetActive(active == settingsPanel);
    }

    private GameObject Group(Transform parent, string name)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        Place(obj.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        return obj;
    }

    private RectTransform Image(Transform parent, string name, Color color, Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        Place(rect, position, size);
        Image image = obj.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return rect;
    }

    private TextMeshProUGUI Label(Transform parent, string value, Vector2 position, Vector2 size, float sizeFont, Color color, FontStyles style, TextAlignmentOptions align)
    {
        GameObject obj = new GameObject("Text - " + value, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        Place(obj.GetComponent<RectTransform>(), position, size);
        TextMeshProUGUI label = obj.GetComponent<TextMeshProUGUI>();
        if (font != null) label.font = font;
        label.text = value;
        label.color = color;
        label.fontStyle = style;
        label.fontSize = sizeFont;
        label.alignment = align;
        label.raycastTarget = false;
        label.overflowMode = TextOverflowModes.Overflow;
        return label;
    }

    private void MenuButton(Transform parent, string text, Vector2 position, Vector2 size, Color bg, Color fg, System.Action onClick)
    {
        TextMeshProUGUI label;
        MenuButton(parent, text, position, size, bg, fg, onClick, out label);
    }

    private void MenuButton(Transform parent, string text, Vector2 position, Vector2 size, Color bg, Color fg, System.Action onClick, out TextMeshProUGUI textObject)
    {
        RectTransform holder = Image(parent, "Button " + text, bg, position, size);
        Image image = holder.GetComponent<Image>();
        image.raycastTarget = true;
        Button button = holder.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock block = button.colors;
        block.normalColor = Color.white;
        block.highlightedColor = new Color32(216, 239, 255, 255);
        block.pressedColor = new Color32(177, 209, 225, 255);
        block.selectedColor = Color.white;
        button.colors = block;
        button.onClick.AddListener(() => onClick());
        textObject = Label(holder, text, Vector2.zero, size - new Vector2(12, 4), portrait ? 26 : 24, fg, FontStyles.Bold, TextAlignmentOptions.Center);
    }

    private static void Place(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
}
