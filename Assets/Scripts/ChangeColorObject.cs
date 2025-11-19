using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorObject : MonoBehaviour
{
    private Renderer renderObject;

    [Header("Colores para el ciclo")]
    public List<Color> coloresExtra; 

    private List<Color> todosLosColores = new List<Color>();
    private int indiceActual = 0;

    void Start()
    {
        renderObject = GetComponent<Renderer>();


        if (renderObject != null)
        {
            todosLosColores.Add(renderObject.material.color);
        }
        
        todosLosColores.AddRange(coloresExtra);
    }

    public void SiguienteColor()
    {
        if (todosLosColores.Count == 0) return;

        indiceActual++;

        if (indiceActual >= todosLosColores.Count)
        {
            indiceActual = 0; 
        }

        // Aplicamos el color
        renderObject.material.color = todosLosColores[indiceActual];
    }
}