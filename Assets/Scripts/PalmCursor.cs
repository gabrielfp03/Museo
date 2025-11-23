using UnityEngine;
using Leap;
using System.Linq;
using UnityEngine.UI;

public class PalmCursor : MonoBehaviour
{
    [Header("Referencias de Leap Motion")]
    [Tooltip("Arrastra aquí el Leap Service Provider de la escena.")]
    public LeapServiceProvider leapProvider;

    [Header("Configuración de Raycast")]
    [Tooltip("Offset aplicado a la profundidad (Z) de la palma para posicionar el cursor.")]
    public float depthOffset = 0.2f; 
    public LayerMask UILayer; 
    public float raycastDistance = 100f;
    
    [Header("Visualización")]
    [Tooltip("La cámara principal de la escena.")]
    public Camera mainCamera;

    // NOTA: Usamos 'currentHoverButton' de tipo object para poder manejar el Raycast original
    // y el PalmPushAction si fuera necesario, pero lo ideal es que este cursor SOLO busque PalmPushAction.
    private PushButtonAction currentHoverButton = null; 

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (leapProvider == null)
            leapProvider = FindAnyObjectByType<LeapServiceProvider>();
        
        // Asegúrate de que este GameObject tenga la etiqueta 'PalmRaycast'
        if (!CompareTag("PalmRaycast"))
        {
            Debug.LogWarning("PalmCursor: Por favor, asegúrate de que el GameObject '" + gameObject.name + 
                             "' tenga la etiqueta 'PalmRaycast' para que el botón exclusivo funcione.");
        }
    }

    void Update()
    {
        if (leapProvider == null || leapProvider.CurrentFrame.Hands.Count == 0) 
        {
            HandleExit();
            return;
        }

        Frame frame = leapProvider.CurrentFrame;
        Hand hand = frame.Hands.FirstOrDefault(h => h.IsRight || h.IsLeft);
        
        if (hand == null) 
        {
            HandleExit(); 
            return;
        }
        
        // 1. OBTENER POSICIÓN Y DIRECCIÓN DE LA PALMA
        Vector3 palmPositionWorld = hand.PalmPosition;
        Vector3 rayDir = hand.PalmNormal; // Rayo saliendo de la palma
        
        // 2. ACTUALIZAR POSICIÓN DEL CURSOR VISUAL
        Vector3 newCursorPosition = palmPositionWorld + rayDir * depthOffset;
        transform.position = newCursorPosition;

        // 3. REALIZAR EL RAYCAST
        RaycastHit hit;
        bool hitButton = Physics.Raycast(palmPositionWorld, rayDir, out hit, raycastDistance, UILayer);
        Color rayColor = hitButton ? Color.green : Color.red; // Verde si golpea, Rojo si falla
        Debug.DrawRay(palmPositionWorld, rayDir * raycastDistance, rayColor);
        if (hitButton)
        {
            // Intentar obtener el script de acción EXCLUSIVO de la palma
            PushButtonAction button = hit.collider.GetComponent<PushButtonAction>();
            
            if (button != null)
            {
                if (currentHoverButton != button)
                {
                    // Salir del botón anterior, pasando la referencia de ESTE objeto
                    if (currentHoverButton != null)
                    {
                        currentHoverButton.OnRaycastExit(gameObject); 
                    }
                    // Entrar en el nuevo botón, pasando la referencia de ESTE objeto
                    currentHoverButton = button;
                    currentHoverButton.OnRaycastEnter(gameObject); 
                }
                
                // MANTENER LA PULSACIÓN TEMPORIZADA (no requiere pasar la referencia aquí)
                currentHoverButton.OnRaycastStay(); 
            }
            else
            {
                HandleExit();
            }
        }
        else 
        {
            HandleExit();
        }
    }
    
    private void HandleExit()
    {
        if (currentHoverButton != null)
        {
            // Llamamos a OnRaycastExit, pasando la referencia de ESTE objeto que se va
            currentHoverButton.OnRaycastExit(gameObject); 
            currentHoverButton = null;
        }
    }
}