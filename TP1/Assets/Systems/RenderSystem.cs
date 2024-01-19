using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Systems
{
    public class RenderSystem : ISystem
    {
        public string Name { get { return "ColorSystem"; } }

        public void UpdateSystem()
        {
            var positionComponents = World.currentWorld.GetAllComponents<PositionComponent>();
            var sizeComponents = World.currentWorld.GetAllComponents<SizeComponent>();
            var colorComponents = World.currentWorld.GetAllComponents<ColorComponent>();

            UpdateColor(colorComponents);

            Render(positionComponents, sizeComponents, colorComponents);
        }

        void UpdateColor(Dictionary<uint, IEntityComponent> colorComponents)
        {
            foreach (var item in colorComponents)
            {
                ColorComponent colorComponent = (ColorComponent)item.Value;
                CollisionComponent collisionComponent = World.currentWorld.GetComponent<CollisionComponent>(item.Key);
                VelocityComponent velocityComponent = World.currentWorld.GetComponent<VelocityComponent>(item.Key);

                if (Math.Abs(velocityComponent.velocity.magnitude) <= float.Epsilon)
                {
                    // Static circle
                    colorComponent.color = Color.red;
                }
                else if (collisionComponent.nbCollisions == 0)
                {
                    // No collisions yet
                    colorComponent.color = Color.blue;
                }
                else if (collisionComponent.nbCollisions == ECSController.Instance.Config.explosionSize - 1)
                {
                    // Will explode next collision
                    colorComponent.color = new Color(1f, 0.5f, 0f);
                }
                else
                {
                    // Has collided
                    colorComponent.color = Color.green;
                }
            }
        }

        void Render(
                Dictionary<uint, IEntityComponent> positionComponents,
                Dictionary<uint, IEntityComponent> sizeComponents,
                Dictionary<uint, IEntityComponent> colorComponents
            )
        {
            if (positionComponents is not null)
            {
                foreach (var item in positionComponents)
                {
                    PositionComponent positionComponent = (PositionComponent)item.Value;
                    ECSController.Instance.UpdateShapePosition(item.Key, positionComponent.position);
                }
            }

            if (sizeComponents is not null)
            {
                foreach (var item in sizeComponents)
                {
                    SizeComponent sizeComponent = (SizeComponent)item.Value;
                    ECSController.Instance.UpdateShapeSize(item.Key, sizeComponent.size);
                }
            }

            if (colorComponents is not null)
            {
                foreach (var item in colorComponents)
                {
                    ColorComponent colorComponent = (ColorComponent)item.Value;
                    ECSController.Instance.UpdateShapeColor(item.Key, colorComponent.color);
                }
            }
        }
    }
}
