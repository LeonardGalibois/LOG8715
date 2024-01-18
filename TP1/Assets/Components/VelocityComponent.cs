using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityComponent : IComponent
{
    public Vector2 velocity;

    public VelocityComponent(Vector2 velocity) => this.velocity = velocity;
}
