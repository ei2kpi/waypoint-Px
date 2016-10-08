using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FlowManager : MonoBehaviour {

    public bool StartFlow = false;
    public List<Transform> wayPointList;

    public float waypointThreshold;
    public float distToWaypoint;

    public Transform CurrentWayPoint;
    public bool GoToNextWayPointBtn;
    public bool WrapWayPoints = false;
    public bool ReachedCurrentWayPoint = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (StartFlow)
        {
            // Get out of SetupMode
            GameObject.Find("Managers").GetComponent<CursorSetupManager>().SetupMode = false;
            //Disable SR Visualization
            MeshRenderer[] SRMeshRends = GameObject.Find("SpatialMapping").GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in SRMeshRends)
                mr.enabled = false;
            // Get and Initialize all waypoints
            GetAllWaypoints();
        }
        else
        {
            if (wayPointList.Count != 0)
            {
                CheckIfEnteredWayPoint();
            }
        }

        // Check if GoToNextWayPoint was pressed
        if (GoToNextWayPointBtn)
        {
            GoToNextWayPoint();
            GoToNextWayPointBtn = false;
        }
    }

    private void CheckIfEnteredWayPoint()
    {
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
        if (transform.childCount != 0)
        {
            foreach (Transform obj in transform)
            {
                wayPointList.Add(obj);
            }
            CurrentWayPoint = wayPointList[0];
            CurrentWayPoint.GetComponent<FlowManagerProps>().IsCurrentWayPoint = true;
        }
        else Debug.logger.Log("No Children found under Waypoints");
        StartFlow = false;
    }

    private void GoToNextWayPoint()
    {
        // Set current waypoint to not current any more
        CurrentWayPoint.GetComponent<FlowManagerProps>().IsCurrentWayPoint = false;

        // Switch to next waypoint, if WrapMode is on, then wrap back to 0
        if (wayPointList.IndexOf(CurrentWayPoint) < wayPointList.Count-1)
            CurrentWayPoint = wayPointList[wayPointList.IndexOf(CurrentWayPoint) + 1];
        else if (WrapWayPoints)
            CurrentWayPoint = wayPointList[0];

        CurrentWayPoint.GetComponent<FlowManagerProps>().IsCurrentWayPoint = true;
    }
}
