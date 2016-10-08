using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class WaypointManager : MonoBehaviour
{
    public GameObject Waypoint;

    GestureRecognizer recognizer;

    void Start()
    {
        Debug.Log("start");
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);

        recognizer.TappedEvent += Recognizer_TappedEvent;

        recognizer.StartCapturingGestures();
    }

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        Debug.Log("tap event");
        var direction = headRay.direction;

        var origin = headRay.origin;

        var position = origin + direction * 2.0f;

        Instantiate(Waypoint, position, Quaternion.identity);
    }
}