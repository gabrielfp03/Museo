using UnityEngine;
using UnityEngine.SceneManagement; // Si planeas cargar escenas

public class MenuNavigator : MonoBehaviour
{
    // Asigna estos objetos vacíos (Checkpoints) desde el Inspector
    public Transform checkpointLobby;
    public Transform checkpointPlaneadores;
    public Transform checkpointGuerra;
    public Transform checkpointComercial;
    public Transform checkpointHelicopteros;
    public Transform checkpointSalida;

    // Asigna el objeto del Jugador que se va a mover
    public GameObject playerObject;

    // Función principal de teletransporte
    public void TeleportTo(Transform targetCheckpoint)
    {
        // 1. Desactivar colisiones/físicas temporalmente si es necesario.
        
        // 2. Teletransportar el objeto del jugador.
        playerObject.transform.position = targetCheckpoint.position;
        playerObject.transform.rotation = targetCheckpoint.rotation;

        // 3. Opcional: Cerrar el menú si todavía está activo
        // gameObject.SetActive(false); 
    }

    // Funciones públicas que se asignarán a los botones
    public void GoToLobby()
    {
        TeleportTo(checkpointLobby);
    }

    public void GoToPlaneadores()
    {
        TeleportTo(checkpointPlaneadores);
    }
    
    public void GoToGuerra()
    {
        TeleportTo(checkpointGuerra);
    }

    public void GoToComercial()
    {
        TeleportTo(checkpointComercial);
    }

    public void GoToHelicopteros()
    {
        TeleportTo(checkpointHelicopteros);
    }

    public void ExitToMainMenu()
    {
        TeleportTo(checkpointSalida);
        // Si deseas cargar una escena de menú principal, descomenta la línea siguiente y asegúrate de que el nombre de la escena sea correcto.
        // SceneManager.LoadScene("MainMenuSceneName");
    }   
}