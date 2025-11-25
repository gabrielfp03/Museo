using UnityEngine;
using Leap;
using System.Collections.Generic;
using System.Collections; // Necesario para la corrutina

/// <summary>
/// Permite cambiar el material completo (textura, color, propiedades) del modelo 3D 
/// mediante un gesto de 'swipe' (movimiento horizontal) de la mano de Ultraleap,
/// detectado por la velocidad de la palma O por la traslación de la palma en el eje X.
/// El script debe adjuntarse al GameObject cuyo material se desea cambiar.
/// </summary>
public class HandMaterialChanger : MonoBehaviour
{
    [Header("Control de Activación")]
    [Tooltip("Si está activo, el cambio de material por swipe está permitido.")]
    public bool isMaterialChangeEnabled = false; // Estado inicial, deshabilitado por defecto

    [Header("Referencias del Sistema")]
    [Tooltip("Arrastra aquí tu Leap Service Provider para obtener datos de la mano.")]
    public LeapServiceProvider leapProvider;
    
    [Header("Materiales para Alternar")]
    [Tooltip("Lista de todos los materiales que se aplicarán al modelo. Arrastra los materiales aquí.")]
    public List<Material> availableMaterials = new List<Material>();

    [Header("Configuración del Gesto")]
    [Tooltip("Umbral de distancia máxima al objeto para que el gesto sea detectado (en metros).")]
    public float proximityThreshold = 50f;
    [Tooltip("Velocidad mínima (en metros/segundo) requerida en el eje X para registrar un swipe rápido. (0.5f a 2.0f es el rango típico).")]
    public float minSwipeVelocity = 50f;
    
    [Header("Configuración de Traslación (Ajuste)")]
    [Tooltip("Distancia mínima (en metros) de traslación en X para registrar un movimiento (ej. 0.05m = 5cm).")]
    public float minSwipeDistance = 0.05f; // Nuevo umbral de distancia (5 cm)

    // --- Variables de Estado ---
    private Renderer targetRenderer;
    private int currentMaterialIndex = 0;
    private bool isReadyForNewSwipe = true; 
    private const float COOLDOWN_TIME = 0.5f; // Reducido para mejor UX, pero puedes usar 5f
    
    // --- Variables de Traslación ---
    private float _lastPalmXPosition = 0f; // Almacena la posición X de la palma en el frame anterior
    private bool _hasPreviousPalmPosition = false; // Bandera para el primer frame

    void Start()
    {
        // 1. Obtener el componente Renderer (donde se asigna el material)
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogError("HandMaterialChanger requiere un componente Renderer en el GameObject adjunto.");
            enabled = false;
            return;
        }

