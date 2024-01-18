using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CollisionComponent : IComponent
{
    public int nbCollisions;

    public CollisionComponent(int nbCollisions) => this.nbCollisions = nbCollisions;
}
