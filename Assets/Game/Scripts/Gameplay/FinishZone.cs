using System.Collections;
using UnityEngine;

public class FinishZone : MonoBehaviour
{
    [SerializeField] private Transform asset;
    [SerializeField] private AnimationCurve curveAnimAsset;
    [SerializeField] private float durationAnimAsset = 0.5f;
    [SerializeField] private Vector3 endScale;
    [SerializeField] private bool sas;

    public bool levelFinished { get; private set; } = false;

    private float counterAnim;
    private Vector3 startScale;

    private void Awake()
    {
        startScale = asset.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        levelFinished = true;
        counterAnim = 0;
        StartCoroutine(DoAnimAsset());
        SoundManager.Instance.PlaySoundSFX(SoundManager.WinLevel, 0.5f);
        GameEvents.EndScene?.Invoke(sas);
    }

    private IEnumerator DoAnimAsset()
    {
        while (counterAnim < durationAnimAsset)
        {
            counterAnim += Time.deltaTime;
            asset.localScale = Vector3.LerpUnclamped(startScale, endScale, curveAnimAsset.Evaluate(counterAnim / durationAnimAsset));
            yield return null;
        }

        yield return null;
    }
}
