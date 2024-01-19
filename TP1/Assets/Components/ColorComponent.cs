using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ColorComponent : IEntityComponent
{
    public Color color;

    public ColorComponent(Color color) => this.color = color;

    public object Clone() => new ColorComponent(color);
}
