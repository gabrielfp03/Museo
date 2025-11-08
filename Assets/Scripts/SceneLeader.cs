using UnityEngine;
using UnityEngine.SceneManagement; // <-- ¡Importante!

public class SceneLoader : MonoBehaviour
{
    // Función genérica para cargar cualquier escena por su nombre
    public void LoadMuseumScene(string sceneName)
    {
        // SceneManager es la clase que gestiona la carga de escenas
        SceneManager.LoadScene(sceneName);
    }

    // Funciones específicas para asignar a los botones
    public void GoToLobby()
    {
        // El nombre debe coincidir EXACTAMENTE con el nombre de tu archivo .unity
        LoadMuseumScene("Museo"); 
    }

    /*
    public void GoToPlaneadores()
    {
        LoadMuseumScene("Sala_Planeadores");
    }
    
    public void GoToAvionesGuerra()
    {
        LoadMuseumScene("Sala_AvionesGuerra");
    }

    // Y así para el resto de tus salas...*/
    
    public void ExitGame()
    {
        Application.Quit(); // Solo funciona en builds, no en el editor
    }
}