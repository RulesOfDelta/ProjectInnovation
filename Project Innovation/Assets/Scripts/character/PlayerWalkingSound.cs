using UnityEngine;

public class PlayerWalkingSound : WalkingSound
{
    [SerializeField] private float minWaterDist;
    [SerializeField] private float maxWaterDist;
    [SerializeField] private AnimationCurve waterFalloff = AnimationCurve.Constant(0f, 1f, 1f);

    protected override void ApplyParameters(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.setParameterByName("wet", SampleParamVal());
    }

    private float SampleParamVal()
    {
        GameObject closestPuddle = null;
        float shortestDistance = float.PositiveInfinity;
        foreach (var puddle in puddles)
        {
            var dist = Vector3.Distance(transform.position, puddle.transform.position);
            if (!closestPuddle || dist < shortestDistance)
            {
                closestPuddle = puddle;
                shortestDistance = dist;
            }
        }

        if (shortestDistance < minWaterDist) return 1f;
        if (shortestDistance > maxWaterDist) return 0f;
        var val = shortestDistance - minWaterDist;
        val /= maxWaterDist - minWaterDist;
        //val can't go below 0 or over 1
        return Mathf.Clamp01(waterFalloff.Evaluate(val));
    }
}