﻿using System;
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

        private bool IsRepeatedSystem { get; set; }
        public CollisionSystem(bool isRepeatedSystem = false)
        {
            IsRepeatedSystem = isRepeatedSystem;
        }

        void CheckBoundsCollisions(
            Dictionary<uint, IEntityComponent> positionComponents,
            Dictionary<uint, IEntityComponent> velocityComponents,
            Dictionary<uint, IEntityComponent> sizeComponents
            )
        {
            foreach (var item in positionComponents)
            {
                PositionComponent positionComponent = (PositionComponent)item.Value;
                VelocityComponent velocityComponent = (VelocityComponent)velocityComponents[item.Key];
                SizeComponent sizeComponent = (SizeComponent)sizeComponents[item.Key];

                // if we are repeating the simulation and the circle is on the left side, we want to continue the iteration
                // otherwise we skip to the next one
                if (IsRepeatedSystem && (positionComponent.position.x - Camera.main.transform.position.x) > 0) continue;

                Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.nearClipPlane));

                float leftBound = -bounds.x - Camera.main.transform.position.x;
                float rightBound = bounds.x - Camera.main.transform.position.x;
                float topBound = bounds.y - Camera.main.transform.position.y;
                float bottomBound = -bounds.y - Camera.main.transform.position.y;

                float radius = sizeComponent.size / 2.0f;

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
        }

        public void CheckMutualCollisions(
            Dictionary<uint, IEntityComponent> positionComponents,
            Dictionary<uint, IEntityComponent> velocityComponents,
            Dictionary<uint, IEntityComponent> sizeComponents,
            Dictionary<uint, IEntityComponent> collisionComponents,
            Dictionary<uint, IEntityComponent> protectedComponents
            )
        {
            for (int self = 0; self < positionComponents.Count - 1; self++)
            {
                for (int other = self + 1; other < positionComponents.Count; other++)
                {
                    var item = positionComponents.ElementAt(self);

                    uint selfEntityID = positionComponents.Keys.ElementAt(self);    
                    PositionComponent selfPositionComponent = (PositionComponent)positionComponents.Values.ElementAt(self);
                    SizeComponent selfSizeComponent = (SizeComponent)sizeComponents[selfEntityID];
                    VelocityComponent selfVelocityComponent = (VelocityComponent)velocityComponents[selfEntityID];
                    CollisionComponent selfCollisionComponent = (CollisionComponent)collisionComponents[selfEntityID];
                    ProtectedComponent protectedComponent = World.currentWorld.GetComponent<ProtectedComponent>(item.Key);

                    uint otherEntityID = positionComponents.Keys.ElementAt(other);
                    PositionComponent otherPositionComponent = (PositionComponent)positionComponents.Values.ElementAt(other);
                    SizeComponent otherSizeComponent = (SizeComponent)sizeComponents[otherEntityID];
                    VelocityComponent otherVelocityComponent = (VelocityComponent)velocityComponents[otherEntityID];
                    CollisionComponent otherCollisionComponent = (CollisionComponent)collisionComponents[otherEntityID];

                    CollisionResult collision = CollisionUtility.CalculateCollision(
                            selfPositionComponent.position, selfVelocityComponent.velocity, selfSizeComponent.size,
                            otherPositionComponent.position, otherVelocityComponent.velocity, otherSizeComponent.size
                        );

                    // If no collision is detected, we move on to the next iteration
                    if (collision is null) continue;

                    // Collision detected!

                    // Increase number of collisions for each circle
                    selfCollisionComponent.nbCollisions++;
                    otherCollisionComponent.nbCollisions++;

                    // Self's new position and velocity
                    selfPositionComponent.position = collision.position1;
                    selfVelocityComponent.velocity = collision.velocity1;

                    // Other's new position and velocity
                    otherPositionComponent.position = collision.position2;
                    otherVelocityComponent.velocity = collision.velocity2;

                    // Only change size if they are both moving
                    if (Math.Abs(otherVelocityComponent.velocity.magnitude) > float.Epsilon &&
                        Math.Abs(selfVelocityComponent.velocity.magnitude) > float.Epsilon)
                    {
                        //saving the number of collisions with the same circle
                        if (selfSizeComponent.size == otherSizeComponent.size)
                        {
                            selfCollisionComponent.nbSameSizeCollisions++;
                            otherCollisionComponent.nbSameSizeCollisions++;
                        }


                        else if (selfSizeComponent.size < otherSizeComponent.size)
                        {
                            // If circle is protected, the colliding bigger circle decreases by 1
                            if (0.0f < protectedComponent.duration && protectedComponent.duration < ECSController.Instance.Config.protectionDuration)
                            {
                                otherSizeComponent.size--;
                            }
                            else
                            {
                                selfSizeComponent.size--;
                                otherSizeComponent.size++;
                            }
                        
                    }
                        else if (selfSizeComponent.size > otherSizeComponent.size)
                        {
                            // If the circle is protected, the colliding smaller circle does not decrease
                            if (0.0f < protectedComponent.duration && protectedComponent.duration < ECSController.Instance.Config.protectionDuration) continue; 

                            else
                            {
                                selfSizeComponent.size++;
                                otherSizeComponent.size--;
                            }
                        
                    }
                    }
                    

                    // Add a tag to specify that there was a collision
                    World.currentWorld.AddComponent<CollidedTagComponent>(selfEntityID, new CollidedTagComponent());
                    World.currentWorld.AddComponent<CollidedTagComponent>(otherEntityID, new CollidedTagComponent());
                    
                }
            }
        }



        public void UpdateSystem()
        {
            var positionComponents = World.currentWorld.GetAllComponents<PositionComponent>();
            var velocityComponents = World.currentWorld.GetAllComponents<VelocityComponent>();
            var sizeComponents = World.currentWorld.GetAllComponents<SizeComponent>();
            var collisionComponents = World.currentWorld.GetAllComponents<CollisionComponent>();
            var protectedComponents = World.currentWorld.GetAllComponents<ProtectedComponent>();

            CheckBoundsCollisions(positionComponents, velocityComponents, sizeComponents);

            CheckMutualCollisions(positionComponents, velocityComponents, sizeComponents, collisionComponents, protectedComponents);
        }
    }
}
