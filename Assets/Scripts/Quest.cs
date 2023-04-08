using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{

    public int levelDifference;
    public int amountOfKillsStart;
    public int amountOfKillsEnd;
    [HideInInspector]
    public int currentKills;

    Questline[] quests = new Questline[1000];
    public Questline currentQuest;
    [HideInInspector]
    public int questCounter = 0;
    public int amountOfPoints;

    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TextMeshProUGUI questText;
    public TextMeshProUGUI questAmountText;
    public TextMeshProUGUI questRewardText;

    public string[] enemyTypes;

    public PauseMenu pauseMenu;
    public GameObject wall;

    void Start()
    {
        for (int i = 0; i < quests.Length; i++)
        {
            {
                int id = Random.Range(0, enemyTypes.Length);
                quests[i] = new Questline("", 
                    Random.Range(amountOfKillsStart, amountOfKillsEnd+1), Random.Range(levelDifference * i +1, levelDifference * (i+1)-1), enemyTypes[id]);
                quests[i].setName("Q" + (i + 1) + ": Defeat " + quests[i].getAmountOfKills() + " " + quests[i].getEnemyType() + " above or equal to Lvl. " + quests[i].getLevel());
            }
        }
        currentQuest = quests[questCounter];
        slider.value = 0;
        slider.maxValue = currentQuest.getAmountOfKills();
        questText.SetText(currentQuest.getName());
        questAmountText.SetText("0 / " + currentQuest.getAmountOfKills());
    }

    // Update is called once per frame
    void Update()
    {
        if(currentKills >= quests[questCounter].getAmountOfKills())
        {
            currentKills = 0;
            questCounter++;
            wall.transform.position = new Vector3(0, -100, 0);

            currentQuest = quests[questCounter];
            pauseMenu.addPoint(amountOfPoints);
            StartCoroutine(setQuestRewardText(amountOfPoints, 1));

            slider.value = 0;
            slider.maxValue = currentQuest.getAmountOfKills();
            questAmountText.SetText("0 / " + currentQuest.getAmountOfKills());

            questText.SetText(currentQuest.getName());
       
        }
    }

    public IEnumerator setQuestRewardText(int points, float time)
    {
        questRewardText.SetText("+" + points + " Points");
        yield return new WaitForSecondsRealtime(time);
        questRewardText.SetText("");
    }

    public class Questline
    {
        private string name;
        private int amountOfKills;
        private int level;
        private string enemyType;
        public Questline(string name, int amountOfKills, int level, string enemyType)
        {
            this.name = name;
            this.amountOfKills = amountOfKills;
            this.level = level;
            this.enemyType = enemyType;
        }

        public string getName()
        {
            return name;
        }
        
        public int getAmountOfKills()
        {
            return amountOfKills;
        }

        public int getLevel()
        {
            return level;
        }

        public string getEnemyType()
        {
            return enemyType;
        }

        public void setName(string name)
        {
            this.name = name;
        }
    }
}
