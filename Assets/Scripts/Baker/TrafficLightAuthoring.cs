using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class TrafficLightAuthoring : MonoBehaviour
{
    public TrafficLightType TrafficLightType;
    
    public float RedLightDuration;
    public float GreenLightDuration;
}

class TrafficLightAuthoringBaker : Baker<TrafficLightAuthoring>
{
    public override void Bake(TrafficLightAuthoring authoring)
    {
        float4 color = new float4(0, 0, 0, 1);

        switch (authoring.TrafficLightType)
        {
            case TrafficLightType.Green:
                color = new float4(0, 1, 0, 1);
                break;
            case TrafficLightType.Red:
                color = new float4(1, 0, 0, 1);
                break;
        }
        
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new TrafficLightData
        {
            RedDuration = authoring.RedLightDuration,
            GreenDuration = authoring.GreenLightDuration,
        });

        AddComponent(entity, new TrafficLightTypeData
        {
            Type = authoring.TrafficLightType,
            Timer = 0f
        });
        
        AddComponent(entity, new URPMaterialPropertyBaseColor
        {
            Value = color
        });
    }
}
