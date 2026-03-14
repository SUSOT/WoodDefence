using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC
{
    public class NpcAnimationComponent : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        public void PlayAnimation(NpcAnimationType animationType)
        {
            if (!animator) return;

            animator.Play(animationType.ToString());
        }
    }

    public enum NpcAnimationType
    {
        Walk,
        Idle,
        Chopping
    }
}