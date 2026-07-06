using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

    public partial struct VehicleMoveSystem: ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (
                         transform, 
                         vehicle) 
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRW<VehicleData>>())
            {
                Entity laneEntity = vehicle.ValueRO.CurrentLane;
                RefRO<LaneData> laneData = SystemAPI.GetComponentRO<LaneData>(laneEntity);
                DynamicBuffer<WaypointBuffer> waypoints =
                    SystemAPI.GetBuffer<WaypointBuffer>(laneEntity);
                
                float3 targetPos = waypoints[vehicle.ValueRO.CurrentIndex].Destination;
                float3 currentPos = transform.ValueRO.Position;

                
                float3 direction = math.normalize(targetPos - currentPos);
                quaternion q = quaternion.LookRotationSafe(direction, math.up());
                
                transform.ValueRW.Position +=
                    direction * vehicle.ValueRO.MoveSpeed * deltaTime;
                transform.ValueRW.Rotation =
                    Quaternion.Lerp(
                        transform.ValueRW.Rotation,
                        q,
                        vehicle.ValueRO.RotateSpeed * deltaTime);
                
                if (math.distance(currentPos, targetPos) < 0.1f)
                {
                    vehicle.ValueRW.CurrentIndex++;
                    if (vehicle.ValueRO.CurrentIndex >= waypoints.Length)
                    {
                        if (laneData.ValueRO.NextLane == Entity.Null)
                            return;
                                                                       
                        vehicle.ValueRW.CurrentLane = laneData.ValueRO.NextLane;
                        vehicle.ValueRW.CurrentIndex = 1;
                    }
                }
            }
        }
    }
