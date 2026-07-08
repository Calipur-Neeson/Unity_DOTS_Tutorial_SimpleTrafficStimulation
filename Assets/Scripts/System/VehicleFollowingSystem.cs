using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

partial struct VehicleFollowingSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (var (
                     transform, 
                     vehicle,
                     vehicleMove)
                 in SystemAPI.Query<
                     RefRO<LocalTransform>,
                     RefRO<VehicleData>,
                     RefRW<VehicleMoveData>>())
        {
            if (vehicleMove.ValueRO.IsWaiting)
                continue;
            
            float3 position = transform.ValueRO.Position;
            float3 forward = transform.ValueRO.Forward();
            
            RaycastInput input = new RaycastInput
            {
                Start = position + forward * 1,
                End = position + forward * vehicle.ValueRO.DetectDistance,
                Filter = new CollisionFilter
                {
                    BelongsTo = 1u << 0,
                    CollidesWith = 1u << 0,
                    GroupIndex = 0
                }
            };
            
            bool hit = physicsWorld.CastRay(input, out RaycastHit raycastHit);
            // Debug.DrawLine(
            //     input.Start,
            //     input.End,
            //     hit ? Color.red : Color.green);
            
            vehicleMove.ValueRW.CurrentMoveSpeed += hit? 
                vehicle.ValueRO.Deceleration * deltaTime * (-1) : 
                vehicle.ValueRO.Acceleration * deltaTime;

            vehicleMove.ValueRW.CurrentMoveSpeed = math.clamp(vehicleMove.ValueRO.CurrentMoveSpeed,
                0,
                vehicle.ValueRO.MoveSpeed);
        }
    }
    
}
