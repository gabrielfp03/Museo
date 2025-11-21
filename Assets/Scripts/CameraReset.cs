using UnityEngine;

public class CameraReset : MonoBehaviour
{
    public Vector3 targetLocalRotation = Vector3.zero;
    // public Vector3 targetPosition = Vector3.zero;
    public void CenterCameraToFixedPoint() // Cambiamos el nombre para mayor claridad
    {
        // 1. Mover el objeto a la posición global fija
        // transform.position = targetPosition; 
        
        // 2. Resetear la rotación local (si el script está en el objeto cámara)
        transform.localRotation = Quaternion.Euler(targetLocalRotation);
        
        // Debug.Log($"Cámara movida a {targetPosition} y centrada.");
    }
}