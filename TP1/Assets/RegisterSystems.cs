using System.Collections.Generic;
using Assets.Systems;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        var toRegister = new List<ISystem>();
        
        World.currentWorld = new World();

        MovementSystem repeatedMovementSystem = new MovementSystem(true);
        CollisionSystem repeatedCollisionSystem = new CollisionSystem(true);
        ExplosionSystem repeatedExplosionSystem = new ExplosionSystem(true);
        CircleLifetimeSystem repeatedCircleLifetimeSystem = new CircleLifetimeSystem(true);

        // Add your systems here
        // A person can only click once per frame
        toRegister.Add(new ClickSystem()); // Make sure this is the first system registered
        toRegister.Add(new CircleLifetimeSystem());
        toRegister.Add(new MovementSystem());
        toRegister.Add(new CollisionSystem());
        toRegister.Add(new ExplosionSystem());
        toRegister.Add(new ProtectionSystem());

        toRegister.Add(repeatedCircleLifetimeSystem);
        toRegister.Add(repeatedMovementSystem);
        toRegister.Add(repeatedCollisionSystem);
        toRegister.Add(repeatedExplosionSystem);

        toRegister.Add(repeatedCircleLifetimeSystem);
        toRegister.Add(repeatedMovementSystem);
        toRegister.Add(repeatedCollisionSystem);
        toRegister.Add(repeatedExplosionSystem);

        toRegister.Add(repeatedCircleLifetimeSystem);
        toRegister.Add(repeatedMovementSystem);
        toRegister.Add(repeatedCollisionSystem);
        toRegister.Add(repeatedExplosionSystem);

        toRegister.Add(new RenderSystem()); // We only want to change the color once per frame
        // We want to take a time stamp of each frame and not every simulation
        toRegister.Add(new RewindSystem()); // Make sure this is the last system registered

        return toRegister;
    }
}