
// using UnityEngine;
// using Leap;

// public class LeapGrabObject : MonoBehaviour
// {
//     // === Leap Motion Settings ===
//     [Header("Leap Motion Settings")]
//     [Tooltip("Fuerza mínima de agarre (0.0 a 1.0) para INICIAR el agarre.")]
//     public float grabThreshold = 0.9f; 
//     public float grabReleaseThreshold = 0.8f;
//     [Tooltip("Fuerza mínima de agarre para MANTENER el agarre.")]
//     public float releaseThreshold = 0.7f; 
//     [Tooltip("Radio de detección para buscar objetos cerca de la palma (en metros).")]
//     public float overlapRadius = 0.05f; 

//     // === Grab Physics and Movement ===
//     [Header("Grab Physics and Movement")]
//     [Tooltip("Suavidad del movimiento al seguir la mano (más bajo es más suave).")]
//     public float moveSpeed = 0.1f;
//     [Tooltip("Factor de escala aplicado al objeto al agarrarlo para normalizar su tamaño.")]
//     public float scaleFactor = 5f; 

//     // Referencias privadas y datos de estado
//     private HandModelBase handModelBase; // Componente que nos da la información de la mano.
//     private Rigidbody grabbedRb;
//     private bool isGrabbing = false;
    
//     // Datos para restaurar el objeto al soltarlo
//     private RigidbodyConstraints previousConstraints;
//     private Transform originalParent;
//     private Vector3 originalScale;
//     private bool hadGravity;

//     // Handle (punto de pivote) y Offset para seguir la mano
//     private GameObject grabHandle;
//     private Vector3 positionOffset; // Offset de posición entre la mano y el objeto al agarrar
//     private Quaternion rotationOffset; // Offset de rotación entre la mano y el objeto al agarrar
//     private Vector3 currentVelocity; // Variable para SmoothDamp (aunque usaremos Lerp simple para simplicidad)
//     private Vector3 originalPosition;  // Posición Mundial al agarrar
//     private Quaternion originalRotation; // Rotación Mundial al agarrar

//     [Header("Pinch Settings (Agarrar/Inspección y Rotación)")]
//     [Tooltip("Fuerza mínima de pinza (pinza) para iniciar el agarre de inspección.")]
//     public float pinchThreshold = 0.9f;
//     [Tooltip("Fuerza mínima de pinza para mantener el agarre de inspección.")]
//     public float pinchReleaseThreshold = 0.8f; 
//     private bool isPinchingGrab = false;
//     void Start()
//     {
//         // Buscar HandModelBase en el padre (la mano real)
//         handModelBase = transform.parent.GetComponent<HandModelBase>(); 
        
//         if (handModelBase == null)
//         {
//             Debug.LogError("LeapGrabObject debe ser hijo de un objeto de la mano con HandModelBase (ej. Capsule Hand Right).");
//             enabled = false;
//         }
//     }

//     void Update()
//     {
//         // 1. Obtener datos de la mano
//         if (handModelBase == null || !handModelBase.IsTracked)
//         {
//             if (grabbedRb != null) Drop(); // Soltar si la mano desaparece
//             return;
//         }

//         Hand hand = handModelBase.GetLeapHand();
//         if (hand == null) return; 

//         float grabStrength = hand.GrabStrength;
//         float pinchStrength = hand.PinchStrength;
//         bool isCurrentlyGrabbing;
        
//         bool tryPinchGrab = pinchStrength > pinchThreshold;
//         bool tryStrongGrab = grabStrength > grabThreshold;

//         // Aplica el umbral de soltar para evitar el parpadeo (Hysteresis)
//         if (isGrabbing)
//         {
//             float releaseThreshold = isPinchingGrab ? pinchReleaseThreshold : grabReleaseThreshold;
//             isCurrentlyGrabbing = (isPinchingGrab ? pinchStrength : grabStrength) > releaseThreshold;
//         }
//         else // Si no estamos agarrando, comprobamos el agarre (Pinza tiene prioridad)
//         {
//             if (tryPinchGrab)
//             {
//                 isPinchingGrab = true;
//                 isCurrentlyGrabbing = true;
//             }
//             else if (tryStrongGrab)
//             {
//                 isPinchingGrab = false;
//                 isCurrentlyGrabbing = true;
//             }
//             else
//             {
//                 isCurrentlyGrabbing = false;
//             }
//         }

