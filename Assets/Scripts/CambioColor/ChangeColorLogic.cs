using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorLogic : MonoBehaviour
{
    void Update()
    {
        // Detectar clic izquierdo del ratón (0)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
<<<<<<< HEAD
                // Buscamos si el objeto tiene el script NUEVO
                ChangeColorObject objeto = hit.collider.GetComponent<ChangeColorObject>();
=======
                // Comprobamos si el objeto golpeado tiene el script 'ChangeColorObject'
                if (hit.collider.GetComponent<ChangeColorObject>())
                {
                    // Guardamos la referencia del objeto golpeado
                    changeColor newChangeColorObject = hit.collider.GetComponent<changeColor>();
>>>>>>> 4c6cc2e029a61d57d9cce798d13880dd96744bc1

                if (objeto != null)
                {
                    // Llamamos a la función nueva que controla la lista
                    objeto.SiguienteColor(); 
                }
            }
        }
    }
}