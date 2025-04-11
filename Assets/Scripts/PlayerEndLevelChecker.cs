using System;
using TMPro;
using UnityEngine;

public class PlayerEndLevelChecker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private GameObject objectiveText;
    void Start()
    {
        if (tutorialManager == null)
        {
            tutorialManager = GameObject.FindAnyObjectByType<TutorialManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialManager.aliveDustParticles <= 0)
        {
            objectiveText.GetComponent<TMP_Text>().text = "Good Job! You cleaned the whole place Up";
            objectiveText.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EndLevelArea"))
        {
            if (tutorialManager.aliveDustParticles <= 0)
            {
                objectiveText.GetComponent<TMP_Text>().text = "Congratulations on completing the tutorial!";
                objectiveText.SetActive(true);
            }
            else
            {
                objectiveText.GetComponent<TMP_Text>().text = "There are still some dusty places you haven't checked!";
                objectiveText.SetActive(true);
            }
        }
    }
}
