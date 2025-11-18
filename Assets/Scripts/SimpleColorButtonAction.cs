
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class SimpleColorButtonAction : MonoBehaviour
{
    [Header("Timings")]
    [Tooltip("Tiempo que el dedo debe mantener la pulsación para activarla (ej: 0.5s).")]
    public float requiredPressDuration = 0.5f;

    [Tooltip("Retardo entre la confirmación de la pulsación y la ejecución de la acción (ej: 0.3s).")]
    public float actionDelay = 0.3f;

    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color pressedColor = Color.green;

    [Tooltip("Distancia de penetración mínima (ej: 0.01).")]
    private float clickThreshold = 0.2f;

    [Header("Button Action")]
    public UnityEvent OnButtonPress = new UnityEvent();

    private Image buttonImage;
    private bool isConfirmed = false; 
    private float pressTimer = 0f;    
    private Coroutine actionCoroutine; 

    void Start()
    {
        Debug.Log("1. Botón Inicializado: " + gameObject.name);
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("SimpleColorButtonAction requiere un componente Image.");
            enabled = false;
        }
        buttonImage.color = normalColor;
    }

    private void OnTriggerStay(Collider other)
    {
        // 1. COMPROBACIONES DE SEGURIDAD (Null/Destruido)
        if (other == null || other.transform == null) 
        {
            ResetButtonState();
            return;
        }
        
        Debug.Log("2. ¡Trigger Detectado! Colisiona con: " + other.gameObject.name);
        
        if (isConfirmed) return; // Si ya se confirmó, no hacemos nada más.

        float penetrationDepth = other.transform.InverseTransformPoint(transform.position).z;
        Debug.Log("Profundidad Z: " + penetrationDepth);

        // 2. LÓGICA DE PULSACIÓN TEMPORIZADA (CORECCIÓN AQUÍ)
        // Dedo DENTRO del umbral
        if (penetrationDepth < clickThreshold)
        {
            // Aumentar el temporizador
            pressTimer += Time.deltaTime;

            // Si el temporizador ha superado el tiempo requerido Y AÚN NO se ha confirmado:
            if (pressTimer >= requiredPressDuration && !isConfirmed)
            {
                // **3. PULSACIÓN CONFIRMADA**
                isConfirmed = true;
                
                // Feedback visual INMEDIATO
                buttonImage.color = pressedColor; 

                // 4. Iniciar el retardo de acción
                actionCoroutine = StartCoroutine(ConfirmAndInvokeAction());
            }
        }
        // Dedo FUERA del umbral o liberado antes del tiempo
        else
        {
            ResetButtonState();
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
        Debug.Log("3. ¡Pulsación Confirmada! Ejecutando acción.");
        OnButtonPress.Invoke();
        ResetButtonState();
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
        
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }
}