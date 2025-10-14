using CursedOnion.Extensions;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class TileSelectorPlayable : MonoBehaviour, IPlayable
    {
        TileSelector tileSelector;
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        [Inject] private LevelManager levelManager;

        void Awake()
        {
            tileSelector = GetComponent<TileSelector>();
        }
        public void OnEnable()
        {
            BattleInputReader reader = InputReaderCollection.GetReader<BattleInputReader>();
            reader.MovePointer += MoveSelector;
            reader.Select += PlaceSelector;
            reader.Inspect += Inspect;
        }

        public void OnDisable()
        {
            BattleInputReader reader = InputReaderCollection.GetReader<BattleInputReader>();
            reader.MovePointer -= MoveSelector;
            reader.Select -= PlaceSelector;
        }

        void MoveSelector(Vector2 direction)
        {
            Vector3 direction3D = direction.normalized;
            direction3D = direction3D.SwizzleXZY();

            float rotateAngle = levelManager.GetCameraPanAngles();
            rotateAngle = Mathf.Round(rotateAngle % 90) == 0 ? rotateAngle : rotateAngle + 45;
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            direction3D = rotation * direction3D;
            
            tileSelector.MovePosition(direction3D);
        }

        void PlaceSelector()
        {
            tileSelector.PlaceAtMousePosition();
        }

        void Inspect()
        {
            tileSelector.InspectSelectedElement();
        }
    }
}