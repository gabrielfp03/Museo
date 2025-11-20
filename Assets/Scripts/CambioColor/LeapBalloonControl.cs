using UnityEngine;
using Leap;
// Sin 'using Leap.Unity' como pediste.
// Si LeapServiceProvider te da error, cámbialo por MonoBehaviour.

public class LeapBalloonControl : MonoBehaviour
{
    [Header("Referencias Leap")]
    public LeapServiceProvider leapProvider;

    [Header("Objetos de la Escena")]
    public GameObject esferaBoton;  // Arrastra aquí la ESFERA (Interruptor)
    public GameObject globoObjeto;  // Arrastra aquí el GLOBO (El objeto entero)

    [Header("Configuración")]
    public float distanciaToque = 0.15f;
    public float velocidadSwipe = 0.8f;
    public float cooldown = 1.0f;

    // Variables internas
    private ChangeColorObject _globoScriptColor; // Aquí guardaremos el script del color
    private bool sistemaConectado = false;
    private float ultimoTiempo = 0f;

    void Start()
    {
        // 1. Buscamos el Leap si está vacío
        if (leapProvider == null)
            leapProvider = FindAnyObjectByType<LeapServiceProvider>();

        // 2. Buscamos el script de color DENTRO del globo que has arrastrado
        if (globoObjeto != null)
        {
            _globoScriptColor = globoObjeto.GetComponent<ChangeColorObject>();
            if (_globoScriptColor == null)
            {
                Debug.LogError("⚠️ ¡Cuidado! El objeto Globo que has puesto NO tiene el script 'ChangeColorObject' puesto.");
            }
        }
    }

    void Update()
    {
        if (leapProvider == null) return;

        Frame frame = leapProvider.CurrentFrame;
        if (frame.Hands.Count == 0) return;

        Hand mano = frame.Hands[0];

        // Datos de la mano (Asumiendo Vector3 de Unity directo como en tu LeapCursor)
        Vector3 puntaDedo = mano.Index.TipPosition;
        Vector3 velocidadPalma = mano.PalmVelocity;

        // --- A. LÓGICA INTERRUPTOR (Esfera) ---
        float distancia = Vector3.Distance(puntaDedo, esferaBoton.transform.position);

        if (distancia < distanciaToque)
        {
            if (Time.time > ultimoTiempo + cooldown)
            {
                sistemaConectado = !sistemaConectado;
                ultimoTiempo = Time.time;

                // Cambiamos color de la ESFERA para saber si está ON/OFF
                Renderer ren = esferaBoton.GetComponent<Renderer>();
                if (ren != null)
                {
                    ren.material.color = sistemaConectado ? Color.green : Color.red;
                }
                Debug.Log(sistemaConectado ? "✅ CONECTADO" : "❌ DESCONECTADO");
            }
        }

        // --- B. LÓGICA SWIPE (Globo) ---
        if (sistemaConectado && _globoScriptColor != null)
        {
            if (Mathf.Abs(velocidadPalma.x) > velocidadSwipe)
            {
                if (Time.time > ultimoTiempo + 0.5f)
                {
                    _globoScriptColor.SiguienteColor();
                    ultimoTiempo = Time.time;
                    Debug.Log("🎨 Cambio de color enviado al globo");
                }
            }
        }
    }
}