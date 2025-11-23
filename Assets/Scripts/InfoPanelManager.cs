using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoPanelManager : MonoBehaviour
{
    [Header("UI Panel Info")]
    public GameObject panelContenedor;
    public TextMeshProUGUI tituloText;
    public TextMeshProUGUI descripcionText;
    public Button botonCerrar;
    
    [Header("Controles del Panel")]
    public TMP_Dropdown dropdownIdioma;
    public TMP_Dropdown dropdownNivel;
    public Button botonAudio;
    public AudioSource fuenteAudio;

    // [Header("COSAS A DESACTIVAR (Arrastra aquí)")]
    // public GameObject interfazMovilRoja; // Arrastra el canvas de las flechas rojas
    // public GameObject jugador; // Arrastra tu objeto "FirstPersonController" o "PlayerCapsule"

    private InfoObraMuseo infoActual; 
    private MonoBehaviour[] scriptsJugador; // Para guardar los scripts del jugador

    void Start()
    {
        if(panelContenedor != null) panelContenedor.SetActive(false);
        Debug.Log("InfoPanelManager iniciado.");
        // Listeners
        if(botonCerrar != null) botonCerrar.onClick.AddListener(CerrarPanel);
        if(dropdownIdioma != null) dropdownIdioma.onValueChanged.AddListener(delegate { ActualizarContenido(); });
        if(dropdownNivel != null) dropdownNivel.onValueChanged.AddListener(delegate { ActualizarContenido(); });
        if(botonAudio != null) botonAudio.onClick.AddListener(ReproducirAudio);

        // // Buscamos los scripts del jugador al inicio para poder desactivarlos luego
        // if (jugador != null)
        // {
        //     scriptsJugador = jugador.GetComponents<MonoBehaviour>();
        // }
    }

    public void AbrirPanel(InfoObraMuseo info)
    {
        infoActual = info;
        tituloText.text = info.nombreObjeto;
        
        // 1. ABRIR PANEL
        panelContenedor.SetActive(true);
        
        // // 2. OCULTAR FLECHAS ROJAS (Para que no tapen los clicks)
        // if (interfazMovilRoja != null) interfazMovilRoja.SetActive(false);

        // // 3. PARALIZAR JUGADOR (Para que el click no bloquee el ratón)
        // if (jugador != null)
        // {
        //     // Desactivamos todos los scripts del jugador temporalmente
        //     foreach (var script in scriptsJugador)
        //     {
        //         // OJO: No desactivamos ESTE script (InfoPanelManager) ni AudioSource
        //         if (script != this && !(script is AudioSource)) 
        //             script.enabled = false;
        //     }
        // }

        // // 4. LIBERAR RATÓN
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;

        ActualizarContenido();
    }

    public void CerrarPanel()
    {
        fuenteAudio.Stop();
        panelContenedor.SetActive(false);

        // 1. MOSTRAR FLECHAS ROJAS OTRA VEZ
        //if (interfazMovilRoja != null) interfazMovilRoja.SetActive(true);

        // 2. REACTIVAR JUGADOR
        // if (jugador != null)
        // {
        //     foreach (var script in scriptsJugador)
        //     {
        //         if (script != this && !(script is AudioSource)) 
        //             script.enabled = true;
        //     }
        // }

        // // 3. BLOQUEAR RATÓN PARA JUGAR
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    // ... (Mantén aquí tus funciones ActualizarContenido y ReproducirAudio igual que antes) ...
    // COPIA AQUÍ EL RESTO DE TU CÓDIGO (ActualizarContenido y ReproducirAudio)
    void ActualizarContenido()
    {
        if (infoActual == null) return;
        fuenteAudio.Stop(); 

        int idioma = dropdownIdioma.value;
        int nivel = dropdownNivel.value;

        if (idioma == 0) // Español
        {
            if (nivel == 0) descripcionText.text = infoActual.textoES_Niño;
            else if (nivel == 1) descripcionText.text = infoActual.textoES_Casual;
            else descripcionText.text = infoActual.textoES_Experto;
        }
        else // Inglés
        {
            if (nivel == 0) descripcionText.text = infoActual.textoEN_Niño;
            else if (nivel == 1) descripcionText.text = infoActual.textoEN_Casual;
            else descripcionText.text = infoActual.textoEN_Experto;
        }
    }

    void ReproducirAudio()
    {
        if (infoActual == null) return;
        
        int idioma = dropdownIdioma.value;
        int nivel = dropdownNivel.value;
        AudioClip clip = null;

        if (idioma == 0) // Español
        {
            if (nivel == 0) clip = infoActual.audioES_Niño;
            else if (nivel == 1) clip = infoActual.audioES_Casual;
            else clip = infoActual.audioES_Experto;
        }
        else // Inglés
        {
            if (nivel == 0) clip = infoActual.audioEN_Niño;
            else if (nivel == 1) clip = infoActual.audioEN_Casual;
            else clip = infoActual.audioEN_Experto;
        }

        if (clip != null) { fuenteAudio.clip = clip; fuenteAudio.Play(); }
    }
}