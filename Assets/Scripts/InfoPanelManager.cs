using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoPanelManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelContenedor;
    public TextMeshProUGUI tituloText;
    public TextMeshProUGUI descripcionText;
    public Button botonCerrar;
    
    [Header("Controles")]
    public TMP_Dropdown dropdownIdioma; // Opción 0=Español, 1=Inglés
    public TMP_Dropdown dropdownNivel;  // Opción 0=Niño, 1=Casual, 2=Experto
    public Button botonAudio;
    public AudioSource fuenteAudio; 

    private InfoObraMuseo infoActual; 
    private CanvasGroup canvasGroup;

    void Start()
    {
        if(panelContenedor != null) panelContenedor.SetActive(false);
        
        // Conexiones automáticas de botones
        botonCerrar.onClick.AddListener(CerrarPanel);
        dropdownIdioma.onValueChanged.AddListener(delegate { ActualizarContenido(); });
        dropdownNivel.onValueChanged.AddListener(delegate { ActualizarContenido(); });
        botonAudio.onClick.AddListener(ReproducirAudio);

        canvasGroup = panelContenedor.GetComponent<CanvasGroup>();
        if(canvasGroup == null) canvasGroup = panelContenedor.AddComponent<CanvasGroup>();
    }

    public void AbrirPanel(InfoObraMuseo info)
    {
        infoActual = info;
        tituloText.text = info.nombreObjeto;
        panelContenedor.SetActive(true);
        ActualizarContenido(); // Refrescar texto al abrir
        
        panelContenedor.SetActive(true);
        canvasGroup.alpha = 0; // Empieza invisible
        StopAllCoroutines();
        StartCoroutine(AnimarEntrada());
    }

    void ActualizarContenido()
    {
        if (infoActual == null) return;
        fuenteAudio.Stop(); // Parar audio anterior

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

    public void CerrarPanel()
    {
        fuenteAudio.Stop();
        panelContenedor.SetActive(false);
    }

    // Añade esta Corrutina al final del script
    System.Collections.IEnumerator AnimarEntrada() {
        float tiempo = 0;
        while(tiempo < 0.3f) { // Duración 0.3 segundos
            tiempo += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, tiempo / 0.3f);
            // Pequeño efecto de escala (pop-up)
            panelContenedor.transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, tiempo / 0.3f);
            yield return null;
        }
        canvasGroup.alpha = 1;
        panelContenedor.transform.localScale = Vector3.one;
    }
}