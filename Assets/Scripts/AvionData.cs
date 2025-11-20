using UnityEngine;

// [System.Serializable] permite que esta clase sea editable en el Inspector de Unity
[System.Serializable]
public class AvionData : MonoBehaviour
{
    public string modelName = "Nombre del Avión";
    public string constructionDate = "Fecha Desconocida";
    public string roomLocation = "Sala Desconocida";
    public string briefDescription = "Breve historia...";
    //public Sprite displayImage; // Imagen para mostrar en el catálogo
}