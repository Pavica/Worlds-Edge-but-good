using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI maxDistance;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!PlayerPrefs.HasKey("maxDistance"))
        {
            PlayerPrefs.SetInt("maxDistance", 0);
        }
        maxDistance.SetText("Highest distance ever: " + PlayerPrefs.GetInt("maxDistance") + " m");
    }
    public void playGame()
    {
        SceneManager.LoadScene(1);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
