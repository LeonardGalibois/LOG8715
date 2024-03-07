using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class Ex4SpawnerAuthoring : MonoBehaviour
{
    public Ex4Config config;
    public GameObject PlantObject;
    public GameObject PreyObject;
    public GameObject PredatorObject;
    class Baker : Baker<Ex4SpawnerAuthoring>
    {
        public override void Bake(Ex4SpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            float size = authoring.config.gridSize;
            float ratio = Camera.main!.aspect;
            int height = (int)Math.Round(Math.Sqrt(size / ratio));
            int width = (int)Math.Round(size / height);

            AddComponent(entity, new SpawnerComp
            {
                plantPrefab = GetEntity(authoring.PlantObject, TransformUsageFlags.Dynamic),
                preyPrefab = GetEntity(authoring.PreyObject, TransformUsageFlags.Dynamic),
                predatorPrefab = GetEntity(authoring.PredatorObject, TransformUsageFlags.Dynamic),

                plantCount = authoring.config.plantCount,
                preyCount = authoring.config.preyCount,
                predatorCount = authoring.config.predatorCount,
                gridSize = authoring.config.gridSize,

                halfWidth = width / 2,
                halfHeight = height / 2
            });
        }
    }
}
