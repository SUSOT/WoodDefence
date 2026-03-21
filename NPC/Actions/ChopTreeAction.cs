using System.Collections;
using _01_Works.CM._01_Scripts.NPC.NPC.Npcs;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC.Actions
{
    public class ChopTreeAction : NpcAction
    {
        private readonly Transform _target;

        public ChopTreeAction(Transform target)
        {
            _target = target;
        }

        public override bool RequiresMovement => true;

        public override bool IsValid(Npc npc)
        {
            return npc is TreeNpc treeNpc &&
                   treeNpc.IsLoadFull == false &&
                   treeNpc.IsTreeTargetValid(_target);
        }

        public override Vector3 GetDestination(Npc npc)
        {
            return _target.position;
        }

        public override float GetArrivalDistance(Npc npc)
        {
            return ((TreeNpc)npc).TreeArrivalDistance;
        }

        public override IEnumerator Execute(Npc npc)
        {
            TreeNpc treeNpc = npc as TreeNpc;
            if (!treeNpc)
            {
                yield break;
            }

            int performedSteps = 0;

            while (treeNpc.IsLoadFull == false && treeNpc.IsTreeTargetValid(_target))
            {
                if (performedSteps > 0)
                {
                    yield return new WaitForSeconds(treeNpc.FellingInterval);
                }

                treeNpc.AnimationCompo.PlayAnimation(NpcAnimationType.Chopping);
                yield return new WaitForSeconds(0.41f);

                if (treeNpc.TryChopOneLoadStep(_target) == false)
                {
                    yield break;
                }

                yield return new WaitForSeconds(0.3f);
                treeNpc.AnimationCompo.PlayAnimation(NpcAnimationType.Idle);
                performedSteps++;
            }
        }
    }
}