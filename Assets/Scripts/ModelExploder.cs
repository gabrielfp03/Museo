// using UnityEngine;
// using System.Collections.Generic;

// public class ModelExploder : MonoBehaviour
// {
//     [Tooltip("Distancia a la que se separarán los componentes.")]
//     public float explosionRadius = 5f;

//     [Tooltip("Velocidad de movimiento para la animación suave.")]
//     public float moveSpeed = 2f;

//     private List<PartData> parts = new List<PartData>();
//     private bool isExploded = false;
//     [HideInInspector]
//     public float explosionProgress = 0f;

//     // ESTRUCTURA FINAL: Almacena la posición local original y el centro visual de la pieza.
//     private struct PartData
//     {
//         public Transform partTransform;
//         public Vector3 originalLocalPosition; 
//         public Vector3 centerForExplosion; // Centro de la malla (World Space)
//     }

//     void Start()
//     {
//         parts.Clear();
        
//         // Iteramos a través de TODOS los hijos directos
//         for (int i = 0; i < transform.childCount; i++)
//         {
//             Transform child = transform.GetChild(i);
//             Renderer renderer = child.GetComponent<Renderer>(); // Buscamos el componente Renderer
            
//             if (renderer != null) 
//             {
//                 parts.Add(new PartData
//                 {
//                     partTransform = child,
//                     originalLocalPosition = child.localPosition, 
//                     // Usamos el centro del Boundig Box del Renderer como punto de referencia
//                     centerForExplosion = renderer.bounds.center 
//                 });
//             }
//             else
//             {
//                 Debug.LogWarning($"La pieza '{child.name}' no tiene Renderer y será ignorada para la explosión.");
//             }
//         }
//     }

//     public void ToggleExplode()
//     {
//         isExploded = !isExploded;
//     }

//     // void Update()
//     // {
        
//     //     Vector3 referenceCenter = transform.position; // Centro World Space del objeto Padre

//     //     foreach (var part in parts)
//     //     {
//     //         Vector3 targetPosition;

//     //         if (isExploded)
//     //         {
//     //             // ** LÍNEA CLAVE SOLUCIONADA: **
//     //             // Calculamos la dirección usando el centro visual de la malla (centerForExplosion)
//     //             Vector3 direction = (part.centerForExplosion - referenceCenter).normalized;
                
//     //             // Si la dirección es cercana a cero (la pieza estaba EXACTAMENTE en el centro global)
//     //             if (direction.sqrMagnitude < 0.0001f)
//     //             {
//     //                 direction = Vector3.forward; 
//     //             }
                
//     //             // Calculamos la posición objetivo en World Space y luego la convertimos a Local Space
//     //             Vector3 targetWorldPosition = referenceCenter + (direction * explosionRadius);
//     //             targetPosition = transform.InverseTransformPoint(targetWorldPosition);
//     //         }
//     //         else
//     //         {
//     //             // Posición de ensamblado
//     //             targetPosition = part.originalLocalPosition;
//     //         }

//     //         // Mover la pieza suavemente
//     //         part.partTransform.localPosition = Vector3.Lerp(
//     //             part.partTransform.localPosition,
//     //             targetPosition,
//     //             Time.deltaTime * moveSpeed
//     //         );
//     //     }
//     // }
//     void Update()
//     {
//         // NO USAR EL TECLADO O EL VIEJO TOGGLE AQUÍ.
//         // El valor de explosionProgress será actualizado por el script de UltraLeap.
        
//         Vector3 referenceCenter = transform.position; 

//         // 1. EL FACTOR DE SEPARACIÓN ES AHORA EL PROGRESO:
//         float currentSeparationFactor = explosionProgress;

//         foreach (var part in parts)
//         {
//             Vector3 targetPosition;

//             // La dirección de separación sigue siendo la misma (calculada radialmente)
//             Vector3 direction = (part.centerForExplosion - referenceCenter).normalized;
//             if (direction.sqrMagnitude < 0.0001f)
//             {
//                 direction = Vector3.forward; 
//             }

