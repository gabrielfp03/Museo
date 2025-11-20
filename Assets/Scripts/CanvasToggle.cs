// using UnityEngine;

// public class CanvasToggle : MonoBehaviour
// {
//     // Referencia al componente Canvas que queremos controlar
//     private Canvas canvasComponent;

//     void Start()
//     {
//         // 1. Obtener el componente Canvas al inicio
//         canvasComponent = GetComponent<Canvas>();

//         if (canvasComponent == null)
//         {
//             Debug.LogError("CanvasToggle requiere un componente Canvas en el mismo GameObject.");
//             enabled = false;
//         }
        
//         // Opcional: Asegurarse de que el Canvas empieza invisible
//         // Si quieres que el menú esté oculto al iniciar el juego, descomenta la siguiente línea:
//         canvasComponent.enabled = false;
//     }

//     // Se conecta al botón "Mostrar Menú" (Boton A)
//     public void ShowCanvas()
//     {
//         if (canvasComponent != null)
//         {
//             canvasComponent.enabled = true;
//             Debug.Log("Canvas de Movimiento: Visible");
//         }
//     }

//     // Se conecta al botón "Ocultar Menú" (Boton B)
//     public void HideCanvas()
//     {
//         if (canvasComponent != null)
//         {
//             canvasComponent.enabled = false;
//             Debug.Log("Canvas de Movimiento: Oculto");
//         }
//     }
// }
using UnityEngine;

public class CanvasToggle : MonoBehaviour
{
    // Arrastra el menú de movimiento completo a este slot (FloatingUI_Navigation)
    public GameObject menuCanvas; 

    // Arrastra el botón de apertura a este slot (BTN_Abrir)
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