public interface ISerializableSpaceshipController : ISpaceshipController
{
    IEntity CurrentProjectile { get; set; }
    IControllableEntity Spaceship { get; }

}
