using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractorFPS : MonoBehaviour
{
    void Update()
    {
        // Detectar clic izquierdo
        if (Input.GetMouseButtonDown(0)) 
        {
            InteractuarConUI();
        }
    }

    void InteractuarConUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = new Vector2(Screen.width / 2, Screen.height / 2);

        List<RaycastResult> resultados = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, resultados);

        // Recorremos todos los objetos que el rayo ha tocado
        foreach (RaycastResult resultado in resultados)
        {
            GameObject objetoGolpeado = resultado.gameObject;

            // --- FILTRO CRÍTICO PARA DROPDOWNS ---
            // Ignoramos objetos que no sean interactuables o que sean solo visuales (como textos sin colisión)
            // Pero si es un "Item" de Dropdown, queremos hacer clic en él.
            
            // Buscamos si el objeto o sus padres tienen un componente seleccionable (Button, Toggle, Dropdown)
            Selectable selectable = objetoGolpeado.GetComponentInParent<Selectable>();

            if (selectable != null && selectable.interactable)
            {
                // ¡ENCONTRADO! Hacemos clic en el componente interactuable (no necesariamente en el objeto hijo golpeado)
                GameObject objetoFinal = selectable.gameObject;

                // Secuencia completa de clic para asegurar que Unity UI responda
                ExecuteEvents.Execute(objetoFinal, pointerData, ExecuteEvents.pointerEnterHandler);
                ExecuteEvents.Execute(objetoFinal, pointerData, ExecuteEvents.pointerDownHandler);
                ExecuteEvents.Execute(objetoFinal, pointerData, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(objetoFinal, pointerData, ExecuteEvents.pointerUpHandler);
                
                // Forzar actualización del EventSystem para cerrar el Dropdown tras elegir
                ExecuteEvents.Execute(objetoFinal, pointerData, ExecuteEvents.submitHandler); 
                
                // Rompemos el bucle para no hacer clic en el panel de fondo también
                break; 
            }
        }
    }
}