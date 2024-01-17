using System.Collections.Generic;
using Assets.Systems;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        var toRegister = new List<ISystem>();

        // Add your systems here
        toRegister.Add(new MovementSystem());
        toRegister.Add(new CollisionSystem());
        toRegister.Add(new DestructionSystem());
        toRegister.Add(new ExplosionSystem());
        toRegister.Add(new ReboundSystem());
        toRegister.Add(new ProtectionSystem());
        toRegister.Add(new ColorSystem());
        toRegister.Add(new RewindSystem());
        toRegister.Add(new ClickSystem());

        return toRegister;
    }
}