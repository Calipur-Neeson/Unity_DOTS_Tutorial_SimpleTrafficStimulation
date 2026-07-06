using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public partial struct TraficLightSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (
                     lightData,
                     type,
                     color)
                 in SystemAPI.Query<
                     RefRO<TrafficLightData>,
                     RefRW<TrafficLightTypeData>,
                    RefRW<URPMaterialPropertyBaseColor>>())
        {
            type.ValueRW.Timer += deltaTime;
            if (type.ValueRO.Type == TrafficLightType.Red &&
                type.ValueRO.Timer >= lightData.ValueRO.RedDuration)
            {
                type.ValueRW.Type = TrafficLightType.Green;
                color.ValueRW.Value = new float4(0,1,0,1);
                type.ValueRW.Timer = 0;
            }

            if (type.ValueRO.Type == TrafficLightType.Green &&
                type.ValueRO.Timer >= lightData.ValueRO.GreenDuration)
            {
                type.ValueRW.Type = TrafficLightType.Red;
                color.ValueRW.Value = new float4(1,0,0,1);
                type.ValueRW.Timer = 0;
            }
        }
    }
}
