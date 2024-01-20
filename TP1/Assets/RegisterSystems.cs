﻿using System.Collections.Generic;
using Assets.Systems;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        var toRegister = new List<ISystem>();
        
        World.currentWorld = new World();

        // Add your systems here
        toRegister.Add(new ClickSystem()); // Make sure this is the first system registered
        toRegister.Add(new CircleLifetimeSystem());
        toRegister.Add(new MovementSystem());
        toRegister.Add(new CollisionSystem());
        toRegister.Add(new RenderSystem());
        toRegister.Add(new ExplosionSystem());
        toRegister.Add(new RewindSystem()); // Make sure this is the last system registered

        return toRegister;
    }
}