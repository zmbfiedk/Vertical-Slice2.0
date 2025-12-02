using System.Collections;
using UnityEngine;

public class OnHitGlow : MonoBehaviour
{
    private Material mat;
    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float glowDuration = 0.2f;

    private void Awake()
    {
        // Get the renderer's material (creates its own instance)
        mat = GetComponent<Renderer>().material;
    }

    private void OnEnable()
    {
        BaseCombat.OnHit += StartGlow;
    }

    private void OnDisable()
    {
        BaseCombat.OnHit -= StartGlow;
    }

    private void StartGlow()
    {
        StartCoroutine(GlowRoutine());
    }

    private IEnumerator GlowRoutine()
    {
        // Enable emission on the material
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", glowColor * glowIntensity);

        yield return new WaitForSeconds(glowDuration);

        // Turn emission off after the flash
        mat.SetColor("_EmissionColor", Color.black);
    }
}
