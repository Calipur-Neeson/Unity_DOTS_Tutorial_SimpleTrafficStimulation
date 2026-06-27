using Unity.Entities;

public struct WaypointIndexData : IComponentData
{
    public int Index;
    public bool IsFinished;
}
