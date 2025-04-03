using UnityEngine;
using UnityEngine.VFX;

public class AutoDestroyVFX : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private VisualEffect vfx;
    [SerializeField] private float vfxDuration = 4.5f;
    private float timer = 0;
    void Awake()
    {
        vfx = GetComponentInChildren<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if ((vfx.aliveParticleCount == 0) || !vfx.HasAnySystemAwake())// || timer >= vfxDuration)
        {
            Destroy(gameObject);
        }

        //Debug.Log("timer: " + timer + ", vfxDuration: " + vfxDuration);
    }
}
