using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Leap;
using System.Collections.Generic;

public class LeapPinchInteractor : MonoBehaviour
{
    [Header("Referencias Necesarias")]
    public LeapServiceProvider leapProvider;
    public Camera mainCamera;
    public EventSystem eventSystem;

    [Header("Configuración de Interacción")]
    [Tooltip("Umbral de fuerza del pellizco (0.0 a 1.0) para registrar un 'clic'.")]
    [Range(0.0f, 1.0f)]
    public float pinchThreshold = 0.75f;
    
    [Tooltip("Tiempo de Cooldown después de un pinch para evitar clics múltiples rápidos.")]
    public float clickCooldown = 0.2f;

    private GameObject currentTarget;
    private bool wasPinching = false;
    private float lastClickTime = 0f;

    void Start()
    {
        // Inicialización segura de referencias
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (leapProvider == null)
            leapProvider = FindAnyObjectByType<LeapServiceProvider>();
            
        if (eventSystem == null)
            eventSystem = EventSystem.current;
    }

    void Update()
    {
        if (leapProvider == null || eventSystem == null || mainCamera == null) return;

        Frame frame = leapProvider.CurrentFrame;
        // Sólo procesamos si hay al menos una mano
        if (frame.Hands.Count == 0)
        {
            HandleHoverExit(null);
            return;
        }

        Hand hand = frame.Hands[0];
        // Usamos la posición del dedo índice para apuntar
        Vector3 fingerTip = hand.Index.TipPosition;

        // 1. Mapeo a Coordenadas de Pantalla
        Vector3 screenPos = mainCamera.WorldToScreenPoint(fingerTip);

        // 2. Raycasting de UI
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = new Vector2(screenPos.x, screenPos.y)
        };

        var raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, raycastResults);

        GameObject newTarget = null;
        // Buscamos el primer objeto con un componente interactivo
        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<Selectable>() != null)
            {
                newTarget = result.gameObject;
                break;
            }
        }

        // 3. Manejo de Hover/Focus
        HandleHoverExit(newTarget);
        currentTarget = newTarget;
        HandleHoverEnter(currentTarget);


        // 4. Manejo de Pinch/Click
        bool isPinching = hand.PinchStrength > pinchThreshold;

        if (currentTarget != null)
        {
            // Ejecutar 'Submit' (equivale a un clic en la mayoría de los casos)
            if (isPinching && !wasPinching && Time.time > lastClickTime + clickCooldown)
            {
                ExecuteEvents.Execute(currentTarget, pointerData, ExecuteEvents.submitHandler);
                // Si el elemento es un botón, también podemos invocar su evento onClick
                Button button = currentTarget.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                }
                lastClickTime = Time.time;
            }
        }

        wasPinching = isPinching;
    }

    private void HandleHoverExit(GameObject newTarget)
    {
        // Si el objetivo ha cambiado (o es nulo), disparamos PointerExit en el objetivo anterior
        if (currentTarget != null && currentTarget != newTarget)
        {
            ExecuteEvents.Execute(currentTarget, new PointerEventData(eventSystem), ExecuteEvents.pointerExitHandler);
            // También se puede llamar a Deselect() si se quiere forzar la pérdida de foco visual.
        }
    }

    private void HandleHoverEnter(GameObject newTarget)
    {
        // Si hay un nuevo objetivo y no es el mismo que el anterior, disparamos PointerEnter
        if (newTarget != null && newTarget != currentTarget)
        {
            PointerEventData pointerData = new PointerEventData(eventSystem);
            ExecuteEvents.Execute(newTarget, pointerData, ExecuteEvents.pointerEnterHandler);
            // Esto le da foco visual (el efecto de resaltado por defecto de Unity)
            eventSystem.SetSelectedGameObject(newTarget);
        }
    }
}