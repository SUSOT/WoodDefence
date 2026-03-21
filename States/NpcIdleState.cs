using _01_Works.CM._01_Scripts.NPC.NPC;

namespace _01_Works.CM._01_Scripts.NPC.States
{
    public class NpcIdleState : NpcState
    {
        public NpcIdleState(Npc owner) : base(owner)
        {
        }

        public override void Enter()
        {
            Owner.StopMovement();
            Owner.AnimationCompo.PlayAnimation(NpcAnimationType.Idle);
        }

        public override void Update()
        {
            if (Owner.CurrentAction == null)
            {
                Owner.TryAcquireNextAction();
            }
        }
    }
}