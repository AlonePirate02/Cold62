using System.Collections;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    [Header("Footstep Audio Clips")]
    [SerializeField] private AudioClip[] grassFootstep;
    [SerializeField] private AudioClip[] soilFootstep;
    [SerializeField] private AudioClip[] concreteFootstep;
    [SerializeField] private AudioClip[] woodFootstep;
    [SerializeField] private AudioClip[] asphaltFootstep;
    [SerializeField] private AudioClip[] snowFootstep;
    [SerializeField] private AudioClip[] ironFootstep;

    [Header("Intervals")]
    [SerializeField] private float walkInterval = 0.5f;
    [SerializeField] private float runInterval = 0.3f;
    [SerializeField] private float crouchInterval = 0.7f;

    [SerializeField] AudioSource audioSource;
    private PlayerMovement playerMovement;
    private bool isPlayingFootsteps = false;
    private Terrain terrain;
    private TerrainData terrainData;
    private Vector3 terrainPosition;
    private string currentSurface = "";

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (Terrain.activeTerrain != null)
        {
            terrain = Terrain.activeTerrain;
            terrainData = terrain.terrainData;
            terrainPosition = terrain.transform.position;
        }
    }

    private void Update()
    {
        if (playerMovement.controller.velocity.magnitude > 0 && !isPlayingFootsteps)
        {
            StartCoroutine(PlayFootsteps());
        }
    }

    private IEnumerator PlayFootsteps()
    {
        isPlayingFootsteps = true;

        while (playerMovement.controller.velocity.magnitude > 0)
        {
            AudioClip footstepClip = GetFootstepClip();
            if (footstepClip != null)
            {
                audioSource.volume = GetFootstepVolume();
                audioSource.PlayOneShot(footstepClip);
            }

            float interval = GetFootstepInterval();
            yield return new WaitForSeconds(interval);
        }

        isPlayingFootsteps = false;
    }

    private void StopFootsteps()
    {
        if (isPlayingFootsteps)
        {
            StopCoroutine(PlayFootsteps());
            isPlayingFootsteps = false;
        }
    }

    private AudioClip GetFootstepClip()
    {
        // 1. Önce Özel Objelerin (Trigger: Tahta, Demir vb.) üzerinde miyiz kontrol et
        if (!string.IsNullOrEmpty(currentSurface))
        {
            return GetClipFromSurfaceName(currentSurface);
        }

        // 2. Eðer tetikleyicide deðilsek Raycast ile Terrain algýlamaya geç
        if (terrain == null) return null;

        // Oyuncunun pozisyonundan aþaðýya doðru kýsa bir ýþýn fýrlatýyoruz
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        RaycastHit hit;

        // Iþýn sadece Terrain'e çarptýðýnda çalýþsýn (Mesafe: 1.5 metre)
        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            // Çarptýðýmýz yerin Terrain üzerindeki normalize edilmiþ (0-1 arasý) koordinatýný alýyoruz
            Vector3 terrainLocalPos = hit.point - terrainPosition;
            float normalizedX = terrainLocalPos.x / terrainData.size.x;
            float normalizedZ = terrainLocalPos.z / terrainData.size.z;

            // Bu normalize koordinatlarý Splatmap piksel karþýlýðýna çeviriyoruz (Dikdörtgen haritalarda tam çalýþýr)
            int mapX = Mathf.RoundToInt(normalizedX * (terrainData.alphamapWidth - 1));
            int mapZ = Mathf.RoundToInt(normalizedZ * (terrainData.alphamapHeight - 1));

            // Sýnýrlar dýþýndaysa güvenli bölge (Kar) döndür
            if (mapX < 0 || mapX >= terrainData.alphamapWidth || mapZ < 0 || mapZ >= terrainData.alphamapHeight)
                return snowFootstep[Random.Range(0, snowFootstep.Length)];

            // O pikseldeki doku karýþýmýný al
            float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
            float[] textureMix = new float[splatmapData.GetUpperBound(2) + 1];

            for (int i = 0; i < textureMix.Length; i++)
            {
                textureMix[i] = splatmapData[0, 0, i];
            }

            int textureIndex = GetMainTexture(textureMix);

            // Terrain Layer sýralamana göre ses döndür
            switch (textureIndex)
            {
                case 0: // Kar
                    return snowFootstep[Random.Range(0, snowFootstep.Length)];

                case 1: // Toprak (Asfaltla ayný)
                case 2: // Asfalt
                    return asphaltFootstep[Random.Range(0, asphaltFootstep.Length)];

                default:
                    return snowFootstep[Random.Range(0, snowFootstep.Length)];
            }
        }

        // Eðer ýþýn hiçbir þeye çarpmadýysa havada demektir, varsayýlan kar dönsün
        return snowFootstep[Random.Range(0, snowFootstep.Length)];
    }

    private AudioClip GetClipFromSurfaceName(string surface)
    {
        switch (surface)
        {
            case "Concrete": return concreteFootstep[Random.Range(0, concreteFootstep.Length)];
            case "Wood": return woodFootstep[Random.Range(0, woodFootstep.Length)];
            case "Snow": return snowFootstep[Random.Range(0, snowFootstep.Length)];
            case "Iron": return ironFootstep[Random.Range(0, ironFootstep.Length)]; // Yeni: Tetikleyici için demir sesi
            default: return null;
        }
    }

    private int GetMainTexture(float[] textureMix)
    {
        float maxMix = 0;
        int maxIndex = 0;

        for (int i = 0; i < textureMix.Length; i++)
        {
            if (textureMix[i] > maxMix)
            {
                maxIndex = i;
                maxMix = textureMix[i];
            }
        }
        return maxIndex;
    }

    private float GetFootstepInterval() { return playerMovement.crouching ? crouchInterval : playerMovement.sprinting ? runInterval : walkInterval; }
    private float GetFootstepVolume() { return playerMovement.crouching ? 0.25f : playerMovement.sprinting ? 0.6f : 0.4f; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Concrete")) currentSurface = "Concrete";
        else if (other.CompareTag("Wood")) currentSurface = "Wood";
        else if (other.CompareTag("Snow")) currentSurface = "Snow";
        else if (other.CompareTag("Iron")) currentSurface = "Iron"; // Yeni: Demir objesine girince
    }

    private void OnTriggerExit(Collider other)
    {
        // Çýkýþ kontrolüne Iron da eklendi
        if (other.CompareTag("Concrete") || other.CompareTag("Wood") || other.CompareTag("Snow") || other.CompareTag("Iron"))
        {
            currentSurface = ""; // Boþ býrakýnca otomatik Terrain splatmap'e döner
        }
    }
}