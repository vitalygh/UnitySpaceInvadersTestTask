using UnityEngine;
public class SerializedGame
{
    public int Score = 0;
    public int HP = 0;
    public int Level = 0;
    public Vector2 MoveDirection = Vector2.zero;
    public SerializedInvader[] Invaders = null;
    public SerializedControlledEntity Spaceship = null;
    public SerializedProjectile[] Projectiles = null;
}
