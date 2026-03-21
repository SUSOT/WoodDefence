namespace _01_Works.CM._01_Scripts.NPC.NPC
{
    public interface INpcRole
    {
        bool TryGetNextAction(Npc npc, out NpcAction action);
    }
}