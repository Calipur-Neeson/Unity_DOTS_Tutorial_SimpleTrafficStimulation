using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleAuthoring : MonoBehaviour
{
    private class Baker : Baker<VehicleAuthoring>
    {
        public override void Bake(VehicleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new VehicleData
            {
                MoveSpeed = Random.Range(10,20),
                RotateSpeed = 2f,
                Acceleration = 1f,
                Deceleration = 5f,
                DetectDistance = 10f
            });
            AddComponent(entity, new VehicleMoveData());
            AddComponent(entity, new VehicleLaneData());
        }
    }
}
