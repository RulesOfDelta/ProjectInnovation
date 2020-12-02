using System;
using UnityEngine;

public class PlayerWalkingSound : WalkingSound
{
    [SerializeField] private float minWaterDist;
    [SerializeField] private float maxWaterDist;
    [SerializeField] private AnimationCurve waterFalloff = AnimationCurve.Constant(0f, 1f, 1f);
    private GameObject[] puddles;
    private bool lellek = false;
    
    protected override void ApplyParameters(FMOD.Studio.EventInstance eventInstance)
    {
        eventInstance.setParameterByName("wet", SampleParamVal());
    }

    private float SampleParamVal()
    {
        if (puddles == null) return 0;
        
        GameObject closestPuddle = null;
        float shortestDistance = float.PositiveInfinity;
        foreach (var puddle in puddles)
        {
            if(!puddle) continue;

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
    public void FindPuddles()
    {
        lellek = true;
    }

    private void LateUpdate()
    {
        if (lellek)
        {
            puddles = GameObject.FindGameObjectsWithTag("Puddle");
            lellek = false;
        }
    }
}