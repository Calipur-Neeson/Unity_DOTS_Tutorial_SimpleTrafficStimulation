using UnityEngine;
using Unity.Entities;

public class Lane : MonoBehaviour
{
    [Header("Way points")]
    public Transform[] Waypoints;
    
    [Header("Next Lane")]
    public Lane NextLane;
    
    [Header("Traffic Light")]
    public TrafficLightAuthoring TrafficLight;
    
    private void OnValidate()
    {
        Waypoints = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            Waypoints[i] = transform.GetChild(i);
        }
    }
}

public class LaneBaker : Baker<Lane>
{
    public override void Bake(Lane authoring)
    {
        Entity laneEntity = GetEntity(TransformUsageFlags.None);

        var data = new LaneData();
        if (authoring.NextLane != null)
            data.NextLane = GetEntity(authoring.NextLane, TransformUsageFlags.None);
        if (authoring.TrafficLight != null)
            data.TrafficLight = GetEntity(authoring.TrafficLight, TransformUsageFlags.None);
        AddComponent(laneEntity, data);

        DynamicBuffer<WaypointBuffer> buffer =
            AddBuffer<WaypointBuffer>(laneEntity);

        foreach (var waypoint in authoring.Waypoints)
        {
            buffer.Add(new WaypointBuffer
            {
                Destination = waypoint.position
            });
        }
        
        DynamicBuffer<VehicleBuffer> vehicleBuffer =
            AddBuffer<VehicleBuffer>(laneEntity);
    }
}