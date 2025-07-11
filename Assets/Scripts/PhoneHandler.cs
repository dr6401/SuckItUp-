using UnityEngine;

public class PhoneHandler : MonoBehaviour
{
    [SerializeField] private GameObject phoneHandler;

    public Animator phoneAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void PullUpPhone()
    {
        phoneAnimator.Play("PhonePullUp");
    }
    
    public void PullDownPhone()
    {
        phoneAnimator.Play("PhonePullDown");
    }
}
