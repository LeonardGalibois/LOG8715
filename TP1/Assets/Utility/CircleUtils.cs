using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CircleUtils
{
    public static uint CreateCircle(Vector2 position, Vector2 velocity, int size)
    {
        uint entity = World.currentWorld.CreateEntity();
        World.currentWorld.AddComponent(entity, new PositionComponent(position));
        World.currentWorld.AddComponent(entity, new SizeComponent(size));
        World.currentWorld.AddComponent(entity, new VelocityComponent(velocity));
        World.currentWorld.AddComponent(entity, new ColorComponent(Color.white));
        World.currentWorld.AddComponent(entity, new CollisionComponent(0,0));
        World.currentWorld.AddComponent(entity, new ProtectedComponent(0f,0f));
        ECSController.Instance.CreateShape(entity, size);

        return entity;
    }

    public static void DestroyCircle(uint entityID)
    {
        ECSController.Instance.DestroyShape(entityID);
        World.currentWorld.DeleteEntity(entityID);
    }
}
