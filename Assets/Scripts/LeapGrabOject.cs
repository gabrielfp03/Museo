
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
    
    [Tooltip("Radio para detectar objetos cercanos a la punta del índice.")]
    public float overlapRadius = 0.05f; 

    // === Grab Physics and Movement ===
    [Header("Grab Physics and Movement")]
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

    // Handle (punto de pivote) + offsets para alineación
    private GameObject grabHandle;
    private Vector3 positionOffset; // Offset: Distancia del objeto al punto de anclaje (dedo)
    private Quaternion rotationOffset;

    [Header("Pinch Settings")]
    [Tooltip("Fuerza mínima de pinza para iniciar agarre tipo inspección.")]
    public float pinchThreshold = 0.9f;
    [Tooltip("Fuerza mínima de pinza para mantener agarre tipo inspección.")]
    public float pinchReleaseThreshold = 0.8f; 

    void Start()
    {
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

        // === Hysteresis ===
        bool isCurrentlyGrabbing;

        if (isGrabbing)
        {
            float releaseT = isPinchingGrab ? pinchReleaseThreshold : grabReleaseThreshold;
            isCurrentlyGrabbing = (isPinchingGrab ? pinchStrength : grabStrength) > releaseT;
        }
        else
        {
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

        // Actualizar estado del frame anterior
        isGrabbing = isCurrentlyGrabbing;
    }

    void LateUpdate()
    {
        if (handModelBase == null || !handModelBase.IsTracked || grabbedRb == null)
        {
            return;
        }

        Hand hand = handModelBase.GetLeapHand();
        if (hand == null) return;

        // ANCLAJE DEDO: Usamos la punta del índice para el seguimiento 
        Vector3 anchorPosition = hand.Index.TipPosition;

        // Posición Objetivo del Handle: La posición del dedo más el offset (distancia)
        Vector3 targetPos = anchorPosition + positionOffset;
        
        // Rotación Objetivo del Handle
        Quaternion targetRot = hand.Rotation * rotationOffset;
        
        // Aplicar movimiento directo (1:1) al Handle
        grabHandle.transform.position = targetPos;
        grabHandle.transform.rotation = targetRot;
    }

    // ====================================================================
    // INTENTAR AGARRAR UN OBJETO
    // ====================================================================
    void TryGrab(Hand hand)
    {
        // PUNTA DEL DEDO: Anclamos el punto de detección al índice
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
        
        // Guardar datos originales
        originalParent = grabbedRb.transform.parent;      
        originalScale = grabbedRb.transform.localScale;   
        hadGravity = grabbedRb.useGravity;                
        previousConstraints = grabbedRb.constraints;      

        // Crear handle y convertirlo en pivote
        grabHandle = new GameObject("LeapGrabHandle");
        
        // HANDLE POSITION: Creamos el handle EN LA POSICIÓN DEL DEDO (anchor)
        grabHandle.transform.position = anchorPosition; 
        grabHandle.transform.rotation = hand.Rotation;

        grabHandle.transform.SetParent(this.transform.parent, true);

        // OFFSET CRÍTICO: Cálculo de la distancia del OBJETO al DEDO
        positionOffset = grabbedRb.transform.position - anchorPosition;
        rotationOffset = Quaternion.Inverse(hand.Rotation) * grabbedRb.transform.rotation;

        grabbedRb.transform.localScale = originalScale * scaleFactor;

        // Collider colRef = grabbedRb.GetComponent<Collider>();
        // if (colRef != null)
        // {
        //     // Normalizar escala
        //     Vector3 size = colRef.bounds.size;
        //     float maxDimension = Mathf.Max(size.x, size.y, size.z);
        //     float baseScaleFactor = 1f / maxDimension; 
        //     grabbedRb.transform.localScale = originalScale * baseScaleFactor * scaleFactor;
        // }

        // Configurar física
        grabbedRb.isKinematic = true; 
        grabbedRb.useGravity = false;
        grabbedRb.constraints = RigidbodyConstraints.None;

        // Hacer el objeto agarrado HIJO del handle
        grabbedRb.transform.SetParent(grabHandle.transform, true);
    }

    // ====================================================================
    // SOLTAR OBJETO
    // ====================================================================
    void Drop()
    {
        if (grabbedRb == null) return;
                
        Collider grabbedCollider = grabbedRb.GetComponent<Collider>();

        // 1. Restaurar jerarquía (El objeto vuelve a su padre original)
        grabbedRb.transform.SetParent(originalParent, true);

        // 2. Restaurar física (CRÍTICO)
        grabbedRb.isKinematic = false; 
        grabbedRb.constraints = previousConstraints;         
        grabbedRb.useGravity = hadGravity;
        
        // 3. Limpieza de velocidad
        grabbedRb.linearVelocity = Vector3.zero;
        grabbedRb.angularVelocity = Vector3.zero;

        // 4. Solución al bug de caer a través (Forzar la re-evaluación de colisión)
        if (grabbedCollider != null)
        {
            grabbedCollider.enabled = false;
            grabbedCollider.enabled = true;
        }

        // 5. Restaurar escala original
        grabbedRb.transform.localScale = originalScale;

        // 6. Limpiar handle
        if (grabHandle != null)
        {
            Destroy(grabHandle);
        }

        // 7. Reset flags
        grabbedRb = null;
        isPinchingGrab = false;
    }
}