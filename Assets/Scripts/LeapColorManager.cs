using UnityEngine;
using Leap;

public class LeapColorManager : MonoBehaviour
{
    [Header("Arrastra aquí tus objetos")]
    public GameObject botonCubo;       // Tu botón (Cubo)
    public ChangeColorObject objetoCambio; // Tu objeto a pintar (Esfera)

    [Header("Configuración")]
    public float distanciaActivacion = 0.15f; // Distancia para tocar botón
    public float distanciaSwipe = 0.30f;      // Distancia para tocar objeto
    public float velocidadSwipe = 0.8f;       // Velocidad de la mano
    public float cooldown = 0.5f;             // Tiempo entre cambios

    // Variables internas
    private LeapProvider _leapProvider;
    private bool _sistemaActivado = false;
    private float _ultimoCambio = 0f;

    void Start()
    {
        // BUSQUEDA AUTOMÁTICA:
        // Buscamos en la escena el objeto que tiene el cerebro del Leap Motion
        _leapProvider = FindFirstObjectByType<LeapProvider>();

        if (_leapProvider == null)
        {
            Debug.LogError("ERROR: No encuentro el 'Leap Service Provider' en la escena. Asegúrate de tener el prefab de Leap Motion puesto.");
        }
    }

    void Update()
    {
        if (_leapProvider == null) return;

        // 1. Pedimos los datos al sensor (Frame actual)
        Frame frame = _leapProvider.CurrentFrame;

        // Si no hay manos visibles, no hacemos nada
        if (frame.Hands.Count == 0) return;

        // Cogemos la primera mano que veamos
        Hand mano = frame.Hands[0];

        // Convertimos la posición de la palma a coordenadas de Unity
        Vector3 manoPos = mano.PalmPosition;
        
        // --- LÓGICA ---

        // A. ACTIVAR CON BOTÓN
        if (!_sistemaActivado)
        {
            
            if (Vector3.Distance(manoPos, botonCubo.transform.position) < distanciaActivacion)
            {
                Debug.Log("Sistema Activado");
                ActivarSistema();
            }
        }

        // B. CAMBIAR COLOR (SWIPE)
        if (_sistemaActivado)
        {
            float distanciaAlObjeto = Vector3.Distance(manoPos, objetoCambio.transform.position);

            // Si la mano está cerca del objeto
            if (distanciaAlObjeto < distanciaSwipe)
            {
                // Si la mano se mueve rápido lateralmente (Eje X)
                if (Mathf.Abs(mano.PalmVelocity.x) > velocidadSwipe)
                {
                    // Y si ha pasado el tiempo de espera
                    if (Time.time > _ultimoCambio + cooldown)
                    {
                        objetoCambio.SiguienteColor();
                        _ultimoCambio = Time.time;
                        Debug.Log("¡Color Cambiado!");
                    }
                }
            }
        }
    }

    void ActivarSistema()
    {
        _sistemaActivado = true;
        botonCubo.GetComponent<Renderer>().material.color = Color.green;
        Debug.Log("SISTEMA ACTIVADO");
    }
}