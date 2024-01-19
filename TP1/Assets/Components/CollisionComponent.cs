using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CollisionComponent : IEntityComponent
{
    public int nbCollisions;

    public CollisionComponent(int nbCollisions) => this.nbCollisions = nbCollisions;

    public object Clone() => new CollisionComponent(nbCollisions);
}
