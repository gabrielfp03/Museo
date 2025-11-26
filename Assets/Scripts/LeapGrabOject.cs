

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
        // Si se puede interactuar con ese objeto
        InspectableObject inspectable = hitRb.GetComponent<InspectableObject>();
        if (inspectable != null && !inspectable.IsInteractionActive())
        {
            return; 
        }
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
        InspectableObject inspectable = grabbedRb.GetComponent<InspectableObject>();
                
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

        // Regresar el objeto a su posición inicial si es inspeccionable
        if (inspectable != null)
        {
            inspectable.ReturnToInitialPosition();
        }

        // 7. Reset flags
        grabbedRb = null;
        isPinchingGrab = false;
    }
}