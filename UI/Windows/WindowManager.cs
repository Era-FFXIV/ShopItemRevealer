using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace ShopItemRevealer.UI.Windows
{
    internal class WindowManager : IManager
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
        public LandingWindow LandingWindow { get; private set; } = null!;

        public static float TextWidth(string text) => ImGui.CalcTextSize(text).X + ItemSpacing.X;
        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();
            Dalamud.PluginInterface.UiBuilder.Draw -= MainWindow.Draw;
            Dalamud.PluginInterface.UiBuilder.OpenMainUi -= MainWindow.HandleMainUiOpen;
        }
        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            MainWindow = new("Shop Item Revealer", plugin);
            WindowSystem.AddWindow(MainWindow);
            LandingWindow = new("Landing Window - Shop Item Revealer");
            WindowSystem.AddWindow(LandingWindow);
            Dalamud.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
            Dalamud.PluginInterface.UiBuilder.OpenMainUi += LandingWindow.Toggle;
        }
    }
}
