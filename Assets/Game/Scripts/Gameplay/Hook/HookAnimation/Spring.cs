using UnityEngine;

public class Spring
{
    private float strength;
    private float damper;
    private float target;
    private float velocity;
    private float value;

    // Nombre maximum de sous-steps par frame (sécurité FPS bas)
    private const int MAX_STEPS = 8;
    private const float FIXED_STEP = 1f / 60f; // ~120 Hz

    public void Update(float deltaTime)
    {
        // Découper le deltaTime en petits steps fixes pour la stabilité
        int iterations = Mathf.Clamp(Mathf.CeilToInt(deltaTime / FIXED_STEP), 1, MAX_STEPS);
        float step = deltaTime / iterations;

        for (int i = 0; i < iterations; i++)
        {
            float diff = target - value;
            float accel = diff * strength;

            // intégration semi-implicite (plus stable que Euler explicite)
            velocity += accel * step;
            velocity *= Mathf.Clamp01(1f - damper * step); // amortissement
            value += velocity * step;
        }
    }

    public void Reset()
    {
        velocity = 0f;
        value = 0f;
    }

    public void SetValue(float value) => this.value = value;
    public void SetTarget(float target) => this.target = target;
    public void SetDamper(float damper) => this.damper = damper;
    public void SetStrength(float strength) => this.strength = strength;
    public void SetVelocity(float velocity) => this.velocity = velocity;

    public float Value => value;
}
