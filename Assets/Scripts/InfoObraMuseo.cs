using UnityEngine;

public class InfoObraMuseo : MonoBehaviour
{
    [Header("Datos Generales")]
    public string nombreObjeto;

    // --- ESPAÑOL ---
    [Header("🇪🇸 Español")]
    [TextArea(3, 5)] public string textoES_Niño;
    public AudioClip audioES_Niño;

    [TextArea(3, 5)] public string textoES_Casual;
    public AudioClip audioES_Casual;

    [TextArea(3, 5)] public string textoES_Experto;
    public AudioClip audioES_Experto;

    // --- INGLÉS ---
    [Header("🇬🇧 English")]
    [TextArea(3, 5)] public string textoEN_Niño;
    public AudioClip audioEN_Niño;

    [TextArea(3, 5)] public string textoEN_Casual;
    public AudioClip audioEN_Casual;

    [TextArea(3, 5)] public string textoEN_Experto;
    public AudioClip audioEN_Experto;
}