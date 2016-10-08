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
            GameObject cursorWaypoint = Instantiate(Waypoint);
            GestureManager.Instance.OverrideFocusedObject = cursorWaypoint;
        }
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, ProposeTransformPosition(), 0.2f);
    }

    Vector3 ProposeTransformPosition()
    {
        // Put the model 2m in front of the user.
        Vector3 retval = Camera.main.transform.position + Camera.main.transform.forward * 2;

        return retval;
    }

    public void OnSelect()
    {
        Instantiate(Waypoint, Camera.main.transform.position, Quaternion.identity);
    }
}
