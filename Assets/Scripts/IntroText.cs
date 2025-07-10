using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class IntroText : MonoBehaviour
{
    [SerializeField] List<string> sentanceList;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject continueTextContainer;
    private int currentText = 0;
    
    void Start()
    {
        text.text = sentanceList.First();
        StartCoroutine(DelayContinueText());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UpdateText();
        }
    }


    public void UpdateText()
    {
        currentText++;
        if (currentText == sentanceList.Count)
        {
            SceneManager.LoadScene("Tutorial");
        }
        else
        {
            text.text = sentanceList.ElementAt(currentText);
        }
    }

    private IEnumerator DelayContinueText()
    {
        yield return new WaitForSeconds(2f);
        continueTextContainer.SetActive(true);
    }
}
