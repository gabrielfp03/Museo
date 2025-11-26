
using UnityEngine;

public class CanvasToggle : MonoBehaviour
{
    public GameObject menuCanvas; 

    public GameObject openButton; 

    void Start()
    {
        // Asegúrate de que el menú de movimiento esté oculto y el botón de abrir esté visible al inicio
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }
        if (openButton != null)
        {
            openButton.SetActive(true);
        }
    }

    public void OpenMenu()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(true); // Muestra el menú
        }
        if (openButton != null)
        {
            openButton.SetActive(false); // Oculta el botón de apertura
        }
    }

    // Oculta el menú y muestra el botón de apertura
    public void CloseMenu()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false); // Oculta el menú
        }
        if (openButton != null)
        {
            openButton.SetActive(true); // Muestra el botón de apertura
        }
    }
}