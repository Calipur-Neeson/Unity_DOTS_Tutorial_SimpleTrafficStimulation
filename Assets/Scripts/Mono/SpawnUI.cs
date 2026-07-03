using TMPro;
using Unity.Entities;
using UnityEngine;

public class SpawnUI : MonoBehaviour
{
    public TMP_InputField inputText;
    public TMP_Text totalCountText;

    private EntityManager entityManager;
    private int totalCount = 0;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public void Spawn()
    {
        Entity spawner =
            entityManager.CreateEntityQuery(typeof(Spawner))
                .GetSingletonEntity();

        Spawner data =
            entityManager.GetComponentData<Spawner>(spawner);

        data.Count = int.Parse(inputText.text);
        totalCount += data.Count;
        totalCountText.text = totalCount.ToString();
        

        entityManager.SetComponentData(spawner, data);
    }
}