//         // === Lógica de Interacción ===

//         if (isCurrentlyGrabbing && !isGrabbing)
//         {
//             // TryGrab(hand.PalmPosition, hand.Rotation);
//             TryGrab(hand);
//         }
//         else if (!isCurrentlyGrabbing && isGrabbing)
//         {
//             Drop();
//         }

//         // 2. Mover y rotar el objeto agarrado para que siga la mano
//         if (grabbedRb != null)
//         {
//             // MoveObject(hand.PalmPosition, hand.Rotation);
//             Vector3 handCurrentPosition = hand.Index.TipPosition;
//            // grabHandle.transform.position = hand.PalmPosition;
//             // grabHandle.transform.position = hand.Index.TipPosition; // Usar la punta del dedo índice para un agarre más natural
//             grabHandle.transform.position = handCurrentPosition + positionOffset;
//             // 2. Seguimiento de ROTACIÓN (Usando el offset para mantener la orientación)
//             grabHandle.transform.rotation = hand.Rotation * rotationOffset;
            
//         }
        
//         isGrabbing = isCurrentlyGrabbing;
//     }

//     // ====================================================================
//     // LÓGICA DE AGARRE
//     // ====================================================================

//     void TryGrab(Hand hand)
//     {
//         Vector3 anchorPosition = hand.Index.TipPosition;

//         // 1. Buscar objeto cerca de la palma
//         Collider[] colliders = Physics.OverlapSphere(anchorPosition, overlapRadius); 
//         Rigidbody hitRb = null;

//         foreach (Collider col in colliders)
//         {
//             if (col.CompareTag("Grabbable") && col.attachedRigidbody != null)
//             {
//                 hitRb = col.attachedRigidbody;
//                 break; 
//             }
//         }
        
//         if (hitRb != null && grabbedRb == null) // Agarrar si se encontró algo y no tienes nada ya
//         {
//             grabbedRb = hitRb;
//             Collider col = grabbedRb.GetComponent<Collider>();
            
//             originalPosition = grabbedRb.transform.position; 
//             originalRotation = grabbedRb.transform.rotation;

//             // --- Guardar y Configurar Física ---
//             hadGravity = grabbedRb.useGravity;
//             previousConstraints = grabbedRb.constraints;
//             originalParent = grabbedRb.transform.parent;
//             originalScale = grabbedRb.transform.localScale;

//             positionOffset = grabbedRb.transform.position - anchorPosition;
//             rotationOffset = Quaternion.Inverse(hand.Rotation) * grabbedRb.transform.rotation;

//             grabbedRb.useGravity = false;
//             // Congelar solo la rotación para permitirnos mover el objeto
//             grabbedRb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezeRotation;; 
            
//             // --- Normalizar Escala (del script original) ---
//             Vector3 size = col.bounds.size;
//             float maxDimension = Mathf.Max(size.x, size.y, size.z);
//             float baseScaleFactor = 1f / maxDimension; 
//             grabbedRb.transform.localScale = originalScale * baseScaleFactor * scaleFactor;

//             // --- Crear y Configurar Handle ---
//             grabHandle = new GameObject("LeapGrabHandle");
            
//             // Posicionar el handle en el centro del objeto que se agarró
//             grabHandle.transform.position = col.bounds.center;
//             grabHandle.transform.rotation = grabbedRb.transform.rotation;
            
//             // Calcular el offset: qué tan rotado está el objeto con respecto a la mano
//             rotationOffset = Quaternion.Inverse(hand.Rotation) * grabbedRb.transform.rotation;
            
//             // El handle debe seguir a la mano (el padre de este script)
//             grabHandle.transform.SetParent(this.transform.parent, true); 

//             // El objeto agarrado sigue al handle
//             grabbedRb.transform.SetParent(grabHandle.transform, true);
//         }
//     }

//     // Dentro de la clase LeapGrabObject.cs


//     void Drop()
//     {
//         if (grabbedRb == null) return;
        
//         // 1. Desvincular el objeto del handle y restaurar el padre original
//         grabbedRb.transform.SetParent(originalParent, true);
        
