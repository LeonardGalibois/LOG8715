using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Systems
{
    public class ClickSystem : ISystem
    {
        public string Name { get { return "ClickSystem"; } }

        public void UpdateSystem()
        {
            RemoveLastFrameClickTags();

            if (Input.GetMouseButtonDown(0)) OnUserClick();
        }

        void RemoveLastFrameClickTags()
        {
            var components = World.currentWorld.GetAllComponents<ClickedComponent>();
            if (components is null) return;

            foreach (uint entityID in components.Keys)
            {
                World.currentWorld.RemoveComponent<ClickedComponent>(entityID);
            }
        }

        void OnUserClick()
        {
            var positionsComponents = World.currentWorld.GetAllComponents<PositionComponent>();
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            foreach (var item in positionsComponents)
            {
                uint entityID = item.Key;
                PositionComponent positionComponent = (PositionComponent)item.Value;
                SizeComponent sizeComponent = World.currentWorld.GetComponent<SizeComponent>(entityID);

                if (positionComponent is null || sizeComponent is null) continue;

                if (Vector2.Distance(positionComponent.position, clickPosition) <= sizeComponent.size)
                {
                    // Circle was clicked

                    World.currentWorld.AddComponent(entityID, new ClickedComponent());
                }
            }
        }
    }
}
