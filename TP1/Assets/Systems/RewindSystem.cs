using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Systems
{
    public class RewindSystem : ISystem
    {
        const float cooldown = 3.0f;
        const float rewindTime = 3.0f;

        class Snapshot
        {
            public Snapshot next;
            public Snapshot previous;
            public float time;

            public List<uint> entities;
            public Dictionary<Type, Dictionary<uint, IEntityComponent>> components;
        }

        Snapshot newestSnapshot;
        Snapshot oldestSnapshot;

        float nextAvailableRewindTime = 0;
        
        public string Name { get { return "RewindSystem"; } }

        public void UpdateSystem()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (CanRewind())
                {
                    nextAvailableRewindTime = Time.time + cooldown;

                    Debug.Log($"Rewinding {rewindTime} seconds in the past...");

                    Rewind();
                }
                else Debug.Log($"Rewind is on cooldown for another { nextAvailableRewindTime - Time.time } seconds");
            }
            else
            {
                RefreshSnapshotList();
                TakeSnapshot();
            }
        }

        bool CanRewind() => Time.time >= nextAvailableRewindTime;

        void Rewind()
        {
            if (oldestSnapshot is null) return;

            // Add previously added shapes
            foreach (uint entity in oldestSnapshot.entities)
            {
                if (World.currentWorld.entities.Contains(entity)) continue;

                ECSController.Instance.CreateShape(entity, 1);
            }

            // Remove newly added shapes
            foreach (uint entity in World.currentWorld.entities)
            {
                if (oldestSnapshot.entities.Contains(entity)) continue;

                else ECSController.Instance.DestroyShape(entity);
            }

            World.currentWorld.entities = oldestSnapshot.entities;
            World.currentWorld.components = oldestSnapshot.components;
        }

        void RefreshSnapshotList()
        {
            while (oldestSnapshot is not null && Time.time > oldestSnapshot.time + rewindTime)
            {
                // Oldest Snapshot has expired
                oldestSnapshot = oldestSnapshot.next;
                if (oldestSnapshot is not null) oldestSnapshot.previous = null;
            }
        }

        void TakeSnapshot()
        {
            Snapshot snapshot = new Snapshot();
            snapshot.previous = newestSnapshot;
            snapshot.next = null;
            snapshot.time = Time.time;
            snapshot.entities = World.currentWorld.GetEntitiesCopy();
            snapshot.components = World.currentWorld.GetComponentsCopy();

            if (newestSnapshot is not null) newestSnapshot.next = snapshot;
            newestSnapshot = snapshot;

            if (oldestSnapshot is null) oldestSnapshot = snapshot;
        }
    }
}
