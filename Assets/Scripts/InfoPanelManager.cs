// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;
// using UnityEngine.Events;

// public class InfoPanelManager : MonoBehaviour
// {
//     [Header("UI Panel Info")]
//     public GameObject panelContenedor;
//     public TextMeshProUGUI tituloText;
//     public TextMeshProUGUI descripcionText;
//     public Button botonCerrar;
    
//     [Header("Controles del Panel")]
//     public TMP_Dropdown dropdownIdioma;
//     public TMP_Dropdown dropdownNivel;
//     public Button botonAudio;
//     public AudioSource fuenteAudio;
//     private InfoObraMuseo infoActual; 

//     void Start()
//     {
//         if(panelContenedor != null) panelContenedor.SetActive(false);
//         Debug.Log("InfoPanelManager iniciado.");
//         // Listeners
//         if(botonCerrar != null) botonCerrar.onClick.AddListener(CerrarPanel);
//         if(dropdownIdioma != null) dropdownIdioma.onValueChanged.AddListener(delegate { ActualizarContenido(); });
//         if(dropdownNivel != null) dropdownNivel.onValueChanged.AddListener(delegate { ActualizarContenido(); });
//     }

//     public void AbrirPanel(InfoObraMuseo info)
//     {
//         infoActual = info;
//         tituloText.text = info.nombreObjeto;

//         panelContenedor.SetActive(true);
//         ActualizarContenido();
//     }

//     public void CerrarPanel()
//     {
//         fuenteAudio.Stop();
//         panelContenedor.SetActive(false);
//     }

//     public void ActualizarContenido()
//     {
//         if (infoActual == null) return;
//         fuenteAudio.Stop(); 

//         int idioma = dropdownIdioma.value;
//         int nivel = dropdownNivel.value;

//         if (idioma == 0) // Español
//         {
//             if (nivel == 0) descripcionText.text = infoActual.textoES_Niño;
//             else if (nivel == 1) descripcionText.text = infoActual.textoES_Casual;
//             else descripcionText.text = infoActual.textoES_Experto;
//         }
//         else // Inglés
//         {
//             if (nivel == 0) descripcionText.text = infoActual.textoEN_Niño;
//             else if (nivel == 1) descripcionText.text = infoActual.textoEN_Casual;
//             else descripcionText.text = infoActual.textoEN_Experto;
//         }
//     }

//     public void ReproducirAudio()
//     {
//         if (infoActual == null) return;
        
//         int idioma = dropdownIdioma.value;
//         int nivel = dropdownNivel.value;
//         AudioClip clip = null;

//         if (idioma == 0) // Español
//         {
//             if (nivel == 0) clip = infoActual.audioES_Niño;
//             else if (nivel == 1) clip = infoActual.audioES_Casual;
//             else clip = infoActual.audioES_Experto;
//         }
//         else // Inglés
//         {
//             if (nivel == 0) clip = infoActual.audioEN_Niño;
//             else if (nivel == 1) clip = infoActual.audioEN_Casual;
//             else clip = infoActual.audioEN_Experto;
//         }

