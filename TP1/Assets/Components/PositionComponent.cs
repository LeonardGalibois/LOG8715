using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionComponent : IEntityComponent
{
    public Vector2 position;

    public PositionComponent(Vector2 position) => this.position = position;

    public object Clone() => new PositionComponent(position);
}
