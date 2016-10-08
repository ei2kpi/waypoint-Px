using UnityEngine;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.Persistence;

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
    }



    Vector3 ProposeTransformPosition()
    {
        // Put the model 2m in front of the user.
        Vector3 retval = GameObject.Find("CursorWithFeedback").transform.position;

        return retval;
    }
}
