using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewritterEffect : MonoBehaviour
{
    [SerializeField] private float typewritterSpeed = 50f;

    public Coroutine Run(string textTotType, TMP_Text textLabel)
    {
        return StartCoroutine(TypeText(textTotType, textLabel));
    }

    private IEnumerator TypeText(string textTotType, TMP_Text textLabel)
    {
        textLabel.text = string.Empty;

        float t = 0;
        int charIndex = 0;

        while(charIndex < textTotType.Length)
        {
            t += Time.deltaTime * typewritterSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textTotType.Length);
            textLabel.text = textTotType.Substring(0, charIndex);
            yield return null;
        }

        textLabel.text = textTotType;
    }
}
