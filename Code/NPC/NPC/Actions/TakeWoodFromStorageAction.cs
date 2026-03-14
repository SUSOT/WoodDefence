using System.Collections;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Actions
{
    public class TakeWoodFromStorageAction : NpcAction
    {
        public override bool RequiresMovement => true;

        public override bool IsValid(Npc npc)
        {
            return npc is StorageCarrierNpc carrier && carrier.StoragePos;
        }

        public override Vector3 GetDestination(Npc npc)
        {
            return ((StorageCarrierNpc)npc).StoragePos.position;
        }

        public override bool HasReached(Npc npc)
        {
            return ((StorageCarrierNpc)npc).IsInStorageRange();
        }

        public override IEnumerator Execute(Npc npc)
        {
            StorageCarrierNpc carrier = npc as StorageCarrierNpc;
            if (!carrier)
            {
                yield break;
            }

            while (carrier.IsLoadFull == false)
            {
                yield return new WaitForSeconds(carrier.TakeInterval);

                if (carrier.TryTakeOneLoadStepFromStorage() == false)
                {
                    break;
                }
            }

            if (carrier.HasLoad == false)
            {
                carrier.HideGauge();
            }
        }
    }
}