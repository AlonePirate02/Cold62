using UnityEngine;

public class GunAnimationProxy : MonoBehaviour
{

    // This script exists so I can use animation events.

    public GunScript gunScript;

    private void Start()
    {
        if (gunScript == null)
        {
            gunScript = GetComponentInParent<GunScript>();
        }
    }

    public void OnAnimationEnd()
    {
        if (gunScript != null)
        {
            gunScript.OnAnimationEnd();
        }
    }
}
