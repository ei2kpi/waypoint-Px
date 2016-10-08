using UnityEngine;
using HoloToolkit.Unity;
using System.Collections;

public class CursorSetupManager : Singleton<CursorSetupManager> {
    public GameObject Waypoint;
    public bool SetupMode = true;
    private GameObject cursorWayPoint;

    // Use this for initialization
    void Start () {
        if (SetupMode)
        {
            cursorWayPoint = (GameObject)Instantiate(Waypoint, ProposeTransformPosition(), Quaternion.identity);
            cursorWayPoint.transform.SetParent(GameObject.Find("Waypoints").transform);
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
