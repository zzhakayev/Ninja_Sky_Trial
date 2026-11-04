using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject settingsPanel;
    public AudioSource bgSound;

    public void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Settings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void Back()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void SetVolume(float value)
    {
        if (bgSound != null) bgSound.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void OpenPanel(GameObject panel)
    {
        if (panel != null) panel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel != null) panel.SetActive(false);
    }
}
