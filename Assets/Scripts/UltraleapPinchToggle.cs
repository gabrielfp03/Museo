// // using UnityEngine;
// // using Leap;     

// // public class UltraleapPinchToggle : MonoBehaviour
// // {
// //     [Tooltip("Asigna aquí el script ModelExploder que controla el desarmado.")]
// //     public ModelExploder targetExploder;

// //     [Tooltip("Asigna el componente Hand Model (ej. Rigged Hand Left/Right)")]
// //     public HandModelBase handModel; 
    
// //     [Tooltip("Asigna el script InspectableObject para comprobar si la interacción está activa.")]
// //     public InspectableObject interactionController; // <-- ¡NUEVA REFERENCIA!

// //     [Tooltip("La fuerza mínima para empezar a desarmar (0.0 a 1.0).")]
// //     public float minPinchStart = 0.1f; 
    
// //     [Tooltip("La fuerza con la que el modelo está completamente ensamblado.")]
// //     public float maxPinchEnd = 0.9f; 

// //     void Update()
// //     {

// //         if (targetExploder == null || handModel == null || interactionController == null)
// //         {
// //             return;
// //         }

// //         // Si el modo de interacción NO está activo, salimos de la función.
// //         if (!interactionController.IsInteractionActive())
// //         {
// //             // Opcional: Aseguramos que el modelo esté completamente ensamblado (progreso 0.0) 
// //             // cuando la interacción está desactivada.
// //             targetExploder.explosionProgress = Mathf.Lerp(targetExploder.explosionProgress, 0f, Time.deltaTime * targetExploder.moveSpeed);
// //             return; 
// //         }
        
// //         // --- Lógica del Pellizco (Solo se ejecuta si la interacción está activa) ---

// //         Hand hand = handModel.GetLeapHand();

// //         if (hand == null)
// //         {
// //             // Si la mano sale mientras la interacción ESTÁ activa, mantenemos el último estado.
// //             return; 
// //         }

// //         // 2. Leer y Remapear la fuerza de pellizco
// //         float rawPinch = hand.PinchStrength;
// //         float linearProgress = Mathf.InverseLerp(minPinchStart, maxPinchEnd, rawPinch);
        
// //         // 3. Inversión de la lógica: Pinch cerrado = Ensamblado (0.0)
// //         float invertedProgress = 1f - linearProgress;
        
// //         // 4. Asignar el progreso invertido al ModelExploder
// //         targetExploder.explosionProgress = invertedProgress;
// //     }
// // }
// using UnityEngine;
// using Leap;     

// public class UltraleapPinchToggle : MonoBehaviour
// {
//     [Tooltip("Asigna aquí el script ModelExploder que controla el desarmado.")]
//     public ModelExploder targetExploder;

//     [Tooltip("Asigna el componente Hand Model (ej. Rigged Hand Left/Right)")]
//     public HandModelBase handModel; 
    
//     [Tooltip("Asigna el script InspectableObject para comprobar si la interacción está activa.")]
//     public InspectableObject interactionController;

//     [Tooltip("La fuerza mínima para empezar a desarmar (0.0 a 1.0).")]
//     public float minPinchStart = 0.1f; 
    
//     [Tooltip("La fuerza con la que el modelo está completamente ensamblado.")]
//     public float maxPinchEnd = 0.9f; 

//     // Variables privadas para manejar la rotación
//     private Quaternion initialHandRotation;
//     private Quaternion initialModelRotation;
//     private bool rotationInitialized = false;

//     void Update()
//     {
//         // --- 1. Bloqueo de Interacción y Comprobación de Referencias ---
//         if (targetExploder == null || handModel == null || interactionController == null)
//         {
//             return;
//         }

//         // Si la interacción NO está activa, aseguramos el ensamblaje, la rotación inicial y salimos.
//         if (!interactionController.IsInteractionActive())
//         {
//             if (rotationInitialized)
//             {
//                 // Devolvemos el modelo a su rotación inicial suavemente (usando el Lerp del InspectableObject)
//                 rotationInitialized = false;
//             }
//             // Aseguramos que el modelo se ensamble suavemente
//             targetExploder.explosionProgress = Mathf.Lerp(targetExploder.explosionProgress, 0f, Time.deltaTime * targetExploder.moveSpeed);
//             return; 
//         }

