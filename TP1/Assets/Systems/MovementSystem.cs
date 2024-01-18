using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Systems
{
    public class MovementSystem : ISystem
    {
        public string Name { get { return "MovementSystem"; } }

        public void UpdateSystem()
        {
            foreach(uint entity in World.currentWorld.entities)
            {
                PositionComponent position = World.currentWorld.GetComponent<PositionComponent>(entity);
                VelocityComponent speed = World.currentWorld.GetComponent<VelocityComponent>(entity);

                if (speed != null && position != null)
                {
                    position.position += Time.deltaTime * speed.velocity;
                }
            }
        }
    }
}

