using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider))] // Asegura que el botón tenga un Collider
public class ThreeDPushButton : MonoBehaviour
{
    [Header("3D Press Settings")]
    [Tooltip("Distancia máxima que el botón se moverá hacia adentro (Eje Z local).")]
    public float pressDepth = -0.1f; // El valor debe ser negativo para hundirse
    
    [Tooltip("Velocidad a la que el botón se hunde/regresa.")]
    public float movementSpeed = 0.5f;

    [Header("Timings")]
    [Tooltip("Tiempo que el botón debe permanecer hundido para activar la acción.")]
    public float requiredPressDuration = 0.5f;
    
    [Header("Button Action")]
    public UnityEvent OnButtonPress = new UnityEvent();
    public UnityEvent OnButtonRelease = new UnityEvent();

    // Variables internas
    private Vector3 originalPosition;
    private bool isPressed = false;
    private float pressTimer = 0f;
    private Coroutine movementCoroutine;

    void Start()
    {
        // Al inicio, guarda la posición original del botón en el mundo.
        originalPosition = transform.localPosition;
        
        // Verifica que el Collider esté configurado como Trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("El botón 3D '" + gameObject.name + "' requiere que su Collider tenga 'Is Trigger' marcado.");
        }
        
        // El Rigidbody es necesario para detectar Triggers si el puntero no tiene Rigidbody.
        if (GetComponent<Rigidbody>() == null)
        {
             Rigidbody rb = gameObject.AddComponent<Rigidbody>();
             rb.isKinematic = true; // No queremos que la física lo mueva
        }
    }

    // ===============================================
    // Detección de Colisión (El dedo/puntero toca el botón)
    // ===============================================
    private void OnTriggerStay(Collider other)
    {
        // Opcional: Puedes verificar el Tag del objeto que colisiona (ej. "LeapPointer")
        // if (!other.CompareTag("LeapPointer")) return;
        Debug.Log($"[{gameObject.name}] CONTACTO: El puntero ({other.name}) está tocando. Tiempo: {pressTimer:F2}s", this);

        if (!isPressed)
        {
            // Inicia el movimiento de hundimiento
            if (movementCoroutine != null) StopCoroutine(movementCoroutine);
            movementCoroutine = StartCoroutine(MoveButton(originalPosition + new Vector3(0, 0, pressDepth)));
        }

        pressTimer += Time.deltaTime;

        if (pressTimer >= requiredPressDuration && !isPressed)
        {
            isPressed = true;
            OnButtonPress.Invoke(); // ¡ACCION DE PULSACIÓN!
        }
    }

    // El dedo/puntero deja de tocar el botón
    private void OnTriggerExit(Collider other)
    {
        // Opcional: Puedes verificar el Tag
        // if (!other.CompareTag("LeapPointer")) return;
        
        // Reinicia el estado del botón y lo regresa a su posición original
        ResetButton();
    }

    // ===============================================
    // Lógica de Movimiento y Animación
    // ===============================================
    private IEnumerator MoveButton(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.localPosition, targetPosition) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * movementSpeed * 10f);
            yield return null;
        }
        transform.localPosition = targetPosition; // Asegura la posición final
        movementCoroutine = null;
    }

    private void ResetButton()
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        
        // Inicia el movimiento para regresar a la posición original
        movementCoroutine = StartCoroutine(MoveButton(originalPosition));
        
        if (isPressed)
        {
            OnButtonRelease.Invoke();
        }
        
        // Resetea el temporizador y el estado
        pressTimer = 0f;
        isPressed = false;
    }
}