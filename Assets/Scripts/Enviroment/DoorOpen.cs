using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorOpen : MonoBehaviour
{
    [SerializeField] private float openTime = 40f;
    [SerializeField] private float openAngle = 90f;
    public bool isOpening = false;
    private Vector3 closedRotation;
    private Vector3 openRotation;
    private Vector3 currentRotation;
    private Vector3 rotationVelocity = Vector3.zero;
    private bool isMoving = false;

    public AudioClip metalDoorOpenSound;
    public AudioClip metalDoorCloseSound;
    public AudioClip woodDoorOpenSound;
    public AudioClip woodDoorCloseSound;

    private AudioSource audioSource;
    private AudioClip openSound;
    private AudioClip closeSound;

    void Start()
    {
        closedRotation = transform.eulerAngles;
        currentRotation = closedRotation;

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        SetDoorSounds();
    }

    void Update()
    {
        if (isMoving)
        {
            if (isOpening)
            {
                currentRotation = Vector3.SmoothDamp(currentRotation, openRotation, ref rotationVelocity, openTime * Time.deltaTime);
                if (Vector3.Distance(currentRotation, openRotation) < 0.01f)
                {
                    currentRotation = openRotation;
                    isMoving = false;
                }
            }
            else
            {
                currentRotation = Vector3.SmoothDamp(currentRotation, closedRotation, ref rotationVelocity, openTime * Time.deltaTime);
                if (Vector3.Distance(currentRotation, closedRotation) < 0.01f)
                {
                    currentRotation = closedRotation;
                    isMoving = false;
                }
            }
            transform.eulerAngles = currentRotation;
        }
    }

    public void ToggleDoor()
    {
        if (!isMoving)
        {
            isOpening = !isOpening;
            isMoving = true;
            PlaySound();
        }
    }

    private void SetDoorSounds()
    {
        switch (tag)
        {
            case "SecurityDoor":
                openSound = metalDoorOpenSound;
                closeSound = metalDoorCloseSound;
                openRotation = transform.eulerAngles + Vector3.up * openAngle;
                break;
            case "Door":
                openSound = woodDoorOpenSound;
                closeSound = woodDoorCloseSound;
                openRotation = transform.eulerAngles + Vector3.up * openAngle;
                break;
            case "Closet":
                openSound = woodDoorOpenSound;
                closeSound = woodDoorCloseSound;
                openRotation = transform.eulerAngles + Vector3.up * openAngle;
                break;
            default:
                openSound = null;
                closeSound = null;
                openRotation = transform.eulerAngles + Vector3.up * openAngle;
                break;
        }
    }

    private void PlaySound()
    {
        if (audioSource != null)
        {
            audioSource.clip = isOpening ? openSound : closeSound;
            audioSource.Play();
        }
    }
}
