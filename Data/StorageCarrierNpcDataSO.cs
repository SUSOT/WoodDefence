using UnityEngine;
using UnityEngine.Serialization;

namespace _01_Works.CM._01_Scripts.NPC.Data
{
    public abstract class StorageCarrierNpcDataSO : ScriptableObject
    {
        [field: SerializeField, FormerlySerializedAs("NpcMoveSpeed"), Min(0f)]
        public float MoveSpeed { get; private set; }

        [field: SerializeField, FormerlySerializedAs("StorageChekcerBoxSize")]
        public Vector2 StorageCheckerBoxSize { get; private set; }

        [field: SerializeField, FormerlySerializedAs("TakeWoodNumber"), Min(1f)]
        public float LoadStepCount { get; private set; }

        [field: SerializeField, FormerlySerializedAs("TakeTime"), Min(0f)]
        public float TakeInterval { get; private set; }

        [field: SerializeField, FormerlySerializedAs("SupplyTime"), Min(0f)]
        public float DeliveryInterval { get; private set; }

        [field: SerializeField, FormerlySerializedAs("TakeWoodAmount"), Min(1)]
        public int WoodAmountPerStep { get; private set; }

        [field: SerializeField, Min(0f)]
        public float TargetSearchRadius { get; private set; }

        [field: SerializeField, FormerlySerializedAs("TowerCheckDistance"), Min(0f)]
        public float TargetArrivalDistance { get; private set; }

        public int MaxLoadSteps => Mathf.Max(1, Mathf.RoundToInt(LoadStepCount));
    }
}