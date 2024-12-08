namespace ShopItemRevealer.Game.Player
{
    internal enum CompletionStatus
    {
        Incomplete,
        Complete,
        NotLoaded
    }
    internal class Achievement : IGameInfo
    {
        public string Name { get; private set; }
        public uint Id { get; private set; }

        public CompletionStatus CompletionStatus { get; private set; } = CompletionStatus.Incomplete;
        public override string ToString()
        {
            return $"Achievement: {Name}";
        }
        public Achievement(uint id)
        {
            Id = id;
            Name = SheetManager.AchievementSheet.GetRow(Id).Name.ExtractText();
        }

        public unsafe CompletionStatus IsComplete()
        {
            if (CompletionStatus == CompletionStatus.Complete)
            {
                return CompletionStatus.Complete;
            }
            else
            {
                var instance = FFXIVClientStructs.FFXIV.Client.Game.UI.Achievement.Instance();
                if (!instance->IsLoaded())
                {
                    // player has not loaded achievements yet, notify the player to try again later
                    return CompletionStatus.NotLoaded;
                }
                else if (instance->IsComplete((int)Id))
                {
                    CompletionStatus = CompletionStatus.Complete;
                    return CompletionStatus.Complete;
                }
                else
                {
                    return CompletionStatus.Incomplete;
                }
            }
        }
    }
}
