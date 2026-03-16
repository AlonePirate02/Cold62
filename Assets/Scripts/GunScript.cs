using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunScript : MonoBehaviour
{

    [SerializeField] private int maxAmmo = 35; // The maximum ammo the player can carry
    [SerializeField] private int magCapacity = 7; // The maximum ammo the magazine can hold
    [SerializeField] private int ammoInMag; // The current ammo in the magazine
    [SerializeField] private int ammoInPocket; // The current ammo in the player's pocket
    private int fillMag; // The amount of ammo needed to fill the magazine

    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float reloadSpeed = 1.5f;

    public string forAmmoUI;
    public TextMeshProUGUI ammoText;


    private void Start()
    {
        ammoInMag = 7; // Initial ammo in the magazine
        ammoInPocket = 28; // Initial ammo in the player's pocket
        RefreshAmmoUI();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && ammoInMag > 0)
        {
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.R) && ammoInMag < 7 && ammoInPocket > 0)
        {
            Reload();
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
        fillMag = magCapacity - ammoInMag; // Calculate how much ammo is needed to fill the magazine

        if (ammoInPocket < 7)
        {
            ammoInMag += ammoInPocket;
            ammoInPocket = 0;
            Debug.Log("MagCount: " + ammoInMag + " PocketAmmo: " + ammoInPocket);
        }
        else
        {
            ammoInMag = magCapacity;
            ammoInPocket -= fillMag;
            Debug.Log("MagCount: " + ammoInMag + " PocketAmmo: " + ammoInPocket);
        }

        RefreshAmmoUI();
    }

    public void AddAmmo(int ammoToAdd)
    {
        ammoInPocket += ammoToAdd;
        
        if (ammoInPocket > maxAmmo)
        {
            ammoInPocket = 35;
            Debug.Log("Reached to maximum limit");
        }

        Debug.Log("MagCount: " + ammoInMag + " PocketAmmo: " + ammoInPocket);
        RefreshAmmoUI();
    }

    public void RefreshAmmoUI()
    {
        forAmmoUI = ammoInMag.ToString() + " / " + ammoInPocket.ToString();
        ammoText.text = forAmmoUI;
    }
}
