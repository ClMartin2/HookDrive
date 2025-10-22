using UnityEngine;

public class AnimateDiagonalGradient : MonoBehaviour
{
    public Material mat;
    public float duration = 2f;

    private float timer = 0f;
    private bool swap = false;

    void Update()
    {
        timer += Time.deltaTime;
        float fill = timer / duration;

        if (fill > 1f)
        {
            timer = 0f;
            swap = !swap;
        }

        mat.SetFloat("_Fill", fill);

        if (swap)
        {
            mat.SetColor("_ColorA", new Color(1f, 0.5f, 0f)); // orange
            mat.SetColor("_ColorB", new Color(1f, 1f, 0f));   // jaune
        }
        else
        {
            mat.SetColor("_ColorA", new Color(1f, 1f, 0f));   // jaune
            mat.SetColor("_ColorB", new Color(1f, 0.5f, 0f)); // orange
        }
    }
}
