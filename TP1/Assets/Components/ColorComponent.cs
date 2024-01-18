using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ColorComponent : IComponent
{
    public Color color;

    public ColorComponent(Color color) => this.color = color;
}
