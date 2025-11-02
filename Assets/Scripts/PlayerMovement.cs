using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Obtener input horizontal (A y D) y vertical (W y S)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Mover el personaje
        transform.Translate(new Vector3(horizontal, 0f, vertical) * Time.deltaTime * speed);
    }
}
