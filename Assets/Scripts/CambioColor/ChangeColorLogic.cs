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
            // Crear un rayo desde la cámara hacia la posición del ratón
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Si el rayo golpea algo
            if (Physics.Raycast(ray, out hit))
            {
                // Comprobamos si el objeto golpeado tiene el script 'ChangeColorObject'
                if (hit.collider.GetComponent<ChangeColorObject>())
                {
                    // Guardamos la referencia del objeto golpeado
                    ChangeColorObject newChangeColorObject = hit.collider.GetComponent<ChangeColorObject>();

                    // Lógica para alternar el color (similar al script anterior)
                    if (!newChangeColorObject.colorChanged)
                    {
                        newChangeColorObject.ChangeColor(newChangeColorObject.newColor);
                    }
                    else
                    {
                        newChangeColorObject.ChangeColor(newChangeColorObject.defaultColor);
                    }
                }
            }
        }
    }
}