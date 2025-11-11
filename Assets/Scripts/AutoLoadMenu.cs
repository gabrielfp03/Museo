using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadMenu : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena del menú")]
    public string menuSceneName = "Menu_Scene";

    void Awake()
    {
        // Si la escena activa NO es el menú, carga el menú automáticamente
        if (SceneManager.GetActiveScene().name != menuSceneName)
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }
}
