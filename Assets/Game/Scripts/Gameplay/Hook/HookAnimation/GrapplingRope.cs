using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private int quality;
    [SerializeField] private float damper;
    [SerializeField] private float strength;
    [SerializeField] private float velocity;
    [SerializeField] private float waveCount;
    [SerializeField] private float waveHeight;
    [SerializeField] private AnimationCurve affectCurve;
    [SerializeField] private float lerpSpeed = 12f;

    private Spring spring;
    private LineRenderer lr;
    private Vector3 currentGrapplePosition;
    private bool retracted = false;
    private Vector3 grapplePoint;
    private bool finishAnim = false;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    void FixedUpdate()
    {
        // Update du ressort avec un pas fixe → stabilité FPS
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void DrawRope()
    {
        // Si pas de grappin actif → on reset tout
        if (!player.isGrappling)
        {
            currentGrapplePosition = player.hookStartPoint.position;
            grapplePoint = Vector3.zero;

            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;

            retracted = false;
            finishAnim = false;
            return;
        }
        else if (player.isGrappling && retracted && finishAnim)
        {
            currentGrapplePosition = player.hookStartPoint.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;
            return;
        }

        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }

        if (!retracted)
            grapplePoint = player.hookPointPosition;

        if (!retracted && finishAnim && !player.attachedToHook)
        {
            grapplePoint = player.hookStartPoint.position;
            retracted = true;
        }

        Vector3 gunTipPosition = player.hookStartPoint.position;
        Vector3 dir = (grapplePoint - gunTipPosition).normalized;
        Vector3 up = Quaternion.LookRotation(dir) * Vector3.up;

        // Exp-Lerp pour indépendance FPS
        float t = 1f - Mathf.Exp(-lerpSpeed * Time.deltaTime);
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, t);

        for (int i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;
            float wave = Mathf.Sin(delta * waveCount * Mathf.PI) * waveHeight;
            Vector3 offset = up * wave * spring.Value * affectCurve.Evaluate(delta);

            Vector3 ropePoint = Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset;
            lr.SetPosition(i, ropePoint);
        }

        finishAnim = Vector3.Distance(currentGrapplePosition, grapplePoint) <= 0.1f;
    }
}