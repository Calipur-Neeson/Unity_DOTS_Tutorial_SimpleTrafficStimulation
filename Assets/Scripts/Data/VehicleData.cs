using Unity.Entities;
using Unity.Mathematics;

public struct VehicleData : IComponentData
{
    public float MoveSpeed;
    public float RotateSpeed;
    public float3 TargetPosition;
}
