using UnityEngine;
using TMPro;
using System.Collections;
public class GameManager : MonoBehaviour
{
    private float objectiveTextDuration = 7.5f;
    [SerializeField] private GameObject objectiveText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DisableText());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(objectiveTextDuration);
        objectiveText.SetActive(false);

    }
}
