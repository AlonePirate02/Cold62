using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeManager : MonoBehaviour
{
    public float minDistance = 5f;
    public float maxDistance = 15f;

    // Not array so this works after spawning new enemies
    private List<Ghost_AI> enemies = new List<Ghost_AI>();

    private Volume globalVolume;
    private FilmGrain filmGrain;
    private Vignette vignette;
    private LensDistortion lensDistortion;

    private float oldFilmGrainIntensity;
    private float oldVignetteIntensity;
    private float oldLensDistortionIntensity;
    private float oldLensDistortionScale;

    public Color vignetteChange;

    void Start()
    {
        RefreshEnemyList();

        globalVolume = GetComponent<Volume>();

        globalVolume.profile.TryGet(out filmGrain);
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out lensDistortion);

        oldFilmGrainIntensity = filmGrain.intensity.value;
        oldVignetteIntensity = vignette.intensity.value;
        oldLensDistortionIntensity = lensDistortion.intensity.value;
        oldLensDistortionScale = lensDistortion.scale.value;
    }

    // Find active enemies. Can be called after enemy spawn/despawn or QTE success to update the list.
    public void RefreshEnemyList()
    {
        enemies.Clear();
        Ghost_AI[] foundEnemies = FindObjectsOfType<Ghost_AI>();
        foreach (Ghost_AI enemy in foundEnemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemies.Add(enemy);
            }
        }
    }

    void Update()
    {
        float closestDistance = Mathf.Infinity;
        bool hasActiveEnemy = false;

        foreach (Ghost_AI e in enemies)
        {
            // If enemy is null (destroyed) or inactive, skip it
            if (e == null || !e.gameObject.activeInHierarchy) continue;

            hasActiveEnemy = true;
            float dist = e.DistanceToPlayer();

            if (dist < closestDistance)
                closestDistance = dist;
        }

        // Enemy check
        if (!hasActiveEnemy)
        {
            ResetSettings();
            return;
        }

        // Calculate
        float t = Mathf.InverseLerp(maxDistance, minDistance, closestDistance);

        float film = Mathf.SmoothStep(oldFilmGrainIntensity, 1f, t);
        float vignetteVal = Mathf.SmoothStep(oldVignetteIntensity, 0.7f, t * 0.8f);
        float lens = Mathf.SmoothStep(oldLensDistortionIntensity, 0.7f, Mathf.Pow(t, 1.5f));
        float distortionScale = Mathf.SmoothStep(oldLensDistortionScale, 0.5f, t);

        filmGrain.intensity.value = film;

        vignette.intensity.value = vignetteVal / 3;
        vignette.color.value = vignetteChange;

        lensDistortion.intensity.value = lens;
        lensDistortion.scale.value = distortionScale;
    }

    // Reset settings after QTE success or when no active enemies remain
    public void ResetSettings()
    {
        if (filmGrain != null) filmGrain.intensity.value = oldFilmGrainIntensity;
        if (vignette != null)
        {
            vignette.intensity.value = oldVignetteIntensity;
            vignette.color.value = Color.black;
        }
        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = oldLensDistortionIntensity;
            lensDistortion.scale.value = oldLensDistortionScale;
        }
    }
}