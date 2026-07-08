using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

    public partial struct VehicleMoveSystem: ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (
                         transform, 
                         vehicle,
                         vehicleLaneData,
                         vehicleMoveData,
                         me) 
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRO<VehicleData>, 
                         RefRW<VehicleLaneData>, 
                         RefRW<VehicleMoveData>>().WithEntityAccess())
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
                        
                        DynamicBuffer<VehicleBuffer> vehicles =
                            state.EntityManager.GetBuffer<VehicleBuffer>(laneEntity);
                        for (int i = 0; i < vehicles.Length; i++)
                        {
                            if (vehicles[i].VehicleInLane == me)
                            {
                                vehicles.RemoveAt(i);
                                break;
                            }
                        }
                        
                        DynamicBuffer<VehicleBuffer> newVehicles =
                            state.EntityManager.GetBuffer<VehicleBuffer>(nextLaneEntity);
                        newVehicles.Add(new VehicleBuffer{VehicleInLane = me});
                        
                        vehicleLaneData.ValueRW.CurrentLane = nextLaneEntity;
                        vehicleLaneData.ValueRW.CurrentIndex = 1;
                        continue;
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
