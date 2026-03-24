using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunScript : MonoBehaviour
{
    [Header("AmmoCalculation")]
    [SerializeField] public int maxAmmo = 35; // The maximum ammo the player can carry
    [SerializeField] public int magCapacity = 7; // The maximum ammo the magazine can hold
    [SerializeField] private int ammoInMag; // The current ammo in the magazine
    [SerializeField] public int ammoInPocket; // The current ammo in the player's pocket

    [SerializeField] private Transform firePoint; // Not used yet
    [SerializeField] private float fireRate = 0.5f; // Not used yet
    [SerializeField] private float reloadSpeed = 1.5f;

    public string forAmmoUI;
    public TextMeshProUGUI ammoText;

    private bool isReloading = false;

    [Header("AimDownSight")]
    [SerializeField] private Transform currentPos;
    [SerializeField] private Transform adsPos;
    [SerializeField] private float adsSpeed = 10f;
    private bool onAds = false; // Player can't reload or shoot while aiming down sight

    private bool isAiming = false;

    private void Start()
    {
        ammoInMag = 7; // Initial ammo in the magazine
        ammoInPocket = 28; // Initial ammo in the player's pocket
        RefreshAmmoUI();
    }

    void Update()
    {
        // ADS control
        if (Input.GetKey(KeyCode.Mouse1))
            isAiming = true;
        else
            isAiming = false;

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

        if(Input.GetKeyDown(KeyCode.Mouse0) && ammoInMag > 0 && !isReloading)
        {
            //Fire Rate will be added later
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.R) && ammoInMag < magCapacity && ammoInPocket > 0 && !isReloading)
        {
            StartCoroutine(WaitForReload());
        }
    }

    public void Shoot()
    {
        ammoInMag--;
        Debug.Log("MagCount: " + ammoInMag + " PocketAmmo: " + ammoInPocket);
        RefreshAmmoUI();
    }

    public void Reload()
    {
        if (onAds)
            return; // Prevent reloading while aiming down sight

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

        int maxPocket = maxAmmo - magCapacity; //Currently 28

        if (ammoInPocket > maxPocket)
        {
            ammoInPocket = maxPocket; // Ensure that the total ammo (in pocket + in mag) does not exceed maxAmmo
            Debug.Log("Reached to maximum limit");
        }

        Debug.Log("MagCount: " + ammoInMag + " PocketAmmo: " + ammoInPocket);
        RefreshAmmoUI();
    }

    public void RefreshAmmoUI()
    {
        if(isReloading)
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
        yield return new WaitForSeconds(reloadSpeed);
        Reload();
        isReloading = false;
        RefreshAmmoUI();
        Debug.Log("Reloaded.");
    }
}
