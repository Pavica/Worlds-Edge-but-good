using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public int points;
    public int xp;
    public int nextLevel;
    public GameObject pauseMenuUI;
    public GameObject attributesGUI;

    public Button addHealthButton;
    public Button attackSpeedButton;
    public Button attackDamageButton;
    public Button movementSpeedButton;

    public Text availablePoints;
    public Text experiencePoints;

    // Called at the start to set all the values
    private void Start()
    {
        points = 1;
        xp = 0;
        nextLevel = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (GamePaused)
                Resume();
            else
                Pause();
        }

        if (GamePaused)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        { 
            Cursor.lockState = CursorLockMode.Locked;
        }
}

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        attributesGUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        attributesGUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void Quit()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void AddHealth()
    {
        findButtons();
        Debug.Log("Health");
        points--;
        updatePointsText();
        //Add healthpoints
        if (points == 0)
            disableButtons();
    }

    public void AddAttackDamage()
    {
        findButtons();
        Debug.Log("Damage");
        points--;
        updatePointsText();
        if (points == 0)
            disableButtons();
    }

    public void AddAttackSpeed()
    {
        findButtons();
        Debug.Log("AttSpeed");
        points--;
        updatePointsText();
        if (points == 0)
            disableButtons();
    }

    public void AddMoveSpeed()
    {
        findButtons();
        Debug.Log("MoveSpeed");
        points--;
        updatePointsText();
        if (points == 0)
            disableButtons();
    }

    // Function to set all buttons
    public void findButtons()
    {
        addHealthButton = GameObject.Find("HealthButton").GetComponent<Button>();
        attackSpeedButton = GameObject.Find("AttackSpeedButton").GetComponent<Button>();
        attackDamageButton = GameObject.Find("ADButton").GetComponent<Button>();
        movementSpeedButton = GameObject.Find("MovementSpeedButton").GetComponent<Button>();
    }

    // Called by add-functions if no more points are available
    public void disableButtons()
    {
        addHealthButton.interactable = false;
        attackDamageButton.interactable = false;
        attackSpeedButton.interactable = false;
        movementSpeedButton.interactable = false;
    }

    // Called by addXP-function if the amount of XP surpases the XP required for the next level
    public void addPoint()
    {
        points++;
        addHealthButton.interactable = true;
        attackDamageButton.interactable = true;
        attackSpeedButton.interactable = true;
        movementSpeedButton.interactable = true;

        updatePointsText();
    }

    // Called after every action that adds XP
    public void addXP()
    {
        xp += 10;
        if (xp > nextLevel)
        {
            addPoint();
            xp -= nextLevel;
        }
    }

    // Called after Experience Points are updated, displays the current XP
    public void updateXPText()
    {
        experiencePoints = GameObject.Find("ExperiencePoints").GetComponent<Text>();
        experiencePoints.text = "XP: " + xp + "/" + nextLevel;
    }

    // Called after points are updated, displays the current available points
    public void updatePointsText()
    {
        availablePoints = GameObject.Find("PointsAvailable").GetComponent<Text>();
        availablePoints.text = "Points available: " + points;
    }
}
