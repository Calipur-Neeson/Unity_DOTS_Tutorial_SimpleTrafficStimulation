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
                         vehicle,
                         vehicleLaneData,
                         vehicleMoveData) 
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRO<VehicleData>, 
                         RefRW<VehicleLaneData>, 
                         RefRW<VehicleMoveData>>())
            {
                Entity laneEntity = vehicleLaneData.ValueRO.CurrentLane;
                RefRO<LaneData> laneData = SystemAPI.GetComponentRO<LaneData>(laneEntity);
                
                if (vehicleMoveData.ValueRO.IsWaiting)
                {
                    Entity nextLaneEntity = laneData.ValueRO.NextLane;
                    RefRO<LaneData> nextLaneData = SystemAPI.GetComponentRO<LaneData>(nextLaneEntity);
                    
                    if (nextLaneData.ValueRO.TrafficLight != Entity.Null)
                    {
                        var light =
                            SystemAPI.GetComponentRO<TrafficLightTypeData>(
                                nextLaneData.ValueRO.TrafficLight);
                        
                        if (light.ValueRO.Type == TrafficLightType.Green)
                        {
                            vehicleMoveData.ValueRW.IsWaiting = false;
                            vehicleLaneData.ValueRW.CurrentLane = nextLaneEntity;
                            vehicleLaneData.ValueRW.CurrentIndex = 1;
                        }
                        continue;
                    }
                }
                
                DynamicBuffer<WaypointBuffer> waypoints =
                    SystemAPI.GetBuffer<WaypointBuffer>(laneEntity);
                    
                float3 targetPos = waypoints[vehicleLaneData.ValueRO.CurrentIndex].Destination;
                float3 currentPos = transform.ValueRO.Position;
                
                if (math.distance(currentPos, targetPos) < 0.1f)
                {
                    int nextIndx = vehicleLaneData.ValueRW.CurrentIndex + 1;
                    if (nextIndx < waypoints.Length)
                    {
                        vehicleLaneData.ValueRW.CurrentIndex = nextIndx;
                    }
                    else
                    {
                        if (laneData.ValueRO.NextLane == Entity.Null)
                            continue;
                         
                        Entity nextLaneEntity = laneData.ValueRO.NextLane;
                        RefRO<LaneData> nextLaneData = SystemAPI.GetComponentRO<LaneData>(nextLaneEntity);
                    
                        if (nextLaneData.ValueRO.TrafficLight != Entity.Null)
                        {
                            var light =
                                SystemAPI.GetComponentRO<TrafficLightTypeData>(
                                    nextLaneData.ValueRO.TrafficLight);

                            if (light.ValueRO.Type == TrafficLightType.Red)
                            {
                                vehicleMoveData.ValueRW.IsWaiting = true;
                                continue;
                            }
                        }
                        
                        vehicleLaneData.ValueRW.CurrentLane = nextLaneEntity;
                        vehicleLaneData.ValueRW.CurrentIndex = 1;
                    }
                }
                
                // Move logic
                float3 delta = targetPos - currentPos;
                if (math.lengthsq(delta) > 0.0001f)
                {
                    float3 direction = math.normalize(delta);

                    quaternion targetRotation =
                        quaternion.LookRotationSafe(direction, math.up());

                    transform.ValueRW.Rotation =
                        math.slerp(
                            transform.ValueRO.Rotation,
                            targetRotation,
                            vehicle.ValueRO.RotateSpeed * deltaTime);
                    
                    float moveDistance = vehicleMoveData.ValueRO.CurrentMoveSpeed * deltaTime;
                    float remainDistance = math.length(delta);

                    if (moveDistance >= remainDistance)
                    {
                        transform.ValueRW.Position = targetPos;
                    }
                    else
                    {
                        transform.ValueRW.Position += direction * moveDistance;
                    }
                }
            }
        }
    }
