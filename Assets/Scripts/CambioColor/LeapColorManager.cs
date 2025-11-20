using UnityEngine;
using Leap; // Solo Leap, como pediste

public class LeapHandController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject botonActivador;       // El cubo botón
    public ChangeColorObject objetoCambio;  // La esfera que cambia de color

    [Header("Ajustes")]
    public float distanciaActivacion = 0.15f;
    public float distanciaSwipe = 0.30f;
    public float velocidadNecesaria = 0.8f;
    public float tiempoEspera = 1.0f;

    // Variables internas
    private bool sistemaActivado = false;
    private float ultimoTiempo = 0f;
    
    // Referencia al modelo de la mano (la clase que usabas en tu otro script)
    private HandModelBase handModel; 

    void Start()
    {
        // Buscamos el HandModelBase en este objeto o en los padres
        // (Igual que hacías en LeapGrabObject)
        handModel = GetComponentInParent<HandModelBase>();

        if (handModel == null)
        {
            handModel = GetComponent<HandModelBase>();
        }
    }

    void Update()
    {
        // Si no detecta la mano, no hacemos nada
        if (handModel == null || !handModel.IsTracked) return;

        Hand mano = handModel.GetLeapHand();
        if (mano == null) return;

        // Convertimos la posición de Leap a Unity manualmente para evitar errores de librerías
        Vector3 manoPos = new Vector3(mano.PalmPosition.x, mano.PalmPosition.y, mano.PalmPosition.z);
        Vector3 manoVel = new Vector3(mano.PalmVelocity.x, mano.PalmVelocity.y, mano.PalmVelocity.z);

        // 1. LÓGICA DEL BOTÓN
        if (!sistemaActivado)
        {
            // Usamos la distancia física en Unity
            if (Vector3.Distance(transform.position, botonActivador.transform.position) < distanciaActivacion)
            {
                sistemaActivado = true;
                // Feedback visual (verde)
                if(botonActivador.GetComponent<Renderer>())
                    botonActivador.GetComponent<Renderer>().material.color = Color.green;
                
                Debug.Log("SISTEMA ACTIVADO");
            }
        }

        // 2. LÓGICA DEL SWIPE
        if (sistemaActivado)
        {
            float dist = Vector3.Distance(transform.position, objetoCambio.transform.position);

            // Si la mano está cerca del objeto
            if (dist < distanciaSwipe)
            {
                // Y se mueve rápido en lateral (Eje X)
                if (Mathf.Abs(manoVel.x) > velocidadNecesaria)
                {
                    if (Time.time > ultimoTiempo + tiempoEspera)
                    {
                        objetoCambio.SiguienteColor();
                        ultimoTiempo = Time.time;
                    }
                }
            }
        }
    }
}