

// ------ ARMADO Y DESPIECE CONTINUO BASADO EN PINCH STRENGTH ------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

// Define los diferentes tipos de patrón de despiece que se pueden aplicar.
public enum ExplosionType
{
    Grid,        // Distribución en una cuadrícula.
    Radial,      // Distribución en círculo.
    LinearX,     // Distribución en una sola línea horizontal (eje X).
    LinearY      // Distribución en una sola línea vertical (eje Y).
}

// Estructura para almacenar la información de cada pieza
[System.Serializable]
public class PartData
{
    public Transform partTransform;
    public Vector3 initialPosition;
    public Vector3 targetPosition;
}

public class ExplosionView : MonoBehaviour
{
    // --- Referencias de UltraLeap ---
    [Header("Configuración de UltraLeap")]
    [Tooltip("Referencia al LeapProvider en la escena para obtener datos del frame.")]
    public LeapProvider leapProvider;

    [Tooltip("Si está activo, el script busca el PinchStrength para controlar el despiece/armado.")]
    public bool isGestureControlActive = false; 

    // --- NUEVAS Variables para Control de Pinch ---
    [Header("Configuración de Control Pinch")]
    [Tooltip("Define qué mano usar para controlar la explosión (Right o Left).")]
    public Chirality handToControl = Chirality.Right; 

    [Tooltip("Suavidad/velocidad de reacción del modelo al cambio de Pinch Strength (ej. 10).")]
    public float smoothFactor = 10f; 

    // Variable de estado interna para el factor de explosión actual (entre 0 y 1)
    private float currentExplosionFactor = 0f;

    // --- Parámetros ajustables en el Inspector de Unity ---
    [Header("Configuración de Despiece")]
    [Tooltip("Define el patrón de dispersión de las piezas en el plano XY.")]
    public ExplosionType explosionType = ExplosionType.Grid; 

    [Tooltip("Distancia en el eje Z (local) a la que se alinearán todas las piezas al despiezarse.")]
    public float explosionDistance = 40f; 

    [Tooltip("Espaciado entre piezas. Define la separación entre elementos.")]
    public float explosionSpacing = 10f; 

    // La duración de la animación ya no es relevante en un control continuo
    // public float animationDuration = 1f;

    [Header("Estado del Modelo")]
    [Tooltip("Indica si el modelo está actualmente despiezado (solo informativo).")]
    public bool isExploded = false;

    // Lista de las partes del modelo
    private List<PartData> parts = new List<PartData>();

    // No se usan para el control de Pinch, se mantienen como recordatorio:
    // private float separationThreshold = 0.25f; 
    // private float minTimeBetweenToggles = 1.0f;
    // private float lastToggleTime = 0f;
    // private bool isTrackingSeparation = false;
    // private float initialHandDistance = 0f;

    // ----------------------------------------------------------------------
    // --- Métodos de Ciclo de Vida de Unity ---
    // ----------------------------------------------------------------------

    private void Start()
    {
        InitializeParts();
        
        // Verificación inicial del proveedor de Leap
        if (leapProvider == null)
        {
            Debug.LogError("LeapProvider no asignado. ¡Asigna uno para usar el control de gestos!");
        }
    }

    /// <summary>
    /// Método de Unity que se llama en cada frame.
    /// Lee el PinchStrength y mueve las piezas de forma continua.
    /// </summary>
    private void Update()
    {
        // ** GUARDIA DE MODO: Si el control de gestos está inactivo, no hacemos nada. **
        if (!isGestureControlActive)
        {
            return;
        }

        // 1. Obtener el frame y buscar la mano específica.
        Frame frame = leapProvider?.CurrentFrame;
        Hand controllingHand = null;

        if (frame != null)
        {
            foreach (Hand hand in frame.Hands)
            {
                if (hand.GetChirality() == handToControl) // Asegura que se usa la mano correcta
                {
                    controllingHand = hand;
                    break;
                }
            }
        }
        
        // 2. Determinar el factor de explosión objetivo
        float targetPinchStrength = 0f;
        if (controllingHand != null)
        {
            targetPinchStrength = controllingHand.PinchStrength; 
        }

        // Mapeo: 
        // Pinch = 1.0 (máxima fuerza) -> Explosión = 0.0 (Armado)
        // Pinch = 0.0 (mínima fuerza) -> Explosión = 1.0 (Desarmado)
        float targetExplosionFactor = 1.0f - targetPinchStrength;
        
        // 3. Suavizar el factor de explosión (Lerp)
        currentExplosionFactor = Mathf.Lerp(
            currentExplosionFactor, 
            targetExplosionFactor, 
            Time.deltaTime * smoothFactor
        );

        // 4. Mover las piezas según el factor de explosión continuo
        SetExplosionFactor(currentExplosionFactor);
        
        // 5. Actualizar el estado informativo de 'isExploded'
        // Consideramos que está "despiezado" si el factor es alto.
        isExploded = currentExplosionFactor > 0.5f; 
    }

