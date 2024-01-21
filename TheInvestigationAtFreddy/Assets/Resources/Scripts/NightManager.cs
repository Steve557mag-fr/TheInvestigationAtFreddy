using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class NightManager : MonoBehaviour
{

    /// <summary>
    /// Keywords detected by Windows.Speech;
    /// </summary>
    KeywordRecognizer keywords;
    /// <summary>
    /// Set word => list of animatronic (dispatch the text detected)
    /// </summary>
    Dictionary<string, List<Animatronic>> animatronicsBinding;

    /// <summary>
    /// Generate and Start the detection of the set of words.
    /// </summary>
    void InitialiseWords()
    {
        animatronicsBinding = new Dictionary<string, List<Animatronic>>();
        Animatronic[] animatronics = FindObjectsByType<Animatronic>(FindObjectsSortMode.InstanceID);
        foreach(Animatronic animatronic in animatronics)
        {
            foreach(BanWord bword in animatronic.banWords)
            {
                if (animatronicsBinding.ContainsKey(bword.word))
                {
                    animatronicsBinding[bword.word].Add(animatronic);
                }
                else
                {
                    animatronicsBinding.Add(bword.word, new List<Animatronic>() { animatronic });
                }
            }
        }

        keywords = new KeywordRecognizer(animatronicsBinding.Keys.ToArray());
        keywords.OnPhraseRecognized += OnWordDetected;
        keywords.Start();
    }

    private void OnWordDetected(PhraseRecognizedEventArgs args)
    {
        if (!animatronicsBinding.ContainsKey(args.text)) return;
        foreach(var animatronic in animatronicsBinding[args.text])
        {
            animatronic.Receive(args.text);
        }
    }


    /// <summary>
    /// Get the NightManager of the scene
    /// </summary>
    public static NightManager Instance
    {
        get
        {
            return FindAnyObjectByType<NightManager>();
        }
    }
    
}
