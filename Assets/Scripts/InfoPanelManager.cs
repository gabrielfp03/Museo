using UnityEngine;
using TMPro;       // Necesario para TextMeshPro
using UnityEngine.UI; // Necesario para los componentes de UI estándar

public class InfoPanelManager : MonoBehaviour
{
    [Header("Referencias Visuales (Arrastra desde el Canvas)")]
    public GameObject panelContenedor;
    public TextMeshProUGUI tituloText;
    public TextMeshProUGUI descripcionText;
    public Button botonCerrar;
    
    [Header("Controles de Interacción (Arrastra desde el Canvas)")]
    public TMP_Dropdown dropdownIdioma; 
    public TMP_Dropdown dropdownNivel;  
    public Button botonAudio;

    [Header("Audio (Arrastra el propio GAME_MANAGER aquí)")]
    public AudioSource fuenteAudio; 

    // Variable privada para recordar qué avión estamos mirando
    private InfoObraMuseo infoActual; 

    void Start()
    {
        // Aseguramos que el panel empiece cerrado
        if(panelContenedor != null) 
            panelContenedor.SetActive(false);
        
        // Conectamos los eventos de los botones y dropdowns
        if(botonCerrar != null)
            botonCerrar.onClick.AddListener(CerrarPanel);
        
        if(dropdownIdioma != null)
            dropdownIdioma.onValueChanged.AddListener(delegate { ActualizarContenido(); });
            
        if(dropdownNivel != null)
            dropdownNivel.onValueChanged.AddListener(delegate { ActualizarContenido(); });
        
        if(botonAudio != null)
            botonAudio.onClick.AddListener(ReproducirAudio);
    }

    // Esta función la llama el Símbolo "i" al hacer clic
    public void AbrirPanel(InfoObraMuseo info)
    {
        infoActual = info;
        if(tituloText != null) 
            tituloText.text = info.nombreObjeto;
            
        if(panelContenedor != null) 
            panelContenedor.SetActive(true);
        
        // Refresca el texto inmediatamente
        ActualizarContenido();
    }

    void ActualizarContenido()
    {
        if (infoActual == null) return;

        // Paramos el audio si cambian las opciones para no mezclar sonidos
        if(fuenteAudio != null) fuenteAudio.Stop();

        int idioma = dropdownIdioma.value; // 0=Español, 1=Inglés
        int nivel = dropdownNivel.value;   // 0=Niño, 1=Casual, 2=Experto

        // Lógica de selección de texto
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
        if (infoActual == null || fuenteAudio == null) return;
        
        int idioma = dropdownIdioma.value;
        int nivel = dropdownNivel.value;
        AudioClip clipA_Reproducir = null;

        // Lógica de selección de audio (idéntica a la del texto)
        if (idioma == 0) // Español
        {
            if (nivel == 0) clipA_Reproducir = infoActual.audioES_Niño;
            else if (nivel == 1) clipA_Reproducir = infoActual.audioES_Casual;
            else clipA_Reproducir = infoActual.audioES_Experto;
        }
        else // Inglés
        {
            if (nivel == 0) clipA_Reproducir = infoActual.audioEN_Niño;
            else if (nivel == 1) clipA_Reproducir = infoActual.audioEN_Casual;
            else clipA_Reproducir = infoActual.audioEN_Experto;
        }

        // Solo reproducimos si existe un archivo de audio asignado
        if (clipA_Reproducir != null)
        {
            fuenteAudio.clip = clipA_Reproducir;
            fuenteAudio.Play();
        }
        else
        {
            Debug.Log("No hay audio asignado para esta selección.");
        }
    }

    public void CerrarPanel()
    {
        if(fuenteAudio != null) fuenteAudio.Stop(); 
        if(panelContenedor != null) panelContenedor.SetActive(false);
    }
}