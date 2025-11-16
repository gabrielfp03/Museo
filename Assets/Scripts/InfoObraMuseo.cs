using UnityEngine;

public class InfoObraMuseo : MonoBehaviour
{
    // Estas son las variables que deben aparecer en el Inspector del AVIÓN
    [Header("Datos del Objeto")]
    public string nombreObjeto;

    [Header("Contenido en Español")]
    [TextArea(3, 5)] public string descripcion_es_niño;
    [TextArea(3, 5)] public string descripcion_es_casual;
    [TextArea(3, 5)] public string descripcion_es_experto;

    [Header("Contenido en Inglés")]
    [TextArea(3, 5)] public string descripcion_en_niño;
    [TextArea(3, 5)] public string descripcion_en_casual;
    [TextArea(3, 5)] public string descripcion_en_experto;
}
