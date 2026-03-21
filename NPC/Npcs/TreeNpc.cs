using System.Collections.Generic;
using _01_Works.CM._01_Scripts.NPC.Data;
using _01_Works.CM._01_Scripts.NPC.NPC.Roles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Npcs
{
    public class TreeNpc : Npc
    {
        private int _currentLoadSteps;
        private float _moveSpeed;
        private float _defaultMoveSpeed;

        [field: SerializeField] public LayerMask WhatIsTree { get; private set; }
        [field: SerializeField] public TreeNpcDataSO TreeNpcData { get; private set; }

        public override float MoveSpeed => _moveSpeed;
        public int CurrentLoadSteps => _currentLoadSteps;
        public int MaxLoadSteps => TreeNpcData.MaxLoadSteps;
        public bool HasLoad => _currentLoadSteps > 0;
        public bool IsLoadFull => _currentLoadSteps >= MaxLoadSteps;
        public float FellingInterval => TreeNpcData.FellingInterval;
        public float SaveInterval => TreeNpcData.SaveInterval;
        public float TreeArrivalDistance => TreeNpcData.TreeArrivalDistance;

        protected override void Awake()
        {
            base.Awake();
            Role = new TreeNpcRole();
        }

        protected override void InitializeNpc()
        {
            ResetNpcRuntime();
            StoragePos = GameManager.Instance.GroundStoragePos;
            _currentLoadSteps = 0;
            _defaultMoveSpeed = TreeNpcData.MoveSpeed;
            _moveSpeed = _defaultMoveSpeed;
            SetGaugeValue(0f);
            HideGauge();
            EnterInitialState(NpcStateType.Idle);
        }

        public bool IsInStorageRange()
        {
            return Physics2D.OverlapBox(transform.position, TreeNpcData.StorageCheckerBoxSize, 0f, WhatIsStorage);
        }

        public bool IsTreeTargetValid(Transform target)
        {
            if (!target || !target.gameObject.activeInHierarchy)
            {
                return false;
            }

            TreeFunction treeFunction = target.GetComponent<TreeFunction>();
            if (!treeFunction || treeFunction.HealthComp == null)
            {
                return false;
            }

            return treeFunction.HealthComp.CurrentHealth > 0;
        }

        public bool TryAcquireTreeTarget(out Transform target)
        {
            target = null;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, TreeNpcData.TreeTargetingRange, WhatIsTree);
            if (hits == null || hits.Length == 0)
            {
                return false;
            }

            List<Transform> candidates = new List<Transform>();

            foreach (var hit in hits)
            {
                if (!hit)
                {
                    continue;
                }

                if (IsTreeTargetValid(hit.transform))
                {
                    candidates.Add(hit.transform);
                }
            }

            if (candidates.Count == 0)
            {
                return false;
            }

            target = candidates[Random.Range(0, candidates.Count)];
            return true;
        }

        public bool TryChopOneLoadStep(Transform target)
        {
            if (IsLoadFull || IsTreeTargetValid(target) == false)
            {
                return false;
            }

            TreeFunction treeFunction = target.GetComponent<TreeFunction>();
            if (!treeFunction)
            {
                return false;
            }

            ShowGauge();
            treeFunction.CutDownTree(TreeNpcData.WoodPerStep);
            _currentLoadSteps = Mathf.Min(_currentLoadSteps + 1, MaxLoadSteps);
            RefreshLoadState();
            return true;
        }

        public bool TryStoreOneLoadStep()
        {
            if (!HasLoad)
            {
                return false;
            }

            GameManager.Instance.AddWoodCurrency(TreeNpcData.WoodPerStep);
            _currentLoadSteps = Mathf.Max(0, _currentLoadSteps - 1);
            RefreshLoadState();
            return true;
        }

        private void RefreshLoadState()
        {
            float ratio = MaxLoadSteps <= 0 ? 0f : (float)_currentLoadSteps / MaxLoadSteps;
            _moveSpeed = _defaultMoveSpeed * (1f - 0.5f * ratio);
            SetGaugeValue(ratio);
        }

        public override void ResetItem()
        {
            StopMovement();
            _currentLoadSteps = 0;
            _moveSpeed = TreeNpcData ? TreeNpcData.MoveSpeed : 0f;
            SetGaugeValue(0f);
            HideGauge();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (TreeNpcData == null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, TreeNpcData.TreeTargetingRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, TreeNpcData.TreeArrivalDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, TreeNpcData.StorageCheckerBoxSize);
        }
#endif
    }
}
