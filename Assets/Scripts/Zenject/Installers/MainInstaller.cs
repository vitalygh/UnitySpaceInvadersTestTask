//USING_ZENJECT
#if USING_ZENJECT
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public GameObject MainUI = null;
    public SpaceshipController.Settings SpaceshipControllerSettings = null;
    public InvadersController.Settings InvadersControllerSettings = null;

    public override void InstallBindings()
    {
        Container.BindInstances(MainUI);
        Container.BindInstances(InvadersControllerSettings);
        Container.BindInstances(SpaceshipControllerSettings);
        Container.BindInterfacesAndSelfTo<UnityDebugLogController>().AsSingle();
        Container.BindInterfacesAndSelfTo<KeyboardInputController>().AsSingle();
        Container.BindInterfacesAndSelfTo<MouseInputController>().AsSingle();
        Container.BindInterfacesAndSelfTo<TouchInputController>().AsSingle();
        Container.BindInterfacesAndSelfTo<Xbox360InputController>().AsSingle();
        Container.BindInterfacesAndSelfTo<EffectsController>().AsSingle();
        Container.BindInterfacesAndSelfTo<SpaceshipController>().AsSingle();
        Container.BindInterfacesAndSelfTo<InvadersController>().AsSingle();
        Container.BindInterfacesAndSelfTo<XmlGameSerialization>().AsSingle();
        Container.BindInterfacesAndSelfTo<SerializationController>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameController>().FromNewComponentOnNewGameObject().AsSingle();
        Container.BindFactory<Object, Vector3, IEntity, Effect.Factory>().FromFactory<ExtendedPrefabFactory<IEntity>>();
        Container.BindFactory<Object, Vector3, IEntity, Projectile.Factory>().FromFactory<ExtendedPrefabFactory<IEntity>>();
        Container.BindFactory<Object, Vector3, IControllableEntity, Invader.Factory>().FromFactory<ExtendedPrefabFactory<IControllableEntity>>();
        Container.BindFactory<Object, Vector3, IControllableEntity, Spaceship.Factory>().FromFactory<ExtendedPrefabFactory<IControllableEntity>>();
    }
}
#endif
