using System.Collections;
using TMPro;
using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    public static DebugCanvas instance;
    public static DebugCanvas Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<DebugCanvas>();
            return instance;
        }
    }

    [SerializeField] private TMP_Text[] debugTexts;

    public void SetDebugText(string s, int textIndex = 0)
    {
        TMP_Text selectedText = debugTexts[textIndex];

        selectedText.text = s;
        selectedText.color = Random.ColorHSV();
        StopAllCoroutines();
        StartCoroutine(CleanDebugText());
    }

    private IEnumerator CleanDebugText()
    {
        yield return new WaitForSeconds(2);
        foreach (TMP_Text text in debugTexts)
        {
            text.text = "";
        }
    }
}
