using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Animatronic : MonoBehaviour
{
    /// <summary>
    /// 0 to 1. 1 is when the animatronic start to chase you 
    /// </summary>
    public BanWord[] banWords;
    public AnimationCurve chanceCurve;
    [SerializeField] float agressivityState = 0;

    public void Speak() { }

    /// <summary>
    /// Response and adjust his agressivity on depends of this text.
    /// </summary>
    /// <param name="textVoice"> text detected </param>
    public void Receive(string textVoice) {
        Debug.Log(textVoice);
        for(int i = 0; i < banWords.Length; i++)
        {
            if (banWords[i].word == textVoice) Agressitivy += banWords[i].agressitivyAmount;
        }
    }
    
    public float Agressitivy
    {
        get { return agressivityState; }
        set { agressivityState = Mathf.Clamp01(value); }
    }
    
    public virtual void Jumpscare() { }

}

[System.Serializable]
public struct BanWord
{
    public string word;
    [Range(0f,1f)]
    public float agressitivyAmount;
}
