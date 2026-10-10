using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class PostProcessZone : MonoBehaviour
{
    public enum EffectType
    {
        BloomIntensity,
        VignetteIntensity,
        ChromaticAberrationIntensity,
        FilmGrainIntensity,
        PaniniDistance
    }

    [System.Serializable]
    public class Effect
    {
        [Header("Effect")]
        public EffectType type;

        [Tooltip("Value when player is at the center of the zone.")]
        public float centerValue = 1f;

        [Tooltip("How quickly this effect changes.")]
        public float changeSpeed = 3f;

        [HideInInspector]
        public float originalValue;

        [HideInInspector]
        public bool initialized;
    }

    [Header("Global Volume")]
    public Volume globalVolume;

    [Header("Player")]
    public Transform player;

    [Header("Zone")]
    public float radius = 5f;

    [Tooltip("How smoothly the zone responds to distance.")]
    public float distanceSmoothSpeed = 5f;

    [Header("Post Processing Effects")]
    public List<Effect> effects = new List<Effect>();

    private float influence = 0f;

    private Bloom bloom;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;
    private PaniniProjection panini;

    void Start()
    {
        FindEffects();
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // 0 = outside zone
        // 1 = center
        float targetInfluence =
            1f - Mathf.Clamp01(distance / radius);

        influence = Mathf.Lerp(
            influence,
            targetInfluence,
            Time.deltaTime * distanceSmoothSpeed
        );

        ApplyEffects();
    }

    void FindEffects()
    {
        if (globalVolume == null)
        {
            Debug.LogError(
                "PostProcessZone: Global Volume is not assigned!"
            );
            return;
        }

        if (globalVolume.profile == null)
        {
            Debug.LogError(
                "PostProcessZone: Global Volume has no Profile!"
            );
            return;
        }

        globalVolume.profile.TryGet(out bloom);
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out filmGrain);
        globalVolume.profile.TryGet(out panini);

        foreach (Effect effect in effects)
        {
            effect.originalValue = GetCurrentValue(effect.type);
            effect.initialized = true;
        }
    }

    float GetCurrentValue(EffectType type)
    {
        switch (type)
        {
            case EffectType.BloomIntensity:

                if (bloom != null)
                    return bloom.intensity.value;

                break;

            case EffectType.VignetteIntensity:

                if (vignette != null)
                    return vignette.intensity.value;

                break;

            case EffectType.ChromaticAberrationIntensity:

                if (chromaticAberration != null)
                    return chromaticAberration.intensity.value;

                break;

            case EffectType.FilmGrainIntensity:

                if (filmGrain != null)
                    return filmGrain.intensity.value;

                break;

            case EffectType.PaniniDistance:

                if (panini != null)
                    return panini.distance.value;

                break;
        }

        return 0f;
    }

    void SetValue(EffectType type, float value)
    {
        switch (type)
        {
            case EffectType.BloomIntensity:

                if (bloom != null)
                    bloom.intensity.value = value;

                break;

            case EffectType.VignetteIntensity:

                if (vignette != null)
                    vignette.intensity.value = value;

                break;

            case EffectType.ChromaticAberrationIntensity:

                if (chromaticAberration != null)
                    chromaticAberration.intensity.value = value;

                break;

            case EffectType.FilmGrainIntensity:

                if (filmGrain != null)
                    filmGrain.intensity.value = value;

                break;

            case EffectType.PaniniDistance:

                if (panini != null)
                    panini.distance.value = value;

                break;
        }
    }

    void ApplyEffects()
    {
        foreach (Effect effect in effects)
        {
            if (!effect.initialized)
                continue;

            float targetValue = Mathf.Lerp(
                effect.originalValue,
                effect.centerValue,
                influence
            );

            float currentValue = GetCurrentValue(effect.type);

            float newValue = Mathf.Lerp(
                currentValue,
                targetValue,
                Time.deltaTime * effect.changeSpeed
            );

            SetValue(effect.type, newValue);
        }
    }

    void OnDestroy()
    {
        RestoreOriginalValues();
    }

    void RestoreOriginalValues()
    {
        foreach (Effect effect in effects)
        {
            if (!effect.initialized)
                continue;

            SetValue(
                effect.type,
                effect.originalValue
            );
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            radius
        );

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(
            transform.position,
            0.15f
        );
    }
}