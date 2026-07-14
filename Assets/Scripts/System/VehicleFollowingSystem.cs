using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct VehicleFollowingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        
        VehicleFollowingJob followingJob = new VehicleFollowingJob
        {
            DeltaTime = deltaTime,
            TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true)
        };

        followingJob.ScheduleParallel();

        //  foreach (var (vehicle, 
        //               transform,
        //               vehicleData,
        //               moveData)
        //           in SystemAPI.Query<
        //               RefRO<VehicleFollowingData>, 
        //               RefRO<LocalTransform>, 
        //               RefRO<VehicleData>, 
        //               RefRW<VehicleMoveData>>())
        //  {
        //      
        //      if (vehicle.ValueRO.CurrentFollowing == Entity.Null)
        //          continue;
        //      
        //
        //      float3 frontPos = SystemAPI.GetComponentRO<LocalTransform>(vehicle.ValueRO.CurrentFollowing).ValueRO.Position;
        //      float3 backPos = transform.ValueRO.Position;
        //
        //      float distanceSq = math.lengthsq(frontPos - backPos);
        //      float safeDistanceSq = vehicleData.ValueRO.DetectDistanceSq;
        //      
        //      if (distanceSq < safeDistanceSq)
        //      {
        //          moveData.ValueRW.CurrentMoveSpeed -=
        //              vehicleData.ValueRO.Deceleration * deltaTime;
        //      }
        //      else
        //      {
        //          moveData.ValueRW.CurrentMoveSpeed +=
        //              vehicleData.ValueRO.Acceleration * deltaTime;
        //      }
        //
        //      moveData.ValueRW.CurrentMoveSpeed = math.clamp(moveData.ValueRW.CurrentMoveSpeed,
        //          0,
        //          vehicleData.ValueRO.MaxSpeed);
        //      
        // }
    }
    
}
