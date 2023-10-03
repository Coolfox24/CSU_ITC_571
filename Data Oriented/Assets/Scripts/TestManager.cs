using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{
    bool isSetup = false;
    public bool isAnimated = false;

    bool isStarted = false;
    bool hasCompleted = false;
    float timeToStartTest = 3f;

    public float testTime = 60f; //Needs to be set on init

    [SerializeField] string fileName = "DO_EEPS_";

    [SerializeField] GameObject statsObject;
    [SerializeField] TMP_Text infoText;
    [SerializeField] GameObject endButton;

    GameStats stats;


    //Test Params
    public int entitiesToSpawnPerSecond = 120;
    public bool spawnAllAtOnce = false;
    public int maxSpawnCount = 0;

    private TestQueuer testQueuer;

    // Start is called before the first frame update
    void Start()
    {

        stats = statsObject.GetComponent<GameStats>();

        //Get Test Queuer Reference
        testQueuer = TestQueuer.testQueuer;

        if(!spawnAllAtOnce)
        {
            DisableSystems();
        }
    }

    bool Setup()
    {
        EntityManager _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entitySpawnerQuery = _entityManager.CreateEntityQuery(new ComponentType[] { typeof(EntitySpawnerComponent) });
        entitySpawnerQuery.TryGetSingletonRW<EntitySpawnerComponent>(out RefRW<EntitySpawnerComponent> entitySpawner);

        if(!entitySpawner.IsValid)
        {
            return false;
        }

        entitySpawner.ValueRW.entitySpawnTimer = 1f / entitiesToSpawnPerSecond;
        entitySpawner.ValueRW.spawnAllAtOnce = spawnAllAtOnce;
        entitySpawner.ValueRW.maxSpawnCount = maxSpawnCount;
        return true;
    }

    private void DisableSystems()
    {
        //Disable our systems used for pathfinding
        var teleportSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<TeleportISystem>();
        var movementSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<MovementISystem>();
        var spawnSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<EntitySpawnerSystem>();


        World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(teleportSystem).Enabled = false;
        World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(movementSystem).Enabled = false;
        World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(spawnSystem).Enabled = false;
    }

    private void EnableSystems()
    {
        //Disable our systems used for pathfinding
        var teleportSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<TeleportISystem>();
        var spawnSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<EntitySpawnerSystem>();
        var movementSystem =  World.DefaultGameObjectInjectionWorld.GetExistingSystem<MovementISystem>();

        //Enable Correct movement system
        if(!isAnimated)
        {
            World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(teleportSystem).Enabled = true;
        }
        else
        {
            World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(movementSystem).Enabled = true;
        }

        World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(spawnSystem).Enabled = true;
    }

    // Update is called once per frame
    void Update()
    {      
        
        if(!isSetup)
        {
            isSetup = Setup();
            if(!isSetup)
            {
                return;
            }
            else
            {
                DisableSystems();
            }
            if(spawnAllAtOnce)
            {

                //Turn on spawning system
                var spawnSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<EntitySpawnerSystem>();
                World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(spawnSystem).Enabled = true;
            }
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

                EnableSystems();
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

            DisableSystems();

            StartCoroutine(SetupTests());
        }
    }

    private IEnumerator SetupTests()
    {
        if(!(testQueuer == null))
        {
            testQueuer.testsRun ++;
            if(testQueuer.testsRun  < 3)
            {
                infoText.text = "TESTS CONCLUDED \nQueueing up new test in 3s";
                yield return new WaitForSeconds(3);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                
            }
            else
            {
                if(TestScheduler.testScheduler != null)
                {
                    if(TestScheduler.testScheduler.nextTestID > 0)
                    {
                        //Load Main Scene
                        Debug.Log("Returning to menu");
                        yield return new WaitForSeconds(3);
                        SceneManager.LoadScene(0);
                    }
                }

                //Show button to return to main menu

                infoText.text = "TESTS CONCLUDED\nTest Data Saved at: " + stats.fileNameExt;
                endButton.SetActive(true);            
                gameObject.SetActive(false);
                Destroy(testQueuer.gameObject);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            //Show button to return to main menu

            infoText.text = "TESTS CONCLUDED\nTest Data Saved at: " + stats.fileNameExt;
            endButton.SetActive(true);            
            gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
        }
    }
}