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
                SizeComponent sizeComponent = World.currentWorld.GetComponent<SizeComponent>(item.Key);
                ProtectedComponent protectedComponent = World.currentWorld.GetComponent<ProtectedComponent>(item.Key);

                if (Math.Abs(velocityComponent.velocity.magnitude) <= float.Epsilon)
                {
                    // Static circle
                    colorComponent.color = Color.red;
                }
                else
                {
                    if (World.currentWorld.GetComponent<CreatedTagComponent>(item.Key) != null)
                    {
                        // If it has exploded
                        colorComponent.color = new Color(1, 0.1f, 0.4f);
                        World.currentWorld.RemoveComponent<CreatedTagComponent>(item.Key);
                        continue;
                    }

                    if (World.currentWorld.GetComponent<CollidedTagComponent>(item.Key) != null)
                    {
                        // If it has collided
                        colorComponent.color = Color.green;
                        World.currentWorld.RemoveComponent<CollidedTagComponent>(item.Key);
                        continue;
                    }

                    if (protectedComponent.duration > 0)
                    {
                        colorComponent.color = new Color(1, 1, 1);
                        continue;
                    }

                    if (protectedComponent.cooldown > 0)
                    {
                        colorComponent.color = new Color(1, 1, 0);
                        continue;
                    }

                    if (sizeComponent.size == ECSController.Instance.Config.explosionSize - 1)
                    {
                        colorComponent.color = new Color(1, 0.5f, 0);
                        continue;
                    }

                    if (sizeComponent.size <= ECSController.Instance.Config.protectionSize)
                    {
                        colorComponent.color = new Color(0.3f, 0.3f, 1);
                        continue;
                    }  

                    colorComponent.color = new Color(0, 0, 0.25f);
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
