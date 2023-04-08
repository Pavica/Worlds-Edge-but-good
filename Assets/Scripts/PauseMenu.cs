using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool GamePaused = false;
    public int points;

    public int healthLevel;
    public int damageLevel;
    public int speedLevel;
    public int attackSpeedLevel;

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
    public XpBar xpBar;
    public RigidbodyFirstPersonController playerScript;
    public Animator sword;


    // Called at the start to set all the values
    void Start()
    {
        updatePointsText();
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
        SceneManager.LoadScene(0);
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
    public void addPoint(int amountOfPoints)
    {
        points+= amountOfPoints;
        addHealthButton.interactable = true;
        attackDamageButton.interactable = true;
        attackSpeedButton.interactable = true;
        movementSpeedButton.interactable = true;

        updatePointsText();
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
        healthLevel++;
        updateLevelText("health", healthLevel);
        updatePointsText();
        playerScript.maxHealth = (float)Math.Round(Mathf.Log(5 + healthLevel - 1, 5) * playerScript.baseMaxHealth,1); 
        healthBar.setMaxHealth(playerScript.maxHealth);
        playerScript.health = playerScript.maxHealth;
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
        damageLevel++;
        updateLevelText("damage", damageLevel);
        updatePointsText();
        playerScript.damage = (float)Math.Round(Mathf.Log(5 + damageLevel - 1, 5) * playerScript.baseDamage,1);
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
        attackSpeedLevel++;
        updateLevelText("attackSpeed", attackSpeedLevel);
        updatePointsText();
        playerScript.attackSpeedAmount = (float)Math.Round(Mathf.Log(8 + attackSpeedLevel - 1, 8) * playerScript.baseAttackSpeed, 2);
        sword.SetFloat("attackSpeed", playerScript.attackSpeedAmount);
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
        speedLevel++;
        updateLevelText("speed", speedLevel);
        updatePointsText();
        if (points <= 0)
        {
            disableButtons();
        }
        playerScript.ForwardSpeed = (float)Math.Round(Mathf.Log(8 + speedLevel - 1, 8) * playerScript.baseSpeed, 2);
        playerScript.BackwardSpeed = playerScript.ForwardSpeed / 2;
        playerScript.StrafeSpeed = playerScript.ForwardSpeed / 2;
        updateStatsText();
    }

    public void updateLevelText(String identifier, int level)
    {
        String obj = identifier + "Level";
        GameObject.Find(obj).GetComponent<TextMeshProUGUI>().text = "Lvl. " + level;
    }

    // Called after stats are updated, displays the current values
    public void updateStatsText()
    {
        GameObject.Find("MovementSpeedText").GetComponent<Text>().text = " Movement Speed: " + Math.Round(playerScript.ForwardSpeed,2);
        GameObject.Find("AttackSpeedText").GetComponent<Text>().text = " Attack Speed: " + Math.Round(playerScript.attackSpeedAmount,2);
        GameObject.Find("ADText").GetComponent<Text>().text = " Attack Damage: " + Math.Round(playerScript.damage,1);
        GameObject.Find("HealthText").GetComponent<Text>().text = " Health: " + Math.Round(playerScript.maxHealth,1);
    }
}