//         grabbedRb.transform.position = originalPosition;
//         grabbedRb.transform.rotation = originalRotation;
//         // 2. Restaurar la física
//         grabbedRb.useGravity = hadGravity; 
//         grabbedRb.constraints = previousConstraints;

//         // 3. Restaurar la escala original
//         grabbedRb.transform.localScale = originalScale;

//         // 4. Limpiar
//         Destroy(grabHandle);
//         grabbedRb = null;
//         isPinchingGrab = false;
//     }

//     void MoveObject(Vector3 targetPosition, Quaternion targetRotation)
//     {
//         if (grabHandle == null) return;

//         // Mueve el handle a la posición de la mano con suavidad
//         // Vector3.Lerp() es una forma sencilla de interpolación (suavizado)
//         grabHandle.transform.position = Vector3.Lerp(grabHandle.transform.position, targetPosition, Time.deltaTime * (1f / moveSpeed));
        
//         // Aplica la rotación de la mano más el offset inicial
//         // Esto mantiene el objeto estable mientras la mano se mueve
//         grabHandle.transform.rotation = targetRotation * rotationOffset;
//     }
// }

using UnityEngine;
using Leap;

public class LeapGrabObject : MonoBehaviour
{
    // === Leap Motion Settings ===
    [Header("Leap Motion Settings")]
    [Tooltip("Fuerza mínima de agarre (0.0 a 1.0) para INICIAR el agarre.")]
    public float grabThreshold = 0.9f; 
    [Tooltip("Fuerza mínima de agarre para soltar un agarre fuerte.")]
    public float grabReleaseThreshold = 0.8f;
    [Tooltip("Fuerza mínima de agarre para MANTENER el agarre.")]
    public float releaseThreshold = 0.7f; 
    [Tooltip("Radio para detectar objetos cercanos a la punta del índice.")]
    public float overlapRadius = 0.05f; 

    // === Grab Physics and Movement ===
    [Header("Grab Physics and Movement")]
    [Tooltip("Velocidad de seguimiento. Valores bajos suavizan más.")]
    public float moveSpeed = 0.1f;
    [Tooltip("Escala aplicada al objeto al agarrarlo para normalizar tamaños.")]
    public float scaleFactor = 5f; 

    // Referencias internas
    private HandModelBase handModelBase; 
    private Rigidbody grabbedRb;
    private bool isGrabbing = false;
    private bool isPinchingGrab = false;

    // Datos para restauración cuando se suelta el objeto
    private RigidbodyConstraints previousConstraints;
    private Transform originalParent;
    private Vector3 originalScale;
    private bool hadGravity;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // Handle (punto de pivote) + offsets para alineación
    private GameObject grabHandle;
    private Vector3 positionOffset;
    private Quaternion rotationOffset;

    [Header("Pinch Settings")]
    [Tooltip("Fuerza mínima de pinza para iniciar agarre tipo inspección.")]
    public float pinchThreshold = 0.9f;
    [Tooltip("Fuerza mínima de pinza para mantener agarre tipo inspección.")]
    public float pinchReleaseThreshold = 0.8f; 

    void Start()
    {
        // Buscar HandModelBase en el padre (mano generada por Leap)
        if (transform.parent != null)
            handModelBase = transform.parent.GetComponent<HandModelBase>(); 
        
        if (handModelBase == null)
        {
            Debug.LogError("LeapGrabObject debe ser hijo de un objeto con HandModelBase.");
            enabled = false;
        }
    }

