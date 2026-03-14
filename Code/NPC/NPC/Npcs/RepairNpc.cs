using System.Collections.Generic;
using _01_Works.CM._01_Scripts.NPC.Data;
using _01_Works.CM._01_Scripts.NPC.NPC.Roles;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Npcs
{
    public class RepairNpc : StorageCarrierNpc
    {
        [field: SerializeField] public RepairNpcDataSO RepairNpcData { get; private set; }
        [SerializeField] private LayerMask whatIsTower;

        protected override StorageCarrierNpcDataSO CarrierData => RepairNpcData;

        protected override void Awake()
        {
            base.Awake();
            Role = new RepairNpcRole();
        }

        protected override void InitializeNpc()
        {
            InitializeCarrierRuntime(GameManager.Instance.UnderGroundStoragePos);
        }

        public bool TryGetBestRepairTarget(out Transform target)
        {
            target = null;

            var hits = Physics2D.OverlapCircleAll(transform.position, TargetSearchRadius, whatIsTower);
            if (hits == null || hits.Length == 0)
            {
                return false;
            }

            var lowestHealth = float.MaxValue;
            var visitedTargets = new HashSet<Transform>();

            foreach (var hit in hits)
            {
                if (!hit)
                {
                    continue;
                }

                if (!TryResolveRepairTarget(hit, out var candidate))
                {
                    continue;
                }

                if (!candidate.gameObject.activeInHierarchy || !visitedTargets.Add(candidate))
                {
                    continue;
                }

                if (!candidate.TryGetComponent(out BuildObject buildObject))
                {
                    continue;
                }

                if (!candidate.TryGetComponent(out IHealable _))
                {
                    continue;
                }

                if (buildObject.GetIsMaxHealth())
                {
                    continue;
                }

                float currentHealth = buildObject.GetCurrentHealth();
                if (currentHealth < lowestHealth)
                {
                    lowestHealth = currentHealth;
                    target = candidate;
                }
            }

            return target;
        }

        public bool IsRepairTargetValid(Transform target)
        {
            if (!target || !target.gameObject.activeInHierarchy)
            {
                return false;
            }

            if (!target.TryGetComponent(out BuildObject buildObject))
            {
                return false;
            }

            if (!target.TryGetComponent(out IHealable _))
            {
                return false;
            }

            return !buildObject.GetIsMaxHealth();
        }

        public bool TryRepairOnce(Transform target)
        {
            if (!HasLoad || !IsRepairTargetValid(target))
            {
                return false;
            }

            var healable = target.GetComponent<IHealable>();

            if (!TryConsumeOneLoadStep())
            {
                return false;
            }

            healable.TakeHeal(WoodAmountPerStep);
            return true;
        }

        private bool TryResolveRepairTarget(Collider2D hit, out Transform target)
        {
            target = null;

            if (hit.TryGetComponent(out IHealable _) && hit.TryGetComponent(out BuildObject _))
            {
                target = hit.transform;
                return true;
            }

            if (hit.transform.parent &&
                hit.transform.parent.TryGetComponent(out IHealable _) &&
                hit.transform.parent.TryGetComponent(out BuildObject _))
            {
                target = hit.transform.parent;
                return true;
            }

            return false;
        }

        public override void ResetItem()
        {
            StopMovement();
            ClearLoad();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (RepairNpcData == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, RepairNpcData.StorageCheckerBoxSize);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, RepairNpcData.TargetSearchRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, RepairNpcData.TargetArrivalDistance);
        }
#endif
    }
}