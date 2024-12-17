using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Dalamud.Plugin;

namespace ShopItemRevealer
{
    /// <summary>
    /// Provides access to various Dalamud services.
    /// </summary>
    public class Dalamud
    {
        /// <summary>
        /// Gets the Dalamud plugin interface.
        /// </summary>
        [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud texture provider.
        /// </summary>
        [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud command manager.
        /// </summary>
        [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud plugin log.
        /// </summary>
        [PluginService] public static IPluginLog Log { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud game GUI.
        /// </summary>
        [PluginService] public static IGameGui GameGui { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud data manager.
        /// </summary>
        [PluginService] public static IDataManager DataManager { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud addon lifecycle.
        /// </summary>
        [PluginService] public static IAddonLifecycle AddonLifecycle { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud client state.
        /// </summary>
        [PluginService] public static IClientState ClientState { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud framework.
        /// </summary>
        [PluginService] public static IFramework Framework { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud condition.
        /// </summary>
        [PluginService] public static ICondition Condition { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud target manager.
        /// </summary>
        [PluginService] public static ITargetManager Target { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud chat GUI.
        /// </summary>
        [PluginService] public static IChatGui ChatGui { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud object table.
        /// </summary>
        [PluginService] public static IObjectTable ObjectTable { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud game interop provider.
        /// </summary>
        [PluginService] public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud context menu.
        /// </summary>
        [PluginService] public static IContextMenu ContextMenu { get; private set; } = null!;
    }
}
