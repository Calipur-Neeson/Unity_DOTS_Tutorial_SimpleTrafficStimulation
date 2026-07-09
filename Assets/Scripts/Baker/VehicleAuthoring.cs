using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleAuthoring : MonoBehaviour
{
    private class Baker : Baker<VehicleAuthoring>
    {
        public override void Bake(VehicleAuthoring authoring)
        {
            float speed = Random.Range(5, 20);
            
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new VehicleData
            {
                MaxSpeed = speed,
                RotateSpeed = 2f,
                Acceleration = 5f,
                Deceleration = 40f,
                DetectDistanceSq = 25f
            });
            AddComponent(entity, new VehicleMoveData
            {
                CurrentMoveSpeed = speed,
            });
            AddComponent(entity, new VehicleLaneData());
        }
    }
}
