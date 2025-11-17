// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Events;
// using System.Collections; // ¡Necesario para Coroutines!

// public class SimpleColorButtonAction : MonoBehaviour
// {
//     [Header("Timings")]
//     [Tooltip("Tiempo que el dedo debe mantener la pulsación para activarla (ej: 0.5s).")]
//     public float requiredPressDuration = 0.5f;

//     [Tooltip("Retardo entre la confirmación de la pulsación y la ejecución de la acción (ej: 0.3s).")]
//     public float actionDelay = 0.3f;

//     [Header("Color Settings")]
//     public Color normalColor = Color.white;
//     public Color pressedColor = Color.green;

//     [Tooltip("Distancia de penetración mínima (ej: 0.01).")]
//     public float clickThreshold = 0.01f;

//     [Header("Button Action")]
//     public UnityEvent OnButtonPress = new UnityEvent();

//     private Image buttonImage;
//     private bool isConfirmed = false; // Indica si se ha confirmado el tiempo de pulsación
//     private float pressTimer = 0f;    // Temporizador para la duración de la pulsación
//     private Coroutine actionCoroutine; // Referencia a la coroutine de acción

//     void Start()
//     {
//         buttonImage = GetComponent<Image>();
//         if (buttonImage == null)
//         {
//             Debug.LogError("SimpleColorButtonAction requiere un componente Image.");
//             enabled = false;
//         }
//         buttonImage.color = normalColor;
//     }

//     // private void OnTriggerStay(Collider other)
//     // {
//     //     // Si ya está confirmado, no hacemos nada hasta que el dedo se retire
//     //     if (isConfirmed) return; 

//     //     float penetrationDepth = other.transform.InverseTransformPoint(transform.position).z;
        
//     //     // 1. Dedo DENTRO del umbral (Está intentando presionar)
//     //     if (penetrationDepth < -clickThreshold)
//     //     {
//     //         // Aumentar el temporizador
//     //         pressTimer += Time.deltaTime;

//     //         // Si el temporizador ha superado el tiempo requerido Y AÚN NO se ha confirmado:
//     //         if (pressTimer >= requiredPressDuration && !isConfirmed)
//     //         {
//     //             // **CONFIRMACIÓN DE PULSACIÓN**
//     //             isConfirmed = true;
//     //             buttonImage.color = pressedColor; // Cambio de color INMEDIATO al confirmar

//     //             // 2. Iniciar el retardo de acción
//     //             actionCoroutine = StartCoroutine(ConfirmAndInvokeAction());
//     //         }
//     //     }
//     //     // 3. Dedo FUERA del umbral (Está soltando o no ha presionado lo suficiente)
//     //     else
//     //     {
//     //         ResetButtonState();
//     //     }
//     // }
//     private void OnTriggerStay(Collider other)
// {
//     Debug.Log("2. ¡Trigger Detectado! Colisiona con: " + other.gameObject.name);
//     // === COMPROBACIÓN DE SEGURIDAD (FIX) ===
//     // 1. Verificar si el objeto que colisiona ha sido destruido o no es válido.
//     if (other == null || other.transform == null) 
//     {
//         // Si el objeto desapareció (destruido), restablecer el estado y salir.
//         ResetButtonState();
//         return;
//     }
//     // === FIN DEL FIX ===

//     // Si ya está confirmado, no hacemos nada hasta que el dedo se retire
//     if (isConfirmed) return; 

//     // Línea donde ocurría el error (ahora segura):
//     float penetrationDepth = other.transform.InverseTransformPoint(transform.position).z;
    
//     // ... resto de tu lógica de pulsación ...
    
//     if (penetrationDepth < -clickThreshold)
//     {
//         isConfirmed = true;
//         buttonImage.color = pressedColor; // Cambio de color INMEDIATO al confirmar

//         // 2. Iniciar el retardo de acción
//         actionCoroutine = StartCoroutine(ConfirmAndInvokeAction());
//     }
//     else
//     {
//         ResetButtonState();
//     }
// }
//     private void OnTriggerExit(Collider other)
//     {
//         // Si el dedo se va, reseteamos el estado inmediatamente
//         ResetButtonState();
//     }

//     // --- LÓGICA DE RETARDO Y EJECUCIÓN ---

//     private IEnumerator ConfirmAndInvokeAction()
//     {
//         // Retardo para que el usuario vea el cambio de color
//         yield return new WaitForSeconds(actionDelay);

//         // 🚀 Ejecutar la acción del juego (cambio de escena, etc.)
//         OnButtonPress.Invoke();

//         // Nota: Después de Invoke(), es mejor que el código de cambio de escena 
//         // se encargue de la transición y cargue la nueva escena. 
//         // Por si acaso, reseteamos aquí también, aunque la escena cambie.
//         ResetButtonState();
//     }

//     private void ResetButtonState()
//     {
//         // Detener la coroutine si está corriendo (se soltó el botón antes del tiempo)
//         if (actionCoroutine != null)
//         {
//             StopCoroutine(actionCoroutine);
//             actionCoroutine = null;
//         }
        
//         // Resetear el temporizador y el estado
//         pressTimer = 0f;
//         isConfirmed = false;
        
//         // Restablecer color solo si tenemos el Image
//         if (buttonImage != null)
//         {
//             buttonImage.color = normalColor;
//         }
//     }
// }

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
    public float clickThreshold = 0.1f;

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