using UnityEngine;

public class InfoSymbolTrigger : MonoBehaviour
{
    public InfoObraMuseo infoDeEsteAvion; 
    private InfoPanelManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<InfoPanelManager>();
    }

    private void OnMouseDown()
    {
        if (manager != null && infoDeEsteAvion != null)
        {
            manager.AbrirPanel(infoDeEsteAvion);
        }
    }
}