using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Systems
{
    public class ExplosionSystem : ISystem
    {
        const int EXPLOSION_DIVIDER = 4;
        const float CHILD_CELL_OFFSET = 1f;
        const int CELL_MIN_SIZE = 1;
        const int MIN_SIZE_TO_EXPLODE_WHEN_CLICKED = 4;
        public string Name { get { return "ExplosionSystem"; } }

        public void UpdateSystem()
        {
            VerifyCirclesClicked();
            CheckForCirclesThatShouldExplode();
        }

        void VerifyCirclesClicked()
        {
            var clickedComponents = World.currentWorld.GetAllComponents<ClickedComponent>();
            if (clickedComponents is null) return;

            foreach (uint entityID in clickedComponents.Keys)
            {
                Debug.Log("Clicked!");
                SizeComponent sizeComponent = World.currentWorld.GetComponent<SizeComponent>(entityID);
                if (sizeComponent is null || sizeComponent.size < MIN_SIZE_TO_EXPLODE_WHEN_CLICKED) continue;
                Debug.Log("Went through");
                Explode(entityID, sizeComponent.size);
            }
        }

        void CheckForCirclesThatShouldExplode()
        {
            var sizeComponents = World.currentWorld.GetAllComponents<SizeComponent>();

            foreach (var item in sizeComponents)
            {
                uint entity = item.Key;
                SizeComponent sizeComponent = (SizeComponent)item.Value;

                if (sizeComponent.size == ECSController.Instance.Config.explosionSize) Explode(entity, sizeComponent.size);
            }
        }

        void Explode(uint entity, int sizeAtExplosion)
        {
            int childrenSize = sizeAtExplosion / 4;
            if (childrenSize < CELL_MIN_SIZE) childrenSize = CELL_MIN_SIZE;

            Vector2 parentPosition = World.currentWorld.GetComponent<PositionComponent>(entity).position;
            float parentSpeed = World.currentWorld.GetComponent<VelocityComponent>(entity).velocity.magnitude;

            CircleUtils.DestroyCircle(entity);

            float angleIncrement = 2 * Mathf.PI / EXPLOSION_DIVIDER;
            float angleOffset = angleIncrement / 2.0f;

            for (int i = 0; i < EXPLOSION_DIVIDER; i++)
            {
                float angle = i * angleIncrement + angleOffset;

                Vector2 childDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                Debug.Log($"Creating a new child {i + 1} at {parentPosition + childDirection * CHILD_CELL_OFFSET} toward {childDirection * parentSpeed}");
                
                CircleUtils.CreateCircle(
                    parentPosition + childDirection * CHILD_CELL_OFFSET,
                    childDirection * parentSpeed,
                    childrenSize
                );
            }
        }
    }
}
