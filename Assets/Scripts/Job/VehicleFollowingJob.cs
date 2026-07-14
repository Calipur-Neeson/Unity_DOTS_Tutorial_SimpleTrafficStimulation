using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct VehicleFollowingJob : IJobEntity
{
    public float DeltaTime;
    [ReadOnly]
    public ComponentLookup<LocalTransform> TransformLookup;
    public void Execute(in VehicleData vehicle,
        ref VehicleMoveData moveData,
        in VehicleFollowingData followingData,
        in LocalTransform transform)
    {
        if (followingData.CurrentFollowing == Entity.Null)
            return;


        float3 frontPos = TransformLookup[followingData.CurrentFollowing].Position;
        float3 backPos = transform.Position;
    
        float distanceSq = math.lengthsq(frontPos - backPos);
        float safeDistanceSq = vehicle.DetectDistanceSq;
             
        if (distanceSq < safeDistanceSq)
        {
            moveData.CurrentMoveSpeed -= vehicle.Deceleration * DeltaTime;
        }
        else
        {
            moveData.CurrentMoveSpeed += vehicle.Acceleration * DeltaTime;
        }
    
        moveData.CurrentMoveSpeed = math.clamp(moveData.CurrentMoveSpeed,
            0,
            vehicle.MaxSpeed);

    }
}
