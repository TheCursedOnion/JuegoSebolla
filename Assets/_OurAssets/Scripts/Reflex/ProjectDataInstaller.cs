using CursedOnion.Game.Entity.Effects;
using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class ProjectDataInstaller : MonoBehaviour, IInstaller 
    {
        [Expandable, SerializeField] private ConfusionData confusionData;
        [Expandable, SerializeField] private AttackBoostData attackBoostData;
        [Expandable, SerializeField] private MovementBoostData movementBoostData;
        [Expandable, SerializeField] private HealthBoostData healthBoostData;
        public void InstallBindings(ContainerBuilder _)
        {
            EntityEffectFactory.RegisterEffect<ConfusionEffect>(confusionData);
            EntityEffectFactory.RegisterEffect<AttackBoostEffect>(attackBoostData);
            EntityEffectFactory.RegisterEffect<MovementBoostEffect>(movementBoostData);
            EntityEffectFactory.RegisterEffect<HealthBoostEffect>(healthBoostData);
        }
    }
}