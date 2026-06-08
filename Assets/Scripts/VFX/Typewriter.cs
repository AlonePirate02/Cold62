using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Typewriter : MonoBehaviour
{
    // Settings
    [SerializeField] private string[] dialogueLines; // Write in Unity Inspector
    [SerializeField] private float dialogueDisplayTime = 3f;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float textAppearSpeed = 1f;

    // References
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private float waitInterval = 2f;
    [SerializeField] private InterludeManager im;
    [SerializeField] private TextMeshProUGUI conttext;

    private void Start()
    {
        StartCoroutine(TypeWriterEffect());
    }

    IEnumerator TypeWriterEffect()
    {
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            dialogueText.text = ""; // Empty start
            string line = dialogueLines[i];
            foreach (char letter in line)
            {
                dialogueText.text += letter;

                yield return new WaitForSeconds(typewriterSpeed);
            }

            yield return new WaitForSeconds(waitInterval);
        }

        yield return new WaitForSeconds(1f);
        im.enabled = true;

        StartCoroutine(Flash());
    }

    public IEnumerator Flash()
    {
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * textAppearSpeed;
            conttext.color = new Color(255, 255, 255, alpha);
            yield return null;
        }
    }
}
