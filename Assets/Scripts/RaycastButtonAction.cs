
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class RaycastButtonAction : MonoBehaviour
{
    [Header("Timings")]
    [Tooltip("Tiempo que el dedo debe mantener la pulsación para activarla (ej: 0.5s).")]
    public float requiredPressDuration = 0.5f;

    [Tooltip("Retardo entre la confirmación de la pulsación y la ejecución de la acción (ej: 0.3s).")]
    public float actionDelay = 0.3f;

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
    private bool isHovering = false; // El raycast del dedo está sobre el botón
    void Start()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
        
    }

    public void OnRaycastEnter()
    {
        if (!enabled) return;

        if (isHovering) return;
        
        isHovering = true;
        isConfirmed = false;
        pressTimer = 0f;
        
        if (buttonImage != null)
        {
            // Feedback visual al entrar/apuntar
            buttonImage.color = pressedColor; 
        }
    }
    public void OnRaycastExit()
    {   
        if (!enabled) return;

        if (isHovering)
        {
            OnButtonRelease.Invoke();
        }
        ResetButtonState();
    }

    public void OnRaycastStay()
    {
        if (!enabled) return;

        if (isConfirmed) return;

        pressTimer += Time.deltaTime; // Incrementa el temporizador

        // Si la demora se cumple:
        if (pressTimer >= requiredPressDuration)
        {
            isConfirmed = true;
            actionCoroutine = StartCoroutine(ConfirmAndInvokeAction());
        }
    }
    
    // ... (OnTriggerExit y Coroutines son correctos)
    private void OnTriggerExit(Collider other)
    {
        ResetButtonState();
    }

    private IEnumerator ConfirmAndInvokeAction()
    {
        yield return new WaitForSeconds(actionDelay);
        OnButtonPress.Invoke();
        //ResetButtonState();
    }

    public void ResetButtonState()
    {
        if (actionCoroutine != null)
        {
            StopCoroutine(actionCoroutine);
            actionCoroutine = null;
        }
        
        pressTimer = 0f;
        isConfirmed = false;
        isHovering = false;
        
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }
}