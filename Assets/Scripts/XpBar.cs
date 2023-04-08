using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{


    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public int level = 1;
    public int xp;
    int nextLevel;
    public int nextLevelBase;
    public int amountOfPoints;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public PauseMenu pauseMenu;

    private void Start()
    {
        xp = 0;
        nextLevel = nextLevelBase;
        slider.maxValue = nextLevel;
    }

    public void increaseLevel()
    {
        level++;
        levelText.SetText("Lvl. " + level);
    }

    public void setLevel(int level)
    {
        this.level = level;
        levelText.SetText("Lvl. " + this.level);
    }

    public IEnumerator setXpText(int xp, float time)
    {
        xpText.SetText("+" + xp + "xp");
        yield return new WaitForSecondsRealtime(time);
        xpText.SetText("");
    }

    // Called after every action that adds XP
    public void addXP(int xpValue)
    {
        StartCoroutine(setXpText(xpValue, 1));
        xp += xpValue;
        slider.value = xp;
        if (xp >= nextLevel)
        {
            increaseLevel();
            pauseMenu.addPoint(amountOfPoints);
            xp -= nextLevel;
            slider.value = xp;
            nextLevel = Mathf.RoundToInt(Mathf.Log(2 + level - 1, 2) * nextLevelBase);
            slider.maxValue = nextLevel;
        }
        updateXPText();
        Debug.Log(xp);
    }


    // Called after Experience Points are updated, displays the current XP
    public void updateXPText()
    {
        GameObject.Find("ExperiencePoints").GetComponent<Text>().text = "XP: " + xp + "/" + nextLevel;
    }
}
