using System;
using TMPro;
using Unity.Entities;
using UnityEngine;

public class SpawnUI : MonoBehaviour
{
    public TMP_InputField inputText;
    public TMP_Text totalCountText;

    private EntityManager entityManager;

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

        data.RemainCount += int.Parse(inputText.text);
        entityManager.SetComponentData(spawner, data);
    }

    private void Update()
    {
        Entity spawner =
            entityManager.CreateEntityQuery(typeof(Spawner))
                .GetSingletonEntity();

        Spawner data =
            entityManager.GetComponentData<Spawner>(spawner);

        totalCountText.text =
            data.TotalCount.ToString();
    }
}
