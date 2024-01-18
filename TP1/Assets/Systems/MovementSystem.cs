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
            List<uint> entities = new List<uint>();

            foreach(uint entity in entities)
            {
                PositionComponent position = null;//world.GetComponent<PositionComponent>(entity);

                if (true/*world.GetComponent<PositionComponent>(entity) && world. */) ;
            }
        }
    }
}

