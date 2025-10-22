using System.Collections;
using UnityEngine;

public class AnimScriptScale : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private Vector3 endScale;
    [SerializeField] private bool scaleDown = true;

    public bool endAnim { get; private set; }

    private Vector3 startScale;

    [ContextMenu("Scale")]
    public void Scale()
    {
        startScale = transform.localScale;
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
            transform.localScale = Vector3.LerpUnclamped(startScale, endScale, curveValue); 

            time += Time.deltaTime;
            yield return null; 
        }

        transform.localScale = endScale;
        if(scaleDown)
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
            transform.localScale = Vector3.LerpUnclamped(endScale,startScale , curveValue);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = startScale;
        endAnim = true;
        yield return null;
    }
}
