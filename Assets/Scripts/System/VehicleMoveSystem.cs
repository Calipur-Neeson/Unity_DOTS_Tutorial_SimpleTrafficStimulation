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
                
                if (vehicle.ValueRO.IsWaiting)
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
                            vehicle.ValueRW.IsWaiting = false;
                            vehicle.ValueRW.CurrentLane = nextLaneEntity;
                            vehicle.ValueRW.CurrentIndex = 1;
                        }
                        continue;
                    }
                }
                
                DynamicBuffer<WaypointBuffer> waypoints =
                    SystemAPI.GetBuffer<WaypointBuffer>(laneEntity);
                    
                float3 targetPos = waypoints[vehicle.ValueRO.CurrentIndex].Destination;
                float3 currentPos = transform.ValueRO.Position;
                
                if (math.distance(currentPos, targetPos) < 0.1f)
                {
                    int nextIndx = vehicle.ValueRW.CurrentIndex + 1;
                    if (nextIndx < waypoints.Length)
                    {
                        vehicle.ValueRW.CurrentIndex = nextIndx;
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
                                vehicle.ValueRW.IsWaiting = true;
                                continue;
                            }
                        }
                        
                        vehicle.ValueRW.CurrentLane = nextLaneEntity;
                        vehicle.ValueRW.CurrentIndex = 1;
                    }
                }
                
                float3 direction = math.normalize(targetPos - currentPos);
                quaternion q = quaternion.LookRotationSafe(direction, math.up());

                transform.ValueRW.Position += 
                    direction * vehicle.ValueRO.MoveSpeed * deltaTime;
                transform.ValueRW.Rotation = Quaternion.Lerp(
                        transform.ValueRW.Rotation,
                        q,
                        vehicle.ValueRO.RotateSpeed * deltaTime);
            }
        }
    }
