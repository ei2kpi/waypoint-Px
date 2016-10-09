﻿using UnityEngine;
using System.Collections;

public class FlowManagerProps : MonoBehaviour {
    
    public bool ReachedCurrentWayPoint;
    private MeshRenderer[] meshR;
    //private Renderer meshBottleR;
    public Texture2D fire;
    public Texture2D blue;
    private TextMesh meshText;
    public int pill_count;
    public string pill_name;

    public FlowManager.WaypointState CurrWayPointState = FlowManager.WaypointState.Invisible;

	// Use this for initialization
	void Start () {
        pill_count = 2;
        pill_name = "viagra";

        meshR = gameObject.GetComponentsInChildren<MeshRenderer>();
        meshText = gameObject.GetComponentInChildren<TextMesh>();
        
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

            switch (GameObject.Find("Waypoints").GetComponent<FlowManager>().wayPointList.IndexOf(this.gameObject.transform))
            {
                case (0):
                    pill_count = 3;
                    pill_name = "Adderall";
                    break;
                case (1):
                    pill_count = 2;
                    pill_name = "Anacin";
                    break;
                case (2):
                    pill_count = 5;
                    pill_name = "Aricept";
                    break;
                default:
                    break;
            }

            switch (CurrWayPointState)
                {
                    case (FlowManager.WaypointState.Invisible):
                        foreach (MeshRenderer mr in meshR)
                        {
                            mr.enabled = false;
                        }
                        break;
                    case (FlowManager.WaypointState.GoToWaypoint):
                        meshText.text = string.Format("Collect {0} {1}", pill_count, pill_name);
                        foreach (MeshRenderer mr in meshR)
                        {
                            mr.enabled = true;
                        }
                        break;
                    case (FlowManager.WaypointState.FinishedWithWaypoint):
                        // todo figure out how to change texture
                        //meshBottleR.material.mainTexture = blue;
                        meshText.text = "Move on to the next Rx";
                        break;
                    case (FlowManager.WaypointState.CloseToWaypoint):
                        // todo: this event isn't triggering
                        meshText.text = string.Format("Tap when complete. Collect {0} {1}", pill_count, pill_name);
                        break;
                    default:
                        break;
                }

        if (ReachedCurrentWayPoint)
            gameObject.transform.Rotate(0.0f, 0.0f, 1.0f);
	}
}
