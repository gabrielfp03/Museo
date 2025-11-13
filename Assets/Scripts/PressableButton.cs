using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple pressable button component.
/// Attach to a button GameObject that has a Collider set to "Is Trigger".
/// Configure a LayerMask to limit what can press it (hands, pointers), a
/// hold delay (time required to count as a press) and UnityEvents for press/release.
/// The component also animates the button moving inward when pressed.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PressableButton : MonoBehaviour
{
    [Tooltip("Which layers can trigger the button. Default = Everything.")]
    public LayerMask presserLayer = ~0;

    [Tooltip("Time (seconds) the presser must remain in contact for the button to activate.")]
    public float pressDelay = 0.2f;

    [Tooltip("How far (meters) the button moves inward when pressed.")]
    public float pressDepth = 0.02f;

    [Tooltip("Speed used when the button returns to rest after release.")]
    public float releaseSpeed = 8f;

    [Tooltip("Event raised when the button is considered pressed (after hold time).")]
    public UnityEvent onPressed;

    [Tooltip("Event raised when the button is released (after it was pressed).")]
    public UnityEvent onReleased;

    // Internal state
    Vector3 _localStartPos;
    Coroutine _pressRoutine;
    Coroutine _releaseRoutine;
    int _touchingCount = 0;
    bool _isPressed = false;

    void Awake()
    {
        _localStartPos = transform.localPosition;
        // Ensure our collider is trigger (we animate localPosition, so physics isn't needed)
        var col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("PressableButton: collider is not marked as Trigger — marking it automatically.", this);
            col.isTrigger = true;
        }
    }

    void OnValidate()
    {
        pressDelay = Mathf.Max(0f, pressDelay);
        pressDepth = Mathf.Max(0f, pressDepth);
        releaseSpeed = Mathf.Max(0.1f, releaseSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsPresser(other)) return;

        _touchingCount++;
        if (_pressRoutine == null)
        {
            if (_releaseRoutine != null)
            {
                StopCoroutine(_releaseRoutine);
                _releaseRoutine = null;
            }
            _pressRoutine = StartCoroutine(PressRoutine(other));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsPresser(other)) return;

        _touchingCount = Mathf.Max(0, _touchingCount - 1);

        if (_touchingCount == 0)
        {
            if (_pressRoutine != null)
            {
                StopCoroutine(_pressRoutine);
                _pressRoutine = null;
            }
            // If it was pressed, invoke release and animate back
            if (_isPressed)
            {
                _isPressed = false;
                onReleased?.Invoke();
            }
            _releaseRoutine = StartCoroutine(ReleaseRoutine());
        }
    }

    bool IsPresser(Collider other)
    {
        return ((presserLayer.value & (1 << other.gameObject.layer)) != 0);
    }

    System.Collections.IEnumerator PressRoutine(Collider presser)
    {
        float t = 0f;
        while (t < pressDelay && _touchingCount > 0)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / pressDelay));
            transform.localPosition = _localStartPos - transform.forward * (pressDepth * p);
            yield return null;
        }

        _pressRoutine = null;

        if (_touchingCount > 0 && !_isPressed)
        {
            _isPressed = true;
            onPressed?.Invoke();
        }
    }

    System.Collections.IEnumerator ReleaseRoutine()
    {
        // Smoothly return to the start position
        while (Vector3.Distance(transform.localPosition, _localStartPos) > 0.0005f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _localStartPos, Time.deltaTime * releaseSpeed);
            yield return null;
        }
        transform.localPosition = _localStartPos;
        _releaseRoutine = null;
    }
}
