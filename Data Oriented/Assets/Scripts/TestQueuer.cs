using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestQueuer : MonoBehaviour
{
    public int testsRun = 0;

    public static TestQueuer testQueuer;

    void Start()
    {

        if(testQueuer == null)
        {
            testQueuer = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
