﻿using UnityEngine;
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
        if (!GameObject.Find("Waypoints").GetComponent<FlowManager>().SetupMode && this.transform.parent.GetComponent<FlowManagerProps>().IsCurrentWayPoint)
        {
            GameObject.Find("Waypoints").GetComponent<FlowManager>().GoToNextWayPoint();
        }
    }
}