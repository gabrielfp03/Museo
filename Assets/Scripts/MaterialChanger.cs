using UnityEngine;
using Leap;
using System.Collections.Generic;

/// <summary>
/// Permite cambiar el material completo (textura, color, propiedades) del modelo 3D 
/// mediante un gesto de 'swipe' (movimiento horizontal) de la mano de Ultraleap,
/// detectado por la velocidad de la palma.
/// El script debe adjuntarse al GameObject cuyo material se desea cambiar.
/// </summary>
public class HandMaterialChanger : MonoBehaviour
{
    [Header("Referencias del Sistema")]
    [Tooltip("Arrastra aquí tu Leap Service Provider para obtener datos de la mano.")]
    public LeapServiceProvider leapProvider;
    
    [Header("Materiales para Alternar")]
    [Tooltip("Lista de todos los materiales que se aplicarán al modelo. Arrastra los materiales aquí.")]
    public List<Material> availableMaterials = new List<Material>();

    [Header("Configuración del Gesto")]
    [Tooltip("Umbral de distancia máxima al objeto para que el gesto sea detectado (en metros).")]
    public float proximityThreshold = 50f;
    [Tooltip("Velocidad mínima (en metros/segundo) requerida en el eje X para registrar un swipe. (0.5f a 2.0f es el rango típico).")]
    public float minSwipeVelocity = 100f;

    // --- Variables de Estado ---
    private Renderer targetRenderer;
    private int currentMaterialIndex = 0;
    private bool isReadyForNewSwipe = true; 
    private const float COOLDOWN_TIME = 0.5f; // Tiempo de espera entre swipes

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

    void Update()
    {
        if (leapProvider == null || availableMaterials.Count == 0) return;

        Frame frame = leapProvider.CurrentFrame;
        // Tomamos la primera mano detectada
        Hand hand = frame.Hands.Count > 0 ? frame.Hands[0] : null;

        if (hand != null)
        {
            // 1. Chequeo de Proximidad: la mano debe estar cerca del objeto (este GameObject)
            float distanceToHand = Vector3.Distance(transform.position, hand.PalmPosition);
            if (distanceToHand > proximityThreshold)
            {   
                Debug.Log("Mano fuera de rango para cambiar material.");
                // Mano muy lejos, reiniciamos el estado de cooldown
                isReadyForNewSwipe = true;
                return;
            }

            // 2. Detección de Swipe Horizontal basada en VELOCIDAD (en m/s)
            float palmVelocityX = hand.PalmVelocity.x;

            if (isReadyForNewSwipe)
            {
                if (palmVelocityX > minSwipeVelocity)
                {
                    // Swipe hacia la DERECHA (Velocidad positiva en X)
                    ChangeMaterial(1);
                    isReadyForNewSwipe = false;
                    Invoke("ResetSwipeCooldown", COOLDOWN_TIME);
                }
                else if (palmVelocityX < -minSwipeVelocity)
                {
                    // Swipe hacia la IZQUIERDA (Velocidad negativa en X)
                    ChangeMaterial(-1);
                    isReadyForNewSwipe = false;
                    Invoke("ResetSwipeCooldown", COOLDOWN_TIME);
                }
            }
        }
        else // Si no hay mano, permitimos un nuevo swipe cuando reaparezca
        {
            isReadyForNewSwipe = true;
        }
    }

    /// <summary>
    /// Cambia el índice del material (con efecto de bucle) y aplica el nuevo material.
    /// </summary>
    /// <param name="direction">1 para siguiente, -1 para anterior.</param>
    private void ChangeMaterial(int direction)
    {
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

    /// <summary>
    /// Resetea el estado de cooldown para permitir un nuevo swipe.
    /// </summary>
    private void ResetSwipeCooldown()
    {
        isReadyForNewSwipe = true;
    }
}