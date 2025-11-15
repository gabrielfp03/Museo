using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorObject : MonoBehaviour
{
    private Renderer renderObject;
    
    public Color defaultColor;
    public Color newColor;
    
    // Variable para saber si ya hemos cambiado de color (semáforo)
    public bool colorChanged; 

    void Start()
    {
        // Obtenemos el componente Renderer del objeto y ponemos el color por defecto
        renderObject = GetComponent<Renderer>();
        renderObject.material.color = defaultColor;
    }

    void Update()
    {
        // Al pulsar la tecla 'C', cambia el color de TODOS los objetos con este script
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!colorChanged)
            {
                ChangeColor(newColor);
            }
            else
            {
                ChangeColor(defaultColor);
            }
        }
    }

    // Método público para poder ser llamado desde el otro script también
    public void ChangeColor(Color colorToChange)
    {
        // Invertimos el valor de la booleana (true -> false / false -> true)
        colorChanged = !colorChanged;
        renderObject.material.color = colorToChange;
    }
}