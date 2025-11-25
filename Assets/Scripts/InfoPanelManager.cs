using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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
    private InfoObraMuseo infoActual; 

    void Start()
    {
        if(panelContenedor != null) panelContenedor.SetActive(false);
        Debug.Log("InfoPanelManager iniciado.");
        // Listeners
        if(botonCerrar != null) botonCerrar.onClick.AddListener(CerrarPanel);
        if(dropdownIdioma != null) dropdownIdioma.onValueChanged.AddListener(delegate { ActualizarContenido(); });
        if(dropdownNivel != null) dropdownNivel.onValueChanged.AddListener(delegate { ActualizarContenido(); });

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

        panelContenedor.SetActive(true);
        ActualizarContenido();
    }

    public void CerrarPanel()
    {
        fuenteAudio.Stop();
        panelContenedor.SetActive(false);
    }

    public void ActualizarContenido()
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

    public void ReproducirAudio()
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