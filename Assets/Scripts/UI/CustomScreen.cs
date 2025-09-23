using UnityEngine;

public class CustomScreen : MonoBehaviour
{
    virtual public void Show()
    {
        gameObject.SetActive(true);
    }

    virtual public void Hide()
    {
        gameObject.SetActive(false);
    }
}
