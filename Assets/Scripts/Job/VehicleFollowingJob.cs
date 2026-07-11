using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct VehicleFollowingJob : IJobEntity
{
    public float DeltaTime;
    
    [ReadOnly]
    public ComponentLookup<LocalTransform> TransformLookup;
    [ReadOnly]
    public ComponentLookup<VehicleData> VehicleDataLookup;
    
    public ComponentLookup<VehicleMoveData> VehicleMoveLookup;
    public void Execute(in DynamicBuffer<VehicleBuffer> vehicles)
    {
        if (vehicles.Length < 2 )
            return;
        
        for (int i = 0; i < vehicles.Length - 1; i++)
        {
            Entity frontVehicle = vehicles[i].VehicleInLane;
            Entity backVehicle  = vehicles[i + 1].VehicleInLane;

            float3 frontPos = TransformLookup[frontVehicle].Position;
            float3 backPos = TransformLookup[backVehicle].Position;
            float distanceSq = math.lengthsq(frontPos - backPos);
            
            VehicleData backVehicleData = VehicleDataLookup[backVehicle];
            VehicleMoveData backMoveData = VehicleMoveLookup[backVehicle];
            if (distanceSq < backVehicleData.DetectDistanceSq)
            {
                backMoveData.CurrentMoveSpeed -=
                    backVehicleData.Deceleration * DeltaTime;
            }
            else
            {
                backMoveData.CurrentMoveSpeed +=
                    backVehicleData.Acceleration * DeltaTime;
            }

            backMoveData.CurrentMoveSpeed = math.clamp(backMoveData.CurrentMoveSpeed,
                0,
                backVehicleData.MaxSpeed);
        }
    }
}
