using System.Collections;
using _01_Works.CM._01_Scripts.NPC.NPC.Npcs;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Actions
{
    public class SupplyFuelAction : NpcAction
    {
        private readonly Transform _target;

        public SupplyFuelAction(Transform target)
        {
            _target = target;
        }

        public override bool RequiresMovement => true;

        public override bool IsValid(Npc npc)
        {
            return npc is TowerNpc towerNpc &&
                   towerNpc.HasLoad &&
                   towerNpc.IsFuelTargetValid(_target);
        }

        public override Vector3 GetDestination(Npc npc)
        {
            return _target.position;
        }

        public override float GetArrivalDistance(Npc npc)
        {
            return ((TowerNpc)npc).TargetArrivalDistance;
        }

        public override IEnumerator Execute(Npc npc)
        {
            TowerNpc towerNpc = npc as TowerNpc;
            if (!towerNpc)
            {
                yield break;
            }

            while (towerNpc.HasLoad && towerNpc.IsFuelTargetValid(_target))
            {
                yield return new WaitForSeconds(towerNpc.DeliveryInterval);

                if (towerNpc.TrySupplyFuelOnce(_target) == false)
                {
                    break;
                }
            }

            yield return new WaitForSeconds(0.7f);

            if (towerNpc.HasLoad == false)
            {
                towerNpc.HideGauge();
            }
        }
    }
}