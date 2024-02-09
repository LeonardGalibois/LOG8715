using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CollidedTagComponent : IEntityComponent
{
    public CollidedTagComponent() { }
    public object Clone() => new CollidedTagComponent();
}
