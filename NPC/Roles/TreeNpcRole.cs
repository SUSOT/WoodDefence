using _01_Works.CM._01_Scripts.NPC.NPC.Actions;
using _01_Works.CM._01_Scripts.NPC.NPC.Npcs;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Roles
{
    public class TreeNpcRole : INpcRole
    {
        public bool TryGetNextAction(Npc npc, out NpcAction action)
        {
            TreeNpc treeNpc = npc as TreeNpc;
            action = null;

            if (!treeNpc)
            {
                return false;
            }

            if (treeNpc.IsLoadFull)
            {
                action = new StoreWoodAction();
                return true;
            }

            if (treeNpc.TryAcquireTreeTarget(out Transform target))
            {
                action = new ChopTreeAction(target);
                return true;
            }

            return false;
        }
    }
}