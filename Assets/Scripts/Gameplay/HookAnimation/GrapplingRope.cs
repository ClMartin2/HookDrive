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

    void LateUpdate()
    {
        DrawRope();
    }

    void DrawRope()
    {
        //If not grappling, don't draw rope
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

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        if (!retracted)
            grapplePoint = player.hookPoint;

        if (!retracted && finishAnim && !player.attachedToHook)
        {
            grapplePoint = player.hookStartPoint.position;
            retracted = true;
        }

        Vector3 gunTipPosition = player.hookStartPoint.position;
        Vector3 up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * lerpSpeed);
        Vector3 newPosition = Vector3.zero;

        for (var i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);

            newPosition = Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset;
            lr.SetPosition(i, newPosition);
        }

        finishAnim = Vector3.Distance(currentGrapplePosition, grapplePoint) <= 0.1f;
    }
}