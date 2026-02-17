using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class DustCuller : MonoBehaviour
{
    private bool isPlaying = false;
    [SerializeField] private VisualEffect vFX;
    [SerializeField] private Transform target;
    [SerializeField] private float maxDistance = 1000f;
    private MeshRenderer meshRenderer;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        vFX = GetComponentInChildren<VisualEffect>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }
        
        float distanceToPlayer = (transform.position - target.transform.position).sqrMagnitude;
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

    private void SetMeshRendererActive()
    {
        meshRenderer.enabled = true;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerKilledAllEnemies += SetMeshRendererActive;
    }
    private void OnDisable()
    {
        GameEvents.OnPlayerKilledAllEnemies -= SetMeshRendererActive;
    }
}