using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProtectedComponent : IEntityComponent
{
    public float duration;
    public float cooldown;
    public ProtectedComponent(float duration, float cooldown) 
    { 
        this.duration = duration; this.cooldown = cooldown; 
    }
    public object Clone() => new ProtectedComponent(duration, cooldown);
}
