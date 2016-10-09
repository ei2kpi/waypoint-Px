﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.VR.WSA.Persistence;
using HoloToolkit.Unity;


[Serializable()]
public class DrugData
{
	public string name;
	public GameObject productCard;

}

public class FlowManager : MonoBehaviour
{ 
	public DrugData[] drugs = new DrugData[10];

	public List<Transform> wayPointList;

    public float waypointThreshold;
    public float distToWaypoint;

    public Transform CurrentWayPoint;
    public bool GoToNextWayPointBtn;
    public bool WrapWayPoints = false;
    public bool ReachedCurrentWayPoint = false;
    
    private GameObject cursorWayPoint;
    public GameObject WaypointPrefab;
    public GameObject WaypointCursorPrefab;
	private GameObject productCard;
	public bool SetupMode = false;

    public AppState CurrentAppState = AppState.Idle;
    
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckForKeybdInput();

        if (SetupMode)
        {
            CheckForSetupKeybdInput();
            if (cursorWayPoint != null)
            {
                // Update Waypoint position to Cursor position
                cursorWayPoint.transform.position = Vector3.Lerp(cursorWayPoint.transform.position, ProposeTransformPosition(), 0.2f);
            }
        }

        // Regular Update Loop
        if (wayPointList.Count != 0)
        {
            // CheckIfReachedWayPoint();
        }

    }

    private void EnterSetupMode()
    {
        ClearAllWaypoints();

		// Create Waypoint on cursor
		cursorWayPoint = (GameObject)Instantiate(WaypointCursorPrefab, ProposeTransformPosition(), Quaternion.identity);
        GestureManager.Instance.OverrideFocusedObject = cursorWayPoint;
        
        //Enable SR Visualization
        MeshRenderer[] SRMeshRends = GameObject.Find("SpatialMapping").GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in SRMeshRends)
            mr.enabled = true;

        SetupMode = true;
    }

    private void ExitSetupMode()
    {
        // Reset normal cursor instead of waypoint
        if (cursorWayPoint != null)
            DestroyObject(cursorWayPoint);

        // Now that cursor waypoint is destroyed, Get all the others!
        GetAllWaypoints();
        
        //Disable SR Visualization
        MeshRenderer[] SRMeshRends = GameObject.Find("SpatialMapping").GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in SRMeshRends)
            mr.enabled = false;

        GameObject.Find("Agent").transform.position = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
        SetupMode = false;
    }

    private void CheckIfReachedWayPoint()
    {
        if (CurrentWayPoint == null)
            return;

        distToWaypoint = Vector3.Distance(Camera.main.transform.position, CurrentWayPoint.position);

        if (distToWaypoint <= waypointThreshold)
        {
            ReachedCurrentWayPoint = true;
            CurrentWayPoint.GetComponent<FlowManagerProps>().ReachedCurrentWayPoint = true;
        }
        else
        {
            ReachedCurrentWayPoint = false;
            CurrentWayPoint.GetComponent<FlowManagerProps>().ReachedCurrentWayPoint = false;
        }
    }

    private void GetAllWaypoints()
    {
        wayPointList.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            wayPointList.Add(obj.transform);
            obj.layer = 0;
            foreach (Transform trans in obj.transform)
                trans.gameObject.layer = 0;
        }
        CurrentWayPoint = wayPointList[0];
        CurrentWayPoint.GetComponent<FlowManagerProps>().IsCurrentWayPoint = true;
        Debug.logger.Log("Enumerating all waypoints");

		if (drugs[0].productCard != null)
		{
			productCard = (GameObject)Instantiate(drugs[0].productCard, CurrentWayPoint.transform.position, Quaternion.identity);
		}

	}

    private void ClearAllWaypoints()
    {
        if (wayPointList.Count != 0)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Waypoint"))
            {
                DestroyObject(obj);
            }
            wayPointList.Clear();
            CurrentWayPoint = null;
            Debug.logger.Log("Clearing all waypoints");
        }
    }
    public void GoToNextWayPoint()
    {
        if (CurrentWayPoint != null)
        {
            // Set current waypoint to not current any more
            CurrentWayPoint.GetComponent<FlowManagerProps>().IsCurrentWayPoint = false;

			int nextWaypointIndex = wayPointList.IndexOf(CurrentWayPoint) + 1;
			// Switch to next waypoint, if WrapMode is on, then wrap back to 0
			if (wayPointList.IndexOf(CurrentWayPoint) < wayPointList.Count - 1)
                CurrentWayPoint = wayPointList[nextWaypointIndex];
            else if (WrapWayPoints)
                CurrentWayPoint = wayPointList[0];

            CurrentWayPoint.GetComponent<FlowManagerProps>().IsCurrentWayPoint = true;
			
			if (productCard != null)
			{
				Destroy(productCard);
			}

			productCard = (GameObject)Instantiate(drugs[nextWaypointIndex].productCard, CurrentWayPoint.transform.position, Quaternion.identity);
		}
    }

    private void  CheckForKeybdInput()
    {
        // Check if GoToNextWayPoint was pressed or user presses A
        if (GoToNextWayPointBtn || Input.GetKeyDown(KeyCode.A))
        {
            GoToNextWayPoint();
            GoToNextWayPointBtn = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            EnterSetupMode();
        }
    }

    private void CheckForSetupKeybdInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ExitSetupMode();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject newWaypoint = (GameObject)Instantiate(WaypointPrefab, cursorWayPoint.transform.position, Quaternion.identity);
            newWaypoint.transform.SetParent(GameObject.Find("Waypoints").transform);
            WorldAnchorManager.Instance.AttachAnchor(newWaypoint, newWaypoint.GetInstanceID().ToString());
		}
        else if (Input.GetKeyDown(KeyCode.L))
        {
            WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;
            string[] ids = store.GetAllIds();
            for (int index = 0; index < ids.Length; index++)
            {
                GameObject newWaypoint = (GameObject)Instantiate(WaypointPrefab, WaypointPrefab.transform.position, Quaternion.identity);
                newWaypoint.transform.SetParent(GameObject.Find("Waypoints").transform);
                store.Load(ids[index], newWaypoint);
            }
            //if(GameObject.Find("Waypoints").transform.childCount > 0)
            //{
            //    SetupMode = false;
            //}
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

            for (int index = 0; index < waypoints.Length; index++)
            {
                if (waypoints[index] != WaypointPrefab)
                {
                    Destroy(waypoints[index]);
                }
            }

            WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;
            store.Clear();
        }
    }

    Vector3 ProposeTransformPosition()
    {
        Vector3 retval = GameObject.Find("CursorWithFeedback").transform.position;

        return retval;
    }

    public enum WaypointState
    {
        Idle,
        GoToWaypoint,
        CollectingWaypoint
    }

    public enum AppState
    {
        Idle,
        TrailHeightSetup,
        WaypointSetup,
        Intro,
        Collection,
        Finish
    }
}
