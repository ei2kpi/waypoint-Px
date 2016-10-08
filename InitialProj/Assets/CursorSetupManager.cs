using UnityEngine;
using HoloToolkit.Unity;
using System.Collections;

public class CursorSetupManager : Singleton<CursorSetupManager> {
    public GameObject Waypoint;
    public bool SetupMode = true;

    // Use this for initialization
    void Start () {
        if (SetupMode)
        {
            GestureManager.Instance.OverrideFocusedObject = Waypoint;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (SetupMode)
        {
            Waypoint.transform.position = Vector3.Lerp(Waypoint.transform.position, ProposeTransformPosition(), 0.2f);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject newWaypoint = (GameObject)Instantiate(Waypoint, Waypoint.transform.position, Quaternion.identity);
                WorldAnchorManager.Instance.AttachAnchor(newWaypoint, newWaypoint.GetInstanceID().ToString());
            }
        }
    }



    Vector3 ProposeTransformPosition()
    {
        // Put the model 2m in front of the user.
        Vector3 retval = GameObject.Find("CursorWithFeedback").transform.position;

        return retval;
    }
}
