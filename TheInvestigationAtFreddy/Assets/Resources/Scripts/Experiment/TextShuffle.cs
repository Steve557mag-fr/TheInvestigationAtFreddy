using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextShuffle : MonoBehaviour
{
    public TextMeshProUGUI textField;
    public string text;
    public int shuffleAmount = 1;
    public float deltaTimeText = 1;
    float dlt = 0;

    void Update()
    {
        dlt = Mathf.Max(0, dlt - Time.deltaTime);
        if (dlt > 0) return;
        dlt = deltaTimeText;

        char[] fText = text.ToCharArray();
        for(int i = 0; i < shuffleAmount; i++)
        {
            int txtR = Random.Range(0, fText.Length);
            fText[txtR] = text[ (txtR + Random.Range(0, fText.Length)) % fText.Length];
        }
        textField.text = fText.ArrayToString();
    }
}
