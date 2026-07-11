using Unity.Entities;

public struct LaneData : IComponentData
{
    public Entity NextLane;
    public Entity TrafficLight;
    public Entity RecentVehicle;
}
