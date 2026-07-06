using Unity.Entities;

public struct TrafficLightData : IComponentData
{
    public float RedDuration;
    public float GreenDuration;
}

public struct TrafficLightTypeData : IComponentData
{
    public TrafficLightType Type;
    
    public float Timer;
}
public enum TrafficLightType : byte
{
    Red,
    Green
}