//             // 2. CÁLCULO DE LA POSICIÓN INTERMEDIA:
//             // a) Calcula la posición final de separación (World Space)
//             Vector3 finalExplodedWorldPosition = referenceCenter + (direction * explosionRadius);
            
//             // b) Convierte la posición final a Local Space
//             Vector3 finalExplodedLocalPosition = transform.InverseTransformPoint(finalExplodedWorldPosition);

//             // c) Interpolación (Lerp) entre la posición original y la final, usando el progreso.
//             targetPosition = Vector3.Lerp(
//                 part.originalLocalPosition,
//                 finalExplodedLocalPosition,
//                 currentSeparationFactor // <-- ¡Usamos el progreso aquí!
//             );
            
//             // 3. Mover la pieza suavemente (usamos un Lerp adicional para suavizar el movimiento)
//             part.partTransform.localPosition = Vector3.Lerp(
//                 part.partTransform.localPosition,
//                 targetPosition,
//                 Time.deltaTime * moveSpeed
//             );
//         }
//     }
// }

using UnityEngine;
using System.Collections.Generic;

// Este script controla el movimiento suave de las piezas del modelo 
// basándose en un valor de progreso de explosión (0.0 = ensamblado, 1.0 = explotado).
public class ModelExploder : MonoBehaviour
{
    [Tooltip("Distancia máxima a la que se separarán los componentes (cuando explosionProgress = 1.0).")]
    public float explosionRadius = 5f;

    [Tooltip("Velocidad de interpolación para el movimiento suave de las piezas.")]
    public float moveSpeed = 5f; // Aumentado ligeramente para mejor respuesta

    // El progreso actual del despiece, controlado por el script de interacción.
    [HideInInspector]
    public float explosionProgress = 0f;

    private List<PartData> parts = new List<PartData>();

    // ESTRUCTURA: Almacena la información necesaria de cada pieza.
    private struct PartData
    {
        public Transform partTransform;
        public Vector3 originalLocalPosition; 
        public Vector3 centerForExplosion; // Centro de la malla en World Space (para calcular la dirección radial)
    }

    void Start()
    {
        parts.Clear();
        
        // Inicializa todas las piezas que tienen un Renderer.
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Renderer renderer = child.GetComponent<Renderer>(); 
            
            if (renderer != null) 
            {
                parts.Add(new PartData
                {
                    partTransform = child,
                    originalLocalPosition = child.localPosition, 
                    // Usamos el centro del Boundig Box del Renderer
                    centerForExplosion = renderer.bounds.center 
                });
            }
            else
            {
                Debug.LogWarning($"La pieza '{child.name}' no tiene Renderer y será ignorada.");
            }
        }
    }

    void Update()
    {
        // Usamos la variable explosionProgress (actualizada por el script de control)
        float currentSeparationFactor = explosionProgress;
        Vector3 referenceCenter = transform.position; 

        foreach (var part in parts)
        {
            // 1. Calcular la dirección radial de la explosión
            Vector3 direction = (part.centerForExplosion - referenceCenter).normalized;
            // Manejar el caso donde el centro de la pieza coincide con el centro del modelo
            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = Vector3.forward; 
            }

            // 2. Calcular la posición final de separación máxima (Local Space)
            Vector3 finalExplodedWorldPosition = referenceCenter + (direction * explosionRadius);
            Vector3 finalExplodedLocalPosition = transform.InverseTransformPoint(finalExplodedWorldPosition);

            // 3. Interpolar (Lerp) la posición objetivo entre la posición original (0%) y la final (100%)
            Vector3 targetPosition = Vector3.Lerp(
                part.originalLocalPosition,
                finalExplodedLocalPosition,
                currentSeparationFactor // Usa el progreso del Ultraleap
            );
            
            // 4. Mover la pieza suavemente hacia la posición objetivo (Lerp de suavizado)
            part.partTransform.localPosition = Vector3.Lerp(
                part.partTransform.localPosition,
                targetPosition,
                Time.deltaTime * moveSpeed
            );
        }
    }
}