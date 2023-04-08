using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public TextMeshProUGUI runDistance;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        runDistance.SetText("Highest distance: " + PlayerPrefs.GetInt("runDistance")+ " m");


        if (PlayerPrefs.GetInt("runDistance") > PlayerPrefs.GetInt("maxDistance"))
        {
            PlayerPrefs.SetInt("maxDistance", PlayerPrefs.GetInt("runDistance"));
        }
    }
    public void returnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
