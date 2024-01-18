using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionComponent : IComponent
{
    public Vector2 position;

    public PositionComponent(Vector2 position) => this.position = position;
}
