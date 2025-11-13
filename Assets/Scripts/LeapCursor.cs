// using Leap;
// using UnityEngine;
// using UnityEngine.InputSystem; 

// public class LeapCursor : MonoBehaviour
// {
//     public LeapServiceProvider leapProvider;
//     public Finger.FingerType fingerType = Finger.FingerType.INDEX;
//     public Camera mainCamera;

//     private float zPos;
//     private float xPos;
//     private float yPos;

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         zPos = Camera.main.transform.position.z + Camera.main.nearClipPlane + 10.0f;
//     }

//     private void Update()
//     {
//         Vector3 mousePos = MousePosition();
//         xPos = mousePos.x;
//         yPos = mousePos.y;

//         transform.position = new Vector3(xPos, yPos, zPos);
//     }

//     private Vector3 MousePosition()
//     {
//         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

//         return mousePos;
//     }
// }

using UnityEngine;
using Leap;

public class LeapCursor : MonoBehaviour
{
    [Header("Referencia al Leap Service Provider")]
    public LeapServiceProvider leapProvider;

    [Header("Tipo de dedo a usar")]
    public Finger.FingerType fingerType = Finger.FingerType.INDEX; // Dedo índice

    [Header("Cámara principal")]
    public Camera mainCamera;

    [Header("Offset en Z para que aparezca sobre la UI")]
    public float zOffset;
    private static LeapCursor instance;

    void Awake()
    {
        // Singleton: si ya hay un cursor, destruimos duplicados
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        // DontDestroyOnLoad(gameObject); // <-- Esto hace que el cursor persista entre escenas

        // if (mainCamera == null)
        //     mainCamera = Camera.main;

        // if (leapProvider == null)
        //     leapProvider = FindAnyObjectByType<LeapServiceProvider>();
 
    }

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (leapProvider == null)
            leapProvider = FindAnyObjectByType<LeapServiceProvider>();

        zOffset = Camera.main.transform.position.z + Camera.main.nearClipPlane + 1300.0f;
        
    }

    void Update()
    {

        if (leapProvider == null) return;

        Frame frame = leapProvider.CurrentFrame;

        if (frame.Hands.Count == 0) return;
        
        Hand hand = frame.Hands[0];  // Primera mano detectada
        if (hand == null) return;

        Vector3 fingerTip = hand.Index.TipPosition;  // Punta del dedo índice

        // Convertimos a coordenadas de pantalla y luego a mundo
        Vector3 screenPos = mainCamera.WorldToScreenPoint(fingerTip);
        Vector3 cursorPos = mainCamera.ScreenToWorldPoint(new Vector3(
            screenPos.x,
            screenPos.y,
            zOffset
        ));

        transform.position = cursorPos;
    }
}
