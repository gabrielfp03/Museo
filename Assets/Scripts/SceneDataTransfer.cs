using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneDataTransfer : MonoBehaviour
{
    // 1. Singleton: Permite acceder a esta instancia desde cualquier script.
    public static SceneDataTransfer Instance;

    // 2. Dato a transferir: Almacena el nombre del checkpoint al que ir.
    public string targetCheckpointName = "Checkpoint_Lobby"; // Valor por defecto

    private void Awake()
    {
        // Si ya existe otra instancia, destrúyete (evita duplicados).
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Esta es la única instancia válida.
        Instance = this;


        // ¡Crucial! Evita que este objeto se elimine al cargar una nueva escena.
        // DontDestroyOnLoad(this.gameObject);
    }

    // Función para el menú: establece el destino y carga la escena
    private void SetDestinationAndLoad(string destinationName, string museumSceneName)
    {
        targetCheckpointName = destinationName;
        SceneManager.LoadScene(museumSceneName);
    }

    public void GoToLobby() // SIN PARÁMETROS
    {
        SetDestinationAndLoad("Checkpoint_Lobby", "Museo");
    }

    public void GoToPlaneadores() // SIN PARÁMETROS
    {
        SetDestinationAndLoad("Checkpoint_Planeadores", "Museo");
    }

    public void GoToAvionesGuerra() // SIN PARÁMETROS
    {
        SetDestinationAndLoad("Checkpoint_Guerra", "Museo");
    }

    public void GoToComercial() // SIN PARÁMETROS
    {
        SetDestinationAndLoad("Checkpoint_Comerciales", "Museo");
    }

    public void GoToHelicopteros() // SIN PARÁMETROS
    {
        SetDestinationAndLoad("Checkpoint_Helicopteros", "Museo");
    }

    public void GoToSalida() // SIN PARÁMETROS
    {
        SetDestinationAndLoad("Checkpoint_Salida", "Museo");
    }
}