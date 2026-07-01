using Unity.Entities;

public struct VehicleData : IComponentData
{
    public float MoveSpeed;
    public float RotateSpeed;
    
    public Entity CurrentLane;
    public int CurrentIndex;
}
