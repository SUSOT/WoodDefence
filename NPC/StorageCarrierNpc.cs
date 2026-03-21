using _01_Works.CM._01_Scripts.NPC.Data;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC
{
    public abstract class StorageCarrierNpc : Npc
    {
        private int _currentLoadSteps;
        private float _moveSpeed;
        private float _defaultMoveSpeed;

        protected abstract StorageCarrierNpcDataSO CarrierData { get; }

        public override float MoveSpeed => _moveSpeed;
        public int MaxLoadSteps => CarrierData.MaxLoadSteps;
        public bool HasLoad => _currentLoadSteps > 0;
        public bool IsLoadFull => _currentLoadSteps >= MaxLoadSteps;
        public float TakeInterval => CarrierData.TakeInterval;
        public float DeliveryInterval => CarrierData.DeliveryInterval;
        public int WoodAmountPerStep => CarrierData.WoodAmountPerStep;
        public float TargetSearchRadius => CarrierData.TargetSearchRadius;
        public float TargetArrivalDistance => CarrierData.TargetArrivalDistance;
        public Vector2 StorageCheckerBoxSize => CarrierData.StorageCheckerBoxSize;

        protected void InitializeCarrierRuntime(Transform storagePos)
        {
            ResetNpcRuntime();
            StoragePos = storagePos;
            _currentLoadSteps = 0;
            _defaultMoveSpeed = CarrierData.MoveSpeed;
            _moveSpeed = _defaultMoveSpeed;
            SetGaugeValue(0f);
            HideGauge();
            EnterInitialState(NpcStateType.Idle);
        }

        public bool CanTakeFromStorage()
        {
            return IsLoadFull == false &&
                   GameManager.Instance != null &&
                   GameManager.Instance.woodCurrency.Value >= WoodAmountPerStep;
        }

        public bool IsInStorageRange()
        {
            return Physics2D.OverlapBox(transform.position, StorageCheckerBoxSize, 0f, WhatIsStorage);
        }

        public bool TryTakeOneLoadStepFromStorage()
        {
            if (CanTakeFromStorage() == false)
            {
                return false;
            }

            ShowGauge();
            GameManager.Instance.woodCurrency.Value -= WoodAmountPerStep;
            _currentLoadSteps = Mathf.Min(_currentLoadSteps + 1, MaxLoadSteps);
            RefreshLoadState();
            return true;
        }

        public bool TryConsumeOneLoadStep()
        {
            if (HasLoad == false)
            {
                return false;
            }

            _currentLoadSteps = Mathf.Max(0, _currentLoadSteps - 1);
            RefreshLoadState();
            return true;
        }

        public void ClearLoad()
        {
            _currentLoadSteps = 0;
            RefreshLoadState();
            HideGauge();
        }

        private void RefreshLoadState()
        {
            float ratio = MaxLoadSteps <= 0 ? 0f : (float)_currentLoadSteps / MaxLoadSteps;
            _moveSpeed = _defaultMoveSpeed * (1f - 0.5f * ratio);
            SetGaugeValue(ratio);
        }
    }
}