using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (positionComponents is not null)
            {
                foreach (var item in positionComponents)
                {
                    PositionComponent positionComponent = (PositionComponent) item.Value;
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
