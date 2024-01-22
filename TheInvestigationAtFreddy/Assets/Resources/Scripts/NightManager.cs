using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows.Speech;

public class NightManager : MonoBehaviour
{
    [Header("Generals")]
    public float currentTime = 0f;

    [Header("Game Events System")]
    public AnimationCurve chanceCurve;
    public string[] globalWords;
    public GameEvent[] gameEvents;

    /// <summary>
    /// Keywords detected by Windows.Speech;
    /// </summary>
    KeywordRecognizer animatronicKeywords, globalKeywords;
    /// <summary>
    /// Set word => list of animatronic (dispatch the text detected)
    /// </summary>
    Dictionary<string, List<Animatronic>> animatronicsBinding;

    private void Start()
    {
        InitialiseWords();
    }

    /// <summary>
    /// Generate and Start the detection of the set of words.
    /// </summary>
    void InitialiseWords()
    {
        // init the animatronics
        animatronicsBinding = new Dictionary<string, List<Animatronic>>();
        Animatronic[] animatronics = FindObjectsByType<Animatronic>(FindObjectsSortMode.InstanceID);
        foreach(Animatronic animatronic in animatronics)
        {
            foreach(BanWord bword in animatronic.banWords)
            {
                if (animatronicsBinding.ContainsKey(bword.word)) animatronicsBinding[bword.word].Add(animatronic);
                else animatronicsBinding.Add(bword.word, new List<Animatronic>() { animatronic });
            }
        }
        animatronicKeywords = new KeywordRecognizer(animatronicsBinding.Keys.ToArray());
        animatronicKeywords.OnPhraseRecognized += OnAnimatronicWordDetected;
        animatronicKeywords.Start();

        // init the globals
        globalKeywords = new KeywordRecognizer(globalWords);
        globalKeywords.OnPhraseRecognized += OnGlobalWordDetected;
        globalKeywords.Start();

    }

    private void OnGlobalWordDetected(PhraseRecognizedEventArgs args)
    {
        float dice = Random.Range(0, 100);
        print($"txt: {args.text} || dice: ${dice} || chance: ${chanceCurve.Evaluate(currentTime)}");
        if (dice <= chanceCurve.Evaluate(currentTime)) SpawnRandomEvent();
    }

    private void OnAnimatronicWordDetected(PhraseRecognizedEventArgs args)
    {
        if (!animatronicsBinding.ContainsKey(args.text)) return;
        foreach(var animatronic in animatronicsBinding[args.text])
        {
            animatronic.Receive(args.text);
        }
    }




    /// <summary>
    /// Start an random event from the GameEvent List
    /// </summary>
    public void SpawnRandomEvent()
    {
        List<int> possibilities = new List<int>();
        float rng = Random.Range(0f, 100f);
        for(int i = 0; i < gameEvents.Length; i++)
        {
            if (rng <= gameEvents[i].chance) possibilities.Add(i);
        }

        print($"possibilities : {possibilities.Count}");
        if (possibilities.Count == 0) return;
        SpawnEvent(possibilities[Random.Range(0,possibilities.Count)]);
    }

    /// <summary>
    /// Start the event 'index' from the GameEvent List
    /// </summary>
    /// <param name="index"></param>
    public void SpawnEvent(int index)
    {
        gameEvents[index].onSpawned.Invoke();
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

[System.Serializable]
public struct GameEvent {
    [Range(0f,100f)]public float chance;
    public UnityEvent onSpawned;
}
