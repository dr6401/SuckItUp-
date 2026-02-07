using MoreMountains.Feedbacks;
using UnityEngine;

public class MmfRotationTestController : MonoBehaviour
{
    [SerializeField] private MMFeedbacks shakeFeedback1;
    [SerializeField] private MMFeedbacks shakeFeedback2;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            shakeFeedback2.StopFeedbacks();
            shakeFeedback1?.PlayFeedbacks();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            shakeFeedback1.StopFeedbacks();
            shakeFeedback2?.PlayFeedbacks();
        }
    }
}
