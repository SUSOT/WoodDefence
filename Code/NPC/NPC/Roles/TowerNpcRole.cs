using _01_Works.CM._01_Scripts.NPC.NPC.Actions;
using _01_Works.CM._01_Scripts.NPC.NPC.Npcs;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Roles
{
    public class TowerNpcRole : INpcRole
    {
        public bool TryGetNextAction(Npc npc, out NpcAction action)
        {
            TowerNpc towerNpc = npc as TowerNpc;
            action = null;

            if (!towerNpc)
            {
                return false;
            }

            if (towerNpc.HasLoad)
            {
                if (towerNpc.TryGetBestFuelTarget(out Transform target))
                {
                    action = new SupplyFuelAction(target);
                    return true;
                }

                return false;
            }

            if (towerNpc.CanTakeFromStorage())
            {
                action = new TakeWoodFromStorageAction();
                return true;
            }

            return false;
        }
    }
}