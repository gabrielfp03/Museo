using UnityEngine;
using TMPro; // Necesario para TextMeshPro
using UnityEngine.UI; // Necesario para el Botónk

public class InfoPanelManager : MonoBehaviour
{
    // 1. Referencias a la UI
    [Header("Conexiones de la UI")]
    public GameObject panelContenedor; // Arrastra tu "PanelContenedor" aquí
    public TextMeshProUGUI tituloText;   // Arrastra tu "Texto_Titulo"
    public TextMeshProUGUI descripcionText; // Arrastra tu "Texto_Descripcion"
    public Button botonCerrar; // Arrastra tu "Boton_Cerrar"

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Oculta el panel al empezar
        if (panelContenedor != null)
        {
            panelContenedor.SetActive(false);
        }
        
        // Configura el botón de cerrar
        if (botonCerrar != null)
        {
            botonCerrar.onClick.AddListener(OcultarPanel);
        }
    }

    // Esta función pública será llamada por el token
    public void MostrarInformacion(InfoObraMuseo info)
    {
        if (info == null) return;

        tituloText.text = info.nombreObjeto;
        
        // --- AQUÍ DECIDES QUÉ NIVEL MOSTRAR ---
        // De momento, mostramos el casual como ejemplo
        descripcionText.text = info.descripcion_es_casual;

        panelContenedor.SetActive(true);
    }

    // Esta función es para el botón de cerrar
    public void OcultarPanel()
    {
        panelContenedor.SetActive(false);
    }
}
