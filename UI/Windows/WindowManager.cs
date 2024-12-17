using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace ShopItemRevealer.UI.Windows
{
    /// <summary>
    /// Manages the windows for the ShopItemRevealer plugin.
    /// </summary>
    public class WindowManager : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;
        public static Vector2 IconSize = ImGuiHelpers.ScaledVector2(40, 40);
        public static Vector2 SmallIconSize = ImGuiHelpers.ScaledVector2(20, 20);
        public static Vector2 LineIconSize = new(ImGui.GetFrameHeight(), 0);
        public static Vector2 ItemSpacing = ImGui.GetStyle().ItemSpacing;
        public static Vector2 FramePadding = ImGui.GetStyle().FramePadding;
        public static Vector2 IconButtonSize = new(ImGui.GetFrameHeight(), 0);
        public static float SelectorWidth = Math.Max(ImGui.GetWindowSize().X * 0.15f, 150 * Scale);
        public static Vector2 HorizontalSpace = Vector2.Zero;
        public static float TextHeight = ImGui.GetTextLineHeight();
        public static float TextHeightSpacing = ImGui.GetTextLineHeightWithSpacing();
        public static float Scale = ImGuiHelpers.GlobalScale;

        internal WindowSystem WindowSystem { get; private set; } = new(ShopItemRevealer.Name);
        public MainWindow MainWindow { get; private set; } = null!;

        /// <summary>
        /// Calculates the width of the specified text.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <returns>The width of the text.</returns>
        public static float TextWidth(string text) => ImGui.CalcTextSize(text).X + ItemSpacing.X;

        /// <summary>
        /// Disposes the window manager and removes all windows.
        /// </summary>
        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();
            Dalamud.PluginInterface.UiBuilder.Draw -= MainWindow.Draw;
            Dalamud.PluginInterface.UiBuilder.OpenMainUi -= MainWindow.HandleMainUiOpen;
        }

        /// <summary>
        /// Initializes the window manager with the specified plugin.
        /// </summary>
        /// <param name="plugin">The plugin instance.</param>
        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            MainWindow = new("Shop Item Revealer", plugin);
            WindowSystem.AddWindow(MainWindow);
            Dalamud.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
            Dalamud.PluginInterface.UiBuilder.OpenMainUi += MainWindow.HandleMainUiOpen;
        }
    }
}
