using System.Collections.Generic;
using _01_Works.CM._01_Scripts.NPC.Data;
using _01_Works.CM._01_Scripts.NPC.NPC.Roles;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Npcs
{
    public class TowerNpc : StorageCarrierNpc
    {
        [field: SerializeField] public TowerNpcDataSO TowerNpcData { get; private set; }
        [SerializeField] private LayerMask whatIsTower;

        protected override StorageCarrierNpcDataSO CarrierData => TowerNpcData;

        protected override void Awake()
        {
            base.Awake();
            Role = new TowerNpcRole();
        }

        protected override void InitializeNpc()
        {
            InitializeCarrierRuntime(GameManager.Instance.UnderGroundStoragePos);
        }

        public bool TryGetBestFuelTarget(out Transform target)
        {
            target = null;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, TargetSearchRadius, whatIsTower);
            if (hits == null || hits.Length == 0)
            {
                return false;
            }

            float lowestFuel = float.MaxValue;
            HashSet<Transform> visitedTargets = new HashSet<Transform>();

            foreach (var hit in hits)
            {
                if (!hit)
                {
                    continue;
                }

                if (!TryResolveFuelTarget(hit, out var candidate))
                {
                    continue;
                }

                if (!candidate.gameObject.activeInHierarchy || !visitedTargets.Add(candidate))
                {
                    continue;
                }

                IFuelable fuelable = candidate.GetComponent<IFuelable>();
                if (fuelable.GetIsMaxFuel())
                {
                    continue;
                }

                float currentFuel = fuelable.GetFuel();
                if (currentFuel < lowestFuel)
                {
                    lowestFuel = currentFuel;
                    target = candidate;
                }
            }

            return target;
        }

        public bool IsFuelTargetValid(Transform target)
        {
            if (!target || !target.gameObject.activeInHierarchy)
            {
                return false;
            }

            if (!target.TryGetComponent(out IFuelable fuelable))
            {
                return false;
            }

            return !fuelable.GetIsMaxFuel();
        }

        public bool TrySupplyFuelOnce(Transform target)
        {
            if (!HasLoad || !IsFuelTargetValid(target))
            {
                return false;
            }

            IFuelable fuelable = target.GetComponent<IFuelable>();

            if (!TryConsumeOneLoadStep())
            {
                return false;
            }

            fuelable.FillFuel(WoodAmountPerStep * 3);
            return true;
        }

        private bool TryResolveFuelTarget(Collider2D hit, out Transform target)
        {
            target = null;

            if (hit.TryGetComponent(out IFuelable _))
            {
                target = hit.transform;
                return true;
            }

            if (hit.transform.parent && hit.transform.parent.TryGetComponent(out IFuelable _))
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
            if (TowerNpcData == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, TowerNpcData.StorageCheckerBoxSize);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, TowerNpcData.TargetSearchRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, TowerNpcData.TargetArrivalDistance);
        }
#endif
    }
}
