using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleAuthoring : MonoBehaviour
{
    public Transform[] target;
    private class Baker : Baker<VehicleAuthoring>
    {
        public override void Bake(VehicleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new VehicleData
            {
                MoveSpeed = Random.Range(5,10),
                RotateSpeed = 2f,
                TargetPosition = authoring.target[0].position
            });
            
            AddComponent(entity, new WaypointIndexData
            {
                Index = 0
            });

            var buffer = AddBuffer<WaypointBuffer>(entity);

            foreach (var point in authoring.target)
            {
                buffer.Add(new WaypointBuffer
                {
                    Destination = point.position
                });
            }
        }
    }
}
