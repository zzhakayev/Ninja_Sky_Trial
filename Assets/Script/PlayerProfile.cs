using UnityEngine;
using TMPro;

public class PlayerProfile : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    public Color[] colors =
    {
        Color.red,
        Color.blue,
        Color.green
    };

    void Start()
    {
        nameText.text = PlayerPrefs.GetString("PlayerName", "Player");

        int colorIndex = PlayerPrefs.GetInt("PlayerColor", 0);
        GetComponent<Renderer>().material.color = colors[colorIndex];
    }
}