using UnityEngine;
public interface ISpaceshipController : IEntityController
{
    bool IsPositionOutOfBorders(Vector2 position);
}
