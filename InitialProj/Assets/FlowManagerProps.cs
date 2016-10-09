using UnityEngine;
using System.Collections;

public class FlowManagerProps : MonoBehaviour {
    
    public bool ReachedCurrentWayPoint;
    private MeshRenderer meshR;

    public FlowManager.WaypointState CurrWayPointState = FlowManager.WaypointState.Invisible;

	// Use this for initialization
	void Start () {
        meshR = gameObject.GetComponentInChildren<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if(GameObject.Find("Waypoints").GetComponent<FlowManager>().CurrentAppState != FlowManager.AppState.WaypointSetup)
            meshR.enabled = CurrWayPointState != FlowManager.WaypointState.Invisible;
        if (ReachedCurrentWayPoint)
            gameObject.transform.Rotate(0.0f, 0.0f, 1.0f);
	}
}
