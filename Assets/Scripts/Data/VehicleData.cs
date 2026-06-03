using Unity.Entities;
using Unity.Mathematics;

public struct VehicleData : IComponentData
{
    public float MaxSpeed;
    public float3 TargetPosition;
}
