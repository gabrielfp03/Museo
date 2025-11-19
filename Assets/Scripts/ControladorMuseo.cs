using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMuseo : MonoBehaviour
{
    [Header("Configuración")]
    public Transform jugador;
    public float velocidad = 2.0f; // Ajusta la velocidad aquí

    // Variables "interruptor" para saber si debemos movernos
    private bool avanzando = false;
    private bool retrocediendo = false;
    private bool izquierda = false;
    private bool derecha = false;

    void Update()
    {
        if (jugador == null) return;

        // Mientras los interruptores estén encendidos, movemos al jugador
        if (avanzando)     jugador.Translate(Vector3.forward * velocidad * Time.deltaTime);
        if (retrocediendo) jugador.Translate(Vector3.back * velocidad * Time.deltaTime);
        if (izquierda)     jugador.Translate(Vector3.left * velocidad * Time.deltaTime);
        if (derecha)       jugador.Translate(Vector3.right * velocidad * Time.deltaTime);
    }

    // --- FUNCIONES PARA CONECTAR AL PRESSED (Al tocar) ---
    public void EmpezarAvanzar() { avanzando = true; }
    public void EmpezarRetroceder() { retrocediendo = true; }
    public void EmpezarIzquierda() { izquierda = true; }
    public void EmpezarDerecha() { derecha = true; }

    // --- FUNCIONES PARA CONECTAR AL UNPRESSED (Al soltar) ---
    public void DetenerMovimiento() 
    { 
        // Apagamos todo por seguridad
        avanzando = false;
        retrocediendo = false;
        izquierda = false;
        derecha = false;
    }

    // --- MENU ---
    public void IrAlMenu()
    {
        SceneManager.LoadScene(0);
    }
}