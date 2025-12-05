using CursedOnion.Game.Commands;
using CursedOnion.Game.Events;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Miscellaneous;
using Reflex.Core;
using UnityEngine;
using CursedOnion.Game.Modes.General.UI.Events;
using CursedOnion.Game.Settings;
using NaughtyAttributes;

namespace CursedOnion.Installers
{
    public class ProjectMiscellaneousInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] GameObject textParticlePrefab;
        [Expandable, SerializeField] ParticleManager particleManager;
        [Expandable, SerializeField] InputReaderCollection inputReaderCollection;
        [Expandable, SerializeField] GameSettings gameSettings;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(new UIEvents(), typeof(UIEvents));
            
            gameSettings.Initialize();
            containerBuilder.AddSingleton(gameSettings, typeof(GameSettings));
            
            inputReaderCollection.Initialize();
            containerBuilder.AddSingleton(inputReaderCollection, typeof(InputReaderCollection));
            
            particleManager.Initialize();
            containerBuilder.AddSingleton(particleManager, typeof(ParticleManager));
            
            containerBuilder.AddSingleton(new CommandManager(), typeof(CommandManager));
            
            containerBuilder.AddSingleton(new TextParticleManager(textParticlePrefab), typeof(TextParticleManager));
        }
    }
}