    void Update()
    {
        // No continuar si no hay mano detectada
        if (handModelBase == null || !handModelBase.IsTracked)
        {
            if (grabbedRb != null) Drop();
            return;
        }

        Hand hand = handModelBase.GetLeapHand();
        if (hand == null) return;

        float grabStrength = hand.GrabStrength;
        float pinchStrength = hand.PinchStrength;

        bool tryPinchGrab = pinchStrength > pinchThreshold;
        bool tryStrongGrab = grabStrength > grabThreshold;

        // === Hysteresis para evitar parpadeos en el agarre ===
        bool isCurrentlyGrabbing;

        if (isGrabbing)
        {
            // Si ya agarramos, comprobamos si debemos soltar (umbral menor)
            float releaseT = isPinchingGrab ? pinchReleaseThreshold : grabReleaseThreshold;
            isCurrentlyGrabbing = (isPinchingGrab ? pinchStrength : grabStrength) > releaseT;
        }
        else
        {
            // Prioridad: pinza > agarre fuerte > ninguno
            if (tryPinchGrab)
            {
                isPinchingGrab = true;
                isCurrentlyGrabbing = true;
            }
            else if (tryStrongGrab)
            {
                isPinchingGrab = false;
                isCurrentlyGrabbing = true;
            }
            else isCurrentlyGrabbing = false;
        }

        // === Cambios de estado ===
        if (isCurrentlyGrabbing && !isGrabbing)
            TryGrab(hand);
        else if (!isCurrentlyGrabbing && isGrabbing)
            Drop();

        // === Si hay objeto agarrado, seguir la mano ===
        if (grabbedRb != null)
        {
            // Usar la punta del índice para una sensación más natural
            Vector3 handPos = hand.Index.TipPosition;

            grabHandle.transform.position = handPos + positionOffset;
            grabHandle.transform.rotation = hand.Rotation * rotationOffset;
        }

        // Actualizar estado del frame anterior
        isGrabbing = isCurrentlyGrabbing;
    }

    // ====================================================================
    // INTENTAR AGARRAR UN OBJETO
    // ====================================================================
    void TryGrab(Hand hand)
    {
        Vector3 anchorPosition = hand.Index.TipPosition;

        Collider[] colliders = Physics.OverlapSphere(anchorPosition, overlapRadius);
        Rigidbody hitRb = null;

        // Buscar collider con etiqueta Grabbable
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Grabbable") && col.attachedRigidbody != null)
            {
                hitRb = col.attachedRigidbody;
                break;
            }
        }
        
        if (hitRb == null || grabbedRb != null) return;

        grabbedRb = hitRb;
        Collider colRef = grabbedRb.GetComponent<Collider>();

        // Guardar datos originales
        originalParent = grabbedRb.transform.parent;      // Guardar parent original primero
        originalPosition = grabbedRb.transform.position;  // Guardar posición mundial real
        originalRotation = grabbedRb.transform.rotation;  // Guardar rotación mundial real
        originalScale = grabbedRb.transform.localScale;   // Guardar escala
        hadGravity = grabbedRb.useGravity;                // Guardar gravedad
        previousConstraints = grabbedRb.constraints;      // Guardar restricciones

        // Offset inicial desde la mano al objeto
        positionOffset = grabbedRb.transform.position - anchorPosition;
        rotationOffset = Quaternion.Inverse(hand.Rotation) * grabbedRb.transform.rotation;

        // Configurar física
        grabbedRb.useGravity = false;
        grabbedRb.constraints = RigidbodyConstraints.FreezeRotation;

        // Normalizar escala según el tamaño del objeto
        Vector3 size = colRef.bounds.size;
        float maxSize = Mathf.Max(size.x, size.y, size.z);
        float normalizedScale = 1f / maxSize;
        grabbedRb.transform.localScale = originalScale * normalizedScale * scaleFactor;

        // Crear handle y convertirlo en pivote
        grabHandle = new GameObject("LeapGrabHandle");
        grabHandle.transform.position = colRef.bounds.center;
        grabHandle.transform.rotation = grabbedRb.transform.rotation;

        grabHandle.transform.SetParent(this.transform.parent, true);
        grabbedRb.transform.SetParent(grabHandle.transform, true);
    }

    // ====================================================================
    // SOLTAR OBJETO
    // ====================================================================
    void Drop()
    {
        if (grabbedRb == null) return;
        
        // Restaurar jerarquía
        grabbedRb.transform.SetParent(originalParent, true);
        grabbedRb.transform.position = originalPosition;
        grabbedRb.transform.rotation = originalRotation;

        // Restaurar física
        grabbedRb.useGravity = hadGravity;
        grabbedRb.constraints = previousConstraints;

        // Restaurar escala original
        grabbedRb.transform.localScale = originalScale;

        // Limpiar handle
        Destroy(grabHandle);

        // Reset flags
        grabbedRb = null;
        isPinchingGrab = false;
    }
}
