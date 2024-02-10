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

        public void UpdateSystem()
        {
            CheckProtectableEntities();
        }

        void CheckProtectableEntities()
        {
            foreach(uint entity in World.currentWorld.entities)
            {   
                CollisionComponent collision = World.currentWorld.GetComponent<CollisionComponent>(entity);
                SizeComponent size = World.currentWorld.GetComponent<SizeComponent>(entity);
                ProtectedComponent protect = World.currentWorld.GetComponent<ProtectedComponent>(entity);

                if (size.size <= ECSController.Instance.Config.protectionSize 
                  && collision.nbSameSizeCollisions ==  ECSController.Instance.Config.protectionCollisionCount
                  && protect.cooldown <= 0f)
                  {
                      protect.duration += Time.deltaTime;

                      if (protect.duration >= ECSController.Instance.Config.protectionDuration)
                      {
                        protect.duration = 0.0f;
                        protect.cooldown += Time.deltaTime;
                      }
                  }
                
                else if(protect.cooldown >= ECSController.Instance.Config.protectionCooldown)
                {
                    protect.cooldown += 0.0f;
                }
        }   
    }
}
}
