using UnityEngine;
using System.Collections;

public class ActivateDeactivateWayPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    private void OnTriggerEnter (Collider myTrigger)
    {
        Debug.Log("COLLIDED!");
    }
}
