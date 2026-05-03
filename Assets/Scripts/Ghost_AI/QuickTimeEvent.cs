using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class QuickTimeEvent : MonoBehaviour
{
    // References
    public Image fillBar;
    public GameObject qteUI;
    private PlayerMovement pm;
    private GlobalVolumeManager gvm;
    private GunScript gunSc;

    // Settings
    public float timeLimit = 5f;
    public float maxAmount = 1f;
    public float currentAmount = 0f;
    public float amountToDrain = 0.3f;
    public float amountToFill = 0.15f;

    private bool isActive = false;

    private void OnEnable()
    {
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        gvm = GameObject.Find("Global Volume").GetComponent<GlobalVolumeManager>();
        gunSc = GameObject.FindGameObjectWithTag("Gun").GetComponent<GunScript>();
        gunSc.canShoot = false; // Disable shooting during QTE
        currentAmount = 0f;
        StartCoroutine(QTE());
    }

    public IEnumerator QTE()
    {
        isActive = true;
        qteUI.SetActive(true);
        pm.canMove = false;

        float timer = 0f;

        while (isActive)
        {
            timer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.F))
            {
                currentAmount += amountToFill;
            }

            currentAmount -= amountToDrain * Time.deltaTime;
            currentAmount = Mathf.Clamp(currentAmount, 0f, maxAmount);

            fillBar.fillAmount = currentAmount / maxAmount;

            if (currentAmount >= maxAmount)
            {
                Debug.Log("SUCCESS");
                gvm.ResetSettings(); // Reset visual effects on success
                gunSc.canShoot = true; // Re-enable shooting after QTE
                gameObject.SetActive(false);
                break;
            }

            if (timer >= timeLimit)
            {
                Debug.Log("FAIL");
                EditorApplication.isPlaying = false;
                // You can implement a death method for player here
                break;
            }

            yield return null;
        }

        qteUI.SetActive(false);
        isActive = false;
        pm.canMove = true;

        this.enabled = false;
    }
}