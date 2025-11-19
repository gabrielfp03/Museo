using UnityEngine;

public class InfoSymbolTrigger : MonoBehaviour
{
    public InfoObraMuseo infoDeEsteAvion; // Arrastra aquí el avión correspondiente
    private InfoPanelManager manager;

    void Start()
    {
        manager = FindObjectOfType<InfoPanelManager>();
    }

    private void OnMouseDown()
    {
        // 1. ¿Detecta el clic?
        Debug.Log("¡He hecho clic en el objeto!"); 

        if (manager != null && infoDeEsteAvion != null)
        {
            // 2. ¿Intenta abrir el panel?
            Debug.Log("Intentando abrir panel con: " + infoDeEsteAvion.nombreObjeto);
            manager.AbrirPanel(infoDeEsteAvion);
        }
        else
        {
            // 3. ¿Falta alguna conexión?
            Debug.Log("ERROR: Manager es " + manager + " | InfoAvion es " + infoDeEsteAvion);
        }
        if (manager != null && infoDeEsteAvion != null)
        {
            manager.AbrirPanel(infoDeEsteAvion);
        }
    }
    
    // Opcional: Efecto al pasar el ratón por encima
    private void OnMouseEnter()
    {
        transform.localScale = transform.localScale * 1.2f; // Se hace grande
    }
    private void OnMouseExit()
    {
        transform.localScale = transform.localScale / 1.2f; // Vuelve a tamaño normal
    }
}
