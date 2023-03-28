using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public bool GamePaused = false;
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


    public Text movementSpeedValue;
    public Text attackSpeedValue;
    public Text attackDamageValue;
    public Text healthValue;

    public HealthBar healthBar;
    public RigidbodyFirstPersonController playerScript;


    // Called at the start to set all the values
    void Start()
    {
        points = 1;
        xp = 0;
        nextLevel = 100;
        findButtons();
        Resume();

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
            Cursor.visible = true;
        }
        else
        { 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
}
    // Functions
    public void Pause()
    {
        //pauseMenuUI.SetActive(true);
        //attributesGUI.SetActive(true);
        pauseMenuUI.transform.localScale = new Vector3(1, 1, 1);
        attributesGUI.transform.localScale = new Vector3(1, 1, 1);
        Time.timeScale = 0f;
        GamePaused = true;
        updateStatsText();
    }
    public void Resume()
    {
        //pauseMenuUI.SetActive(false);
        //attributesGUI.SetActive(false);
        pauseMenuUI.transform.localScale = new Vector3(0, 0, 0);
        attributesGUI.transform.localScale = new Vector3(0, 0, 0);
        Time.timeScale = 1f;
        GamePaused = false;
    }

    public void Quit()
    {
        Debug.Log("QUIT!");
        Application.Quit();
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
    // Doesnt work because you can not acces xp while the pause menu is disabled !!!!!!!!!!!!!!!!!
    public void addXP(int xpValue)
    {
        xp += xpValue;
        if (xp >= nextLevel)
        {
            addPoint();
            xp -= nextLevel;
        }
        updateXPText();
        Debug.Log(xp);
    }

    // Called after Experience Points are updated, displays the current XP
    // Doesnt work because you can not acces xp while the pause menu is disabled !!!!!!!!!!!!!!!!!
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
    
    public void AddHealth()
    {
        findButtons();
        Debug.Log("Health");
        points--;
        updatePointsText();
        playerScript.maxHealth += 10;
        healthBar.SetMaxHealth(playerScript.maxHealth);
        Debug.Log(playerScript.maxHealth);
        if (points <= 0)
        {
            disableButtons();
        }
        updateStatsText();
    }

    public void AddAttackDamage()
    {
        findButtons();
        Debug.Log("Damage");
        points--;
        updatePointsText();
        playerScript.damage += 2;
        if (points <= 0)
        {
            disableButtons();
        }
        updateStatsText();
    }

    public void AddAttackSpeed()
    {
        findButtons();
        Debug.Log("AttSpeed");
        points--;
        updatePointsText();
        playerScript.attackSpeedAmount *= 1.1f;
        if (points <= 0)
        {
            disableButtons();
        }
        updateStatsText();
    }

    public void AddMoveSpeed()
    {
        findButtons();
        Debug.Log("MoveSpeed");
        points--;
        updatePointsText();
        if (points <= 0)
        {
            disableButtons();
        }

        playerScript.ForwardSpeed *= 1.1f;
        playerScript.BackwardSpeed *= 1.1f;
        playerScript.StrafeSpeed *= 1.1f;
        updateStatsText();
    }

    // Called after stats are updated, displays the current values
    public void updateStatsText()
    {
        GameObject.Find("MovementSpeedText").GetComponent<Text>().text = " Movement Speed: " + playerScript.ForwardSpeed;
        GameObject.Find("AttackSpeedText").GetComponent<Text>().text = " Attack Speed: " + playerScript.attackSpeedAmount;
        GameObject.Find("ADText").GetComponent<Text>().text = " Attack Damage: " + playerScript.damage;
        GameObject.Find("HealthText").GetComponent<Text>().text = " Health: " + playerScript.maxHealth;
    }
}
