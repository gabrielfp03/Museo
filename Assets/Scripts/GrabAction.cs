using UnityEngine;


public class GrabAction : MonoBehaviour
{
    public Leap.GrabDetector grabDetector;
    private bool wasGrabbingLastFrame = false;

    void Update()
    {
        if (grabDetector == null)
            return;

        // Comenzó a agarrar este frame
        if (grabDetector.GrabStartedThisFrame)
        {
            Debug.Log("[GrabLogger] Mano comenzó a agarrar");
            Debug.Log("GrabStrength: " + grabDetector.IsGrabbing);
            wasGrabbingLastFrame = true;
        }
        // Soltó el objeto
        else if (wasGrabbingLastFrame && !grabDetector.IsGrabbing)
        {
            Debug.Log("[GrabLogger] Mano soltó el objeto");
            Debug.Log("GrabStrength: " + grabDetector.IsGrabbing);
            wasGrabbingLastFrame = false;
        }
    }
}

