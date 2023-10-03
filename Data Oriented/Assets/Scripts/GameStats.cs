using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using Unity.Entities;

public class GameStats : MonoBehaviour
{
    private class GameStatEntry
    {
        public int frameNum;
        public float deltaTime;
        public float systemMemory;
        public float mainThreadTime;
        public float gpuFrameTime;
        public int entityCount;
        public float testTime;

        private static int frameNumG = 0;

        public GameStatEntry(float deltaTime, float systemMemory, float mainThreadTime, float gpuFrameTime, int entityCount, float testTime)
        {
            this.deltaTime = deltaTime;
            this.systemMemory = systemMemory;
            this.mainThreadTime = mainThreadTime;
            this.gpuFrameTime = gpuFrameTime;
            this.entityCount = entityCount;
            this.testTime = testTime;
            frameNum = frameNumG;
            frameNumG++;
        }

        public string GeCSVStringStats()
        {
            return frameNum + "," + deltaTime + "," + systemMemory + "," + mainThreadTime + "," + gpuFrameTime + "," + entityCount + "," + testTime; 
        }
    }

    //List to store all records
    List<GameStatEntry> stats;

    //Profilers
    //System Memory
    ProfilerRecorder systemMemory;

    //CPU Thread Time
    ProfilerRecorder mainThreadTime;

    //RENDER Thread Time
    ProfilerRecorder gpuFrameTime;

    public string fileNameExt;

    private float testTime = 0;

    EntityQuery query;

    void OnEnable()
    {
        gpuFrameTime =  ProfilerRecorder.StartNew(ProfilerCategory.Internal, "GPU Frame Time"); //Discard If 0 as we're faster than frame time
        mainThreadTime = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread");
        systemMemory = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");

        stats = new();

        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        query = em.CreateEntityQuery(ComponentType.ReadOnly<PathIndexComponent>());
    }

    void OnDisable()
    {
        gpuFrameTime.Dispose();
        mainThreadTime.Dispose();
        systemMemory.Dispose();
    }

    public void SaveTestData()
    {
        //Log to File here

        int testNum = TestQueuer.testQueuer.testsRun;

        string file = Application.persistentDataPath + "/"+ this.fileNameExt + "_" + testNum + ".csv";


        StreamWriter writer = new StreamWriter(file, true);
        writer.WriteLine("frameNum,deltaTime,systemMemory,mainThreadTime,gpuFrameTime,entityCount,testTime");

        for(int i = 0; i < stats.Count; i++)
        {
            writer.WriteLine(stats[i].GeCSVStringStats());
        }

        writer.Close();
    }



    void Update()
    {

        GameStatEntry entry = new GameStatEntry(
            Time.deltaTime,
            systemMemory.LastValue / (1024 * 1024),
            mainThreadTime.LastValue * (1e-6f),
            gpuFrameTime.LastValue * (1e-6f),
            query.CalculateEntityCount(),
            testTime += Time.deltaTime
        );

        stats.Add(entry);
    }
}
