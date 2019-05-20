namespace MonsterMatch
{
    public sealed class DialogEndedEvent
    {
        public DatingProfileItem profile { get; set; }
        public DialogueItem item { get; set; }
        public bool succeeded { get; set; }
    }
}