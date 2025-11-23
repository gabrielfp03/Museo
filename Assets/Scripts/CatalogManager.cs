using UnityEngine;
using UnityEngine.UI; // Necesario para Image y Button
using System.Collections.Generic; // <--- ADD THIS LINE
using TMPro;

public class CatalogManager : MonoBehaviour
{
    // --- Configuración Pública en el Inspector ---
    
    [Header("Datos del Catálogo")]
    public GameObject[] allAvions; // Array donde cargarás los 16 aviones
    public GameObject infoPanelTemplate; // Template de InfoPanel[Header("Referencias de Componentes de Texto")]
    // Cada variable está ligada a un componente TMPro en el Inspector
    public TextMeshProUGUI modelNameText;
    public TextMeshProUGUI constructionDateText;
    public TextMeshProUGUI roomLocationText;
    public TextMeshProUGUI briefDescriptionText;

    [Header("RaycastButtonAction de los botones a desactivar cuando se abra la info")]
    public RaycastButtonAction[] buttonsToDisable; // The array holds references to the actual RaycastButtonAction components


    // --- Variables de Estado ---
    private int currentAvionIndex = 0;
    private GameObject currentAvion;   // El avion actual
    private AvionData currentAvionData; // Información del avión actual
    
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
        else if (index >= allAvions.Length)
        {
            currentAvionIndex = 0; // Volver al inicio
        }

        HideModel();
        HideInfoPanel();
        
        currentAvion = allAvions[currentAvionIndex];
        currentAvionData = currentAvion.GetComponent<AvionData>();

        UpdateInfoPanel();
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

    private void UpdateInfoPanel()
    {
        if (infoPanelTemplate != null)
        {
            if (currentAvionData != null)
            {
                modelNameText.text = currentAvionData.modelName;
                constructionDateText.text = currentAvionData.constructionDate;
                roomLocationText.text = currentAvionData.roomLocation;
                briefDescriptionText.text = currentAvionData.briefDescription;
            } else
            {
                modelNameText.text = "N/A";
                constructionDateText.text = "Fecha Desconocida";
                roomLocationText.text = "Sala Desconocida";
                briefDescriptionText.text = "No hay descripción disponible...";
            }
        }
    }

    private bool ShowInfoPanel()
    {
        if (infoPanelTemplate != null)
        {
            infoPanelTemplate.SetActive(true);
            return true;
        }

        return false;
    }
    private bool HideInfoPanel()
    {
        if (infoPanelTemplate != null)
        {
            infoPanelTemplate.SetActive(false);
            return true;
        }

        return false;
    }

    public void ShowInfoHideModel()
    {
        if (ShowInfoPanel())
        {
            HideModel();
            DisableButtons();
        }
    }

    public void HideInfoShowModel()
    {
        if (HideInfoPanel())
        {
            ShowModel();
            EnableButtons();
        }
    }

    public void DisableButtons()
    {
        // Loop through every RaycastButtonAction component in the array
        foreach (RaycastButtonAction script in buttonsToDisable)
        {
            if (script != null)
            {
                // Disable the component directly
                script.ResetButtonState();
                script.enabled = false;
            }
        }
        Debug.Log($"Successfully disabled all RaycastButtonAction components in the array.");
    }

    public void EnableButtons()
    {
        // Loop through every RaycastButtonAction component in the array
        foreach (RaycastButtonAction script in buttonsToDisable)
        {
            if (script != null)
            {
                // Enable the component directly
                script.enabled = true;
            }
        }
        Debug.Log($"Successfully enabled all RaycastButtonAction components in the array.");
    }
}