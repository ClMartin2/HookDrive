using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextWobble : MonoBehaviour
{
    [SerializeField] private float amplitude = 5f;   // hauteur de la vague
    [SerializeField] private float frequency = 5f;   // vitesse du mouvement
    [SerializeField] private bool waveMode = true;   // si false, shake aléatoire

    private TMP_Text textMesh;
    private Mesh mesh;
    private Vector3[] vertices;

    private void Start()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            var charInfo = textMesh.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int index = charInfo.vertexIndex;
            Vector3 offset;

            if (waveMode)
            {
                // effet de vague
                float wave = Mathf.Sin(Time.time * frequency + i * 0.5f) * amplitude;
                offset = new Vector3(0, wave, 0);
            }
            else
            {
                // effet de tremblement
                offset = new Vector3(
                    Mathf.Sin(Time.time * frequency + i) * amplitude * 0.2f,
                    Mathf.Cos(Time.time * frequency + i) * amplitude * 0.2f,
                    0);
            }

            vertices[index + 0] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }
}
