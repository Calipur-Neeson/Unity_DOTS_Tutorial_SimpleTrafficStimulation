using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct VehicleFollowingSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        VehicleFollowingJob followingJob = new VehicleFollowingJob
        {
            DeltaTime = deltaTime,
            TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
            VehicleDataLookup = SystemAPI.GetComponentLookup<VehicleData>(true),
            VehicleMoveLookup = SystemAPI.GetComponentLookup<VehicleMoveData>()
        };
        followingJob.Schedule();


        // foreach (var (lane,
        //              entity) 
        //          in SystemAPI.Query<RefRW<LaneData>>().WithEntityAccess())
        // {
        //     DynamicBuffer<VehicleBuffer> vehicles =
        //         state.EntityManager.GetBuffer<VehicleBuffer>(entity);
        //     
        //     if (vehicles.Length < 2)
        //         continue;
        //     for (int i = 0; i < vehicles.Length - 1; i++)
        //     {
        //         Entity frontVehicle = vehicles[i].VehicleInLane;
        //         Entity backVehicle  = vehicles[i + 1].VehicleInLane;
        //
        //         float3 frontPos =
        //             SystemAPI.GetComponentRO<LocalTransform>(frontVehicle).ValueRO.Position;
        //
        //         float3 backPos =
        //             SystemAPI.GetComponentRO<LocalTransform>(backVehicle).ValueRO.Position;
        //
        //         float distanceSq = math.lengthsq(frontPos - backPos);
        //
        //         RefRO<VehicleData> vehicleData = 
        //             SystemAPI.GetComponentRO<VehicleData>(backVehicle);
        //         RefRW<VehicleMoveData> backVehicleMove = 
        //             SystemAPI.GetComponentRW<VehicleMoveData>(backVehicle);
        //         
        //         if (distanceSq < vehicleData.ValueRO.DetectDistanceSq)
        //         {
        //             backVehicleMove.ValueRW.CurrentMoveSpeed -=
        //                 vehicleData.ValueRO.Deceleration * deltaTime;
        //         }
        //         else
        //         {
        //             backVehicleMove.ValueRW.CurrentMoveSpeed +=
        //                 vehicleData.ValueRO.Acceleration * deltaTime;
        //         }
        //
        //         backVehicleMove.ValueRW.CurrentMoveSpeed = math.clamp(backVehicleMove.ValueRW.CurrentMoveSpeed,
        //             0,
        //             vehicleData.ValueRO.MaxSpeed);
        //     }
        //}
    }
    
}
