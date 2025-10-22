using System.Collections;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private Transform fillImage;
    public AnimScriptScale animScriptScale { get; private set; }

    private void Awake()
    {
        animScriptScale = GetComponentInChildren<AnimScriptScale>();
    }

    public void Deactivate()
    {
        fillImage.gameObject.SetActive(false);

    }

    public void FillStar()
    {
        fillImage.localScale = Vector3.zero;
        fillImage.gameObject.SetActive(true);
        animScriptScale.Scale();
    }
}
