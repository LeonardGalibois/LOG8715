using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedComponent : IComponent
{
    public Vector2 speed;

    public SpeedComponent(Vector2 speed) => this.speed = speed;
}
