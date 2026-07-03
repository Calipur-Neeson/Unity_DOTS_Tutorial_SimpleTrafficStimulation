using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleAuthoring : MonoBehaviour
{
    public Lane startLane;
    private class Baker : Baker<VehicleAuthoring>
    {
        public override void Bake(VehicleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new VehicleData
            {
                MoveSpeed = Random.Range(5,10),
                RotateSpeed = 2f,
                CurrentLane = Entity.Null,

                CurrentIndex = 1
            });
        }
    }
}
