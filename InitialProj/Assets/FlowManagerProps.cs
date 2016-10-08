using UnityEngine;
using System.Collections;

public class FlowManagerProps : MonoBehaviour {

    public bool IsCurrentWayPoint;
    public bool ReachedCurrentWayPoint;
    private MeshRenderer meshR;
	// Use this for initialization
	void Start () {
        meshR = gameObject.GetComponentInChildren<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if(!GameObject.Find("Managers").GetComponent<CursorSetupManager>().SetupMode)
            meshR.enabled = IsCurrentWayPoint;
        if (ReachedCurrentWayPoint)
            gameObject.transform.Rotate(0.0f, 0.0f, 1.0f);
	}
}
