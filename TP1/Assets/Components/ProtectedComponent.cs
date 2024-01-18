using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProtectedComponent : IComponent
{
    public float duration;
    public float cooldown;

    public ProtectedComponent(float duration, float cooldown) { this.cooldown = cooldown; this.duration = duration; }
}
