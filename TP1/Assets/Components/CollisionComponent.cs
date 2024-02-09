using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CollisionComponent : IEntityComponent
{
    public int nbCollisions;
    public int nbSameSizeCollisions;

    public CollisionComponent(int nbCollisions, int nbSameSizeCollisions) 
    { 
        this.nbCollisions = nbCollisions; this.nbSameSizeCollisions = nbSameSizeCollisions;
    }

    public object Clone() => new CollisionComponent(nbCollisions, nbSameSizeCollisions);
}
