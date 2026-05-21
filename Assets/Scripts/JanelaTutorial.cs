using UnityEngine;

public class JanelaTutorial : MonoBehaviour
{
    public GameObject painelTutorial;

    void Start()
    {
        if (painelTutorial != null)
        {
            painelTutorial.SetActive(true);
            Time.timeScale = 0f; 
        }
    }

    public void FecharTutorial()
    {
        if (painelTutorial != null)
        {
            painelTutorial.SetActive(false);
            Time.timeScale = 1f; 
            

            AudioManager.Instance?.TocarUI();
        }
    }
}