using UnityEngine;
using UnityEngine.VFX;

public class AutoDestroyVFX : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private VisualEffect vfx;
    void Awake()
    {
        vfx = GetComponentInChildren<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (vfx.aliveParticleCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
