using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private float objectiveTextDuration = 7.5f;
    [SerializeField] private GameObject objectiveText;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WeaponHandler weaponHandler;
    private bool keyBindingTextToggled = false;
    public bool gameNotOver = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DisableText());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameNotOver){
            keyBindingTextToggled = !keyBindingTextToggled;
            settingsCanvas.SetActive(keyBindingTextToggled);

            objectiveText.SetActive(false);

            //Enabling/Disabling the cursor if the game is paused
            if (keyBindingTextToggled)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

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
