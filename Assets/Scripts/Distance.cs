using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Distance : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform player;
    public Transform northpole;
    public Transform enemyLegs;
    public Transform enemyShark;
    public Transform enemySlime;
    Transform helpEnemy;
    public Transform quest;

    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI levelRangeText;

    public PauseMenu pauseMenu;
    int distanceIndex = 0;
    int distance = 0;
    [HideInInspector]
    public int distanceHelp = 0;

    public int distanceMultiplier;
    public int levelRangeDifference;

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    [HideInInspector]
    public DistanceLevelScale currentLevelRange;

    DistanceLevelScale[] distanceLevelScales = new DistanceLevelScale[1000];

    public GameObject wall;

   

    void Start()
    {
        for(int i = 0; i<distanceLevelScales.Length; i++)
        {
  
            distanceLevelScales[i] = new DistanceLevelScale(distanceMultiplier * i + distanceMultiplier, i* levelRangeDifference + 1, levelRangeDifference * (i + 1));
            
        }
        currentLevelRange = distanceLevelScales[distanceIndex];
        levelRangeText.SetText("Lvl. " + currentLevelRange.getStartValue() + "-" + currentLevelRange.getEndValue());
        slider.maxValue = distanceLevelScales[distanceIndex].getDistanceThreshold();
        slider.value = 0;
        
        StartCoroutine(spawnEnemy(5));
    }

    // Update is called once per frame
    void Update()
    {
        distance = (int)Vector3.Distance(player.position, northpole.position);

        if(distance > distanceHelp)
        {
            distanceHelp = distance;
            PlayerPrefs.SetInt("runDistance", distanceHelp);
        }

        slider.value = distance % distanceMultiplier;
        if(distance%(distanceMultiplier * (quest.GetComponent<Quest>().questCounter+1)) == 0 && distance != 0)
        {
            wall.transform.position = new Vector3(player.position.x, player.position.y, player.position.z);
            wall.transform.rotation = Quaternion.LookRotation(player.position);
        }
        distanceText.text = distance + " m";

        if (distance >= distanceLevelScales[distanceLevelScales.Length-1].getDistanceThreshold())
        {
            currentLevelRange = distanceLevelScales[distanceLevelScales.Length-1];
            levelRangeText.SetText("Lvl. " + currentLevelRange.getStartValue() + "-" + currentLevelRange.getEndValue());
        }
        else if (distance >= distanceLevelScales[distanceIndex].getDistanceThreshold())
        {
            distanceIndex++;
            currentLevelRange = distanceLevelScales[distanceIndex];
            levelRangeText.SetText("Lvl. " + currentLevelRange.getStartValue() + "-" + currentLevelRange.getEndValue());
        }
        else if (distance <= distanceLevelScales[distanceIndex - 1 != -1 ? distanceIndex - 1 : 0].getDistanceThreshold())
        {
            if(distanceIndex != 0)
            {
                distanceIndex--;
                currentLevelRange = distanceLevelScales[distanceIndex];
                levelRangeText.SetText("Lvl. " + currentLevelRange.getStartValue() + "-" + currentLevelRange.getEndValue());
            }
        }
    }

    IEnumerator spawnEnemy(float time)
    {
        while (true)
        {
            if (!pauseMenu.GetComponent<PauseMenu>().GamePaused)
            {
                float x = Random.Range(player.position.x - 10, player.position.x + 10);
                float z = Random.Range(player.position.z - 10, player.position.z + 10);

                string type = "none";
                if(quest.GetComponent<Quest>().currentQuest != null)
                {
                    type = quest.GetComponent<Quest>().currentQuest.getEnemyType();
                }
                
                //decide which type is needed for the quest
                switch (type)
                {
                    case "Legs":
                        helpEnemy = enemyLegs;
                        break;
                    case "Sharks":
                        helpEnemy = enemyShark;
                        break;
                    case "Slimes":
                        helpEnemy = enemySlime;
                        break;
                }

                int whichType;
                if (type == "none")
                {
                    whichType = Random.Range(0, 3);
                }
                else
                {
                    whichType = Random.Range(0, 4);
                }

                switch (whichType)
                {
                    case 0:
                        Instantiate(enemyLegs, new Vector3(x, -2, z), Quaternion.identity, GameObject.Find("Enemies").transform);
                        break;
                    case 1:
                        Instantiate(enemyShark, new Vector3(x, -2, z), Quaternion.identity, GameObject.Find("Enemies").transform);
                        break;
                    case 2:
                        Instantiate(enemySlime, new Vector3(x, -2, z), Quaternion.identity, GameObject.Find("Enemies").transform);
                        break;
                    case 3:
                        //added Value increasing the spawn rate of the type needed for current quest
                        Instantiate(helpEnemy, new Vector3(x, -2, z), Quaternion.identity, GameObject.Find("Enemies").transform);
                        break;
                }
            }
            yield return new WaitForSecondsRealtime(time);
        }
    }
}


public class DistanceLevelScale{
    int distanceThreshold;
    int startValue;
    int endValue;

    public DistanceLevelScale(int distanceThreshold, int startValue, int endValue)
    {
        this.distanceThreshold = distanceThreshold;
        this.startValue = startValue;
        this.endValue = endValue;
    }

    public int getDistanceThreshold()
    {
        return distanceThreshold;
    }

    public int getStartValue()
    {
        return startValue;
    }

    public int getEndValue()
    {
        return endValue;
    }
}