        // 2. Aplicar el material inicial
        if (availableMaterials.Count > 0)
        {
            ApplyMaterial(availableMaterials[currentMaterialIndex]);
        } 
        else
        {
            Debug.LogWarning("No hay materiales asignados en la lista 'availableMaterials'.");
        }
    }

    /// <summary>
    /// Función pública que puede ser llamada por un botón (Button.OnClick) u otro script
    /// para habilitar o deshabilitar el cambio de material.
    /// </summary>
    /// <param name="enable">True para activar la funcionalidad, False para desactivarla.</param>
    public void SetMaterialChangeEnabled(bool enable)
    {
        isMaterialChangeEnabled = enable;
    }


    void Update()
    {
        if (leapProvider == null || availableMaterials.Count == 0) return;
        
        // --- NUEVA CONDICIÓN DE ACTIVACIÓN ---
        if (!isMaterialChangeEnabled)
        {
            return;
        }


        Frame frame = leapProvider.CurrentFrame;
        // Tomamos la primera mano detectada
        Hand hand = frame.Hands.Count > 0 ? frame.Hands[0] : null;

        if (hand != null)
        {
            // 1. Chequeo de Proximidad: la mano debe estar cerca del objeto (este GameObject)
            float distanceToHand = Vector3.Distance(transform.position, hand.PalmPosition);
            if (distanceToHand > proximityThreshold)
            {   
                // Mano muy lejos, reiniciamos el estado de cooldown y salimos
                isReadyForNewSwipe = true;
                _hasPreviousPalmPosition = false; // Reseteamos la posición para evitar un delta grande al volver
                return;
            }

            // --- Lógica de Detección de Gesto Combinada (Velocidad y Traslación) ---

            if (isReadyForNewSwipe)
            {
                // **A. Detección por Velocidad (El 'Swipe' original y rápido)**
                // Si la velocidad es alta, disparamos inmediatamente
                float palmVelocityX = hand.PalmVelocity.x;
                if (palmVelocityX > minSwipeVelocity)
                {
                    // Swipe rápido hacia la DERECHA
                    ChangeMaterial(1);
                    StartCooldown();
                    return;
                }
                else if (palmVelocityX < -minSwipeVelocity)
                {
                    // Swipe rápido hacia la IZQUIERDA
                    ChangeMaterial(-1);
                    StartCooldown();
                    return;
                }
                
                // **B. Detección por Traslación (El 'Saludo' lento pero largo)**
                if (_hasPreviousPalmPosition)
                {
                    float currentPalmXPosition = hand.PalmPosition.x;
                    // Calcula la distancia recorrida en X desde el frame anterior
                    float deltaX = currentPalmXPosition - _lastPalmXPosition; 

                    if (deltaX > minSwipeDistance)
                    {
                        // Traslación hacia la DERECHA (lenta)
                        ChangeMaterial(1);
                        StartCooldown();
                        _lastPalmXPosition = currentPalmXPosition; // Actualizar para evitar doble disparo en el mismo movimiento
                        return;

                    }
                    else if (deltaX < -minSwipeDistance)
                    {
                        // Traslación hacia la IZQUIERDA (lenta)
                        ChangeMaterial(-1);
                        StartCooldown();
                        _lastPalmXPosition = currentPalmXPosition; // Actualizar para evitar doble disparo en el mismo movimiento
                        return;
                    }
                }

                // Finalmente, actualizamos la posición anterior de la palma para el siguiente frame
                _lastPalmXPosition = hand.PalmPosition.x;
                _hasPreviousPalmPosition = true;

            }
            // Si no está ready (en cooldown), solo actualizamos la posición sin intentar el cambio
            else if (_hasPreviousPalmPosition)
            {
                _lastPalmXPosition = hand.PalmPosition.x;
            }
        }
        else // Si no hay mano, permitimos un nuevo swipe cuando reaparezca y reseteamos la posición
        {
            isReadyForNewSwipe = true;
            _hasPreviousPalmPosition = false;
        }
    }

    /// <summary>
    /// Inicia el tiempo de espera (cooldown) después de un gesto.
    /// </summary>
    private void StartCooldown()
    {
        isReadyForNewSwipe = false;
        // Usamos StartCoroutine en lugar de Invoke para mejor gestión
        StartCoroutine(ResetSwipeCooldown()); 
    }
    
    /// <summary>
    /// Resetea el estado de cooldown para permitir un nuevo swipe (usando corrutina).
    /// </summary>
    private IEnumerator ResetSwipeCooldown()
    {
        yield return new WaitForSeconds(COOLDOWN_TIME);
        isReadyForNewSwipe = true;
    }

    /// <summary>
    /// Cambia el índice del material (con efecto de bucle) y aplica el nuevo material.
    /// </summary>
    /// <param name="direction">1 para siguiente, -1 para anterior.</param>
    private void ChangeMaterial(int direction)
    {
        // ... (Lógica de bucle de materiales no cambia) ...
        currentMaterialIndex += direction;

        // Asegurar que el índice se mantenga dentro de los límites de la lista (con bucle)
        if (currentMaterialIndex >= availableMaterials.Count)
        {
            currentMaterialIndex = 0; // Vuelve al inicio
        }
        else if (currentMaterialIndex < 0)
        {
            currentMaterialIndex = availableMaterials.Count - 1; // Vuelve al final
        }

        ApplyMaterial(availableMaterials[currentMaterialIndex]);
        Debug.Log($"Material cambiado a: {availableMaterials[currentMaterialIndex].name}");
    }

    /// <summary>
    /// Aplica el material al Renderer.
    /// Esto asume que el objeto tiene un solo material (Element 0).
    /// </summary>
    private void ApplyMaterial(Material newMaterial)
    {
        if (targetRenderer != null)
        {
            // Reemplaza el material principal del objeto
            targetRenderer.material = newMaterial;
        }
    }
}
