using _01_Works.CM._01_Scripts.NPC.NPC;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.States
{
    public class NpcMoveState : NpcState
    {
        public NpcMoveState(Npc owner) : base(owner)
        {
        }

        public override void Enter()
        {
            Owner.AnimationCompo.PlayAnimation(NpcAnimationType.Walk);
        }

        public override void FixedUpdate()
        {
            if (Owner.CurrentAction == null)
            {
                Owner.TransitionState(NpcStateType.Idle);
                return;
            }

            if (Owner.CurrentAction.IsValid(Owner) == false)
            {
                Owner.CancelCurrentAction();
                return;
            }

            Vector3 destination = Owner.CurrentAction.GetDestination(Owner);
            Vector2 direction = Owner.GetDirectionTo(destination);

            Owner.MoveHorizontally(direction, Owner.MoveSpeed);

            if (Owner.CurrentAction.HasReached(Owner))
            {
                Owner.TransitionState(NpcStateType.Work);
            }
        }
    }
}