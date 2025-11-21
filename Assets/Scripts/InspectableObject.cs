using UnityEngine;

public class InspectableObject : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;
    private bool interactionEnabled = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) { enabled = false; return; }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        // Fija el avión al inicio
        SetInteractionActive(false); 
    }

    // Llamado por el botón "Activar Inspección"
    public void SetInteractionActive(bool state)
    {
        interactionEnabled = state;
        
        // Si se activa (state=true), Is Kinematic se DESACTIVA (se puede agarrar)
        // Si se desactiva (state=false), Is Kinematic se ACTIVA (se fija en su sitio)
        rb.isKinematic = !state; 
        
        if (!state)
        {
             // Si desactivas el modo de inspección con el botón, regresa el avión.
             ReturnToInitialPosition(); 
        }
    }

    // Llamado por el script de agarre (LeapGrabObject.cs) al soltar
    public void ReturnToInitialPosition()
    {
        // Solo regresa si el modo de interacción está APAGADO.
        if (!interactionEnabled)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            rb.isKinematic = true; 
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public bool IsInteractionActive()
    {
        return interactionEnabled;
    }
}