//         if (clip != null) { fuenteAudio.clip = clip; fuenteAudio.Play(); }
//     }
// }

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class InfoPanelManager : MonoBehaviour
{
    // --- Referencias a los Tres Paneles ---
    [Header("Estructura de Paneles")]
    // Contenedor principal (para activarlo/desactivarlo desde fuera)
    public GameObject panelContenedor; 
    public GameObject panelIdioma;
    public GameObject panelNivel;
    public GameObject panelNivelIngles;
    public GameObject panelInformacionFinal;

    // --- Elementos de UI del Panel Final ---
    [Header("Elementos del Panel Final")]
    public TextMeshProUGUI tituloText;
    public TextMeshProUGUI descripcionText;
    public Button botonAudio; // El botón de Audio puede estar aquí.
    public AudioSource fuenteAudio;
    public Button botonCerrar; // Botón para cerrar todo el flujo
    public Button botonCambiarOpciones; // Botón para volver al inicio del flujo

    // --- Botones de Control ---
    [Header("Controles de Selección (Asignar Botones)")]
    // Idioma
    public Button botonIdiomaEspanol;
    public Button botonIdiomaIngles;
    // Nivel
    public Button botonNivelNino;
    public Button botonNivelCasual;
    public Button botonNivelExperto;
    public Button botonNivelNinoEN;
    public Button botonNivelCasualEN;
    public Button botonNivelExpertoEN;

    // --- Variables de Estado Internas ---
    private InfoObraMuseo infoActual; 
    private int idiomaSeleccionado = 0; // 0 = Español, 1 = Inglés
    private int nivelSeleccionado = 0;  // 0 = Niño, 1 = Casual, 2 = Experto

    void Start()
    {
        // Se asegura de que el contenedor principal esté apagado al inicio
        if (panelContenedor != null) panelContenedor.SetActive(false);
        Debug.Log("InfoPanelManager iniciado.");

        // Listeners para los botones de UI tradicional (Cerrar y Cambiar Opciones)
        if (botonCerrar != null) botonCerrar.onClick.AddListener(CerrarPanel);
        if (botonCambiarOpciones != null) botonCambiarOpciones.onClick.AddListener(VolverASeleccionIdioma);

        // --- Asignación de métodos a los botones (para ser llamados desde Leap Motion) ---
        // Aunque se llamen externamente, es buena práctica tener métodos dedicados:
        
        // Idioma (Panel 1)
        if (botonIdiomaEspanol != null) botonIdiomaEspanol.onClick.AddListener(OnPress_IdiomaEspanol);
        if (botonIdiomaIngles != null) botonIdiomaIngles.onClick.AddListener(OnPress_IdiomaIngles);

        // Nivel (Panel 2)
        if (botonNivelNino != null) botonNivelNino.onClick.AddListener(OnPress_NivelNino);
        if (botonNivelCasual != null) botonNivelCasual.onClick.AddListener(OnPress_NivelCasual);
        if (botonNivelExperto != null) botonNivelExperto.onClick.AddListener(OnPress_NivelExperto);

        // Audio (Panel 3)
        if (botonAudio != null) botonAudio.onClick.AddListener(ReproducirAudio);

        // Nivel Inglés (Panel 2 en Inglés)
        if (botonNivelNinoEN != null) botonNivelNinoEN.onClick.AddListener(OnPress_NivelNino);
        if (botonNivelCasualEN != null) botonNivelCasualEN.onClick.AddListener(OnPress_NivelCasual);
        if (botonNivelExpertoEN != null) botonNivelExpertoEN.onClick.AddListener(OnPress_NivelExperto);
    }

    // ==========================================================
    // MÉTODOS PÚBLICOS DE INTERACCIÓN (Llamados desde Leap Motion)
    // ==========================================================

    // --- Lógica del Panel 1: Idioma ---
    public void OnPress_IdiomaEspanol() => SeleccionarIdioma(0);
    public void OnPress_IdiomaIngles() => SeleccionarIdioma(1);

    // --- Lógica del Panel 2: Nivel ---
    public void OnPress_NivelNino() => SeleccionarNivel(0);
    public void OnPress_NivelCasual() => SeleccionarNivel(1);
    public void OnPress_NivelExperto() => SeleccionarNivel(2);

    // ==========================================================
    // GESTIÓN DE PANELES Y NAVEGACIÓN
    // ==========================================================

    public void AbrirPanel(InfoObraMuseo info)
    {
        infoActual = info;
        tituloText.text = info.nombreObjeto;

        // 1. Activa el contenedor principal
        panelContenedor.SetActive(true);
        
        // 2. Muestra solo el Panel de Idioma e inicia el flujo
        panelNivel.SetActive(false);
        panelNivelIngles.SetActive(false);
        panelInformacionFinal.SetActive(false);
        panelIdioma.SetActive(true);
    }

    public void CerrarPanel()
    {
        fuenteAudio.Stop();
        panelContenedor.SetActive(false);
    }

    public void VolverASeleccionIdioma()
    {
        // Vuelve al Panel 1 (limpia la pantalla de información)
        panelInformacionFinal.SetActive(false);
        panelNivel.SetActive(false);
        panelNivelIngles.SetActive(false);
        panelIdioma.SetActive(true);
        fuenteAudio.Stop();
    }
    
    // ==========================================================
    // LÓGICA DE SELECCIÓN Y CONTENIDO
    // ==========================================================

    public void SeleccionarIdioma(int idioma)
    {
        idiomaSeleccionado = idioma;
        ActualizarAparienciaBotonesIdioma();
        
        panelIdioma.SetActive(false);
        if (idiomaSeleccionado == 0) // Español
        {
            panelNivel.SetActive(true);
            panelNivelIngles.SetActive(false);
        }
        else // Inglés
        {
            panelNivel.SetActive(false);
            panelNivelIngles.SetActive(true);
        }
    }

    public void SeleccionarNivel(int nivel)
    {
        nivelSeleccionado = nivel;
        ActualizarAparienciaBotonesNivel();
        
        // 1. Carga y actualiza el contenido
        ActualizarContenido(); 

        // 2. TRANSICIÓN: Oculta nivel, Muestra Información Final
        panelNivel.SetActive(false);
        panelNivelIngles.SetActive(false);
        panelInformacionFinal.SetActive(true);
    }

    public void ActualizarContenido()
    {
        if (infoActual == null) return;
        fuenteAudio.Stop(); 

        int idioma = idiomaSeleccionado;
        int nivel = nivelSeleccionado;

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

    public void ReproducirAudio()
    {
        if (infoActual == null) return;
        if (fuenteAudio.isPlaying) 
        {
            fuenteAudio.Stop();
            return; 
        }
        
        int idioma = idiomaSeleccionado;
        int nivel = nivelSeleccionado;
        AudioClip clip = null;

        // ... Lógica para seleccionar el AudioClip según idioma y nivel
        if (idioma == 0)
        {
            if (nivel == 0) clip = infoActual.audioES_Niño;
            else if (nivel == 1) clip = infoActual.audioES_Casual;
            else clip = infoActual.audioES_Experto;
        }
        else
        {
            if (nivel == 0) clip = infoActual.audioEN_Niño;
            else if (nivel == 1) clip = infoActual.audioEN_Casual;
            else clip = infoActual.audioEN_Experto;
        }

        if (clip != null) { 
            fuenteAudio.clip = clip; 
            fuenteAudio.Play(); 
        } else {
            Debug.LogWarning("Audio clip no encontrado para la selección actual.");
        }
    }
    
    // ==========================================================
    // APARIENCIA DE BOTONES (CRUCIAL PARA GESTUAL)
    // ==========================================================
    
    private void ActualizarAparienciaBotonesIdioma()
    {
        // Ejemplo de retroalimentación visual (cambio a Amarillo/Blanco)
        botonIdiomaEspanol.image.color = (idiomaSeleccionado == 0) ? Color.yellow : Color.white;
        botonIdiomaIngles.image.color = (idiomaSeleccionado == 1) ? Color.yellow : Color.white;
    }

    private void ActualizarAparienciaBotonesNivel()
    {
        if (idiomaSeleccionado == 1)
        {
            botonNivelNinoEN.image.color = (nivelSeleccionado == 0) ? Color.yellow : Color.white;
            botonNivelCasualEN.image.color = (nivelSeleccionado == 1) ? Color.yellow : Color.white;
            botonNivelExpertoEN.image.color = (nivelSeleccionado == 2) ? Color.yellow : Color.white;
        }else
        {
            botonNivelNino.image.color = (nivelSeleccionado == 0) ? Color.yellow : Color.white;
            botonNivelCasual.image.color = (nivelSeleccionado == 1) ? Color.yellow : Color.white;
            botonNivelExperto.image.color = (nivelSeleccionado == 2) ? Color.yellow : Color.white;
        }
    }
}