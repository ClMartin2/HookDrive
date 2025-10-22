using System.Collections;
using UnityEngine;

public class AnimScriptScale : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private Vector3 endScale;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private bool scaleDown = true;
    [SerializeField] private bool useStartScale;

    public bool endAnim { get; private set; }

    private Vector3 _startScale;

    [ContextMenu("Scale")]
    public void Scale()
    {
        if (!useStartScale)
            _startScale = transform.localScale;
        else
            _startScale = startScale;

        transform.localScale = _startScale;
        endAnim = false;
        StartCoroutine(_Scale());
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
            StartCoroutine(ScaleDown());
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
        endAnim = true;
        yield return null;
    }
}
