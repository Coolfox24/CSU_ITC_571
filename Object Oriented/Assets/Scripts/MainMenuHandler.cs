using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
 
    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        if(TestScheduler.testScheduler == null)
        {
            return;
        }

        if (TestScheduler.testScheduler.nextTestID > 0)
        {
            if(TestScheduler.testScheduler.nextTestID < 14)
            {
                StartCoroutine(StartNextTest());
            }
            else
            {
                titleText.text = "All Tests Completed";
            }
        }
    }

    private IEnumerator StartNextTest()
    {
        titleText.text = "Starting Next Test in 3s";

        yield return new WaitForSeconds(3f);

        TestScheduler.testScheduler.nextTestID ++;

        SceneManager.LoadScene(TestScheduler.testScheduler.nextTestID);

    }

    public void StartTestScheduler()
    {
        StartCoroutine(StartNextTest());
    }
}
