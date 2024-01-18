using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Systems
{
    public class CollisionSystem : ISystem
    {
        public string Name { get { return "CollisionSystem"; } }

        void CheckBoundsCollisions(PositionComponent positionComponent, VelocityComponent velocityComponent, SizeComponent sizeComponent)
        {
            Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.nearClipPlane));

            float leftBound = -bounds.x - Camera.main.transform.position.x;
            float rightBound = bounds.x - Camera.main.transform.position.x;
            float topBound = bounds.y - Camera.main.transform.position.y;
            float bottomBound = -bounds.y - Camera.main.transform.position.y;

            float radius = sizeComponent.size / 2;

            Vector2 collisionNormal = Vector2.zero;

            if (positionComponent.position.x - radius < leftBound)
            {
                // Outside of left bound
                collisionNormal = Vector2.right;
                positionComponent.position.x = leftBound + radius;
            }
            else if (positionComponent.position.x + radius > rightBound)
            {
                // Outside of right bound
                collisionNormal = Vector2.left;
                positionComponent.position.x = rightBound - radius;
            }
            else if (positionComponent.position.y + radius > topBound)
            {
                // Outside of top bound
                collisionNormal = Vector2.down;
                positionComponent.position.y = topBound - radius;
            }
            else if (positionComponent.position.y - radius < bottomBound)
            {
                // Outisde of bottom bound
                collisionNormal = Vector2.up;
                positionComponent.position.y = bottomBound + radius;
            }

            if (collisionNormal.magnitude > 0 && Vector2.Dot(velocityComponent.velocity, collisionNormal) < 0)
            {
                velocityComponent.velocity = Vector2.Reflect(velocityComponent.velocity, collisionNormal);
            }
        }



        public void UpdateSystem()
        {
            var positionComponents = World.currentWorld.GetAllComponents<PositionComponent>();
            var velocityComponents = World.currentWorld.GetAllComponents<VelocityComponent>();
            var sizeComponents = World.currentWorld.GetAllComponents<SizeComponent>();

            foreach (var item in positionComponents)
            {
                PositionComponent positionComponent = (PositionComponent)item.Value;
                VelocityComponent velocityComponent = (VelocityComponent)velocityComponents[item.Key];
                SizeComponent sizeComponent = (SizeComponent)sizeComponents[item.Key];

                CheckBoundsCollisions(positionComponent, velocityComponent, sizeComponent);
            }

            /*
            for (int self = 0; self < positionComponents.Count; self++)
            {
                for (int other = self + 1; other < positionComponents.Count - 1; other++)
                {
                    var item = positionComponents.ElementAt(self);

                    uint selfEntityID = positionComponents.Keys.ElementAt(self);
                    PositionComponent selfPositionComponent = (PositionComponent)positionComponents.Values.ElementAt(self);
                    SizeComponent selfSizeComponent = (SizeComponent)sizeComponents[selfEntityID];
                    
                }
            }
            */
        }
    }
}
