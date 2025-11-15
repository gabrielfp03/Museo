using UnityEngine;

public class InfoToken : MonoBehaviour
{
    // Esta es la variable que debe aparecer en el Inspector del TOKEN
    [Header("Conexión")]
    public InfoObraMuseo infoParaMostrar;

    private InfoPanelManager panelManager;

    void Start()
    {
        // Encuentra el manager en la escena
        panelManager = FindObjectOfType<InfoPanelManager>();
        
        if (panelManager == null)
        {
            Debug.LogError("¡No se encuentra el InfoPanelManager en la escena!");
        }
    }

    // Se llama cuando haces clic en el Collider de este objeto
    private void OnMouseDown()
    {
        if (infoParaMostrar != null && panelManager != null)
        {
            panelManager.MostrarInformacion(infoParaMostrar);
        }
        else
        {
            Debug.LogWarning("Token sin información asignada o sin panel manager.");
        }
    }
}
