using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolume_Controller : MonoBehaviour
{
    // References
    private Transform player;
    private Volume globalVolume;

    // Settings
    public float minDistance = 5f; // Minimum distance for full effect
    public float maxDistance = 15f; // Maximum distance for no effect

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
        player = GameObject.FindGameObjectWithTag("Player").transform;
        globalVolume = GameObject.Find("Global Volume").GetComponent<Volume>();

        globalVolume.profile.TryGet(out filmGrain);
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out lensDistortion);

        oldFilmGrainIntensity = filmGrain.intensity.value;
        oldVignetteIntensity = vignette.intensity.value;

        oldLensDistortionIntensity = lensDistortion.intensity.value;
        oldLensDistortionScale = lensDistortion.scale.value;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);


        float t = Mathf.InverseLerp(maxDistance, minDistance, distance);

        float film = Mathf.SmoothStep(oldFilmGrainIntensity, 1f, t);
        float vignetteVal = Mathf.SmoothStep(oldVignetteIntensity, 0.7f, t * 0.8f);
        float lens = Mathf.SmoothStep(oldLensDistortionIntensity, 0.7f, Mathf.Pow(t, 1.5f));
        float distortionScale = Mathf.SmoothStep(oldLensDistortionScale, 0.5f, t * 1.2f);


        filmGrain.intensity.value = film;

        vignette.intensity.value = vignetteVal / 3;
        vignette.color.value = vignetteChange;

        lensDistortion.intensity.value = lens;
        lensDistortion.scale.value = distortionScale;
    }

    private void OnDisable()
    {
        filmGrain.intensity.value = oldFilmGrainIntensity;
        vignette.intensity.value = oldVignetteIntensity;
        lensDistortion.intensity.value = oldLensDistortionIntensity;
        lensDistortion.scale.value = oldLensDistortionScale;
        vignette.color.value = Color.black;
    }
}
