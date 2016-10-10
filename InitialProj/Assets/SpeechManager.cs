using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Use this for initialization
    void Start()
    {
        keywords.Add("Action one", () =>
        {
            // Call the OnReset method on every descendant object.
            InputBroker.SetKeyDown(KeyCode.Alpha1);
        });

        keywords.Add("Action two", () =>
        {
            // Call the OnReset method on every descendant object.
            InputBroker.SetKeyDown(KeyCode.Alpha2);
        });

        keywords.Add("Action three", () =>
        {
            // Call the OnReset method on every descendant object.
            InputBroker.SetKeyDown(KeyCode.Alpha3);
        });

        keywords.Add("Action four", () =>
        {
            // Call the OnReset method on every descendant object.
            InputBroker.SetKeyDown(KeyCode.Alpha4);
        });

        keywords.Add("Next", () =>
        {
            // Call the OnReset method on every descendant object.
            this.SendMessage("OnSelect");
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}