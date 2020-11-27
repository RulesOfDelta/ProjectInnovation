using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

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
        GameObject closestPuddle = null;
        float shortestDistance = float.PositiveInfinity;
        for (int i = 0; i < puddles.Length; i++)
        {
            var dist = Vector3.Distance(transform.position, puddles[i].transform.position);
            if (closestPuddle == null || dist < shortestDistance)
            {
                closestPuddle = puddles[i];
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