//         // --- 2. Lógica de Rotación y Desensamblaje ---
        
//         Hand hand = handModel.GetLeapHand();

//         if (hand == null)
//         {
//             return; 
//         }

//         // 🚨 Rotación (Versión más segura) 🚨
        
//         // Obtenemos la rotación actual de la mano directamente del objeto 'Hand' de Leap
//         Quaternion currentHandRotation = hand.Rotation; // Usamos la rotación del objeto Hand

//         if (!rotationInitialized)
//         {
//             // Guardamos la rotación inicial en el primer frame de la interacción activa
//             initialHandRotation = currentHandRotation;
//             initialModelRotation = targetExploder.transform.rotation;
//             rotationInitialized = true;
//         }
//         else
//         {
//             // 1. Calculamos la diferencia de rotación (Delta)
//             Quaternion deltaRotation = Quaternion.Inverse(initialHandRotation) * currentHandRotation;
//             // 2. Aplicamos el Delta a la rotación inicial del modelo
//             Quaternion fullRotation = initialModelRotation * deltaRotation;
//             // FILTRADO: Tomamos solo el eje Y de la rotación completa
//             targetExploder.transform.rotation = Quaternion.Euler(
//             initialModelRotation.eulerAngles.x, // Mantenemos la rotación X inicial o 0
//             fullRotation.eulerAngles.y, // APLICAMOS la rotación del eje Y del gesto
//             initialModelRotation.eulerAngles.z // Mantenemos la rotación Z inicial o 0
//             );
//         }


//         float rawPinch = hand.PinchStrength;
//         float linearProgress = Mathf.InverseLerp(minPinchStart, maxPinchEnd, rawPinch);
        
//         float invertedProgress = 1f - linearProgress;
        
//         targetExploder.explosionProgress = invertedProgress;
//     }
// }
using UnityEngine;
using Leap;
using System.Collections.Generic;

