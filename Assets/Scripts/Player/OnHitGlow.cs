using System.Collections;
using UnityEngine;

public class OnHitGlow : MonoBehaviour
{
    private Material mat;

    [Header("Glow Settings")]
    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private float glowDuration = 0.2f;

    [Header("Alpha Clip Settings")]
    [SerializeField] private float normalClip = 0.5f; // default value
    [SerializeField] private float attackClip = 0f;   // fully disable clipping

    private void Awake()
    {
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
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", glowColor * glowIntensity);

        if (mat.HasProperty("_Cutoff"))
            mat.SetFloat("_Cutoff", attackClip);

        yield return new WaitForSeconds(glowDuration);

        mat.SetColor("_EmissionColor", Color.black);

        if (mat.HasProperty("_Cutoff"))
            mat.SetFloat("_Cutoff", normalClip);
    }
}
