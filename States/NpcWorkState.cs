using System.Collections;
using _01_Works.CM._01_Scripts.NPC.NPC;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.States
{
    public class NpcWorkState : NpcState
    {
        private Coroutine _workRoutine;

        public NpcWorkState(Npc owner) : base(owner)
        {
        }

        public override void Enter()
        {
            Owner.StopMovement();
            Owner.AnimationCompo.PlayAnimation(NpcAnimationType.Idle);

            if (Owner.CurrentAction == null)
            {
                Owner.TransitionState(NpcStateType.Idle);
                return;
            }

            if (!Owner.CurrentAction.IsValid(Owner))
            {
                Owner.CancelCurrentAction();
                return;
            }

            _workRoutine = Owner.StartCoroutine(ExecuteRoutine());
        }

        public override void Exit()
        {
            if (_workRoutine != null)
            {
                Owner.StopCoroutine(_workRoutine);
                _workRoutine = null;
            }
        }

        private IEnumerator ExecuteRoutine()
        {
            NpcAction runningAction = Owner.CurrentAction;
            yield return runningAction.Execute(Owner);
            _workRoutine = null;

            if (Owner.CurrentAction == runningAction)
            {
                Owner.CompleteCurrentAction();
            }
        }
    }
}