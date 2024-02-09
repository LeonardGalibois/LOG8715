using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Systems
{
    public class MovementSystem : ISystem
    {
        public string Name { get { return "MovementSystem"; } }

        private bool IsRepeatedSystem { get; set; }
        public MovementSystem(bool isRepeatedSystem = false)
        {
            IsRepeatedSystem = isRepeatedSystem;
        }

        public void UpdateSystem()
        {
            foreach(uint entity in World.currentWorld.entities)
            {
                PositionComponent position = World.currentWorld.GetComponent<PositionComponent>(entity);
                VelocityComponent speed = World.currentWorld.GetComponent<VelocityComponent>(entity);

                // if we are repeating the simulation and the circle is on the left side, we want to continue the iteration
                // otherwise we skip to the next one
                if (IsRepeatedSystem && position.position.x > 0) continue;

                if (speed != null && position != null)
                {
                    position.position += Time.deltaTime * speed.velocity;
                }
            }
        }
    }
}

