using _01_Works.CM._01_Scripts.NPC.NPC.Actions;
using _01_Works.CM._01_Scripts.NPC.NPC.Npcs;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Roles
{
    public class RepairNpcRole : INpcRole
    {
        public bool TryGetNextAction(Npc npc, out NpcAction action)
        {
            RepairNpc repairNpc = npc as RepairNpc;
            action = null;

            if (!repairNpc)
            {
                return false;
            }

            if (repairNpc.HasLoad)
            {
                if (repairNpc.TryGetBestRepairTarget(out Transform target))
                {
                    action = new RepairTowerAction(target);
                    return true;
                }

                return false;
            }

            if (repairNpc.CanTakeFromStorage())
            {
                action = new TakeWoodFromStorageAction();
                return true;
            }

            return false;
        }
    }
}