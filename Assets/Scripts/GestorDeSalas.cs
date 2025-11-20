using UnityEngine;

public class GestorDeSalas : MonoBehaviour
{
    // Aquí arrastraremos tus 4 paneles de información
    public GameObject[] panelesInfo; 

    // Esta función la llamarán tus botones (1, 2, 3, 4)
    public void AbrirInformacion(int numeroDeSala)
    {
        // Recorremos la lista de paneles
        for (int i = 0; i < panelesInfo.Length; i++)
        {
            // Si el índice coincide con la sala que queremos ver...
            if (i == numeroDeSala)
            {
                panelesInfo[i].SetActive(true); // ...lo encendemos
            }
            else
            {
                panelesInfo[i].SetActive(false); // ...y apagamos los demás para que no se solapen
            }
        }
    }

    // Función extra para un botón de "Cerrar" (si quieres poner una X en los paneles)
    public void CerrarTodo()
    {
        foreach(GameObject panel in panelesInfo)
        {
            panel.SetActive(false);
        }
    }
}