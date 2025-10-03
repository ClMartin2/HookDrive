using System.Collections;
using UnityEngine;

public class AnimScriptScale : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private Vector3 endScale;

    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
    }

    public void Scale()
    {
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
        StartCoroutine(ScaleDown());
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
        yield return null;
    }
}
