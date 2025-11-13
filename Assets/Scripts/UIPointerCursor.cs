using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Leap;

public class UIPointerCursor : MonoBehaviour
{
    public LeapServiceProvider leapProvider;
    public Camera mainCamera;
    public float pinchThreshold = 0.75f;

    private GameObject currentButton;
    private bool wasPinching = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (leapProvider == null)
            leapProvider = FindAnyObjectByType<LeapServiceProvider>();
    }

    void Update()
    {
        if (leapProvider == null) return;

        Frame frame = leapProvider.CurrentFrame;
        if (frame.Hands.Count == 0) return;

        Hand hand = frame.Hands[0];
        Vector3 fingerTip = hand.Index.TipPosition;

        // Transformar a pantalla
        Vector3 screenPos = mainCamera.WorldToScreenPoint(fingerTip);

        // Lanzar rayo al UI
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(screenPos.x, screenPos.y)
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        GameObject hovered = null;
        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                hovered = result.gameObject;
                break;
            }
        }

        currentButton = hovered;

        // // Detectar pinch
        // bool isPinching = hand.PinchStrength > pinchThreshold;

        // if (currentButton != null)
        // {
        //     // Visual feedback opcional: Select()
        //     currentButton.GetComponent<Button>().Select();

        //     if (isPinching && !wasPinching)
        //     {
        //         currentButton.GetComponent<Button>().onClick.Invoke();
        //     }
        // }

        // wasPinching = isPinching;
    }
}
