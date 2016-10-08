using UnityEngine;
using HoloToolkit.Unity;
using System.Collections;
using UnityEngine.VR.WSA.Persistence;

public class CursorSetupManager : Singleton<CursorSetupManager> {
    public GameObject Waypoint;
    public bool SetupMode = true;
    private GameObject cursorWayPoint;

    // Use this for initialization
    void Start () {
        if (SetupMode)
        {
            cursorWayPoint = (GameObject)Instantiate(Waypoint, ProposeTransformPosition(), Quaternion.identity);
            GestureManager.Instance.OverrideFocusedObject = cursorWayPoint;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (SetupMode)
        {
            cursorWayPoint.transform.position = Vector3.Lerp(cursorWayPoint.transform.position, ProposeTransformPosition(), 0.2f);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject newWaypoint = (GameObject)Instantiate(Waypoint, cursorWayPoint.transform.position, Quaternion.identity);
                newWaypoint.transform.SetParent(GameObject.Find("Waypoints").transform);
                WorldAnchorManager.Instance.AttachAnchor(newWaypoint, newWaypoint.GetInstanceID().ToString());
            }
			else if (Input.GetKeyDown(KeyCode.L))
			{
				WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;
				string[] ids = store.GetAllIds();
				for (int index = 0; index < ids.Length; index++)
				{
					GameObject newWaypoint = (GameObject)Instantiate(Waypoint, Waypoint.transform.position, Quaternion.identity);
					store.Load(ids[index], newWaypoint);
				}
			}
			else if (Input.GetKeyDown(KeyCode.C))
			{
				GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

				for (int index = 0; index < waypoints.Length; index++)
				{
					if (waypoints[index] != Waypoint)
					{
						Destroy(waypoints[index]);
					}
				}

				WorldAnchorStore store = WorldAnchorManager.Instance.AnchorStore;
				store.Clear();
			}
		}
        else
        {
            DestroyObject(cursorWayPoint);
        }
    }



    Vector3 ProposeTransformPosition()
    {
        Vector3 retval = GameObject.Find("CursorWithFeedback").transform.position;

        return retval;
    }
}
