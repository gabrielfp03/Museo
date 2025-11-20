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
                // Guardamos la referencia del objeto golpeado
                ChangeColorObject objeto = hit.collider.GetComponent<ChangeColorObject>();

                if (objeto != null)
                {
                    // Llamamos a la función nueva que controla la lista
                    objeto.SiguienteColor(); 
                }
            }
        }
    }
}