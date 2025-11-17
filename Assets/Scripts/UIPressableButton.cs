using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Pressable behavior for Unity UI Buttons (RectTransform).
/// Attach to the same GameObject as a Button to get animated press/release callbacks
/// that can be invoked from a pointer (e.g. UIPointerCursor).
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UIPressableButton : MonoBehaviour
{
    [Tooltip("Scale multiplier when pressed (0.9 = 90% scale).")]
    [Range(0.6f, 1f)]
    public float pressedScale = 0.95f;

    [Tooltip("Speed of press/release animation.")]
    public float animSpeed = 10f;

    [Tooltip("Invoke the Button.onClick when the press is released (default). If unset, onPressed will be used.)")]
    public bool invokeOnRelease = true;

    public UnityEvent onPressed;
    public UnityEvent onReleased;

    RectTransform _rt;
    Vector3 _startScale;
    Coroutine _animRoutine;
    Button _button;

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _startScale = _rt.localScale;
        _button = GetComponent<Button>();
    }

    public void BeginPress()
    {
        if (_animRoutine != null) StopCoroutine(_animRoutine);
        _animRoutine = StartCoroutine(AnimateScale(_startScale * pressedScale));
        onPressed?.Invoke();
    }

    public void EndPress()
    {
        if (_animRoutine != null) StopCoroutine(_animRoutine);
        _animRoutine = StartCoroutine(AnimateScale(_startScale));
        onReleased?.Invoke();

        if (invokeOnRelease && _button != null)
        {
            _button.onClick.Invoke();
        }
    }

    System.Collections.IEnumerator AnimateScale(Vector3 target)
    {
        while (Vector3.Distance(_rt.localScale, target) > 0.001f)
        {
            _rt.localScale = Vector3.Lerp(_rt.localScale, target, Time.deltaTime * animSpeed);
            yield return null;
        }
        _rt.localScale = target;
        _animRoutine = null;
    }
}
