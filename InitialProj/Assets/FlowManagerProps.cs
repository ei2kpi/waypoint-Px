using UnityEngine;
using System.Collections;

public class FlowManagerProps : MonoBehaviour {
    
    public bool ReachedCurrentWayPoint;
    private MeshRenderer[] meshR;
    //private Renderer meshBottleR;
    public Texture2D fire;
    public Texture2D blue;

    public FlowManager.WaypointState CurrWayPointState = FlowManager.WaypointState.Invisible;

	// Use this for initialization
	void Start () {
        meshR = gameObject.GetComponentsInChildren<MeshRenderer>();
        //meshBottleR = gameObject.GetComponentInChildren<Renderer>();
        //meshBottleR.material.mainTexture = blue;
    }
	
	// Update is called once per frame
	void Update () {
        if (GameObject.Find("Waypoints").GetComponent<FlowManager>().CurrentAppState != FlowManager.AppState.WaypointSetup)
            //    foreach (MeshRenderer mr in meshR)
            //    {
            //        mr.enabled = CurrWayPointState != FlowManager.WaypointState.Invisible; 
            //    }

            //Debug.Log(CurrWayPointState);
            switch (CurrWayPointState)
            {
                case (FlowManager.WaypointState.Invisible):
                    foreach (MeshRenderer mr in meshR)
                    {
                        mr.enabled = false;
                    }
                    break;
                case (FlowManager.WaypointState.GoToWaypoint):
                    foreach (MeshRenderer mr in meshR)
                    {
                        mr.enabled = true;
                    }
                    break;
                case (FlowManager.WaypointState.FinishedWithWaypoint):
                    // todo figure out how to change texture
                    //meshBottleR.material.mainTexture = blue;
                    break;
                case (FlowManager.WaypointState.CloseToWaypoint):
                    break;
                default:
                    break;
            }

        if (ReachedCurrentWayPoint)
            gameObject.transform.Rotate(0.0f, 0.0f, 1.0f);
	}
}
