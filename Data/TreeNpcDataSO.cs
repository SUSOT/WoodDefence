using UnityEngine;
using UnityEngine.Serialization;

namespace _01_Works.CM._01_Scripts.NPC.Data
{
    [CreateAssetMenu(menuName = "SO/NPC/TreeNpcData")]
    public class TreeNpcDataSO : ScriptableObject
    {
        [field: SerializeField, FormerlySerializedAs("StorageChekcerBoxSize")]
        public Vector2 StorageCheckerBoxSize { get; private set; }

        [field: SerializeField, FormerlySerializedAs("NpcMoveSpeed"), Min(0f)]
        public float MoveSpeed { get; private set; }

        [field: SerializeField, FormerlySerializedAs("TreeChekcerSize"), Min(0f)]
        public float TreeArrivalDistance { get; private set; }

        [field: SerializeField, FormerlySerializedAs("FellingNumber"), Min(1f)]
        public float FellingStepCount { get; private set; }

        [field: SerializeField, FormerlySerializedAs("FellingTime"), Min(0f)]
        public float FellingInterval { get; private set; }

        [field: SerializeField, FormerlySerializedAs("SaveTime"), Min(0f)]
        public float SaveInterval { get; private set; }

        [field: SerializeField, FormerlySerializedAs("TreeGetAmount"), Min(1)]
        public int WoodPerStep { get; private set; }

        [field: SerializeField, FormerlySerializedAs("TreeTargetingRange"), Min(0f)]
        public float TreeTargetingRange { get; private set; }

        public int MaxLoadSteps => Mathf.Max(1, Mathf.RoundToInt(FellingStepCount));
    }
}
