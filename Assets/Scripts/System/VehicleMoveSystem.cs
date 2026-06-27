using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace System
{
    public partial struct VehicleMoveSystem: ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (
                         transform, 
                         vehicle,
                         index,
                         waypoints) 
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRW<VehicleData>,
                         RefRW<WaypointIndexData>,
                         DynamicBuffer<WaypointBuffer>>())
            {
                float3 currentPos = transform.ValueRO.Position;
                float3 targetPos = vehicle.ValueRO.TargetPosition;
                
                if (math.distance(currentPos, targetPos) < 0.1f)
                {
                    index.ValueRW.Index++;

                    if (index.ValueRW.Index < waypoints.Length)
                    {
                        vehicle.ValueRW.TargetPosition = waypoints[index.ValueRW.Index].Destination;
                    }
                    else
                    {
                        index.ValueRW.IsFinished = true;
                    }
                }

                if (!index.ValueRW.IsFinished)
                {
                    float3 direction = math.normalize(targetPos - currentPos);
                    quaternion q = Quaternion.LookRotation(targetPos - currentPos);
                    
                    transform.ValueRW.Position +=
                        direction * vehicle.ValueRO.MoveSpeed * deltaTime;
                    transform.ValueRW.Rotation =
                        Quaternion.Lerp(
                            transform.ValueRW.Rotation,
                            q,
                            vehicle.ValueRO.RotateSpeed * deltaTime);
                }
            }
        }
    }
}