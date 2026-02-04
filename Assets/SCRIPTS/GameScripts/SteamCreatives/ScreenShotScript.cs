using UnityEngine;

public class ScreenShotScript : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            string fileName = "screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(fileName);
        }
    }
}
