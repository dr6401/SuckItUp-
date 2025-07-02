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
        
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToPlayer > maxDistance)
        {
            vFX.Stop();
            Debug.Log("Vfx " + name + " is STOPped! since distance to player is " + distanceToPlayer);
            isPlaying = false;
        }
        else
        {
            if (!isPlaying)
            {
                StartPlayingVFX();
                isPlaying = true;
            }
        }
    }

    void StartPlayingVFX()
    {
        vFX.Play();
        Debug.Log("Vfx " + name + " is PLAYing!");
    }
}
