// using UnityEngine;
// using Leap;     

// public class UltraleapPinchToggle : MonoBehaviour
// {
//     [Tooltip("Asigna aquí el script ModelExploder que controla el desarmado.")]
//     public ModelExploder targetExploder;

//     [Tooltip("Asigna el componente Hand Model (ej. Rigged Hand Left/Right)")]
//     public HandModelBase handModel; 
    
//     [Tooltip("Asigna el script InspectableObject para comprobar si la interacción está activa.")]
//     public InspectableObject interactionController; // <-- ¡NUEVA REFERENCIA!

//     [Tooltip("La fuerza mínima para empezar a desarmar (0.0 a 1.0).")]
//     public float minPinchStart = 0.1f; 
    
//     [Tooltip("La fuerza con la que el modelo está completamente ensamblado.")]
//     public float maxPinchEnd = 0.9f; 

//     void Update()
//     {

//         if (targetExploder == null || handModel == null || interactionController == null)
//         {
//             return;
//         }

//         // Si el modo de interacción NO está activo, salimos de la función.
//         if (!interactionController.IsInteractionActive())
//         {
//             // Opcional: Aseguramos que el modelo esté completamente ensamblado (progreso 0.0) 
//             // cuando la interacción está desactivada.
//             targetExploder.explosionProgress = Mathf.Lerp(targetExploder.explosionProgress, 0f, Time.deltaTime * targetExploder.moveSpeed);
//             return; 
//         }
        
//         // --- Lógica del Pellizco (Solo se ejecuta si la interacción está activa) ---

//         Hand hand = handModel.GetLeapHand();

//         if (hand == null)
//         {
//             // Si la mano sale mientras la interacción ESTÁ activa, mantenemos el último estado.
//             return; 
//         }

//         // 2. Leer y Remapear la fuerza de pellizco
//         float rawPinch = hand.PinchStrength;
//         float linearProgress = Mathf.InverseLerp(minPinchStart, maxPinchEnd, rawPinch);
        
//         // 3. Inversión de la lógica: Pinch cerrado = Ensamblado (0.0)
//         float invertedProgress = 1f - linearProgress;
        
//         // 4. Asignar el progreso invertido al ModelExploder
//         targetExploder.explosionProgress = invertedProgress;
//     }
// }
using UnityEngine;
using Leap;     

public class UltraleapPinchToggle : MonoBehaviour
{
    [Tooltip("Asigna aquí el script ModelExploder que controla el desarmado.")]
    public ModelExploder targetExploder;

    [Tooltip("Asigna el componente Hand Model (ej. Rigged Hand Left/Right)")]
    public HandModelBase handModel; 
    
    [Tooltip("Asigna el script InspectableObject para comprobar si la interacción está activa.")]
    public InspectableObject interactionController;

    [Tooltip("La fuerza mínima para empezar a desarmar (0.0 a 1.0).")]
    public float minPinchStart = 0.1f; 
    
    [Tooltip("La fuerza con la que el modelo está completamente ensamblado.")]
    public float maxPinchEnd = 0.9f; 

    // Variables privadas para manejar la rotación
    private Quaternion initialHandRotation;
    private Quaternion initialModelRotation;
    private bool rotationInitialized = false;

    void Update()
    {
        // --- 1. Bloqueo de Interacción y Comprobación de Referencias ---
        if (targetExploder == null || handModel == null || interactionController == null)
        {
            return;
        }

        // Si la interacción NO está activa, aseguramos el ensamblaje, la rotación inicial y salimos.
        if (!interactionController.IsInteractionActive())
        {
            if (rotationInitialized)
            {
                // Devolvemos el modelo a su rotación inicial suavemente (usando el Lerp del InspectableObject)
                rotationInitialized = false;
            }
            // Aseguramos que el modelo se ensamble suavemente
            targetExploder.explosionProgress = Mathf.Lerp(targetExploder.explosionProgress, 0f, Time.deltaTime * targetExploder.moveSpeed);
            return; 
        }

        // --- 2. Lógica de Rotación y Desensamblaje ---
        
        Hand hand = handModel.GetLeapHand();

        if (hand == null)
        {
            return; 
        }

        // 🚨 Rotación (Versión más segura) 🚨
        
        // Obtenemos la rotación actual de la mano directamente del objeto 'Hand' de Leap
        Quaternion currentHandRotation = hand.Rotation; // Usamos la rotación del objeto Hand

        if (!rotationInitialized)
        {
            // Guardamos la rotación inicial en el primer frame de la interacción activa
            initialHandRotation = currentHandRotation;
            initialModelRotation = targetExploder.transform.rotation;
            rotationInitialized = true;
        }
        else
        {
            // 1. Calculamos la diferencia de rotación (Delta)
            Quaternion deltaRotation = Quaternion.Inverse(initialHandRotation) * currentHandRotation;
            
            // 2. Aplicamos el Delta a la rotación inicial del modelo
            Quaternion fullRotation = initialModelRotation * deltaRotation;
            
            // FILTRADO: Tomamos solo el eje Y de la rotación completa
            targetExploder.transform.rotation = Quaternion.Euler(
                initialModelRotation.eulerAngles.x, // Mantenemos la rotación X inicial o 0
                fullRotation.eulerAngles.y,         // APLICAMOS la rotación del eje Y del gesto
                initialModelRotation.eulerAngles.z  // Mantenemos la rotación Z inicial o 0
            );
        }


        // 💥 Desensamblaje (Lógica invertida) 💥
        float rawPinch = hand.PinchStrength;
        float linearProgress = Mathf.InverseLerp(minPinchStart, maxPinchEnd, rawPinch);
        
        float invertedProgress = 1f - linearProgress;
        
        targetExploder.explosionProgress = invertedProgress;
    }
}