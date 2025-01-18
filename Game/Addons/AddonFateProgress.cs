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
        public AddonFateProgress(ShopItemRevealer plugin)
        {
            this.plugin = plugin;
            Dalamud.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, ["FateProgress"], OnPostRefresh);
            Dalamud.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, ["FateProgress"], OnFinalize);
        }
        public void Dispose()
        {
            Dalamud.AddonLifecycle.UnregisterListener(AddonEvent.PostRefresh, ["FateProgress"], OnPostRefresh);
            Dalamud.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, ["FateProgress"], OnFinalize);
        }
        private void OnPostRefresh(AddonEvent e, AddonArgs args)
        {
            if (PlayerManager.HasFateRanksInitialized)
            {
                return;
            }
            unsafe
            {
                var addon = (AtkUnitBase*)args.Addon;
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
                Dalamud.Log.Debug($"AddonFateProgress: {addon->AtkValues[0].UInt}");
                var fateAgent = AgentFateProgress.Instance();
                if (fateAgent == null)
                {
                    return;
                }
                foreach (var fate in fateAgent->Tabs)
                {
                    foreach (var entry in fate.Zones)
                    {
                        if (entry.ZoneName.ToString() == "") continue;
                        Dalamud.Log.Debug($"Id: {entry.TerritoryTypeId} - Name: {entry.ZoneName} - Rank: {entry.CurrentRank}");
                        PlayerManager.AddFateRank(entry.TerritoryTypeId, entry.CurrentRank);
                    }
                }
            }
            if (PlayerManager.HasFateRanksInitialized)
            {
                Dalamud.Log.Debug("Fate Ranks Initialized");
                FateRank.Save(PlayerManager.FateRanks, Dalamud.ClientState.LocalContentId);
            }
        }
        private void OnFinalize(AddonEvent type, AddonArgs args)
        {
            Dalamud.Log.Debug("AddonFateProgress Finalize");
        }
    }
}
