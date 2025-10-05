using System.Collections;
using UnityEngine;
using TMPro;   // <-- TMP namespace

public class TypewriterEffectTMP : MonoBehaviour
{
    public TextMeshProUGUI textComponent;  // TMP Text
    public float typingSpeed = 0.1f;
    public string[] textsToShow;

    public float satirArasi = 2;

    private int currentTextIndex = 0;
    private Coroutine typingRoutine;

    private void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // aktif olur olmaz sıfırla ve yazmaya başla
        ResetState();
        typingRoutine = StartCoroutine(PlayTypingEffect());
    }

    private void OnDisable()
    {
        // pasif olunca durdur ve sıfırla
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        ResetState();
    }

    private void ResetState()
    {
        currentTextIndex = 0;
        if (textComponent) textComponent.text = "";
    }

    private IEnumerator PlayTypingEffect()
    {
        if (textComponent == null || textsToShow == null || textsToShow.Length == 0)
            yield break;

        while (currentTextIndex < textsToShow.Length)
        {
            textComponent.text = "";
            string currentText = textsToShow[currentTextIndex];

            foreach (char letter in currentText)
            {
                textComponent.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            currentTextIndex++;
            yield return new WaitForSeconds(satirArasi); // satır arası bekleme
        }
        typingRoutine = null; // bitti
    }
}
