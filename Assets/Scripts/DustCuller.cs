using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class DustCuller : MonoBehaviour
{
    private bool isPlaying = false;
    [SerializeField] private VisualEffect vFX;
    [SerializeField] private Transform target;
    [SerializeField] private float maxDistance = 20f;
    void Start()
    {
        vFX = GetComponentInChildren<VisualEffect>();
        Debug.Log("In start function");
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            Debug.Log("assigned " + target.name + " to target");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Debug.Log("Dust didnt find player to follow");
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToPlayer > maxDistance)
        {
            vFX.Stop();
            isPlaying = false;
        }
        else if (!isPlaying)
        {
            StartPlayingVFX();
            isPlaying = true;
        }
    }
    void StartPlayingVFX()
    {
        vFX.Play();
    }
}
