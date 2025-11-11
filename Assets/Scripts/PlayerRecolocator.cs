using UnityEngine;
using System.Collections;

public class PlayerRelocator : MonoBehaviour
{
    
    void Start()
    {
        // 1. Obtiene el nombre del destino almacenado por el menú
        string targetName = SceneDataTransfer.Instance.targetCheckpointName;

        // 2. Busca el objeto (Checkpoint) con ese nombre en la escena
        GameObject targetCheckpoint = GameObject.Find(targetName);

        if (targetCheckpoint != null)
        {
            // 3. ¡Teletransportar!
            this.transform.position = targetCheckpoint.transform.position;
            this.transform.rotation = targetCheckpoint.transform.rotation;
        }
        else
        {
            // Opcional: Si el checkpoint no se encuentra, avisa.
            Debug.LogError("ERROR: Checkpoint no encontrado: " + targetName + 
                           ". El jugador se queda en la posición inicial de la escena.");
        }
        
        // Opcional: Elimina el Singleton si ya no lo vas a usar hasta el menú
        // Si regresas al menú, probablemente quieras mantenerlo.
    }
}