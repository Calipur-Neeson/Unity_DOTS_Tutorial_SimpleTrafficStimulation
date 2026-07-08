using Unity.Entities;

public struct VehicleData : IComponentData
{
    public float MoveSpeed;
    public float RotateSpeed;
    public float Acceleration;
    public float Deceleration;
    public float DetectDistance;
}

public struct VehicleLaneData : IComponentData
{
    public Entity CurrentLane;
    public int CurrentIndex;
}

public struct VehicleMoveData : IComponentData
{
    public float CurrentMoveSpeed;
    public bool IsWaiting;
}
