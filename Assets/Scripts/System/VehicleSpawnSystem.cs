using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct VehicleSpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        RefRW<Spawner> spawner = SystemAPI.GetSingletonRW<Spawner>();

        if (spawner.ValueRO.Count == 0)
            return;

        EntityQuery laneQuery =
            state.EntityManager.CreateEntityQuery(typeof(LaneData));

        NativeArray<Entity> lanes = laneQuery.ToEntityArray(Allocator.Temp);
        
        for (int i = 0; i < spawner.ValueRO.Count; i++)
        {
            Entity lane = lanes[i % lanes.Length];
            
            Entity vehicle = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
            
            // get Waypoint

            DynamicBuffer<WaypointBuffer> buffer =
                state.EntityManager.GetBuffer<WaypointBuffer>(lane);

            float3 spawnPos =
                buffer[0].Destination;
            
            // set Transform

            LocalTransform transform =
                LocalTransform.FromPosition(spawnPos);

            state.EntityManager.SetComponentData(
                vehicle,
                transform);
            
            // inital Vehicle

            VehicleData data =
                state.EntityManager.GetComponentData<VehicleData>(
                    vehicle);

            data.CurrentLane = lane;
            data.CurrentIndex = 1;

            state.EntityManager.SetComponentData(
                vehicle,
                data);
        }

        spawner.ValueRW.Count = 0;
          
    }

}
