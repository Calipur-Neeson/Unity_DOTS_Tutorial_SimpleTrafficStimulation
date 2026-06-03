using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace System
{
    public partial struct VehicleMoveSystem: ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (transform, vehicle) 
                     in SystemAPI.Query<RefRW<LocalTransform>,RefRO<VehicleData>>())
            {
                float3 currentPos = transform.ValueRO.Position;
                float3 targetPos = vehicle.ValueRO.TargetPosition;

                float3 direction =
                    math.normalize(targetPos - currentPos);

                if (math.distance(currentPos, targetPos) > 0.1f)
                {
                    transform.ValueRW.Position +=
                        direction * vehicle.ValueRO.MaxSpeed * deltaTime;
                }
            }
        }
    }
}