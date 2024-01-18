using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SizeComponent : IComponent
{
    public int size;

    public SizeComponent(int size) => this.size = size;
}
