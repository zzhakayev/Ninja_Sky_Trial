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
    private GameObject levelSelectPanel;
    private GameObject resetConfirmOverlay;
    private TextMeshProUGUI continueButtonText;
    private TextMeshProUGUI vibrationButtonText;
    private TextMeshProUGUI resetWarningText;
    private readonly Button[] levelButtons = new Button[3];
    private readonly TextMeshProUGUI[] levelButtonLabels = new TextMeshProUGUI[3];
    private TMP_FontAsset font;
    private TextMeshProUGUI musicButtonText;
    private TextMeshProUGUI volumeText;
    private Slider volumeSlider, sfxSlider;
    private TextMeshProUGUI sfxText;
    private bool portrait;

    private void Awake()
    {
        portrait = Screen.height > Screen.width;
        font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font == null) font = TMP_Settings.defaultFontAsset;
        BuildInterface();
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
        levelSelectPanel = Group(card, "Level select");
        BuildHome(homePanel.transform);
        BuildHelp(helpPanel.transform);
        BuildSettings(settingsPanel.transform);
        BuildLevelSelect(levelSelectPanel.transform);
        BuildResetConfirmation();
    }

    private void BuildHome(Transform parent)
    {
        float titleY = portrait ? 330 : 204;
        Label(parent, "NINJA", new Vector2(0, titleY + (portrait ? 58 : 24)), new Vector2(620, 100), portrait ? 79 : 69, Highlight, FontStyles.Bold, TextAlignmentOptions.Center);
        Label(parent, "SKY TRIAL", new Vector2(0, titleY - (portrait ? 26 : 45)), new Vector2(650, 91), portrait ? 72 : 62, LightText, FontStyles.Bold, TextAlignmentOptions.Center);
        Label(parent, "3 LEVELS   /   PLATFORMER   /   GUARDIAN", new Vector2(0, portrait ? 182 : 100), new Vector2(650, 40), portrait ? 19 : 18, SoftText, FontStyles.Normal, TextAlignmentOptions.Center);
        Image(parent, "Accent line", Highlight, new Vector2(0, portrait ? 135 : 72), new Vector2(150, 4));

        float firstY = portrait ? 56 : 8;
        float gap = portrait ? 95 : 61;
        float width = portrait ? 500 : 430;
        float height = portrait ? 76 : 50;
        MenuButton(parent, "START GAME", new Vector2(0, firstY), new Vector2(width, height), Highlight, BackgroundColor, ContinueGame, out continueButtonText);
        MenuButton(parent, "LEVEL SELECT", new Vector2(0, firstY - gap), new Vector2(width, height), ButtonColor, LightText, () => Show(levelSelectPanel));
        MenuButton(parent, "HOW TO PLAY", new Vector2(0, firstY - 2 * gap), new Vector2(width, height), ButtonColor, LightText, () => Show(helpPanel));
        MenuButton(parent, "SETTINGS", new Vector2(0, firstY - 3 * gap), new Vector2(width, height), ButtonColor, LightText, () => Show(settingsPanel));
        MenuButton(parent, "QUIT", new Vector2(0, firstY - 4 * gap), new Vector2(width, height), ButtonColor, LightText, Quit);
        Label(parent, "COLLECT COINS. CROSS GAPS. DEFEAT THE GUARDIAN.", new Vector2(0, portrait ? -424 : -289), new Vector2(cardTextWidth(), portrait ? 39 : 33), portrait ? 17 : 15, SoftText, FontStyles.Normal, TextAlignmentOptions.Center);
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
        PageTitle(parent, "SETTINGS", "PLAYER, AUDIO AND PROGRESS");
        float width = portrait ? 500 : 320;
        float lx = portrait ? 0 : -180, rx = portrait ? 0 : 180;
        Label(parent, "PLAYER NAME", new Vector2(lx, portrait ? 206 : 104), new Vector2(width, 36), 22, SoftText, FontStyles.Bold, TextAlignmentOptions.Center);
        CreateNameInput(parent, new Vector2(lx, portrait ? 151 : 59), new Vector2(width, portrait ? 64 : 46));
        Label(parent, "MUSIC VOLUME", new Vector2(lx, portrait ? 76 : 4), new Vector2(width, 32), 22, SoftText, FontStyles.Bold, TextAlignmentOptions.Center);
        volumeSlider = CreateSlider(parent, new Vector2(lx, portrait ? 35 : -32), new Vector2(width - 55, 38), false);
        volumeText = Label(parent, "", new Vector2(lx, portrait ? 0 : -64), new Vector2(width, 28), 18, Highlight, FontStyles.Bold, TextAlignmentOptions.Center);
        Label(parent, "SFX VOLUME", new Vector2(lx, portrait ? -48 : -101), new Vector2(width, 32), 22, SoftText, FontStyles.Bold, TextAlignmentOptions.Center);
        sfxSlider = CreateSlider(parent, new Vector2(lx, portrait ? -89 : -138), new Vector2(width - 55, 38), true);
        sfxText = Label(parent, "", new Vector2(lx, portrait ? -123 : -170), new Vector2(width, 28), 18, Highlight, FontStyles.Bold, TextAlignmentOptions.Center);
        MenuButton(parent, "", new Vector2(rx, portrait ? -180 : 65), new Vector2(width, portrait ? 62 : 52), ButtonColor, LightText, ToggleMusic, out musicButtonText);
        MenuButton(parent, "", new Vector2(rx, portrait ? -253 : -7), new Vector2(width, portrait ? 62 : 52), ButtonColor, LightText, ToggleVibration, out vibrationButtonText);
        MenuButton(parent, "RESET PROGRESS", new Vector2(rx, portrait ? -326 : -79), new Vector2(width, portrait ? 62 : 52), new Color32(110, 38, 52, 255), LightText, AskReset);
        MenuButton(parent, "BACK TO MENU", new Vector2(0, portrait ? -416 : -252), new Vector2(portrait ? 480 : 390, portrait ? 70 : 48), Highlight, BackgroundColor, () => Show(homePanel));

        RefreshVolume();
        RefreshMusic();
        RefreshVibration();
    }

    // All three level buttons are created from the same UI helper.
    // Their locked/unlocked state comes from SaveManager, not PlayerPrefs.
    private void BuildLevelSelect(Transform parent)
    {
        PageTitle(parent, "LEVEL SELECT", "CHOOSE YOUR NEXT CHALLENGE");
        float width = portrait ? 505 : 460;
        float height = portrait ? 92 : 62;
        float top = portrait ? 155 : 91;
        float step = portrait ? 143 : 92;

        for (int i = 0; i < 3; i++)
        {
            int levelNumber = i + 1; // Capture a distinct level for each button.
            TextMeshProUGUI label;
            MenuButton(parent, "LEVEL " + levelNumber,
                new Vector2(0, top - step * i),
                new Vector2(width, height), ButtonColor, LightText,
                () => OpenLevel(levelNumber), out label);
            levelButtonLabels[i] = label;
            levelButtons[i] = label.GetComponentInParent<Button>();
        }

        Label(parent, "FINISH A LEVEL TO UNLOCK THE NEXT ONE",
            new Vector2(0, portrait ? -287 : -210),
            new Vector2(cardTextWidth(), 35),
            portrait ? 18 : 16, SoftText, FontStyles.Normal, TextAlignmentOptions.Center);
        MenuButton(parent, "BACK TO MENU",
            new Vector2(0, portrait ? -390 : -265),
            new Vector2(portrait ? 480 : 390, portrait ? 79 : 58),
            Highlight, BackgroundColor, () => Show(homePanel));
    }

    private bool IsUnlocked(int levelNumber)
    {
        if (levelNumber == 1) return true;
        return SaveManager.Instance != null &&
               SaveManager.Instance.IsLevelUnlocked(levelNumber);
    }

    private int HighestUnlockedLevel()
    {
        for (int level = 3; level >= 1; level--)
            if (IsUnlocked(level)) return level;
        return 1;
    }

    private void RefreshLevelButtons()
    {
        for (int i = 0; i < 3; i++)
        {
            bool unlocked = IsUnlocked(i + 1);
            if (levelButtons[i] == null || levelButtonLabels[i] == null)
                continue;

            levelButtons[i].interactable = unlocked;
            levelButtons[i].GetComponent<Image>().color = unlocked ? Highlight : ButtonColor;
            levelButtonLabels[i].color = unlocked ? BackgroundColor : SoftText;
            levelButtonLabels[i].text = unlocked
                ? "LEVEL " + (i + 1) + "  -  UNLOCKED"
                : "LEVEL " + (i + 1) + "  -  LOCKED";
        }
    }

    private void RefreshContinueButton()
    {
        if (continueButtonText == null) return;
        int level = HighestUnlockedLevel();
        continueButtonText.text = level == 1 ? "START GAME" : "CONTINUE  -  LEVEL " + level;
    }

    private void ContinueGame()
    {
        OpenLevel(HighestUnlockedLevel());
    }

    private void OpenLevel(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > 3 || !IsUnlocked(levelNumber))
        {
            Debug.LogWarning("Level is locked: " + levelNumber);
            return;
        }
        SceneManager.LoadScene("Level" + levelNumber);
    }

    private void BuildResetConfirmation()
    {
        RectTransform overlay = Image(root, "Reset progress confirmation",
            new Color32(3, 7, 19, 220), Vector2.zero, Vector2.zero);
        overlay.anchorMin = Vector2.zero;
        overlay.anchorMax = Vector2.one;
        overlay.offsetMin = Vector2.zero;
        overlay.offsetMax = Vector2.zero;
        overlay.GetComponent<Image>().raycastTarget = true; // Block taps behind the panel.
        resetConfirmOverlay = overlay.gameObject;

        float w = portrait ? 590 : 625;
        float h = portrait ? 340 : 310;
        RectTransform dialog = Image(overlay, "Confirmation dialog", CardColor,
            Vector2.zero, new Vector2(w, h));
        Image(dialog, "Confirmation accent", Orange,
            new Vector2(0, h / 2f - 4), new Vector2(w, 6));
        Label(dialog, "RESET PROGRESS?", new Vector2(0, portrait ? 99 : 92),
            new Vector2(w - 40, 70), portrait ? 37 : 34,
            Orange, FontStyles.Bold, TextAlignmentOptions.Center);
        resetWarningText = Label(dialog,
            "Your completed levels and coins will be deleted.",
            new Vector2(0, portrait ? 12 : 9),
            new Vector2(w - 56, 100), portrait ? 22 : 19,
            LightText, FontStyles.Normal, TextAlignmentOptions.Center);
        MenuButton(dialog, "CANCEL", new Vector2(-w * 0.245f, portrait ? -112 : -101),
            new Vector2(w * 0.41f, portrait ? 65 : 58),
            ButtonColor, LightText, () => resetConfirmOverlay.SetActive(false));
        MenuButton(dialog, "DELETE", new Vector2(w * 0.245f, portrait ? -112 : -101),
            new Vector2(w * 0.41f, portrait ? 65 : 58),
            new Color32(166, 46, 59, 255), LightText, ConfirmReset);
        resetConfirmOverlay.SetActive(false);
    }

    private void AskReset()
    {
        if (resetConfirmOverlay == null) return;
        SaveData data = SaveManager.Instance != null ? SaveManager.Instance.Data : null;
        if (resetWarningText != null)
        {
            resetWarningText.text = data == null
                ? "Delete saved game progress? This cannot be undone."
                : "Delete " + data.totalCoins + " saved coins and all unlocked levels?\nThis cannot be undone.";
        }
        resetConfirmOverlay.SetActive(true);
    }

    private void ConfirmReset()
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            Debug.LogError("SaveManager is missing, progress cannot be reset.");
            resetConfirmOverlay.SetActive(false);
            return;
        }
        manager.ResetProgress();
        SaveSystem.Save(manager.Data); // Persist default progress immediately.
        volumeSlider.SetValueWithoutNotify(manager.Data.musicVolume);
        sfxSlider.SetValueWithoutNotify(manager.Data.sfxVolume);
        RefreshVolume();
        RefreshMusic();
        RefreshVibration();
        resetConfirmOverlay.SetActive(false);
        Show(levelSelectPanel);
    }

    private void ToggleVibration()
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null || manager.Data == null) return;
        manager.SetVibration(!manager.Data.vibrationEnabled);
        RefreshVibration();
    }

    private void RefreshVibration()
    {
        if (vibrationButtonText == null) return;
        SaveManager manager = SaveManager.Instance;
        bool vibration = manager == null || manager.Data == null || manager.Data.vibrationEnabled;
        vibrationButtonText.text = vibration ? "VIBRATION: ON" : "VIBRATION: OFF";
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

    private Slider CreateSlider(Transform parent, Vector2 position, Vector2 size, bool isSfx)
    {
        GameObject sliderObject = new GameObject(isSfx ? "SFX volume slider" : "Music volume slider", typeof(RectTransform), typeof(Slider));
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
        var data = SaveManager.Instance != null ? SaveManager.Instance.Data : null;
        slider.SetValueWithoutNotify(data == null ? 0.8f : (isSfx ? data.sfxVolume : data.musicVolume));
        slider.onValueChanged.AddListener(value =>
        {
            var manager = SaveManager.Instance;
            if (manager != null)
            {
                if (isSfx) manager.SetSfxVolume(value);
                else manager.SetMusicVolume(value);
            }
            RefreshVolume();
        });
        track.GetComponent<Image>().raycastTarget = true;
        handle.GetComponent<Image>().raycastTarget = true;
        fill.GetComponent<Image>().raycastTarget = false;
        return slider;
    }

    private void ToggleMusic()
    {
        var manager = SaveManager.Instance;
        if (manager != null) manager.SetMusicEnabled(!manager.Data.musicEnabled);
        RefreshMusic();
    }

    private void RefreshMusic()
    {
        bool enabled = SaveManager.Instance == null || SaveManager.Instance.Data.musicEnabled;
        if (musicButtonText != null) musicButtonText.text = enabled ? "MUSIC: ON" : "MUSIC: OFF";
    }

    private void RefreshVolume()
    {
        if (volumeText != null && volumeSlider != null)
            volumeText.text = Mathf.RoundToInt(volumeSlider.value * 100f) + "%";
        if (sfxText != null && sfxSlider != null)
            sfxText.text = Mathf.RoundToInt(sfxSlider.value * 100f) + "%";
    }

    // Start/Continue and Level Select both use OpenLevel above.

    private void Quit()
    {
        Application.Quit();
    }

    private void Show(GameObject active)
    {
        if (homePanel != null) homePanel.SetActive(active == homePanel);
        if (helpPanel != null) helpPanel.SetActive(active == helpPanel);
        if (settingsPanel != null) settingsPanel.SetActive(active == settingsPanel);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(active == levelSelectPanel);
        if (active == homePanel) RefreshContinueButton();
        if (active == levelSelectPanel) RefreshLevelButtons();
        if (active == settingsPanel) RefreshVibration();
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
        button.onClick.AddListener(() => { GameAudio.Play("Click", 0.65f); onClick(); });
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
