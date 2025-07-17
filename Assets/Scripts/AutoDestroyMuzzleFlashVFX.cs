using UnityEngine;
using UnityEngine.VFX;

public class AutoDestroyMuzzleFlashVFX : MonoBehaviour
{
    // REASON FOR THIS SCRIPT:
    // for some reason if AutoDestroyVFX script is attached to the muzzleEffect when shooting, it plays when switching from vacuum to riffle.

    private VisualEffect vfx;
    void Awake()
    {
        vfx = GetComponentInChildren<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (vfx.aliveParticleCount == 0 || !vfx.HasAnySystemAwake())
        {
            Destroy(gameObject);
        }

    }
}