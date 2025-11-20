using UnityEngine;
using Leap;

public class LeapBalloonControl : MonoBehaviour
{
    [Header("Referencias Leap")]
    public LeapServiceProvider leapProvider; // Usamos esto porque en tu LeapCursor funciona

    [Header("Objetos de la Escena")]
    public GameObject esferaBoton;        // El interruptor (Esfera)
    public ChangeColorObject globoScript; // El globo que cambia de color

    [Header("Configuración")]
    public float distanciaToque = 0.15f;  
    public float velocidadSwipe = 0.8f;   
    public float cooldown = 1.0f;         

    // Estado interno
    private bool sistemaConectado = false;
    private float ultimoTiempo = 0f;

    void Start()
    {
        if (leapProvider == null)
            leapProvider = FindObjectOfType<LeapServiceProvider>();
    }

    void Update()
    {
        if (leapProvider == null) return;

        Frame frame = leapProvider.CurrentFrame;
        if (frame.Hands.Count == 0) return;

        Hand mano = frame.Hands[0];

        // --- SIN CONVERSIONES RARAS ---
        // Asumimos que tu versión ya devuelve Vector3 de Unity
        Vector3 puntaDedo = mano.Index.TipPosition;
        Vector3 velocidadPalma = mano.PalmVelocity;

        // A. INTERRUPTOR (TOCAR LA ESFERA)
        float distancia = Vector3.Distance(puntaDedo, esferaBoton.transform.position);

        if (distancia < distanciaToque)
        {
            if (Time.time > ultimoTiempo + cooldown)
            {
                sistemaConectado = !sistemaConectado; 
                ultimoTiempo = Time.time;

                // Feedback Visual en la Esfera
                Renderer ren = esferaBoton.GetComponent<Renderer>();
                if (ren != null)
                {
                    ren.material.color = sistemaConectado ? Color.green : Color.red;
                }

                Debug.Log(sistemaConectado ? "✅ GLOBO CONECTADO" : "❌ GLOBO DESCONECTADO");
            }
        }

        // B. SWIPE (CAMBIAR COLOR DEL GLOBO)
        if (sistemaConectado)
        {
            // Detectamos velocidad lateral (Valor absoluto para izquierda o derecha)
            if (Mathf.Abs(velocidadPalma.x) > velocidadSwipe)
            {
                if (Time.time > ultimoTiempo + 0.5f) 
                {
                    globoScript.SiguienteColor();
                    ultimoTiempo = Time.time;
                    Debug.Log("🎨 Color del Globo Cambiado");
                }
            }
        }
    }
}