using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC
{
    public class NpcFlipComponent : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void FaceDirection(Vector2 direction)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            if (Mathf.Approximately(direction.x, 0f))
            {
                return;
            }

            spriteRenderer.flipX = direction.x > 0f;
        }
    }
}
