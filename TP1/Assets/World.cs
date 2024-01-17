using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class World
{
    Dictionary<Type, Dictionary<uint, IComponent>> components;
    List<uint> entities;
    uint currentEntityID = 0;

    public World()
    {
        components = new Dictionary<Type, Dictionary<uint, IComponent>>();
    }

    public uint CreateEntity(List<IComponent> components=null)
    {
        ulong currentTry = 0;

        // Make sure the ID is available and not already in use
        while (currentTry++ < uint.MaxValue)
        {
            uint id = currentEntityID++;

            if (entities.Contains(id)) continue;

            return id;
        }

        if (currentTry++ == uint.MaxValue) throw new Exception("Ran out of IDs for new entity");

        return 0;
    }

    public bool DeleteEntity(uint id)
    {
        if (entities.Remove(id))
        {
            // Remove all components for the deleted entity
            foreach (var dictionary in components.Values) dictionary.Remove(id);

            return true;
        }
        else return false;
    }


    public void AddComponent<T>(uint entityID, T component) where T : IComponent
    {
        Dictionary<uint, IComponent> dictionary;

        if (!components.TryGetValue(typeof(T), out dictionary))
        {
            dictionary = new Dictionary<uint, IComponent>();
            components[typeof(T)] = dictionary;
        }
        
        dictionary[entityID] = component;
    }

    public bool RemoveComponent<T>(uint entityID, T component) where T : IComponent
    {
        Dictionary<uint, IComponent> dictionary;

        return components.TryGetValue(typeof(T), out dictionary) && dictionary.Remove(entityID);
    }

    public T GetComponent<T>(uint entityID) where T : IComponent
    {
        Dictionary<uint, IComponent> dictionary;
        IComponent component = null;

        if (components.TryGetValue(typeof(T), out dictionary))
        {
            dictionary.TryGetValue(entityID, out component);
        }

        return (T) component;
    }
}
