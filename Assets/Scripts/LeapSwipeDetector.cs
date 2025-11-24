using UnityEngine;
using Leap;            // Necesario para acceder a los datos de Leap
using UnityEngine.Events;

public class LeapSwipeDetector : MonoBehaviour
{
    public LeapProvider leapProvider; // Arrastra aquí tu LeapServiceProvider
    public UnityEvent OnRightSwipe = new UnityEvent();
    public UnityEvent OnLeftSwipe = new UnityEvent();

    [Header("Configuración del Gesto")]
    public float velocidadMinima = 1000f; // Qué tan rápido hay que mover la mano
    public float tiempoEntreSwipes = 1.0f; // Segundos de espera para no hacer 2 swipes seguidos

    private float ultimoSwipeTime = 0;

    void Update()
    {
        // 1. Obtener el frame actual de datos
        Frame frame = leapProvider.CurrentFrame;

        // 2. Si hay alguna mano visible...
        if (frame.Hands.Count > 0)
        {
            // Cogemos la primera mano que detecte
            Hand mano = frame.Hands[0];

            // 3. Comprobamos el Cooldown (tiempo de espera)
            if (Time.time - ultimoSwipeTime > tiempoEntreSwipes)
            {
                // 4. Detectar Swipe a la IZQUIERDA (Velocidad negativa en X)
                // Imagina mover la mano para pasar página hacia la izquierda (Siguiente)
                if (mano.PalmVelocity.x < -velocidadMinima)
                {
                    Debug.Log("Swipe Izquierda");
                    OnLeftSwipe.Invoke();
                    ultimoSwipeTime = Time.time;
                }
                
                // 5. Detectar Swipe a la DERECHA (Velocidad positiva en X)
                // Mover la mano hacia la derecha (Anterior)
                else if (mano.PalmVelocity.x > velocidadMinima)
                {
                    Debug.Log("Swipe Derecha");
                    OnRightSwipe.Invoke();
                    ultimoSwipeTime = Time.time;
                }
            }
        }
    }
}