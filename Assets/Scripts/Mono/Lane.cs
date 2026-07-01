using UnityEngine;
using Unity.Entities;

public class Lane : MonoBehaviour
{
    [Header("Way points")]
    public Transform[] Waypoints;
    
    [Header("Next Lane")]
    public Lane NextLane;
    
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

        AddComponent<LaneData>(laneEntity);

        DynamicBuffer<WaypointBuffer> buffer =
            AddBuffer<WaypointBuffer>(laneEntity);

        foreach (var waypoint in authoring.Waypoints)
        {
            buffer.Add(new WaypointBuffer
            {
                Destination = waypoint.position
            });
        }

        if (authoring.NextLane != null)
        {
            Entity nextLaneEntity =
                GetEntity(authoring.NextLane,
                    TransformUsageFlags.None);

            SetComponent(laneEntity,
                new LaneData
                {
                    NextLane = nextLaneEntity
                });
        }
    }
}