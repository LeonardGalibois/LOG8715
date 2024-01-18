using System.Collections.Generic;
using Assets.Systems;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        var toRegister = new List<ISystem>();
        
        World.currentWorld = new World();

        // Add your systems here
        toRegister.Add(new CircleLifetimeSystem());
        toRegister.Add(new RenderSystem());

        return toRegister;
    }
}