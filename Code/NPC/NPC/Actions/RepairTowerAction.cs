using System.Collections;
using _01_Works.CM._01_Scripts.NPC.NPC.Npcs;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Actions
{
    public class RepairTowerAction : NpcAction
    {
        private readonly Transform _target;

        public RepairTowerAction(Transform target)
        {
            _target = target;
        }

        public override bool RequiresMovement => true;

        public override bool IsValid(Npc npc)
        {
            return npc is RepairNpc repairNpc &&
                   repairNpc.HasLoad &&
                   repairNpc.IsRepairTargetValid(_target);
        }

        public override Vector3 GetDestination(Npc npc)
        {
            return _target.position;
        }

        public override float GetArrivalDistance(Npc npc)
        {
            return ((RepairNpc)npc).TargetArrivalDistance;
        }

        public override IEnumerator Execute(Npc npc)
        {
            RepairNpc repairNpc = npc as RepairNpc;
            if (!repairNpc)
            {
                yield break;
            }

            while (repairNpc.HasLoad && repairNpc.IsRepairTargetValid(_target))
            {
                yield return new WaitForSeconds(repairNpc.DeliveryInterval);

                if (repairNpc.TryRepairOnce(_target) == false)
                {
                    break;
                }
            }

            yield return new WaitForSeconds(0.7f);

            if (repairNpc.HasLoad == false)
            {
                repairNpc.HideGauge();
            }
        }
    }
}