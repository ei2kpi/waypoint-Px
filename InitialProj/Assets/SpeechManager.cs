using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.VR.WSA.Persistence;
using HoloToolkit.Unity;

public class SpeechManager : MonoBehaviour { 
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    public GameObject WaypointPrefab;

    // Use this for initialization
    void Start()
    {
        keywords.Add("Action one", () =>
        {
            // Call the OnReset method on every descendant object.
            GameObject.Find("Waypoints").GetComponent<FlowManager>().CurrentAppState = FlowManager.AppState.WaypointSetup;
            GameObject.Find("Waypoints").GetComponent<FlowManager>().AppStateChanged();
        });

        keywords.Add("Action two", () =>
        {
            // Call the OnReset method on every descendant object.
            GameObject.Find("Waypoints").GetComponent<FlowManager>().CurrentAppState = FlowManager.AppState.Intro;
            GameObject.Find("Waypoints").GetComponent<FlowManager>().AppStateChanged();
        });

        keywords.Add("Action three", () =>
        {
            // Call the OnReset method on every descendant object.
            GameObject.Find("Waypoints").GetComponent<FlowManager>().CurrentAppState = FlowManager.AppState.Collection;
            GameObject.Find("Waypoints").GetComponent<FlowManager>().AppStateChanged();
        });

        keywords.Add("Action place", () =>
        {
            Debug.Log(GestureManager.Instance.FocusedObject);
            if (GestureManager.Instance.FocusedObject != null)
            {
                GameObject newWaypoint = (GameObject)Instantiate(WaypointPrefab, GameObject.Find("Waypoints").GetComponent<FlowManager>().cursorWayPoint.transform.position, Quaternion.identity);
                newWaypoint.transform.SetParent(GameObject.Find("Waypoints").transform);
                WorldAnchorManager.Instance.AttachAnchor(newWaypoint, newWaypoint.GetInstanceID().ToString());
            }
        });

        keywords.Add("Next", () =>
        {
            // Call the OnReset method on every descendant object.
            if (GestureManager.Instance.FocusedObject != null)
            {
                GestureManager.Instance.FocusedObject.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
            }
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