    public void SetGestureControlActive(bool active)
    {
        isGestureControlActive = active;
    }
    // ----------------------------------------------------------------------
    // --- Lógica de Piezas y Movimiento ---
    // ----------------------------------------------------------------------

    /// <summary>
    /// Identifica las partes (hijos) y calcula sus posiciones objetivo.
    /// </summary>
    private void InitializeParts()
    {
        parts.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            PartData part = new PartData
            {
                partTransform = child,
                initialPosition = child.localPosition,
            };
            parts.Add(part);
        }

        // Calculamos las posiciones objetivo de despiece según el ExplosionType
        CalculateAllTargetPositions();
    }

    /// <summary>
    /// Calcula las posiciones de las piezas según el tipo de explosión seleccionado.
    /// (Este método se mantiene sin cambios, ya que define el diseño final del despiece).
    /// </summary>
    private void CalculateAllTargetPositions()
    {
        int totalParts = parts.Count;
        if (totalParts == 0) return;

        // La posición Z de alineación es constante para todos los tipos de explosión
        float newZ = explosionDistance;

        for (int i = 0; i < totalParts; i++)
        {
            float newX = 0f;
            float newY = 0f;

            switch (explosionType)
            {
                case ExplosionType.Grid:
                    // --- Lógica de Cuadrícula (Grid) ---
                    int gridSize = Mathf.CeilToInt(Mathf.Sqrt(totalParts));
                    int row = i / gridSize;
                    int col = i % gridSize;

                    float centerOffset = (gridSize - 1) * explosionSpacing / 2f;

                    newX = (col * explosionSpacing) - centerOffset;
                    newY = centerOffset - (row * explosionSpacing);
                    break;

                case ExplosionType.Radial:
                    // --- Lógica Radial (Círculo) ---
                    float angle = i * (360f / totalParts) * Mathf.Deg2Rad;
                    float radius = (i + 1) * explosionSpacing * 0.5f; 
                    
                    newX = Mathf.Cos(angle) * radius;
                    newY = Mathf.Sin(angle) * radius;
                    break;
                
                case ExplosionType.LinearX:
                    // --- Lógica Lineal en X (Horizontal) ---
                    float totalLengthX = (totalParts - 1) * explosionSpacing;
                    float startX = -totalLengthX / 2f;
                    
                    newX = startX + (i * explosionSpacing);
                    newY = 0f; 
                    break;

                case ExplosionType.LinearY:
                    // --- Lógica Lineal en Y (Vertical) ---
                    float totalLengthY = (totalParts - 1) * explosionSpacing;
                    float startY = -totalLengthY / 2f;

                    newX = 0f; 
                    newY = startY + (i * explosionSpacing);
                    break;
            }

            // Asignamos la posición objetivo
            parts[i].targetPosition = new Vector3(newX, newY, newZ);
        }
    }

    /// <summary>
    /// Mueve las piezas según un factor de explosión continuo (0.0 = Armado, 1.0 = Desarmado).
    /// </summary>
    /// <param name="factor">El factor de interpolación entre la posición inicial y la objetivo.</param>
    public void SetExplosionFactor(float factor)
    {
        float clampedFactor = Mathf.Clamp01(factor);

        foreach (var part in parts)
        {
            Vector3 start = part.initialPosition;
            Vector3 end = part.targetPosition;

            // Movemos la parte usando Lerp (interpolación lineal)
            part.partTransform.localPosition = Vector3.Lerp(start, end, clampedFactor);
        }
    }
    
    // El método ToggleExplosion() y la corrutina AnimateExplosion() han sido eliminados.

    // Opcional: Para resetear las posiciones iniciales si cambias el modelo en tiempo de ejecución
    [ContextMenu("Reset Initial Positions")]
    public void ResetInitialPositions()
    {
        SetExplosionFactor(0f);
        InitializeParts();
    }
}