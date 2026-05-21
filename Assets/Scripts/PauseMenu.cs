using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (container.activeSelf)
            {
                ResumeButton();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        container.SetActive(true);
        Time.timeScale = 0;

        AudioManager.Instance?.PararMusica();
        AudioManager.Instance?.PararPassoPlayer();
        AudioManager.Instance?.PararPassoMonstro();
        AudioManager.Instance?.TocarUI();
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;

        AudioManager.Instance?.TocarMusica();
        AudioManager.Instance?.TocarUI();
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }
}