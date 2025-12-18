using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer.Game.Addons
{
    internal class AddonFateProgress : IDisposable
    {
        private readonly ShopItemRevealer plugin;
        private bool IsOpen { get; set; } = false;
        public AddonFateProgress(ShopItemRevealer plugin)
        {
            this.plugin = plugin;
            AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, ["FateProgress"], OnPostRefresh);
            AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, ["FateProgress"], OnFinalize);
        }
        public void Dispose()
        {
            AddonLifecycle.UnregisterListener(AddonEvent.PostRefresh, ["FateProgress"], OnPostRefresh);
            AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, ["FateProgress"], OnFinalize);
        }
        private void OnPostRefresh(AddonEvent e, AddonArgs args)
        {
            if (IsOpen && PlayerManager.HasFateRanksInitialized) return;
            unsafe
            {
                var gameAddon = GameGui.GetAddonByName("FateProgress", 1);
                var addon = (AtkUnitBase*)gameAddon.Address;
                if (addon == null)
                {
                    return;
                }
                if (addon->AtkValuesCount == 0)
                {
                    return;
                }
                if (addon->AtkValues[0].UInt != 268)
                {
                    return;
                }
                Log.Debug($"AddonFateProgress: {addon->AtkValues[0].UInt}");
                var fateAgent = AgentFateProgress.Instance();
                if (fateAgent == null)
                {
                    return;
                }
                if (PlayerManager.HasFateRanksInitialized)
                {
                    PlayerManager.FateRanks.Clear();
                }
                foreach (var fate in fateAgent->Tabs)
                {
                    foreach (var entry in fate.Zones)
                    {
                        if (entry.ZoneName.ToString() == "") continue;
                        Log.Debug($"Id: {entry.TerritoryTypeId} - Name: {entry.ZoneName} - Rank: {entry.CurrentRank}");
                        PlayerManager.AddFateRank(entry.TerritoryTypeId, entry.CurrentRank);
                    }
                }
            }
            if (PlayerManager.HasFateRanksInitialized)
            {
                Log.Debug("Fate Ranks Initialized");
                FateRank.Save(PlayerManager.FateRanks, PlayerState.ContentId);
            }
        }
        private void OnFinalize(AddonEvent type, AddonArgs args)
        {
            IsOpen = false;
            Log.Debug("AddonFateProgress Finalize");
        }
    }
}
