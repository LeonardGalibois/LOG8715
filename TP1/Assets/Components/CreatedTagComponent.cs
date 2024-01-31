using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CreatedTagComponent : IEntityComponent
{
    public CreatedTagComponent() { }
    public object Clone() => new CreatedTagComponent();
}
