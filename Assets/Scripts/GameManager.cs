using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private float objectiveTextDuration = 7.5f;
    [SerializeField] private GameObject objectiveText;
    [SerializeField] private GameObject keyBindingsText;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WeaponHandler weaponHandler;
    private bool keyBindingTextToggled = false;
    public bool gameNotOver = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DisableText());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameNotOver){
            keyBindingTextToggled = !keyBindingTextToggled;
            keyBindingsText.SetActive(keyBindingTextToggled);

            objectiveText.SetActive(false);

            Time.timeScale = keyBindingTextToggled ? 0f : 1f;
            playerMovement.inputBlocked = keyBindingTextToggled;
            weaponHandler.inputBlocked = keyBindingTextToggled;
        }

        if (!gameNotOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Level1");
            }
        }
    }

    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(objectiveTextDuration);
        objectiveText.SetActive(false);

    }
}
