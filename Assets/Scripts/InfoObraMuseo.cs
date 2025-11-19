using UnityEngine;

public class InfoObraMuseo : MonoBehaviour
{
    // Estas son las variables que deben aparecer en el Inspector del AVIÓN
    [Header("Datos del Objeto")]
    public string nombreObjeto;

    [Header("Contenido en Español")]
    [TextArea(3, 5)] public string textoES_Niño;
    public AudioClip audioES_Niño;
    [TextArea(3, 5)] public string textoES_Casual;
    public AudioClip audioES_Casual;
    [TextArea(3, 5)] public string textoES_Experto;
    public AudioClip audioES_Experto;

    [Header("Contenido en Inglés")]
    [TextArea(3, 5)] public string textoEN_Niño;
    public AudioClip audioEN_Niño;
    [TextArea(3, 5)] public string textoEN_Casual;
    public AudioClip audioEN_Casual;
    [TextArea(3, 5)] public string textoEN_Experto;
    public AudioClip audioEN_Experto;
}
