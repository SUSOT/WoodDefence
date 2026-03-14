using _01_Works.CM._01_Scripts.NPC.NPC;

namespace _01_Works.CM._01_Scripts.NPC
{
    public abstract class NpcState : INpcState
    {
        protected readonly Npc Owner;

        protected NpcState(Npc owner)
        {
            Owner = owner;
        }

        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void Exit()
        {
        }
    }

    public enum NpcStateType
    {
        Idle,
        Move,
        Work
    }
}
