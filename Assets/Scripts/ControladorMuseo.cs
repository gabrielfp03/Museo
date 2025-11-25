using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMuseo : MonoBehaviour
{
    [Header("Configuración")]
    public Transform jugador; // Arrastra aquí a tu FirstPersonController
    public float velocidad = 3.0f;

    // Variables privadas para saber si nos estamos moviendo
    private bool moviendoAdelante = false;
    private bool moviendoAtras = false;
    private bool moviendoIzquierda = false;
    private bool moviendoDerecha = false;

    void Update()
    {
        // Si no hay jugador asignado, no hacemos nada
        if (jugador == null) return;

        // Movemos al jugador según qué interruptor esté encendido
        // Usamos Time.deltaTime para que el movimiento sea suave
        if (moviendoAdelante)
            jugador.Translate(Vector3.forward * velocidad * Time.deltaTime);
        
        if (moviendoAtras)
            jugador.Translate(Vector3.back * velocidad * Time.deltaTime);
        
        if (moviendoIzquierda)
            jugador.Translate(Vector3.left * velocidad * Time.deltaTime);
        
        if (moviendoDerecha)
            jugador.Translate(Vector3.right * velocidad * Time.deltaTime);
    }

    // --- FUNCIONES PARA LOS BOTONES UI (ON CLICK) ---

    // Esta función apaga todos los movimientos. 
    // La llamamos antes de activar uno nuevo para no movernos en diagonal por error.
    public void DetenerTodo()
    {
        moviendoAdelante = false;
        moviendoAtras = false;
        moviendoIzquierda = false;
        moviendoDerecha = false;
    }

    public void ToggleAdelante()
    {
        // Si ya nos movíamos hacia adelante, paramos. Si no, empezamos.
        bool estadoActual = moviendoAdelante;
        DetenerTodo(); // Primero reseteamos para que no se mezclen direcciones
        moviendoAdelante = !estadoActual; // Invertimos el estado anterior
    }

    public void ToggleAtras()
    {
        bool estadoActual = moviendoAtras;
        DetenerTodo();
        moviendoAtras = !estadoActual;
    }

    public void ToggleIzquierda()
    {
        bool estadoActual = moviendoIzquierda;
        DetenerTodo();
        moviendoIzquierda = !estadoActual;
    }

    public void ToggleDerecha()
    {
        bool estadoActual = moviendoDerecha;
        DetenerTodo();
        moviendoDerecha = !estadoActual;
    }

    // --- MENU ---
    public void LoadMuseumMainScene()
    {
        SceneManager.LoadScene("Museum_MainScene");
    }
}