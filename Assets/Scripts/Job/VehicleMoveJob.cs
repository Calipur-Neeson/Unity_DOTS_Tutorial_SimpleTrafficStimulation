using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct VehicleMoveJob : IJobEntity
{
    public float DeltaTime;
    
    public void Execute(ref LocalTransform transform,
        in VehicleData vehicle, in VehicleMoveData move)
    {
        if (move.IsWaiting)
            return;

        float3 currentPos = transform.Position;
        float3 targetPos = move.TargetPosition;

        float3 delta = targetPos - currentPos;

        if (math.lengthsq(delta) < 0.0001f)
            return;

        float3 direction = math.normalize(delta);

        quaternion targetRotation =
            quaternion.LookRotationSafe(direction, math.up());

        transform.Rotation =
            math.slerp(
                transform.Rotation,
                targetRotation,
                vehicle.RotateSpeed * DeltaTime);

        float moveDistance = move.CurrentMoveSpeed * DeltaTime;

        float remainDistance = math.length(delta);

        if (moveDistance >= remainDistance)
        {
            transform.Position = targetPos;
        }
        else
        {
            transform.Position += direction * moveDistance;
        }
    }
}
