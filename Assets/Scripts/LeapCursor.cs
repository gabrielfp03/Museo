
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

    public LayerMask UILayer; 
    public float raycastDistance = 10f;

    private static LeapCursor instance;
    private RaycastButtonAction currentHoverButton = null;

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
        if (leapProvider == null) {
            HandleExit();
            return;
        }

        Frame frame = leapProvider.CurrentFrame;
        if (frame.Hands.Count == 0) {
            HandleExit();
            return;
        }
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
        if (hand == null) {HandleExit(); return;}
        
        // 1. Obtener la posición de la punta del dedo índice en COORDENADAS DE MUNDO Leap
        Vector3 fingerTipWorld = hand.GetFinger(fingerType).TipPosition;

        Vector3 rayDir = hand.GetFinger(fingerType).Direction;
        
        Vector3 newCursorPosition = fingerTipWorld;
        
        newCursorPosition.z += depthOffset; 

        transform.position = newCursorPosition;

        RaycastHit hit;
        bool hitButton = Physics.Raycast(fingerTipWorld, rayDir, out hit, raycastDistance, UILayer);

        if (hitButton)
        {
            // Intentar obtener el script de acción del botón
            RaycastButtonAction button = hit.collider.GetComponent<RaycastButtonAction>();
            if (button != null)
            {
                // Entró en un botón o se mantuvo en él
                if (currentHoverButton != button)
                {
                    // Si estaba en otro botón, salimos de él primero
                    if (currentHoverButton != null)
                    {
                        currentHoverButton.OnRaycastExit();
                    }
                    currentHoverButton = button;
                    currentHoverButton.OnRaycastEnter(); // Inicia el feedback visual
                }
                
                // MANTENER LA PULSACIÓN TEMPORIZADA: Esto incrementa el pressTimer
                currentHoverButton.OnRaycastStay(); 
            }
            else
            {
                // Acertó algo en la capa UI, pero no era un botón válido
                HandleExit();
            }
        }
        else // No acertó nada
        {
            HandleExit();
        }
    }
    private void HandleExit()
    {
        if (currentHoverButton != null)
        {
            currentHoverButton.OnRaycastExit();
            currentHoverButton = null;
        }
    }
}