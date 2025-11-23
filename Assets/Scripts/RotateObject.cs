using UnityEngine;
using Leap;
using UnityEngine.UIElements;

/// <summary>
/// Controla la rotación de un objeto 3D usando la orientación de la mano Leap Motion.
/// Cuando se activa, centra el objeto en la pantalla y lo rota según la mano.
/// </summary>
public class RotateObject : MonoBehaviour
{
    [Header("Referencias de Leap Motion")]
    [Tooltip("Arrastra aquí tu Leap Service Provider.")]
    public LeapServiceProvider leapProvider;
    
    [Header("Configuración del Objeto")]
    [Tooltip("El objeto que será examinado/rotado.")]
    public GameObject targetObject;
    [Tooltip("Distancia a la que se centrará el objeto frente a la cámara.")]
    public float examinationDistance = 1.5f;

    [Header("Parámetros de Rotación")]
    [Tooltip("Factor para suavizar la rotación (mayor valor = rotación más suave).")]
    [Range(0.01f, 1f)]
    public float rotationLerpSpeed = 0.5f;
    [Tooltip("Corrección de orientación inicial. Ajusta esto si el objeto no está 'de frente' inicialmente.")]
    public Quaternion rotationOffset = Quaternion.Euler(0, 0, 0);

    [Header("Estado Interno")]
    private bool isExamining = false;
    private Quaternion initialRotation; // Para almacenar la rotación inicial del objeto

    public Vector3 targetPosition; 

    // --- FUNCIONES PÚBLICAS PARA CONTROL ---

    /// <summary>
    /// Inicia el modo de examinación (centrado y rotación).
    /// </summary>
    public void StartExamination()
    {
        if (targetObject == null || leapProvider == null)
        {
            Debug.LogError("Referencia de Objeto o Leap Provider no asignada.");
            return;
        }

        // 1. Centrar el Objeto en la Pantalla (frente a la cámara principal)
        Transform camTransform = Camera.main.transform;
        targetObject.transform.position = targetPosition;
        
        // 2. Almacenar la rotación inicial del objeto por si se necesita
        initialRotation = targetObject.transform.rotation; 

        isExamining = true;
        Debug.Log("Modo de examinación activado.");
    }

    /// <summary>
    /// Detiene el modo de examinación.
    /// </summary>
    public void StopExamination()
    {
        isExamining = false;
        Debug.Log("Modo de examinación desactivado.");
    }

    // --- LÓGICA DE ROTACIÓN ---

    void Update()
    {
        if (!isExamining || leapProvider == null) return;

        // 1. Obtener la mano del Leap Motion
        Hand hand = GetFirstTrackedHand();

        if (hand != null)
        {
            // 2. Calcular la rotación deseada
            Quaternion handRotation = hand.Rotation;
            
            // La rotación deseada es la de la mano, ajustada por un offset.
            // Sumamos la rotación de la mano a la rotación de la cámara para que la orientación sea relativa al usuario.
            Quaternion cameraRelativeRotation = Camera.main.transform.rotation * handRotation * rotationOffset;

            // 3. Aplicar la Rotación Suavemente (Lerp)
            targetObject.transform.rotation = Quaternion.Lerp(
                targetObject.transform.rotation,
                cameraRelativeRotation,
                rotationLerpSpeed
            );
        }
    }

    /// <summary>
    /// Ayudante para obtener la primera mano rastreada.
    /// </summary>
    private Hand GetFirstTrackedHand()
    {
        Frame frame = leapProvider.CurrentFrame;
        // Puedes cambiar esto a frame.Hands.Find(h => h.IsRight) o IsLeft si solo quieres una mano específica.
        return frame.Hands.Count > 0 ? frame.Hands[0] : null; 
    }
}