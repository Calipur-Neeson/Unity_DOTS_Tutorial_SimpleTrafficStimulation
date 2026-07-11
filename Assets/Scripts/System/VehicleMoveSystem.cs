using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

    public partial struct VehicleMoveSystem: ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (
                         transform, 
                         vehicle,
                         vehicleLaneData,
                         vehicleMoveData,
                         vehicleFollowingData,
                         me) 
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRO<VehicleData>, 
                         RefRW<VehicleLaneData>,
                         RefRW<VehicleMoveData>,
                         RefRW<VehicleFollowingData>>().WithEntityAccess())
            {
                Entity laneEntity = vehicleLaneData.ValueRO.CurrentLane;
                RefRW<LaneData> laneData = SystemAPI.GetComponentRW<LaneData>(laneEntity);
                
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

                vehicleMoveData.ValueRW.TargetPosition = waypoints[vehicleLaneData.ValueRO.CurrentIndex].Destination;
                float3 targetPos = vehicleMoveData.ValueRO.TargetPosition;
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
                        RefRW<LaneData> nextLaneData = SystemAPI.GetComponentRW<LaneData>(nextLaneEntity);
                    
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

                        if (nextLaneData.ValueRO.RecentVehicle != Entity.Null)
                        {
                            vehicleFollowingData.ValueRW.CurrentFollowing = nextLaneData.ValueRO.RecentVehicle;
                        }
                        nextLaneData.ValueRW.RecentVehicle = me;
                        
                        vehicleLaneData.ValueRW.CurrentLane = nextLaneEntity;
                        vehicleLaneData.ValueRW.CurrentIndex = 1;
                        continue;
                    }
                }
                
                // Move logic
                // float3 delta = targetPos - currentPos;
                // if (math.lengthsq(delta) > 0.0001f)
                // {
                //     float3 direction = math.normalize(delta);
                //
                //     quaternion targetRotation =
                //         quaternion.LookRotationSafe(direction, math.up());
                //
                //     transform.ValueRW.Rotation =
                //         math.slerp(
                //             transform.ValueRO.Rotation,
                //             targetRotation,
                //             vehicle.ValueRO.RotateSpeed * deltaTime);
                //
                //     float moveDistance = vehicleMoveData.ValueRO.CurrentMoveSpeed * deltaTime;
                //     float remainDistance = math.length(delta);
                //
                //     if (moveDistance >= remainDistance)
                //     {
                //         transform.ValueRW.Position = targetPos;
                //     }
                //     else
                //     {
                //         transform.ValueRW.Position += direction * moveDistance;
                //     }
                // }
            }
            VehicleMoveJob moveJob = new VehicleMoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            };
            moveJob.ScheduleParallel();
        }
    }
