using UnityEngine;
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
    public AppState CurrentAppState = AppState.Idle;

    public DrugData[] drugs = new DrugData[10];

	public List<Transform> wayPointList;

    public float waypointThreshold;
    public float distToWaypoint;

    public Transform PrevWayPoint;
    public Transform CurrentWayPoint;
    public bool GoToNextWayPointBtn;
    public bool WrapWayPoints = false;
    public bool ReachedCurrentWayPoint = false;
    
    private GameObject cursorWayPoint;
    public GameObject WaypointPrefab;
    public GameObject WaypointCursorPrefab;
	private GameObject productCard;
    
    void Start () {
	
	}
	
    public void AppStateChanged()
    {
        // Idle, WayPointSetup, Intro, Collection, Finish
        switch (CurrentAppState)
        {
            case AppState.Idle:
                // Reset State of the app to Idle
                SetIntro(false);
                SetOutro(false);
                break;
            case AppState.WaypointSetup:
                // Go to Waypoint Setup State
                SwitchToWayPointSetup();
                break;
            case AppState.Intro:
                // Go To Intro Mode
                SwitchToIntro();
                break;
            case AppState.Collection:
                // User starts collecting Pills
                SwitchToCollectionMode();
                break;
            case AppState.Finish:
                // User finishes collection all pills
                SwitchToFinishMode();
                break;
        }
    }

    void Update()
    {
        CheckForKeybdInput();

        CheckIfUserIsNearWaypoint();

        // Constantly update Waypoint position to Cursor position to attach Waypoint to Cursor
        if (CurrentAppState == AppState.WaypointSetup)
        {
            if (cursorWayPoint != null)
                cursorWayPoint.transform.position = Vector3.Lerp(cursorWayPoint.transform.position, ProposeTransformPosition(), 0.2f);
        }
    }

    private void CheckIfUserIsNearWaypoint()
    {
        if (CurrentWayPoint != null)
        {
            distToWaypoint = Vector3.Distance(Camera.main.transform.position, CurrentWayPoint.transform.position);

            bool UserIsNear = distToWaypoint <= waypointThreshold;
            // If distance is less than threshold, CloseToWaypoint otherwise, GoToWaypoint
            CurrentWayPoint.GetComponent<FlowManagerProps>().CurrWayPointState =  UserIsNear ? WaypointState.CloseToWaypoint : WaypointState.GoToWaypoint;

            // Disable previous waypoint when you get close to the Current waypoint.
            if (PrevWayPoint!= null && UserIsNear && PrevWayPoint.GetComponent<FlowManagerProps>().CurrWayPointState != WaypointState.Invisible)
                PrevWayPoint.GetComponent<FlowManagerProps>().CurrWayPointState = WaypointState.Invisible;
        }
    }

    private void SwitchToFinishMode()
    {
        SetIntro(false);
        SetOutro(true);
    }

    private void SwitchToCollectionMode()
    {
        SetIntro(false);
        SetOutro(false);
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
        CurrentAppState = AppState.Intro;
    }

    private void SetOutro (bool enabled)
    {
        foreach (Transform child in GameObject.Find("OutroParent").transform)
        {
            child.gameObject.SetActive(enabled);
        }
    }

    private void SetIntro(bool enabled)
    {
        foreach (Transform child in GameObject.Find("IntroParent").transform)
        {
            child.gameObject.SetActive(enabled);
        }
    }

    private void SwitchToWayPointSetup()
    {
        SetIntro(false);
        SetOutro(false);
        ClearAllWaypoints();

        //Turn On table renderer
        //GameObject.Find("TableObstacle").GetComponent<MeshRenderer>().enabled = true;

        // Create Waypoint on cursor
        cursorWayPoint = (GameObject)Instantiate(WaypointCursorPrefab, ProposeTransformPosition(), Quaternion.identity);
        GestureManager.Instance.OverrideFocusedObject = cursorWayPoint;
        
        //Enable SR Visualization
        MeshRenderer[] SRMeshRends = GameObject.Find("SpatialMapping").GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in SRMeshRends)
            mr.enabled = true;

        CurrentAppState = AppState.WaypointSetup;
    }

    private void SwitchToIntro()
    {
        SetIntro(true);
        SetOutro(false);
        // Disable Table Renderer
        // GameObject.Find("TableObstacle").GetComponent<MeshRenderer>().enabled = false;

        // Reset normal cursor instead of waypoint
        if (cursorWayPoint != null)
            DestroyObject(cursorWayPoint);
        GestureManager.Instance.OverrideFocusedObject = GameObject.Find("IntroParent");
    }

    private void GetAllWaypoints()
    {
        wayPointList.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            obj.GetComponent<FlowManagerProps>().CurrWayPointState = WaypointState.Invisible;
            wayPointList.Add(obj.transform);
            obj.layer = 0;
            foreach (Transform trans in obj.transform)
                trans.gameObject.layer = 0;
        }
        CurrentWayPoint = wayPointList[0];
        CurrentWayPoint.GetComponent<FlowManagerProps>().CurrWayPointState = WaypointState.GoToWaypoint;
        Debug.logger.Log("Enumerating all waypoints");

		if (drugs[0].productCard != null)
		{
			productCard = (GameObject)Instantiate(drugs[0].productCard, CurrentWayPoint.transform.position, Quaternion.identity);
		}
	}

    private void ClearAllWaypoints(bool clearStore = false)
    {
        if (wayPointList.Count != 0)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Waypoint"))
            {
                DestroyObject(obj);
            }

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ProductCard"))
            {
                DestroyObject(obj);
            }
            wayPointList.Clear();
            CurrentWayPoint = null;
            PrevWayPoint = null;
            Debug.logger.Log("Clearing all waypoints and PRoduct Cards");
        }

        if (clearStore)
        {
            // Clear Anchor Store completely
            WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;
            if (store != null)
                store.Clear();
        }
    }

    public void GoToNextWayPoint()
    {
        if (CurrentWayPoint != null)
        {
            // Set previous waypoint to Current Waypoint
            PrevWayPoint = CurrentWayPoint;
            // Set current waypoint to not current any more
            CurrentWayPoint.GetComponent<FlowManagerProps>().CurrWayPointState = WaypointState.FinishedWithWaypoint;
            
			int nextWaypointIndex = wayPointList.IndexOf(CurrentWayPoint) + 1;

			// Switch to next waypoint, if WrapMode is on, then wrap back to 0
			if (wayPointList.IndexOf(CurrentWayPoint) < wayPointList.Count - 1)
                CurrentWayPoint = wayPointList[nextWaypointIndex];
            else if (WrapWayPoints)
                CurrentWayPoint = wayPointList[0];
            else
            {
                CurrentAppState = AppState.Finish;
                AppStateChanged();
            }

            CurrentWayPoint.GetComponent<FlowManagerProps>().CurrWayPointState = WaypointState.GoToWaypoint;
			
			if (productCard != null)
			{
				Destroy(productCard);
			}

			productCard = (GameObject)Instantiate(drugs[nextWaypointIndex].productCard, CurrentWayPoint.transform.position, Quaternion.identity);
		}
    }

    private void  CheckForKeybdInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentAppState = AppState.WaypointSetup;
            AppStateChanged();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentAppState = AppState.Intro;
            AppStateChanged();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentAppState = AppState.Collection;
            AppStateChanged();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CurrentAppState = AppState.Finish;
            AppStateChanged();
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            CurrentAppState = AppState.Idle;
            AppStateChanged();
        }

        if (Input.GetKeyDown(KeyCode.Space) && CurrentAppState == AppState.WaypointSetup)
        {
            // Drop a WayPoint
            GameObject newWaypoint = (GameObject)Instantiate(WaypointPrefab, cursorWayPoint.transform.position, Quaternion.identity);
            newWaypoint.transform.SetParent(GameObject.Find("Waypoints").transform);
            WorldAnchorManager.Instance.AttachAnchor(newWaypoint, newWaypoint.GetInstanceID().ToString());
        }

        if ((GoToNextWayPointBtn || Input.GetKeyDown(KeyCode.Space)) && CurrentAppState == AppState.Collection)
        {
            // Move to Next Waypoint
            GoToNextWayPoint();
            GoToNextWayPointBtn = false;
        }
        
        else if (Input.GetKeyDown(KeyCode.L))
        {
            // Load All Waypoints
            WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;
            if (store != null)
            {
                string[] ids = store.GetAllIds();
                for (int index = 0; index < ids.Length; index++)
                {
                    GameObject newWaypoint = (GameObject)Instantiate(WaypointPrefab, WaypointPrefab.transform.position, Quaternion.identity);
                    newWaypoint.transform.SetParent(GameObject.Find("Waypoints").transform);
                    store.Load(ids[index], newWaypoint);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            ClearAllWaypoints(true);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            ClearAllWaypoints(false);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            // Load Table
            WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;
            if (store != null)
            {
                string[] ids = store.GetAllIds();
                for (int index = 0; index < ids.Length; index++)
                {
                    store.Load("Table", GameObject.Find("TableObstacle"));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            WorldAnchorManager.Instance.AttachAnchor(GameObject.Find("TableObstacle"), "Table");
        }
    }

    Vector3 ProposeTransformPosition()
    {
        Vector3 retval = GameObject.Find("CursorWithFeedback").transform.position;

        return retval;
    }

    public enum WaypointState
    {
        Invisible,
        GoToWaypoint,
        CloseToWaypoint,
        FinishedWithWaypoint
    }

    public enum AppState
    {
        Idle,
        WaypointSetup,
        Intro,
        Collection,
        Finish
    }
}
