using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class TapToContinue : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnSelect()
    {
        if(GameObject.Find("Waypoints").GetComponent<FlowManager>().CurrentAppState != FlowManager.AppState.Collection)
        {
            GameObject.Find("Waypoints").GetComponent<FlowManager>().CurrentAppState = FlowManager.AppState.Collection;
            GameObject.Find("Waypoints").GetComponent<FlowManager>().AppStateChanged();
            GestureManager.Instance.OverrideFocusedObject = null;
        }
    }
}
