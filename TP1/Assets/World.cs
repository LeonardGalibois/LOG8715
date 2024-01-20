using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class World
{
    public Dictionary<Type, Dictionary<uint, IEntityComponent>> components;
    public List<uint> entities;
    uint currentEntityID = 0;

    public static World currentWorld;

    public World()
    {
        components = new Dictionary<Type, Dictionary<uint, IEntityComponent>>();
        entities = new List<uint>();
    }

    public List<uint> GetEntitiesCopy() => new List<uint>(entities);

    public Dictionary<Type, Dictionary<uint, IEntityComponent>> GetComponentsCopy()
    {
        Dictionary<Type, Dictionary<uint, IEntityComponent>> componentsCopy = new Dictionary<Type, Dictionary<uint, IEntityComponent>>();

        foreach (var item in components)
        {
            Dictionary<uint, IEntityComponent> dictionaryCopy = new Dictionary<uint, IEntityComponent>();

            foreach (var component in item.Value) dictionaryCopy[component.Key] = (IEntityComponent)component.Value.Clone();

            componentsCopy[item.Key] = dictionaryCopy;
        }

        return componentsCopy;
    }

    public uint CreateEntity()
    {
        uint id = 0;
        ulong currentTry = 0;

        // Make sure the ID is available and not already in use
        while (currentTry++ < uint.MaxValue)
        {
            id = currentEntityID++;

            if (entities.Contains(id)) continue;

            entities.Add(id);
           
            break;
        }

        if (currentTry == uint.MaxValue) throw new Exception("Ran out of IDs for new entity");


        return id;
    }

    public bool DeleteEntity(uint entityID)
    {
        if (entities.Remove(entityID))
        {
            // Remove all components for the deleted entity
            foreach (var dictionary in components.Values) dictionary.Remove(entityID);

            return true;
        }
        else return false;
    }


    public void AddComponent<T>(uint entityID, T component) where T : IEntityComponent
    {
        Dictionary<uint, IEntityComponent> dictionary;

        if (!components.TryGetValue(typeof(T), out dictionary))
        {
            dictionary = new Dictionary<uint, IEntityComponent>();
            components[typeof(T)] = dictionary;
        }
        
        dictionary[entityID] = component;
    }

    public bool RemoveComponent<T>(uint entityID) where T : IEntityComponent
    {
        Dictionary<uint, IEntityComponent> dictionary;

        return components.TryGetValue(typeof(T), out dictionary) && dictionary.Remove(entityID);
    }

    public T GetComponent<T>(uint entityID) where T : IEntityComponent
    {
        Dictionary<uint, IEntityComponent> dictionary;
        IEntityComponent component = null;

        if (components.TryGetValue(typeof(T), out dictionary))
        {
            dictionary.TryGetValue(entityID, out component);
        }

        return (T) component;
    }

    public Dictionary<uint, IEntityComponent> GetAllComponents<T>() where T : IEntityComponent
    {
        Dictionary<uint, IEntityComponent> dictionary;

        return components.TryGetValue(typeof(T), out dictionary) ? new Dictionary<uint, IEntityComponent>(dictionary) : null;
    }
}
