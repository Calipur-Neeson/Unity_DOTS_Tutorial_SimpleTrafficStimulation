using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleAuthoring : MonoBehaviour
{
    public Vector2 SpeedRange = new Vector2(5f, 20f);
    public float RotateSpeed = 3f;
    public float Acceleration = 20f;
    public float Deceleration = 50f;
    public float SafeDistanceSq = 20f;
    private class Baker : Baker<VehicleAuthoring>
    {
        public override void Bake(VehicleAuthoring authoring)
        {
            float speed = Random.Range(authoring.SpeedRange.x, authoring.SpeedRange.y);
            
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new VehicleData
            {
                MaxSpeed = speed,
                RotateSpeed = authoring.RotateSpeed,
                Acceleration = authoring.Acceleration,
                Deceleration = authoring.Deceleration,
                DetectDistanceSq = authoring.SafeDistanceSq
            });
            AddComponent(entity, new VehicleMoveData
            {
                CurrentMoveSpeed = speed,
            });
            AddComponent(entity, new VehicleLaneData());
            AddComponent(entity, new VehicleFollowingData());
        }
    }
}
