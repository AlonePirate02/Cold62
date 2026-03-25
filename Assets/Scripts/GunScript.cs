using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunScript : MonoBehaviour
{
    [Header("AmmoCalculation")]
    [SerializeField] public int maxAmmo = 35;
    [SerializeField] public int magCapacity = 7;
    [SerializeField] private int ammoInMag;
    [SerializeField] public int ammoInPocket;

    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float reloadSpeed = 1.5f;

    public string forAmmoUI;
    public TextMeshProUGUI ammoText;

    private bool isReloading = false;

    [Header("AimDownSight")]
    [SerializeField] private Transform currentPos;
    [SerializeField] private Transform adsPos;
    [SerializeField] private float adsSpeed = 10f;
    private bool isAiming = false;

    private float nextFireTime = 0f; // Firerate control

    [Header("Animation")]
    [SerializeField] private Animator gunAnimator;
    private bool isAnimationPlaying = false;

    private void Start()
    {
        if (gunAnimator == null)
        {
            gunAnimator = GetComponentInChildren<Animator>(); // Main gun object is a child of this
        }

        ammoInMag = 7;
        ammoInPocket = 28;
        RefreshAmmoUI();
    }

    void Update()
    {
        // ADS control
        isAiming = Input.GetKey(KeyCode.Mouse1);

        if (isAiming)
        {
            transform.position = Vector3.Lerp(transform.position, adsPos.position, Time.deltaTime * adsSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, adsPos.rotation, Time.deltaTime * adsSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, currentPos.position, Time.deltaTime * adsSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, currentPos.rotation, Time.deltaTime * adsSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && ammoInMag > 0 && !isReloading && !isAnimationPlaying && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.R) && ammoInMag < magCapacity && ammoInPocket > 0 && !isReloading && !isAnimationPlaying)
        {
            StartCoroutine(WaitForReload());
        }
    }

    public void Shoot()
    {
        ammoInMag--;
        Debug.Log("MagCount: " + ammoInMag + " PocketAmmo: " + ammoInPocket);
        RefreshAmmoUI();

        if (gunAnimator != null)
        {
            isAnimationPlaying = true;
            gunAnimator.SetTrigger("Recoil");
        }
    }

    public void Reload()
    {
        int neededAmmo = magCapacity - ammoInMag;
        int ammoToReload = Mathf.Min(neededAmmo, ammoInPocket);

        ammoInMag += ammoToReload;
        ammoInPocket -= ammoToReload;

        Debug.Log("MagCount: " + ammoInMag + " PocketAmmo: " + ammoInPocket);
        RefreshAmmoUI();
    }

    public void AddAmmo(int ammoToAdd)
    {
        ammoInPocket += ammoToAdd;

        int maxPocket = maxAmmo - magCapacity;

        if (ammoInPocket > maxPocket)
        {
            ammoInPocket = maxPocket;
            Debug.Log("Reached to maximum limit");
        }

        Debug.Log("MagCount: " + ammoInMag + " PocketAmmo: " + ammoInPocket);
        RefreshAmmoUI();
    }

    public void RefreshAmmoUI()
    {
        if (isReloading)
        {
            forAmmoUI = "Reloading...";
            ammoText.text = forAmmoUI;
        }
        else
        {
            forAmmoUI = ammoInMag.ToString() + " / " + ammoInPocket.ToString();
            ammoText.text = forAmmoUI;
        }
    }

    private IEnumerator WaitForReload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        RefreshAmmoUI();

        if (gunAnimator != null)
        {
            isAnimationPlaying = true;
            gunAnimator.SetTrigger("Reload");
        }

        yield return new WaitForSeconds(reloadSpeed);

        Reload();
        isReloading = false;
        RefreshAmmoUI();
        Debug.Log("Reloaded.");
    }

    public void OnAnimationEnd() // Call at the end of the animation
    {
        isAnimationPlaying = false;
    }
}