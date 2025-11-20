using UnityEngine;

public class GestorDeSalas : MonoBehaviour
{
    [Header("Arrastra aquí tus 4 Paneles Info")]
    public GameObject[] panelesInfo; 

    [Header("Opcional: Botón Volver")]
    public GameObject botonVolverGeneral; 

    private int indiceActual = 0;
    private bool hayPanelAbierto = false; 

    void Start()
    {
        CerrarTodo(); 
    }

    // --- FUNCIÓN PARA LOS BOTONES DEL MAPA (1, 2, 3, 4) ---
    public void AbrirPanelEspecifico(int indiceSala)
    {
        indiceActual = indiceSala;
        hayPanelAbierto = true;
        ActualizarVisuales();
    }

    // --- FUNCIONES PARA EL LEAP MOTION (ESTAS SON LAS QUE FALTABAN) ---
    
    public void SiguienteSala() 
    {
        if (!hayPanelAbierto) return; 

        indiceActual++;
        if (indiceActual >= panelesInfo.Length) indiceActual = 0; 
        ActualizarVisuales();
    }

    public void SalaAnterior()
    {
        if (!hayPanelAbierto) return;

        indiceActual--;
        if (indiceActual < 0) indiceActual = panelesInfo.Length - 1; 
        ActualizarVisuales();
    }

    // --- RESTO DE LÓGICA ---
    public void CerrarTodo()
    {
        hayPanelAbierto = false;
        foreach(GameObject p in panelesInfo) p.SetActive(false);
        if(botonVolverGeneral != null) botonVolverGeneral.SetActive(false);
    }

    private void ActualizarVisuales()
    {
        for (int i = 0; i < panelesInfo.Length; i++)
        {
            panelesInfo[i].SetActive(i == indiceActual);
        }
        if(botonVolverGeneral != null) botonVolverGeneral.SetActive(true);
    }
}