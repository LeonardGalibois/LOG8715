using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Systems
{
    public class ProtectionSystem : ISystem
    {
        public string Name { get { return "ProtectionSystem"; } }

        void CheckProtectableEntities(
            Dictionary<uint, IEntityComponent> collisionComponents,
            Dictionary<uint, IEntityComponent> sizeComponents,
            Dictionary<uint, IEntityComponent> protectedComponents
            )
        {
            foreach (var item in collisionComponents)
            {
                CollisionComponent collisionComponent = (CollisionComponent)item.Value;
                SizeComponent sizeComponent = (SizeComponent)item.Value;
                ProtectedComponent protectedComponent = (ProtectedComponent)protectedComponents[item.Key];
                //ProtectedComponent protectedComponent = World.currentWorld.GetComponent<ProtectedComponent>(item.Key);

                if (sizeComponent.size <= ECSController.Instance.Config.protectionSize 
                  && collisionComponent.nbSameSizeCollisions ==  ECSController.Instance.Config.protectionCollisionCount
                  && protectedComponent.cooldown <= 0f)
                  {
                      protectedComponent.duration = 0f;
                      protectedComponent.duration += Time.deltaTime;
                  }
                
                if (protectedComponent.duration >= ECSController.Instance.Config.protectionDuration)
                {
                    protectedComponent.cooldown = 0f;
                    protectedComponent.cooldown += Time.deltaTime;
                }
        }   
    }

    public void UpdateSystem()
        {

        }
}
}
