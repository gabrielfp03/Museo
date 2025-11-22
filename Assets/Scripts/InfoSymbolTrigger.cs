// using UnityEngine;

// public class InfoSymbolTrigger : MonoBehaviour
// {
//     public InfoObraMuseo infoDeEsteAvion; 
//     private InfoPanelManager manager;

//     void Start()
//     {
//         manager = FindFirstObjectByType<InfoPanelManager>();
//     }

//     private void OnMouseDown()
//     {
//         if (manager != null && infoDeEsteAvion != null)
//         {
//             manager.AbrirPanel(infoDeEsteAvion);
//         }
//     }
// }


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class InfoSymbolTrigger : MonoBehaviour
{
    [Header("Timings")]
    [Tooltip("Tiempo que el dedo debe mantener la pulsación para activarla (ej: 0.5s).")]
    public float requiredPressDuration = 0.5f;

    [Tooltip("Retardo entre la confirmación de la pulsación y la ejecución de la acción (ej: 0.3s).")]
    public float actionDelay = 0.3f;

    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color pressedColor = Color.green;

    [Header("Button Action")]
    public UnityEvent OnButtonPress = new UnityEvent();
    public UnityEvent OnButtonRelease = new UnityEvent();    private InfoPanelManager manager;
    private Image buttonImage;
    private bool isConfirmed = false; 
    private float pressTimer = 0f;    
    private Coroutine actionCoroutine; 
    private bool isHovering = false; // El raycast del dedo está sobre el botón
    void Start()
    {
        buttonImage = GetComponent<Image>();
        manager = FindFirstObjectByType<InfoPanelManager>();
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
        if (manager == null)
        {
            Debug.LogError("InfoSymbolTrigger: No se encontró InfoPanelManager en la escena.");
        }
    }

    public void OnRaycastEnter()
    {
        if (isHovering) return;
        
        isHovering = true;
        isConfirmed = false;
        pressTimer = 0f;
        
        if (buttonImage != null)
        {
            // Feedback visual al entrar/apuntar
            buttonImage.color = pressedColor; 
        }
    }

    public void OnRaycastStay()
    {
       if (isConfirmed) return;

        pressTimer += Time.deltaTime; // Incrementa el temporizador

        // Si la demora se cumple:
        if (pressTimer >= requiredPressDuration)
        {
            isConfirmed = true;
            actionCoroutine = StartCoroutine(ConfirmAndInvokeAction());
            Debug.Log("InfoSymbolTrigger: Acción confirmada tras mantener la pulsación.");
        }
    }
    
    public void OnRaycastExit()
    {   
        ResetButtonState();
    }
    // ... (OnTriggerExit y Coroutines son correctos)
    private void OnTriggerExit(Collider other)
    {
        ResetButtonState();
    }

    private IEnumerator ConfirmAndInvokeAction()
    {
        yield return new WaitForSeconds(actionDelay);
        
        OnButtonPress.Invoke();
        // ResetButtonState();
    }

    private void ResetButtonState()
    {
        if (actionCoroutine != null)
        {
            StopCoroutine(actionCoroutine);
            actionCoroutine = null;
        }
        
        pressTimer = 0f;
        isConfirmed = false;
        isHovering = false;
        
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }
}