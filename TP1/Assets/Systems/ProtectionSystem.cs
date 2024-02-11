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

        private bool IsRepeatedSystem { get; set; }
        public ProtectionSystem(bool isRepeatedSystem = false)
        {
            IsRepeatedSystem = isRepeatedSystem;
        }

        public void UpdateSystem()
        {
            CheckProtectableEntities();
        }

        void CheckProtectableEntities()
        {
            foreach(uint entity in World.currentWorld.entities)
            {
                // if we are repeating the simulation and the circle is on the left side, we want to continue the iteration
                // otherwise we skip to the next one
                if (IsRepeatedSystem && (World.currentWorld.GetComponent<PositionComponent>(entity).position.x - Camera.main.transform.position.x) > 0) continue;

                CollisionComponent collision = World.currentWorld.GetComponent<CollisionComponent>(entity);
                SizeComponent size = World.currentWorld.GetComponent<SizeComponent>(entity);
                ProtectedComponent protect = World.currentWorld.GetComponent<ProtectedComponent>(entity);

                if (size.size <= ECSController.Instance.Config.protectionSize 
                  && collision.nbSameSizeCollisions >=  ECSController.Instance.Config.protectionCollisionCount
                  && protect.cooldown <= 0.0f && protect.duration < ECSController.Instance.Config.protectionDuration)
                {
                      protect.duration += Time.deltaTime;
                }
                else if (protect.duration >= ECSController.Instance.Config.protectionDuration)      
                {
                    
                    protect.duration = 0.0f;
                    collision.nbSameSizeCollisions = 0;
                    protect.cooldown = ECSController.Instance.Config.protectionCooldown;
                }
            
                if(protect.cooldown > 0)
                {
                    protect.cooldown -= Time.deltaTime;
                }
            }   
        }
    }
}
