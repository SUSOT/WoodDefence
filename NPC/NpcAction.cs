using System.Collections;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC
{
    public abstract class NpcAction
    {
        public virtual bool RequiresMovement => false;

        public virtual bool IsValid(Npc npc)
        {
            return true;
        }

        public virtual Vector3 GetDestination(Npc npc)
        {
            return npc.transform.position;
        }

        public virtual float GetArrivalDistance(Npc npc)
        {
            return 0.1f;
        }

        public virtual bool HasReached(Npc npc)
        {
            return Vector2.Distance(npc.transform.position, GetDestination(npc)) <= GetArrivalDistance(npc);
        }

        public virtual void OnAssigned(Npc npc)
        {
        }

        public virtual void OnCancelled(Npc npc)
        {
        }

        public abstract IEnumerator Execute(Npc npc);
    }
}