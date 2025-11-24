using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractorFPS : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaInteraccion = 5.0f; // Cuán lejos puedes tocar botones
    public LayerMask capasInteractuables;     // Qué capas puede tocar (UI, Default, etc.)

    [Header("Debug (Opcional)")]
    public Image punteroMire; // Si quieres poner un puntito/cruz en el centro de la pantalla

    void Update()
    {
        // 1. Detectar clic izquierdo del ratón (o botón de acción)
        if (Input.GetMouseButtonDown(0)) 
        {
            InteractuarConUI();
        }
    }

    void InteractuarConUI()
    {
        // Crear un puntero de datos ficticio en el centro de la pantalla
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = new Vector2(Screen.width / 2, Screen.height / 2);

        // Crear una lista para guardar lo que golpeemos
        List<RaycastResult> resultados = new List<RaycastResult>();

        // Lanzar el rayo usando el sistema de eventos de Unity
        EventSystem.current.RaycastAll(pointerData, resultados);

        // Si golpeamos algo
        foreach (RaycastResult resultado in resultados)
        {
            // Intentamos ejecutar un clic en ese objeto
            // Esto busca cualquier botón, dropdown o toggle en el objeto golpeado
            GameObject objetoGolpeado = resultado.gameObject;
            
            // Intenta hacer clic (Click Handler)
            ExecuteEvents.Execute(objetoGolpeado, pointerData, ExecuteEvents.pointerClickHandler);
            
            // Intenta "bajar" el ratón (Down Handler) - Necesario para algunos Dropdowns
            ExecuteEvents.Execute(objetoGolpeado, pointerData, ExecuteEvents.pointerDownHandler);
            
            // IMPORTANTE: Para Dropdowns, a veces necesitamos simular que soltamos el clic
            StartCoroutine(SoltarClic(objetoGolpeado, pointerData));

            // Si encontramos algo UI, paramos el rayo (para no clicar cosas detrás)
            if (objetoGolpeado.GetComponent<Selectable>() != null)
            {
                break; 
            }
        }
    }

    // Pequeña corrutina para simular el "Click Up" (soltar el ratón)
    System.Collections.IEnumerator SoltarClic(GameObject obj, PointerEventData data)
    {
        yield return null; // Espera un frame
        ExecuteEvents.Execute(obj, data, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(obj, data, ExecuteEvents.submitHandler); // Intenta enviar "Submit"
    }
}