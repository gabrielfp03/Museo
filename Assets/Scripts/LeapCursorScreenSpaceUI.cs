using UnityEngine;
using Leap;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LeapCursorScreenSpaceUI : MonoBehaviour
{
    [Header("Referencias de Leap")]
    [Tooltip("El componente Leap Service Provider de la escena.")]
    public LeapServiceProvider leapProvider;
    [Tooltip("La cámara principal que renderiza el Canvas.")]
    public Camera mainCamera;
    
    [Header("Configuración de UI")]
    [Tooltip("El componente Graphic Raycaster del Canvas (Screen Space - Camera).")]
    public GraphicRaycaster targetRaycaster; 
    [Tooltip("El Event System principal de la escena.")]
    public EventSystem eventSystem; 
    
    [Header("Configuración del Dedo")]
    [Tooltip("El tipo de dedo que se usará para el raycasting (normalmente el índice).")]
    public Finger.FingerType fingerType = Finger.FingerType.INDEX;
    
    // Variables internas para la interacción UI
    private RectTransform cursorRect;
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResults;
    private RaycastButtonAction currentHoverButton = null;

    void Start()
    {
        // 1. Inicialización de referencias clave
        if (mainCamera == null) mainCamera = Camera.main;
        if (leapProvider == null) leapProvider = FindAnyObjectByType<LeapServiceProvider>();

        // 2. Verificación de componentes UI
        cursorRect = GetComponent<RectTransform>();

        if (cursorRect == null)
        {
            Debug.LogError("El cursor de Screen Space debe ser un elemento UI (RectTransform).");
            enabled = false;
            return;
        }

        if (targetRaycaster == null)
        {
            Debug.LogError("Asigna el Graphic Raycaster del Canvas.");
            enabled = false;
            return;
        }

        // 3. Inicialización del sistema de eventos
        if (eventSystem == null) eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem == null) { Debug.LogError("EventSystem no encontrado."); enabled = false; return; }

        pointerEventData = new PointerEventData(eventSystem);
        raycastResults = new List<RaycastResult>();
    }

    void Update()
    {
        if (!enabled || leapProvider == null) { HandleExit(); return; }

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
        
        // Obtenemos la posición 3D de la punta del dedo (World Space)
        Vector3 fingerTipWorld = hand.GetFinger(fingerType).TipPosition;
        
        // 1. Proyectar la posición 3D a COORDENADAS DE PANTALLA
        // Unity traduce las coordenadas 3D a píxeles 2D.
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(fingerTipWorld);

        // Si el dedo está detrás de la cámara, salimos
        if (screenPoint.z < 0)
        {
            HandleExit();
            return;
        }

        // 2. Posicionar el cursor 2D
        cursorRect.position = screenPoint;

        // 3. Ejecutar el Raycast de UI (No es Physics.Raycast, es de la UI)
        pointerEventData.position = screenPoint;
        raycastResults.Clear();
        targetRaycaster.Raycast(pointerEventData, raycastResults);

        // 4. Procesar los resultados y activar la acción del botón
        HandleHit(raycastResults);
    }
    
    private void HandleHit(List<RaycastResult> results)
    {
        RaycastButtonAction hitButtonAction = null;
        
        if (results.Count > 0)
        {
            // El primer resultado es el elemento de UI más cercano
            hitButtonAction = results[0].gameObject.GetComponent<RaycastButtonAction>();
        }

        if (hitButtonAction != null)
        {
            // Lógica de Hover/Enter/Stay
            if (currentHoverButton != hitButtonAction)
            {
                if (currentHoverButton != null) currentHoverButton.OnRaycastExit();
                currentHoverButton = hitButtonAction;
                currentHoverButton.OnRaycastEnter();
            }
            currentHoverButton.OnRaycastStay(); 
        }
        else // No hay hit o el hit no tiene la acción
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