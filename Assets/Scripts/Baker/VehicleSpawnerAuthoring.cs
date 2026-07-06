using Unity.Entities;
using UnityEngine;

public class VehicleSpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
}


class SpawnerBaker : Baker<VehicleSpawnerAuthoring>
{
    public override void Bake(VehicleSpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Spawner
        {
            Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            SpawnPerFrame = 10
        });
        
    }
}
public struct Spawner : IComponentData
{
    public Entity Prefab;
    public int TotalCount;
    public int RemainCount;
    public int SpawnPerFrame;
}