using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Config;

namespace Assets.Systems
{
    public class CircleLifetimeSystem : ISystem
    {
        const int MIN_SIZE_TO_STAY_ALIVE_WHEN_CLICKED = 4;

        private bool IsRepeatedSystem { get; set; }

        public CircleLifetimeSystem(bool isRepeatedSystem = false)
        {
            IsRepeatedSystem = isRepeatedSystem;

            if (!IsRepeatedSystem)
            {
                foreach (ShapeConfig shapeConfig in ECSController.Instance.Config.circleInstancesToSpawn)
                {
                    CircleUtils.CreateCircle(shapeConfig.initialPosition, shapeConfig.initialVelocity, shapeConfig.initialSize);
                }
            }
        }

        public string Name => "CircleLifetimeSystem";

        public void UpdateSystem()
        {
            if(!IsRepeatedSystem) VerifyClickedCircles();

            CleanUpDeadCircles();
        }

        void VerifyClickedCircles()
        {
            var clickedComponents = World.currentWorld.GetAllComponents<ClickedComponent>();
            if (clickedComponents is null) return;

            foreach (uint entityID in clickedComponents.Keys)
            {
                var sizeComponent = World.currentWorld.GetComponent<SizeComponent>(entityID);
                if (sizeComponent is null) continue;

                if (sizeComponent.size < MIN_SIZE_TO_STAY_ALIVE_WHEN_CLICKED)
                {
                    CircleUtils.DestroyCircle(entityID);
                }
            }
        }

        void CleanUpDeadCircles()
        {
            var dictionary = World.currentWorld.GetAllComponents<SizeComponent>();

            if (dictionary is null) return;

            foreach (var item in dictionary)
            {
                uint entityID = item.Key;
                SizeComponent sizeComponent = (SizeComponent)item.Value;

                if (sizeComponent is null || sizeComponent.size > 0) continue;

                CircleUtils.DestroyCircle(entityID);
            }
        }
    }
}
