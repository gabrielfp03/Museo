using UnityEngine;

public class GrabObject : MonoBehaviour
{
    public float pickUpDistance = 100f;
    public float distanceToCam = 15f;
    public float moveSpeed = 0.1f;
    public float rotateSpeed = 1000f;
    public float scaleFactor = 10f;

    private Camera cam;
    private Rigidbody grabbedRb;
    private bool hadGravity;
    private RigidbodyConstraints previousConstraints;
    private Transform originalParent;
    private Quaternion rotationOffset;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private Vector3 currentVelocity;
    private GameObject handle;



    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Debug
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickUpDistance, Color.red);

        if (Input.GetMouseButtonDown(0))
        {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out RaycastHit hit, pickUpDistance))
                Debug.Log("Golpeó: " + hit.collider.name);
            else
                Debug.Log("No golpeó nada.");
        }


        // Interacción
        if (Input.GetMouseButtonDown(0))  // clic izquierdo para agarrar/soltar
        {
            if (grabbedRb == null) TryGrab();
            else Drop();
        }

        if (grabbedRb != null)
        {
            MoveObject();
            RotateObject();
        }
    }

    void TryGrab()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, pickUpDistance))
        {
            if (hit.transform.CompareTag("Grabbable"))
            {
                grabbedRb = hit.rigidbody;
                Collider col = grabbedRb.GetComponent<Collider>();



                // Valores para restaurar el objeto al soltarlo
                // Recordar el padre
                originalParent = grabbedRb.transform.parent;

                // Recordar si tenia gravedad y quitarsela
                hadGravity = grabbedRb.useGravity;
                grabbedRb.useGravity = false;

                // Recordar si tenia constraints y ponerle FreezeRotation
                previousConstraints = grabbedRb.constraints;
                grabbedRb.constraints = RigidbodyConstraints.FreezePosition;
                grabbedRb.constraints = RigidbodyConstraints.FreezeRotation;

                // Recordar la posición, orientación y escala originales
                originalPosition = grabbedRb.transform.position;
                originalRotation = grabbedRb.transform.rotation;
                originalScale = grabbedRb.transform.localScale;



                // Normalizar el tamaño
                Vector3 size = col.bounds.size; // dimensiones actuales del objeto
                float maxDimension = Mathf.Max(size.x, size.y, size.z);
                float baseScaleFactor = 1f / maxDimension; // Escala para que la dimensión más grande sea 1
                grabbedRb.transform.localScale *= baseScaleFactor * scaleFactor;

                // Guardar el offset en la rotación del objeto
                rotationOffset = Quaternion.Inverse(cam.transform.rotation) * grabbedRb.transform.rotation;



                // Crear un handle para centrar el objeto en la vista de la camara
                // 1. Create empty handle
                handle = new GameObject("GrabHandle");

                // 2. Calculate visual center of the object (including children)
                Renderer[] renderers = grabbedRb.GetComponentsInChildren<Renderer>();
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    bounds.Encapsulate(renderers[i].bounds);

                // 3. Place the handle at the center of bounds
                handle.transform.position = bounds.center;
                handle.transform.rotation = grabbedRb.transform.rotation;

                // 4. Parent the object to the handle while keeping world position
                grabbedRb.transform.SetParent(handle.transform, true);
            }
        }
    }

    void Drop()
    {
        // Restaurar posición y rotación originales
        grabbedRb.transform.position = originalPosition;
        grabbedRb.transform.rotation = originalRotation;

        // Restaurar la gravedad y los constraints
        grabbedRb.useGravity = hadGravity;
        grabbedRb.constraints = previousConstraints;

        // Restaurar al padre original
        grabbedRb.transform.SetParent(originalParent, true);

        // Restaurar la escala original
        grabbedRb.transform.localScale = originalScale;
        
        // Destruir el handle
        Destroy(handle);

        grabbedRb = null;
    }

    void MoveObject()
    {
        Vector3 targetPos = cam.transform.position + cam.transform.forward * distanceToCam;
        handle.transform.position = Vector3.SmoothDamp(handle.transform.position, targetPos, ref currentVelocity, moveSpeed);
        handle.transform.rotation = cam.transform.rotation * rotationOffset;
    }

    void RotateObject()
    {
        if (Input.GetMouseButton(1)) // clic derecho para rotar
        {
            // Obtener movimiento del mouse
            float rotX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

            // Crear rotaciones incrementales
            Quaternion rotXQuat = Quaternion.AngleAxis(rotX, Vector3.up);   // rotación horizontal
            Quaternion rotYQuat = Quaternion.AngleAxis(rotY, Vector3.right); // rotación vertical

            // Aplicarlas sobre el offset acumulado
            rotationOffset = rotXQuat * rotYQuat * rotationOffset;
        }
    }
}
