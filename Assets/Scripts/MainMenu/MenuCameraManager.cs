using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCameraManager : MonoBehaviour
{
    public Camera[] cameraHolder;
    public int currentCameraIndex = 0;
    public Image blinkImage; // For the blinking effect when switching cameras
    public float blinkSpeed = 2f; // How fast the blink happens
    public float switchInterval = 5f; // Time between camera switches

    private float switchTimer = 0f;
    private bool isBlinking = false;

    private void Awake()
    {
        Time.timeScale = 1f; // Haha I hate this bug

        // Get all child cameras and put them into the array
        cameraHolder = GetComponentsInChildren<Camera>(true);

        // Bugfix that disables all cameras except the first one
        for (int i = 0; i < cameraHolder.Length; i++)
        {
            cameraHolder[i].gameObject.SetActive(i == currentCameraIndex);
        }

        // Set blink image alpha to 0 at start
        if (blinkImage != null)
        {
            var c = blinkImage.color;
            c.a = 0f;
            blinkImage.color = c;
        }
    }

    private void Update()
    {
        if (isBlinking || cameraHolder.Length <= 1)
            return;

        switchTimer += Time.deltaTime;
        if (switchTimer >= switchInterval)
        {
            StartCoroutine(BlinkAndSwitch());
            switchTimer = 0f;
        }
    }

    private IEnumerator BlinkAndSwitch()
    {
        isBlinking = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * blinkSpeed;
            SetBlinkAlpha(Mathf.Clamp01(t));
            yield return null;
        }

        // Switch camera when blinks
        SwitchCamera();

        t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * blinkSpeed;
            SetBlinkAlpha(Mathf.Clamp01(t));
            yield return null;
        }

        SetBlinkAlpha(0f);
        isBlinking = false;
    }

    private void SetBlinkAlpha(float alpha)
    {
        if (blinkImage != null)
        {
            Color c = blinkImage.color;
            c.a = alpha;
            blinkImage.color = c;
        }
    }

    private void SwitchCamera()
    {
        cameraHolder[currentCameraIndex].gameObject.SetActive(false);
        currentCameraIndex = (currentCameraIndex + 1) % cameraHolder.Length;
        cameraHolder[currentCameraIndex].gameObject.SetActive(true);
    }
}
