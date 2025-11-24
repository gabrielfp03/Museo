using UnityEngine;
// Eliminamos el using Leap; ya que no se necesita

public class LeapCursor_MouseAdaptation : MonoBehaviour
{
    [Header("Cámara principal")]
    public Camera mainCamera;

    // Ya no necesitamos LeapServiceProvider, FingerType, ni depthOffset
    // Eliminaremos LeapServiceProvider leapProvider;
    // Eliminaremos Finger.FingerType fingerType = Finger.FingerType.INDEX;
    // Eliminaremos public float depthOffset = 0.05f;

    [Header("Máscara de Capa para UI interactuable")]
    [Tooltip("Asegúrate de que tus elementos de UI 3D estén en esta capa.")]
    public LayerMask UILayer; 
    
    [Header("Distancia máxima del Raycast")]
    public float raycastDistance = 1000f;

    [Header("Posición Z del Plano de Interacción")]
    [Tooltip("La posición Z mundial donde quieres que el cursor aparezca.")]
    public float targetWorldZ = 6.0f; // Set this to the Z plane of your Canvas

    private static LeapCursor_MouseAdaptation instance;
    private RaycastButtonAction currentHoverButton = null;

    void Awake()
    {
        // Implementación del Singleton (asegura que solo haya una instancia)
        if (instance != null && instance != this) { 
            Destroy(gameObject); 
            return; 
        }
        instance = this;
    }

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Cámara principal no encontrada. Asegúrate de que una cámara tenga el tag 'MainCamera'.");
        }
    }

    void Update()
    {
        // 1. Crear el Rayo desde la posición del mouse
        if (mainCamera == null) {
            HandleExit();
            return;
        }
        


        // --- 1. MOVEMENT: Project Mouse onto Fixed Z-Plane ---
        
        // Define the plane: Normal (Vector3.back or Vector3.forward) and a point on the plane (Z=targetWorldZ)
        // Vector3.back is used assuming the plane faces the camera, usually aligned with the world Z-axis.
        Plane targetPlane = new Plane(Vector3.back, new Vector3(0, 0, targetWorldZ)); 
        
        // El Rayo se origina en la posición del mouse en la pantalla y va hacia el mundo 3D.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float distance;
        
        // Check if the ray intersects the plane
        targetPlane.Raycast(ray, out distance);


        RaycastHit hit;
        
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);
        
        // Asumimos que el cursor debe seguir la posición de impacto del raycast.
        bool hitSomething = Physics.Raycast(ray, out hit, raycastDistance);

        if (hitSomething)
        {
            // El cursor (este GameObject) se mueve a la posición donde el rayo impactó
            //transform.position = hit.point;

            // 2. Comprobar si el Rayo impactó un elemento en la capa UILayer
            // Usamos hit.collider.gameObject.layer para asegurar que es un elemento interactuable
            if (((1 << hit.collider.gameObject.layer) & UILayer) != 0) 
            {
                // Intentar obtener el script de acción del botón
                RaycastButtonAction button = hit.collider.GetComponent<RaycastButtonAction>();
                
                if (button != null)
                {
                    // Lógica de HOVER (Entrar/Permanecer)
                    if (currentHoverButton != button)
                    {
                        // Si estaba en otro botón, salimos de él primero
                        if (currentHoverButton != null)
                        {
                            currentHoverButton.OnRaycastExit();
                        }
                        currentHoverButton = button;
                        currentHoverButton.OnRaycastEnter(); // Inicia el feedback visual
                    }
                    
                    // MANTENER LA PULSACIÓN TEMPORIZADA: Esto incrementa el pressTimer
                    currentHoverButton.OnRaycastStay(); 

                    // Lógica de CLICK (Opcional: usar el botón izquierdo del mouse)
                    /*if (Input.GetMouseButtonDown(0)) // 0 es el botón izquierdo
                    {
                        // Llama a la acción de click del botón inmediatamente
                        button.ExecuteAction(); 
                        // Nota: Si quieres simular la "pulsación temporal" del Leap, 
                        // esto puede no ser necesario, pero es un comportamiento común de mouse.
                    }*/
                }
                else
                {
                    // Acertó algo en la capa UI, pero no era un botón válido (no tiene RaycastButtonAction)
                    HandleExit();
                }
            }
            else // Acertó algo, pero no en la capa UILayer
            {
                 HandleExit();
            }
        }
        else // No acertó nada
        {
            HandleExit();
            // Opcional: Si quieres que el cursor vaya a una posición fija cuando no golpea nada,
            // puedes moverlo a un punto lejano o dejarlo donde está.
        }
    }

    private void HandleExit()
    {
        if (currentHoverButton != null)
        {
            currentHoverButton.OnRaycastExit();
            currentHoverButton = null;
        }
    }
    
    // (Opcional: mantener la implementación del Singleton estático si lo necesitas fuera de este script)
    public static LeapCursor_MouseAdaptation Instance { get { return instance; } }
}