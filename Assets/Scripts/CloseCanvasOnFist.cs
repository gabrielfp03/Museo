using UnityEngine;
using Leap;       // Y el namespace principal de Leap

public class CloseCanvasOnFist : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí tu Leap Service Provider de la escena.")]
    public LeapServiceProvider leapProvider;

    [Header("Configuración del Gesto")]
    [Tooltip("Umbral de fuerza de agarre (0.0 a 1.0) para detectar un puño. 0.9 es un puño casi cerrado.")]
    [Range(0.0f, 1.0f)]
    public float grabThreshold = 0.9f;
    
    [Tooltip("Tiempo mínimo que el puño debe estar cerrado antes de activar la acción (para evitar cierres accidentales).")]
    public float requiredHoldTime = 0.5f;

    [Header("Estado Interno")]
    private bool isFistDetected = false;
    private float fistTimer = 0f;

    void Update()
    {
        if (leapProvider == null) return;

        Frame frame = leapProvider.CurrentFrame;

        // Itera sobre todas las manos detectadas en el frame
        foreach (Hand hand in frame.Hands)
        {
            // 1. Detección de Puño (Fuerza de Agarre)
            if (hand.GrabStrength >= grabThreshold)
            {
                // Si la detección es nueva o el puño sigue cerrado
                if (!isFistDetected)
                {
                    isFistDetected = true;
                    fistTimer = 0f;
                }
                
                // 2. Temporizador de Confirmación
                if (isFistDetected)
                {
                    fistTimer += Time.deltaTime;
                    
                    if (fistTimer >= requiredHoldTime)
                    {
                        // Gesto de Puño Confirmado y Sostenido
                        CloseCanvas();
                        
                        // Detenemos el loop para que no procese más manos
                        return; 
                    }
                }
            }
            else
            {
                // Si la mano estaba en puño y se abre, reiniciamos
                isFistDetected = false;
                fistTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Función para desactivar el Canvas.
    /// </summary>
    private void CloseCanvas()
    {
        // 🛑 Importante: Puedes añadir una capa visual aquí (ej. un feedback visual)
        
        Debug.Log("Canvas cerrado por gesto de puño.");
        
        // Desactiva el GameObject raíz del Canvas
        gameObject.SetActive(false); 

        // Reseteamos el estado para que el puño cerrado no lo abra de nuevo inmediatamente
        isFistDetected = false;
        fistTimer = 0f;
    }
}