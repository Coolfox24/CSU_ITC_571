using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticSpawner : Spawner
{

    bool isStarted = false;
    bool hasCompleted = false;
    float timeToStartTest = 3f;

    public float testTime = 60f; //Needs to be set on init
    public int entityCount = 10;

    [SerializeField] string fileName = "STAT_";
    [SerializeField] Pathfinding pf;

    public Movement entityToSpawn;

    bool firstFrame = true;

    // Start is called before the first frame update
    void Start()
    {
        testQueuer = TestQueuer.testQueuer;
        
        stats = statsObject.GetComponent<GameStats>();

        for(int i = 0; i < entityCount; i++)
        {
            var go = Instantiate(entityToSpawn, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
            go.Setup(pf);
            stats.entityCount ++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(firstFrame)
        {
            firstFrame = false;
            return;
        }

        float time = Time.deltaTime;

        if(!isStarted)
        {
            //Decrease clock count here
            timeToStartTest -= time;
            infoText.text = "TEST STARTING IN :" + Mathf.RoundToInt(timeToStartTest);
            if(timeToStartTest <= 0)
            {
                isStarted = true;
                statsObject.SetActive(true);

                if(TestScheduler.testScheduler == null)
                {
                    stats.fileNameExt = fileName + testTime;
                }
                else
                {
                    stats.fileNameExt = TestScheduler.testScheduler.startTestTime + "_" + fileName + testTime;
                }

                infoText.text = "CONDUCTING TEST";
            }
            else
            {
                return;
            }
        }

        //Check if test is concluded
        if((testTime -= time) < 0)
        {
            if(hasCompleted)
            {
                return;
            }
            hasCompleted = true;

            //Write test stats to file
            stats.SaveTestData();
            statsObject.SetActive(false);

            //Show button to return to main menu
            StartCoroutine(SetupTests());

        }
    }
}
