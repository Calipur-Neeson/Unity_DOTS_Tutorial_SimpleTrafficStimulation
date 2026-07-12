using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct VehicleSpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        RefRW<Spawner> spawner = SystemAPI.GetSingletonRW<Spawner>();

        if (spawner.ValueRO.RemainCount == 0)
            return;

        // Get All lanes
        EntityQuery laneQuery =
            state.EntityManager.CreateEntityQuery(typeof(LaneData));
        NativeArray<Entity> lanes = laneQuery.ToEntityArray(Allocator.Temp);
        
        int spawnCount = math.min(spawner.ValueRO.RemainCount, spawner.ValueRO.SpawnPerFrame);
        for (int i = 0; i < spawnCount; i++)
        {
            Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(0, int.MaxValue));
            Entity lane = lanes[random.NextInt(lanes.Length)];
            
            Entity vehicle = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
            
            RefRW<LaneData> laneData = SystemAPI.GetComponentRW<LaneData>(lane);
            if (laneData.ValueRO.RecentVehicle != Entity.Null)
            {
                RefRW<VehicleFollowingData> followingData =
                    SystemAPI.GetComponentRW<VehicleFollowingData>(vehicle);
                followingData.ValueRW.CurrentFollowing = laneData.ValueRO.RecentVehicle;
            }
            laneData.ValueRW.RecentVehicle = vehicle;
            
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

            VehicleLaneData data =
                state.EntityManager.GetComponentData<VehicleLaneData>(vehicle);

            data.CurrentLane = lane;
            data.CurrentIndex = 1;

            state.EntityManager.SetComponentData(
                vehicle,
                data);
            
        }
        spawner.ValueRW.RemainCount -= spawnCount;
        spawner.ValueRW.TotalCount += spawnCount;
    }

}
