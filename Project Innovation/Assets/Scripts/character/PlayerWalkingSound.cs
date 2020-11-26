using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingSound : WalkingSound
{
    [SerializeField] private float minWaterDist;
    [SerializeField] private float maxWaterDist;
    [SerializeField] private AnimationCurve waterFalloff = AnimationCurve.Constant(0f, 1f, 1f);
    [SerializeField] private Transform waterSource;
    
    protected override void ApplyParameters(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.setParameterByName("wet", SampleParamVal());
    }

    private float SampleParamVal()
    {
        if (!waterSource) return 0f;
        var dist = Vector3.Distance(transform.position, waterSource.position);
        if (dist < minWaterDist) return 1f;
        if (dist > maxWaterDist) return 0f;
        var val = dist - minWaterDist;
        val /= maxWaterDist - minWaterDist;
        return Mathf.Clamp01(waterFalloff.Evaluate(Mathf.Clamp01(val)));
    }
}
