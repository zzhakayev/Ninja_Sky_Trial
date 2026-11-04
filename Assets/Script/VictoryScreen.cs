using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    private GUIStyle title;
    private GUIStyle subtitle;

    void OnGUI()
    {
        if (title == null)
        {
            title = new GUIStyle(GUI.skin.label);
            title.fontSize = Mathf.RoundToInt(Mathf.Clamp(Screen.width / 15f, 30f, 68f));
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.normal.textColor = new Color(0.25f, 0.95f, 0.9f);
            subtitle = new GUIStyle(title);
            subtitle.fontSize = Mathf.RoundToInt(Mathf.Clamp(Screen.width / 33f, 14f, 30f));
            subtitle.normal.textColor = Color.white;
        }
        float w = Mathf.Min(Screen.width - 28f, 650f);
        float x = (Screen.width - w) / 2f;
        float y = Screen.height * 0.26f;
        GUI.Box(new Rect(x, y, w, 265f), "");
        GUI.Label(new Rect(x, y + 24, w, 82), "VICTORY!", title);
        GUI.Label(new Rect(x + 12, y + 108, w - 24, 60), "ALL 3 LEVELS COMPLETED\nTHE GUARDIAN IS DEFEATED", subtitle);
        if (GUI.Button(new Rect(x + w * 0.15f, y + 192, w * 0.7f, 52f), "BACK TO MAIN MENU"))
            SceneManager.LoadScene("Main menu");
    }
}
