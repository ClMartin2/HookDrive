using System.Collections;
using UnityEngine;

public class AnimScriptScale : MonoBehaviour
{
    [SerializeField] public float duration;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private Vector3 endScale;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private bool scaleDown = true;
    [SerializeField] private bool useStartScale;
    [SerializeField] private bool startOnEnable;

    public bool loop = false;
    public bool endAnim { get; private set; } = true;

    private Vector3 _startScale;
    private Coroutine coroutineScale = null;

    private void OnEnable()
    {
        if (!startOnEnable)
            return;

        Reset();
        Scale();
    }

    private void Awake()
    {
        SetStartScale();
    }

    [ContextMenu("Scale")]
    public void Scale()
    {
        SetStartScale();

        transform.localScale = _startScale;
        endAnim = false;
        coroutineScale = StartCoroutine(_Scale());
    }

    public void Reset()
    {
        if (coroutineScale != null)
            StopCoroutine(coroutineScale);

        transform.localScale = _startScale;
        endAnim = true;
    }

    private void SetStartScale()
    {
        if (!useStartScale)
            _startScale = transform.localScale;
        else
            _startScale = startScale;
    }

    private IEnumerator _Scale()
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float curveValue = animCurve.Evaluate(t);
            transform.localScale = Vector3.LerpUnclamped(_startScale, endScale, curveValue);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;

        if (scaleDown)
            coroutineScale = StartCoroutine(ScaleDown());
        else
            endAnim = true;

        yield return null;
    }

    private IEnumerator ScaleDown()
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float curveValue = animCurve.Evaluate(t);
            transform.localScale = Vector3.LerpUnclamped(endScale, _startScale, curveValue);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = _startScale;

        if (loop)
            Scale();
        else
            endAnim = true;

        yield return null;
    }
}
