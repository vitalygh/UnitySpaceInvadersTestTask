using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializableProjectile : IProjectile
{
    Vector2 Position { get; }
}
