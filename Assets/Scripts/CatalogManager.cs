using UnityEngine;
using UnityEngine.UI; // Necesario para Image y Button
using System.Collections.Generic; // <--- ADD THIS LINE

public class CatalogManager : MonoBehaviour
{
    // --- Configuración Pública en el Inspector ---
    
    [Header("Datos del Catálogo")]
    public GameObject[] allAvions; // Array donde cargarás los 16 aviones
    public GameObject[] allInfoPanels; // Array donde cargarás los 16 InfoPanels


    // --- Variables de Estado ---
    private int currentAvionIndex = 0;
    private GameObject currentInfoPanel; // El panel de información que se desplegará
    private GameObject currentAvion;   // La imagen central del avión
    
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

        HideModel();
        HideInfoPanel();
        
        currentAvion = allAvions[currentAvionIndex];
        currentInfoPanel = allInfoPanels[currentAvionIndex];

        ShowModel();
    }

    // --- Métodos llamados por las Flechas ---
    
    /// <summary>
    /// Llama el botón de la flecha derecha.
    /// </summary>
    public void NextAvion()
    {
        currentAvionIndex++;
        UpdateCatalogView(currentAvionIndex);
        Debug.Log("Next Avion: " + currentAvion.name);
    }

    /// <summary>
    /// Llama el botón de la flecha izquierda.
    /// </summary>
    public void PreviousAvion()
    {
        currentAvionIndex--;
        UpdateCatalogView(currentAvionIndex);
        Debug.Log("Previous Avion: " + currentAvion.name);
    }




    private void ShowModel()
    {
        if (currentAvion != null)
        {
            currentAvion.SetActive(true);
        }
    }
    private void HideModel()
    {
        if (currentAvion != null)
        {
            currentAvion.SetActive(false);
        }
    }

    // --- Método llamado por el botón 'Información' ---

    public void ShowInfoPanel()
    {
        if (currentInfoPanel != null)
        {
            currentInfoPanel.SetActive(true);
        }
    }
    public void HideInfoPanel()
    {
        if (currentInfoPanel != null)
        {
            currentInfoPanel.SetActive(false);
        }
    }
}