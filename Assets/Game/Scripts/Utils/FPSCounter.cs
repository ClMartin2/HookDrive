using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    float deltaTime = 0.0f;

    void Update()
    {
        // Calcul du temps entre chaque frame pour obtenir le FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int fps = (int)(1.0f / deltaTime);
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = Color.white;

        // Position à droite
        float width = 100;
        float height = 30;
        float x = Screen.width - width - 100; // 10 pixels depuis le bord droit
        float y = 200; // 10 pixels depuis le haut
        GUI.Label(new Rect(x, y, width, height), fps + " FPS", style);
    }
}
