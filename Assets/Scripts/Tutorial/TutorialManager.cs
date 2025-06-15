using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<Tutorial> allTutorials = new List<Tutorial>();
    private const string tutorialString = "isTutorialFinished";
    private const string tutorialID = "TUTORIAL_ID_";
    private static TutorialManager instance;



    private int currentTutorialId = 0;

    public int CurrentTutorialID
    {
        get
        {
            if (!PlayerPrefs.HasKey(tutorialID))
            {
                currentTutorialId = 1;
                PlayerPrefs.SetInt(tutorialID, currentTutorialId);
                return currentTutorialId;
            }
            else
            {
                currentTutorialId = PlayerPrefs.GetInt(tutorialID);
                return currentTutorialId;
            }
        }
        set
        {
            currentTutorialId = value;
            PlayerPrefs.SetInt(tutorialID, currentTutorialId);
        }
    }
    private bool isTutorialFinished = false;

    private void Awake()
    {
        instance = this;
        isTutorialFinished = PlayerPrefs.GetInt(tutorialString, 0) == 0 ? false : true;
    }

    IEnumerator Start()
    {
        yield return null;
        StartTutorial();
    }

    public static TutorialManager GetInstance()
    {
        return instance;
    }

    public void StartTutorial(bool applyDelay = false, float delay = 0f)
    {
        if (applyDelay)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                allTutorials[CurrentTutorialID - 1].OnTutorialStart();
            });
        }

        else 
        {
            allTutorials[CurrentTutorialID - 1].OnTutorialStart();
        }
    
    }

    public void StartTutorial(int id)
    {
        allTutorials[id - 1].OnTutorialStart();
    }

    public void EndTutorial(int id, bool applyDelay = false)
    {
        allTutorials[id - 1].OnTutorialEnd();

        if (applyDelay)
        {
            DOVirtual.DelayedCall(1.25f, () =>
            {
                EndTutioralBehavior(id);
            });
        }
        else
        {
            EndTutioralBehavior(id);
        }

    }

    private void EndTutioralBehavior(int id)
    {
        CurrentTutorialID++;

        if (CurrentTutorialID - 1 >= allTutorials.Count)
        {
            //End of Tutorial

            // #if UNITY_EDITOR
            //             Debug.LogError("Tutorial Finished");
            // #endif
            PlayerPrefs.SetInt(tutorialString, 1);

            isTutorialFinished = true;

            return;
        }

        if(CurrentTutorialID == 3 && GameManager.GetInstance().GetPlayerData().currentLevelId != 3)
        {
            return;
        }

        if(CurrentTutorialID == 4 && GameManager.GetInstance().GetPlayerData().currentLevelId != 6)
        {
            return;
        }

        if(CurrentTutorialID == 5 && GameManager.GetInstance().GetPlayerData().currentLevelId != 10)
        {
            return;
        }

        StartTutorial();
    }

    public bool IsTutorialFinished()
    {
        return isTutorialFinished;
    }
    public bool IsTutorialEnded(int id) => CurrentTutorialID > id;
}
