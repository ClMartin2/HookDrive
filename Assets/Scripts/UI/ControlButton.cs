using UnityEngine;
using UnityEngine.EventSystems;

public delegate void EventHandlerControlButton();

public class ControlButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed { get; private set; }

    public event EventHandlerControlButton onPointerDown;
    public event EventHandlerControlButton onPointerUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        onPointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        onPointerUp?.Invoke();
    }
}
