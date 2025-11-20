using UnityEngine;
using UnityEngine.UI; // Necesario para Image y Button
using System.Collections.Generic; // <--- ADD THIS LINE

public class CatalogManager : MonoBehaviour
{
    // --- Configuración Pública en el Inspector ---
    
    [Header("Datos del Catálogo")]
    public GameObject[] allAvions; // Array donde cargarás los 16 aviones
    public GameObject[] allInfoPanels; // Array donde cargarás los 16 InfoPanels

    [Header("Referencias de UI")]
    //public Image displayImage;   // La imagen central del avión
    // public Text nameText;      // Ejemplo: si tienes un texto para el nombre
    public GameObject infoPanelPrefab; // El panel de información que se desplegará

    public GameObject modelPrefab;   // La imagen central del avión
    
    // A serialized List to hold various disabled UI elements (panels, pop-ups, etc.)
    [SerializeField] 
    private List<GameObject> allUIElements = new List<GameObject>();

    // --- Variables de Estado ---
    private int currentAvionIndex = 0;
    
    // --- Métodos de Control ---

    void Start()
    {
        // Asegúrate de que los datos existan antes de intentar mostrar algo
        if (allAvions.Length > 0)
        {
            UpdateCatalogView(currentAvionIndex);
        }
    }

    /// <summary>
    /// Muestra la información del avión en el índice actual.
    /// </summary>
    private void UpdateCatalogView(int index)
    {
        // Garantiza que el índice se mantenga dentro de los límites del array
        if (index < 0)
        {
            currentAvionIndex = allAvions.Length - 1; // Volver al final
        }
        else if (index >= allAvions.Length && index >= allInfoPanels.Length)
        {
            currentAvionIndex = 0; // Volver al inicio
        }
        
        GameObject currentAvion = allAvions[currentAvionIndex];
        GameObject currentInfoPannel = allInfoPanels[currentAvionIndex];

        // 1. Actualiza el modelo
        if (modelPrefab != null && currentAvion != null)
        {
            modelPrefab.SetActive(false);
            HideInfoPanel();

            modelPrefab = currentAvion;
            infoPanelPrefab = currentInfoPannel;

            modelPrefab.SetActive(true);

        }

        // 2. Aquí puedes actualizar otros textos si los tienes visibles
        // if (nameText != null) nameText.text = currentData.modelName; 
    }

    // --- Métodos llamados por las Flechas ---
    
    /// <summary>
    /// Llama el botón de la flecha derecha.
    /// </summary>
    public void NextAvion()
    {
        currentAvionIndex++;
        UpdateCatalogView(currentAvionIndex);
        Debug.Log("Next Avion: " + modelPrefab.name);
    }

    /// <summary>
    /// Llama el botón de la flecha izquierda.
    /// </summary>
    public void PreviousAvion()
    {
        currentAvionIndex--;
        UpdateCatalogView(currentAvionIndex);
        Debug.Log("Previous Avion: " + modelPrefab.name);
    }

    // --- Método llamado por el botón 'Información' ---

    public void ShowInfoPanel()
    {
        if (infoPanelPrefab != null)
        {
            infoPanelPrefab.SetActive(true);
        }
    }
    public void HideInfoPanel()
    {
        if (infoPanelPrefab != null)
        {
            infoPanelPrefab.SetActive(false);
        }
    }
}