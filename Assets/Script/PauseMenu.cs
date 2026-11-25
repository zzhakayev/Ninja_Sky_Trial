
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    void Start()
    {
        Time.timeScale = 1f;
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void PauseGame()
    {
        if (pausePanel == null) return;

        GameAudio.Play("Click", 0.65f);
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        GameAudio.Pause(true);
    }

    public void ResumeGame()
    {
        GameAudio.Play("Click", 0.65f);
        GameAudio.Pause(false);
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void RestartLevel()
    {
        GameAudio.Play("Click", 0.65f);
        GameAudio.Pause(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        GameAudio.Play("Click", 0.65f);
        GameAudio.Pause(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main menu");
    }
}
