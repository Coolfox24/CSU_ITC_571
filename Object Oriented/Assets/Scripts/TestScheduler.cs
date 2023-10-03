using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScheduler : MonoBehaviour
{

    public int nextTestID = 0;
    public string startTestTime;
    public static TestScheduler testScheduler;

    void Start()
    {
        startTestTime = System.DateTime.Now.ToString("yyyyMMdd'T'HHmmss");
        if(testScheduler == null)
        {
            testScheduler = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