// Este script controla el progreso del despiece de un modelo basado en la distancia 
// entre dos manos, requiriendo un "aplauso" inicial para activar la interacción.
public class UltraleapSeparationControl : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Asigna aquí el script ModelExploder que controla el desarmado.")]
    public ModelExploder targetExploder;

    [Tooltip("Asigna el Hand Model de la mano izquierda (ej. Rigged Hand Left)")]
    public HandModelBase leftHandModel;

    [Tooltip("Asigna el Hand Model de la mano derecha (ej. Rigged Hand Right)")]
    public HandModelBase rightHandModel; 
    
    [Tooltip("Asigna el script InspectableObject para comprobar si la interacción está activa.")]
    public InspectableObject interactionController; 

    [Header("Configuración de Separación")]
    [Tooltip("Distancia máxima (en unidades de Unity) para considerar un 'aplauso' o manos juntas.")]
    public float clapThreshold = 0.1f; // 10 cm, un valor típico para manos juntas.
    
    [Tooltip("Distancia (en unidades de Unity) a la que el modelo está COMPLETAMENTE explotado (progress = 1.0).")]
    public float maxSeparationDistance = 0.5f; // 50 cm. Ajusta este valor al tamaño de tu área de interacción.

    [Tooltip("Velocidad de suavizado para el progreso de ensamblaje (cuando la interacción se desactiva).")]
    public float assemblySpeed = 5f; 

    [Header("Rotación")]
    [Tooltip("Utilizar la rotación de la mano derecha para rotar el modelo.")]
    public bool enableRotation = true;

    // --- Variables de Estado ---
    private bool rotationInitialized = false;
    private bool hasClapped = false; // El estado clave: ¿Se ha cumplido la condición inicial de manos juntas?
    private Quaternion initialHandRotation;
    private Quaternion initialModelRotation;


    void Update()
    {
        // --- 1. Bloqueo de Interacción y Comprobación de Referencias ---
        if (targetExploder == null || leftHandModel == null || rightHandModel == null || interactionController == null)
        {
            Debug.LogError("FALTA REFERENCIA: Asegúrate de que todos los campos 'Referencias' están asignados en el Inspector.");
            return;
        }

        Hand leftHand = leftHandModel.GetLeapHand();
        Hand rightHand = rightHandModel.GetLeapHand();

        // Comprobación de que ambas manos están siendo detectadas
        if (leftHand == null || rightHand == null)
        {
            // Si perdemos una mano, volvemos al estado de 'no aplauso' y ensamblamos.
            if (hasClapped)
            {
                // Solo si había aplaudido, reportamos la pérdida de seguimiento
                Debug.LogWarning("Manos perdidas. Reiniciando estado de 'aplauso'.");
            }
            hasClapped = false; 
            targetExploder.explosionProgress = Mathf.Lerp(targetExploder.explosionProgress, 0f, Time.deltaTime * assemblySpeed);
            return;
        }

        // Si la interacción global (ej. el puntero está sobre el objeto) NO está activa, 
        // forzamos el ensamblaje y reiniciamos el estado de "aplauso".
        if (!interactionController.IsInteractionActive())
        {
            if (hasClapped) Debug.Log("Interacción desactivada. Ensamblando modelo.");
            hasClapped = false;
            rotationInitialized = false;
            // Aseguramos que el modelo se ensamble suavemente
            targetExploder.explosionProgress = Mathf.Lerp(targetExploder.explosionProgress, 0f, Time.deltaTime * assemblySpeed);
            return;
        }

        // --- 2. Lógica de Distancia y Progreso (Despiece) ---
        
        // Usamos la posición del centro de la palma (Palm Position) y la convertimos a Vector3
        Vector3 leftPalmPosition = leftHand.PalmPosition;
        Vector3 rightPalmPosition = rightHand.PalmPosition;
        
        float currentSeparation = Vector3.Distance(leftPalmPosition, rightPalmPosition);

        // A) Determinar el estado de 'aplauso'
        if (currentSeparation < clapThreshold)
        {
            // Las manos están juntas -> Establecer la condición de inicio cumplida
            if (!hasClapped) Debug.Log("CONDICIÓN INICIAL CUMPLIDA: ¡APLAUSO DETECTADO!");
            hasClapped = true;
        }

        // B) Controlar el despiece por separación (solo si la condición de inicio se cumplió)
        if (hasClapped)
        {
            // Normalizar la distancia entre el umbral mínimo (clapThreshold) y la distancia máxima (maxSeparationDistance)
            float linearProgress = Mathf.InverseLerp(clapThreshold, maxSeparationDistance, currentSeparation);
            
            // Limitamos el progreso a [0, 1]
            targetExploder.explosionProgress = Mathf.Clamp01(linearProgress);

            // LOG de depuración de progreso
            Debug.Log($"Separación: {currentSeparation:F3}m | Progreso: {targetExploder.explosionProgress:P0}");
        }
        else
        {
            // Si no ha aplaudido y la interacción está activa, mantenemos el modelo ensamblado
            targetExploder.explosionProgress = 0f;
            Debug.Log($"Esperando aplauso (Separación actual: {currentSeparation:F3}m / Umbral: {clapThreshold:F3}m)");
        }


        // --- 3. Lógica de Rotación (Opcional) ---
        if (enableRotation)
        {
            Quaternion currentHandRotation = rightHand.Rotation; 

            if (!rotationInitialized)
            {
                initialHandRotation = currentHandRotation;
                initialModelRotation = targetExploder.transform.rotation;
                rotationInitialized = true;
            }
            else
            {
                // Calcular el Delta de Rotación y aplicarlo al modelo
                Quaternion deltaRotation = Quaternion.Inverse(initialHandRotation) * currentHandRotation;
                Quaternion fullRotation = initialModelRotation * deltaRotation;
                
                // Aplicamos SOLO la rotación en el eje Y (giro horizontal)
                targetExploder.transform.rotation = Quaternion.Euler(
                    initialModelRotation.eulerAngles.x, // Mantenemos X inicial
                    fullRotation.eulerAngles.y,         // Aplicamos Y de la mano
                    initialModelRotation.eulerAngles.z  // Mantenemos Z inicial
                );
            }
        }
    }
}