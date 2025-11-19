using UnityEngine;

public class MovementButtonActivator : MonoBehaviour
{
    // 🛑 CRITICAL: Drag your "FirstPersonController Variant" object here in the Inspector
    public FirstPersonController firstPersonController; 

    // Velocities to apply when the button is pressed (1f = full speed forward)
    [Header("Movement Direction")]
    public float horizontalInput = 0f;
    public float verticalInput = 1f; // Setting to 1f means moving forward

    // -------------------------------------------------------------
    // Methods called by the RaycastButtonAction (On Button Press)
    // -------------------------------------------------------------

    // 1. Called when the button is pressed down or when the action is triggered
    public void StartMovement()
    {
        if (firstPersonController != null)
        {
            // Activate the FPC's movement state
            firstPersonController.SetMovementActive(true);
            
            // Set the direction (e.g., forward movement)
            firstPersonController.SetExternalInput(horizontalInput, verticalInput);
            
            Debug.Log("Movement started via Raycast Button.");
        }
    }

    // 2. Called when the button is released (Optional, if you want hold-to-move)
    public void StopMovement()
    {
        if (firstPersonController != null)
        {
            // Deactivate the FPC's movement state
            firstPersonController.SetMovementActive(false);
            
            // Stop applying force
            firstPersonController.SetExternalInput(0f, 0f);
            
            Debug.Log("Movement stopped via Raycast Button.");
        }
    }
}