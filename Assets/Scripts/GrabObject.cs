using UnityEngine;

public class GrabObject : MonoBehaviour
{
    public float pickUpDistance = 200f;
    public float distanceToCam = 15f;
    public float scaleFactor = 10f;
    public float rotateSpeed = 1000f;

    private Camera cam;
    private Transform grabbedObject;
    private Rigidbody grabbedRb;
    private bool wasKinematic;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;



    void Start()
    {
        cam = Camera.main;

        // Poner el handle en frente de la camara
        transform.position = cam.transform.position + cam.transform.forward * distanceToCam;
    }

    void Update()
    {
        // Debug raycast visualization
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickUpDistance, Color.red);


        // Interacción
        if (Input.GetMouseButtonDown(0))  // clic izquierdo para agarrar/soltar
        {
            if (grabbedObject == null) TryGrab();
            else Drop();
        }

        if (grabbedObject != null && Input.GetMouseButton(1)) // clic derecho para rotar
        {
            RotateObject();
        }
    }

    void TryGrab()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, pickUpDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Grabbable"))
            {
                Debug.Log("Object grabbed!");

                grabbedObject = hit.collider.transform;
                grabbedRb = hit.collider.gameObject.GetComponent<Rigidbody>(); // could be null

                // Si tiene físicas se desactivan
                if (grabbedRb != null)
                {
                    wasKinematic = grabbedRb.isKinematic;
                    grabbedRb.isKinematic = true;
                }

                // Recordar valores para restaurar el objeto al soltarlo
                originalParent = grabbedObject.parent;
                originalPosition = grabbedObject.position;
                originalRotation = grabbedObject.rotation;
                originalScale = grabbedObject.localScale;



                // Normalizar el tamaño
                Vector3 size = hit.collider.bounds.size; // dimensiones actuales del objeto (sin incluir hijos)
                float maxDimension = Mathf.Max(size.x, size.y, size.z);
                float baseScaleFactor = 1f / maxDimension; // Escala para que la dimensión más grande sea 1
                grabbedObject.localScale *= baseScaleFactor * scaleFactor;

                // Calcular el centro visual del objeto (incluyendo hijos)
                Renderer[] renderers = grabbedObject.GetComponentsInChildren<Renderer>();
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    bounds.Encapsulate(renderers[i].bounds);

                // Mover el objeto para que su centro visual coincida con el centro del handle
                Vector3 offset = bounds.center - grabbedObject.position;
                grabbedObject.position = transform.position - offset;

                // Asignar el handle como padre
                grabbedObject.SetParent(transform, true);
            }
        }
    }

    void Drop()
    {
        if (grabbedObject == null) return;

        // Restaurar posición y rotación originales
        grabbedObject.position = originalPosition;
        grabbedObject.rotation = originalRotation;

        // Restaurar las físicas
        if (grabbedRb != null)
        {
            grabbedRb.isKinematic = wasKinematic;
            grabbedRb = null;
        }

        // Restaurar al padre original
        grabbedObject.SetParent(originalParent, true);

        // Restaurar la escala original
        grabbedObject.localScale = originalScale;

        // Resetear la rotación del handle
        transform.localRotation = Quaternion.identity;

        grabbedObject = null;
    }

    void RotateObject()
    {
        // Obtener movimiento del mouse
        float rotX = -Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        float rotY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        // Crear rotaciones incrementales
        Quaternion rotXQuat = Quaternion.AngleAxis(rotX, Vector3.up);   // rotación horizontal
        Quaternion rotYQuat = Quaternion.AngleAxis(rotY, Vector3.right); // rotación vertical

        // Aplicar las rotaciones
        transform.localRotation = rotXQuat * rotYQuat * transform.localRotation;
    }
}
