using UnityEngine;
using MoreMountains.Feedbacks;
public class CanvasUIManager : MonoBehaviour
{
    [SerializeField] private MMFeedbacks ammoUpdateFeedback;

    public static CanvasUIManager instance;
    [SerializeField] private bool keepItPersistent = true;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        if (keepItPersistent)
        {
            DontDestroyOnLoad(gameObject);   
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlaySuckDustFeedback()
    {
        ammoUpdateFeedback.PlayFeedbacks();
    }
    
    private void OnEnable()
    {
        GameEvents.OnAmmoUpdate += PlaySuckDustFeedback;
    }
    
    private void OnDisable()
    {
        GameEvents.OnAmmoUpdate -= PlaySuckDustFeedback;
    }
}
