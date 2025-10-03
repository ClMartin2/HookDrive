using UnityEngine;
using UnityEngine.EventSystems;

public delegate void EventHandlerControlButton();

public class ControlButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed { get; private set; }

    public event EventHandlerControlButton onPointerDown;
    public event EventHandlerControlButton onPointerUp;

    private AnimScriptScale animScriptScale;

    private void Awake()
    {
        animScriptScale = GetComponent<AnimScriptScale>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        onPointerDown?.Invoke();
        animScriptScale.Scale();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        onPointerUp?.Invoke();
    }
}
