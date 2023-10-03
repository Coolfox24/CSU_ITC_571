using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    protected TestQueuer testQueuer;
    
    [SerializeField] protected GameObject statsObject;

    protected GameStats stats;
    [SerializeField] protected TMP_Text infoText;
    [SerializeField] protected GameObject endButton;

    protected IEnumerator SetupTests()
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

