using UnityEngine;

/// <summary>
/// Gestiona el estado actual del recorrido del museo y las transiciones entre salas.
/// Centraliza la lógica de navegación (principalmente desde el menú de mapa).
/// </summary>
public class RoomStateManager : MonoBehaviour
{
    // === 1. DEFINICIÓN DEL ESTADO ===

    // Enum que define todas las posibles áreas del museo, incluyendo la Salida.
    // Esto asegura que la navegación sea clara y limitada a estas áreas.
    public enum MuseumState
    {
        Lobby,
        Sala_Helicopteros,
        Sala_Comerciales,
        Sala_Planeadores,
        Sala_Guerra,
        Salida
    }

    [Header("Configuración del Estado")]
    // Variable pública que rastrea la sala en la que se encuentra el jugador.
    // [SerializeField] permite ver esta variable en el Inspector de Unity, aunque sea privada.
    [SerializeField]
    private MuseumState currentState = MuseumState.Lobby;

    // Propiedad pública de solo lectura para acceder al estado desde otros scripts (ej. UI)
    public MuseumState CurrentState => currentState;


    // === 2. REFERENCIAS A COMPONENTES ESENCIALES ===

    [Header("Referencias de Escena")]
    // Referencia al Player Rig (el GameObject que representa al jugador/cámara)
    public Transform playerRig; 

    // Referencia al Canvas del Menú de Mapa (para mostrar/ocultar)
    public GameObject mapMenuCanvas;

    // Arreglo de Transforms que definen el punto de inicio (Spawn Point) para cada sala.
    // Se debe asignar manualmente en el Inspector (Lobby[0], Sala1[1], etc.)
    public Transform[] spawnPoints; 


    // === 3. MÉTODOS DE CONTROL DEL FLUJO ===

    /// <summary>
    /// Método llamado por los botones del menú de mapa para cambiar la sala actual.
    /// También se usa para el movimiento por tokens al avanzar el recorrido.
    /// </summary>
    /// <param name="targetState">La sala a la que se desea ir.</param>
    public void NavigateToRoom(MuseumState targetState)
    {
        if (targetState == currentState)
        {
            Debug.Log($"Ya estás en {targetState}.");
            HideMapMenu();
            return;
        }

        currentState = targetState;
        Debug.Log($"[RoomManager] Transición a: {currentState}");

        // 1. Mover al jugador al punto de inicio de la nueva sala.
        MovePlayerToSpawnPoint(targetState);

        // 2. Ocultar el menú de mapa si estaba abierto.
        HideMapMenu();

        // 3. (Futuro) Aquí se activarían los scripts de guía visual.
        // Ejemplo: GuideLineRenderer.Instance.SetTargetRoom(targetState);
    }

    /// <summary>
    /// Método llamado por el script TokenProximityActivator.cs al llegar al token de fin de sala.
    /// Usa el estado actual para determinar a dónde ir después.
    /// </summary>
    public void AdvanceToNextRoom()
    {
        // Simple switch case para la navegación secuencial (Lobby -> Sala 1 -> Sala 2, etc.)
        MuseumState nextState = currentState switch
        {
            MuseumState.Lobby => MuseumState.Sala_Helicopteros,
            MuseumState.Sala_Helicopteros => MuseumState.Sala_Comerciales,
            MuseumState.Sala_Comerciales => MuseumState.Sala_Planeadores,
            MuseumState.Sala_Planeadores => MuseumState.Sala_Guerra,
            MuseumState.Sala_Guerra => MuseumState.Salida,
            _ => MuseumState.Lobby, // Si ya está en Salida, vuelve al Lobby.
        };

        NavigateToRoom(nextState);
    }


    // === 4. MÉTODOS DE UTILIDAD ===

    /// <summary>
    /// Mueve el Player Rig a la posición de inicio predefinida para la sala de destino.
    /// </summary>
    private void MovePlayerToSpawnPoint(MuseumState targetState)
    {
        int index = (int)targetState; // El enum se usa como índice (Lobby=0, Sala1=1, etc.)

        if (playerRig != null && spawnPoints.Length > index && spawnPoints[index] != null)
        {
            // Mueve la posición del jugador al punto de spawn.
            playerRig.position = spawnPoints[index].position;
            // Opcional: Rotar al jugador para que mire en la dirección correcta
            playerRig.rotation = spawnPoints[index].rotation; 
            Debug.Log($"Jugador movido al spawn de: {targetState}");
        }
        else
        {
            Debug.LogError($"Error: PlayerRig o SpawnPoint no asignado/encontrado para el estado: {targetState}");
        }
    }

    /// <summary>
    /// Oculta el menú del mapa.
    /// </summary>
    public void HideMapMenu()
    {
        if (mapMenuCanvas != null && mapMenuCanvas.activeSelf)
        {
            mapMenuCanvas.SetActive(false);
            Debug.Log("Menú de Mapa Ocultado.");
        }
    }

    /// <summary>
    /// Muestra el menú del mapa.
    /// </summary>
    public void ShowMapMenu()
    {
        if (mapMenuCanvas != null && !mapMenuCanvas.activeSelf)
        {
            mapMenuCanvas.SetActive(true);
            Debug.Log("Menú de Mapa Mostrado.");
        }
    }
}