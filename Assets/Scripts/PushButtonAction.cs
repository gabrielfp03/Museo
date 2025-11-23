using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class PushButtonAction : MonoBehaviour
{
    // *** VARIABLE CLAVE: Debe coincidir con la etiqueta del PalmCursor ***
    [Header("Exclusividad de Activación")]
    [Tooltip("Solo responderá al GameObject con esta etiqueta (ej: 'PalmRaycast').")]
    public string requiredRaycasterTag = "PalmRaycast"; 

    [Header("Timings")]
    [Tooltip("Tiempo que la palma debe apuntar para confirmar (ej: 0.1s).")]
    public float requiredPressDuration = 0.1f; 
    public float actionDelay = 0.1f;

    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color pressedColor = Color.green;

    [Header("Button Action")]
    public UnityEvent OnButtonPress = new UnityEvent();
    public UnityEvent OnButtonRelease = new UnityEvent();
    
    private Image buttonImage;
    private bool isConfirmed = false; 
    private float pressTimer = 0f;    
    private Coroutine actionCoroutine; 
    private bool isHovering = false; 
    private GameObject currentActiveRaycaster = null; 

    void Start()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }
    
    // ======================================================
    // === MÉTODOS DE ENTRADA Y SALIDA (CON VERIFICACIÓN) ===
    // ======================================================

    public void OnRaycastEnter(GameObject raycaster)
    {
        // 🛑 COMPROBACIÓN CLAVE: Si el objeto no tiene la etiqueta correcta, ignorar.
        if (!raycaster.CompareTag(requiredRaycasterTag))
        {
            return;
        }

        if (isHovering) return;
        
        isHovering = true;
        isConfirmed = false;
        pressTimer = 0f;
        currentActiveRaycaster = raycaster; // Guardamos la referencia
        
        if (buttonImage != null)
        {
            buttonImage.color = pressedColor; 
        }
    }
    
    public void OnRaycastExit(GameObject raycaster)
    {   
        // 🛑 COMPROBACIÓN CLAVE: Solo salimos si es el puntero activo el que se va.
        if (raycaster != currentActiveRaycaster)
        {
            return;
        }
        
        if (isHovering)
        {
            OnButtonRelease.Invoke();
        }
        ResetButtonState();
    }

    public void OnRaycastStay()
    {
       if (!isHovering || isConfirmed) return;

        pressTimer += Time.deltaTime; 

        if (pressTimer >= requiredPressDuration)
        {
            isConfirmed = true;
            actionCoroutine = StartCoroutine(ConfirmAndInvokeAction());
        }
    }
    
    private IEnumerator ConfirmAndInvokeAction()
    {
        yield return new WaitForSeconds(actionDelay);
        OnButtonPress.Invoke();
    }

    private void ResetButtonState()
    {
        if (actionCoroutine != null)
        {
            StopCoroutine(actionCoroutine);
            actionCoroutine = null;
        }
        
        pressTimer = 0f;
        isConfirmed = false;
        isHovering = false;
        currentActiveRaycaster = null;
        
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }
}