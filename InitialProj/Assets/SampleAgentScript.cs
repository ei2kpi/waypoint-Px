using UnityEngine;
using System.Collections;

public class SampleAgentScript : MonoBehaviour
{
    NavMeshAgent agent;

    // Use this for initialization
    void Start()

    {

        agent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        for (int index = 0; index < waypoints.Length; index++)
        {
            if (waypoints[index].GetComponent<FlowManagerProps>().CurrWayPointState == FlowManager.WaypointState.GoToWaypoint)
            {
                agent.SetDestination(waypoints[index].transform.position);
            }
        }
    }
}