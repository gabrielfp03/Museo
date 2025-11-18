// // using Leap;
// // using UnityEngine;
// // using UnityEngine.InputSystem; 

// // public class LeapCursor : MonoBehaviour
// // {
// //     public LeapServiceProvider leapProvider;
// //     public Finger.FingerType fingerType = Finger.FingerType.INDEX;
// //     public Camera mainCamera;

// //     private float zPos;
// //     private float xPos;
// //     private float yPos;

// //     // Start is called once before the first execution of Update after the MonoBehaviour is created
// //     void Start()
// //     {
// //         zPos = Camera.main.transform.position.z + Camera.main.nearClipPlane + 10.0f;
// //     }

// //     private void Update()
// //     {
// //         Vector3 mousePos = MousePosition();
// //         xPos = mousePos.x;
// //         yPos = mousePos.y;

// //         transform.position = new Vector3(xPos, yPos, zPos);
// //     }

// //     private Vector3 MousePosition()
// //     {
// //         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

// //         return mousePos;
// //     }
// // }

// using UnityEngine;
// using Leap;

// public class LeapCursor : MonoBehaviour
// {
//     [Header("Referencia al Leap Service Provider")]
//     public LeapServiceProvider leapProvider;

//     [Header("Tipo de dedo a usar")]
//     public Finger.FingerType fingerType = Finger.FingerType.INDEX; // Dedo índice

//     [Header("Cámara principal")]
//     public Camera mainCamera;

//     [Header("Offset en Z para que aparezca sobre la UI")]
//     public float zOffset;
//     private static LeapCursor instance;

//     void Awake()
//     {
//         // Singleton: si ya hay un cursor, destruimos duplicados
//         if (instance != null && instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }

//         instance = this;
//         // DontDestroyOnLoad(gameObject); // <-- Esto hace que el cursor persista entre escenas

//         // if (mainCamera == null)
//         //     mainCamera = Camera.main;

//         // if (leapProvider == null)
//         //     leapProvider = FindAnyObjectByType<LeapServiceProvider>();
 
//     }

//     void Start()
//     {
//         if (mainCamera == null)
//             mainCamera = Camera.main;

//         if (leapProvider == null)
//             leapProvider = FindAnyObjectByType<LeapServiceProvider>();

//         zOffset = Camera.main.transform.position.z + Camera.main.nearClipPlane + 1300.0f;
        
//     }

//     void Update()
//     {

//         if (leapProvider == null) return;

//         Frame frame = leapProvider.CurrentFrame;

//         if (frame.Hands.Count == 0) return;
        
//         Hand hand = frame.Hands[0];  // Primera mano detectada
//         if (hand == null) return;

//         Vector3 fingerTip = hand.Index.TipPosition;  // Punta del dedo índice

//         // Convertimos a coordenadas de pantalla y luego a mundo
//         Vector3 screenPos = mainCamera.WorldToScreenPoint(fingerTip);
//         Vector3 cursorPos = mainCamera.ScreenToWorldPoint(new Vector3(
//             screenPos.x,
//             screenPos.y,
//             zOffset
//         ));

//         transform.position = cursorPos;
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

    [Header("Offset aplicado a la profundidad (Z) del dedo")]
    [Tooltip("Ajusta este valor para colocar el cursor delante o detrás del dedo.")]
    public float depthOffset = 0.05f; // Un pequeño offset en metros

    private static LeapCursor instance;

    void Awake()
    {
        // (El código Singleton Awake/Start está bien, lo omitimos para brevedad)
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

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
        
        // Buscamos la primera mano válida
        Hand hand = null;
        foreach(var h in frame.Hands)
        {
            if (h.IsRight || h.IsLeft) // Filtro simple
            {
                hand = h;
                break;
            }
        }
        if (hand == null) return;
        
        // 1. Obtener la posición de la punta del dedo índice en COORDENADAS DE MUNDO Leap
        Vector3 fingerTipWorld = hand.GetFinger(fingerType).TipPosition;

        // 2. Aplicar un pequeño offset en la dirección del dedo (opcional, pero ayuda)
        // Usaremos la posición directa y ajustaremos si es necesario.
        
        // 3. Simplemente asignamos la posición de mundo.
        // Si el cursor es hijo de la cámara, las coordenadas serán relativas.
        // Si el cursor está en el root, serán coordenadas de mundo.
        
        // Usaremos un offset simple en el eje X/Y/Z local del cursor si es necesario 
        // para que aparezca delante del dedo, pero la posición base es la punta.
        
        Vector3 newCursorPosition = fingerTipWorld;
        
        // Opción de Offset simple (Mover el cursor ligeramente hacia adelante):
        // Esto depende de cómo está orientada la mano. Si la mano mira hacia Z+, 
        // restamos de Z, si mira a Z-, sumamos. Lo más fácil es ajustar la posición
        // visualmente con la variable 'depthOffset' en el inspector.
        
        // Si tu mano está frente a la cámara, el eje que importa es la profundidad.
        // Asumiendo que el cursor es independiente de la mano y sigue la punta:
        
        // Mover el cursor un poco más cerca/lejos de la cámara que el dedo.
        // Esto asume que la Z de la cámara está alineada con el mundo.
        newCursorPosition.z += depthOffset; 

        transform.position = newCursorPosition;
    }
}