using UnityEngine;
using System.Collections;

public class NavMarkerManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnSelect()
    {
        GameObject.Find("HeadAudioSource").GetComponent<AudioSource>().Play();
        if (GameObject.Find("Waypoints").GetComponent<FlowManager>().CurrentAppState != FlowManager.AppState.WaypointSetup && this.transform.parent.GetComponent<FlowManagerProps>().IsCurrentWayPoint)
        {
            GameObject.Find("Waypoints").GetComponent<FlowManager>().GoToNextWayPoint();
        }
    }
}
