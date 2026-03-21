using System.Collections;
using _01_Works.CM._01_Scripts.NPC.NPC.Npcs;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Actions
{
    public class StoreWoodAction : NpcAction
    {
        public override bool RequiresMovement => true;

        public override bool IsValid(Npc npc)
        {
            return npc is TreeNpc treeNpc && treeNpc.HasLoad && treeNpc.StoragePos;
        }

        public override Vector3 GetDestination(Npc npc)
        {
            return ((TreeNpc)npc).StoragePos.position;
        }

        public override bool HasReached(Npc npc)
        {
            return ((TreeNpc)npc).IsInStorageRange();
        }

        public override IEnumerator Execute(Npc npc)
        {
            TreeNpc treeNpc = npc as TreeNpc;
            if (!treeNpc)
            {
                yield break;
            }

            while (treeNpc.HasLoad)
            {
                yield return new WaitForSeconds(treeNpc.SaveInterval);

                if (treeNpc.TryStoreOneLoadStep() == false)
                {
                    break;
                }
            }

            yield return new WaitForSeconds(treeNpc.SaveInterval);

            if (treeNpc.HasLoad == false)
            {
                treeNpc.HideGauge();
            }
        }
    }
}