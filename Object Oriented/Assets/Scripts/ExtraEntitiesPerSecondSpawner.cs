using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ExtraEntitiesPerSecondSpawner : Spawner
{
    bool isStarted = false;
    bool hasCompleted = false;
    float timeToStartTest = 3f;

    public float testTime = 60f; //Needs to be set on init
    public float entitiesToSpawnPerSecond = 60f;

    [SerializeField] string fileName = "EEPS_";
    [SerializeField] Pathfinding pf;

    float spawnTime;
    float timeSinceLastSpawn;

    public Movement entityToSpawn;





    // Start is called before the first frame update
    void Start()
    {
        spawnTime = 1f / entitiesToSpawnPerSecond;

        testQueuer = TestQueuer.testQueuer;
        
        stats = statsObject.GetComponent<GameStats>();
    }

    // Update is called once per frame
    void Update()
    {
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

        if((timeSinceLastSpawn -= time) <= 0)
        {
            
            var go = Instantiate(entityToSpawn, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
            go.Setup(pf);
            stats.entityCount ++;
            timeSinceLastSpawn = spawnTime;
        }
    }
}
