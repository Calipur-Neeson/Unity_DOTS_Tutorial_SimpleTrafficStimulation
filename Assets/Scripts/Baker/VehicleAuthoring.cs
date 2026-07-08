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
                MoveSpeed = speed,
                RotateSpeed = 2f,
                Acceleration = 5f,
                Deceleration = 40f,
                DetectDistance = 5f
            });
            AddComponent(entity, new VehicleMoveData
            {
                CurrentMoveSpeed = speed,
            });
            AddComponent(entity, new VehicleLaneData());
        }
    }
}
