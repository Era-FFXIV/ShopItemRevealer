using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace ShopItemRevealer.UI.Windows
{
    internal class LandingWindow(string name) : Window(name, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize), IDisposable
    {
        public void Dispose()
        {
        }
        public override void PreDraw()
        {
            ImGui.SetNextWindowSize(new Vector2(300, 100), ImGuiCond.Always);
            Flags |= ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        }
        public override void Draw()
        {
            ImGui.TextWrapped("This plugin opens its own window when speaking with a shop vendor and has no UI here.");
            ImGui.Separator();
            if (ImGui.Button("Open Docs"))
            {
                UIManager.OpenUrl("https://github.com/Era-FFXIV/ShopItemRevealer");
            }
            ImGui.SameLine();
            if (ImGui.Button("Close"))
            {
                Toggle();
            }
        }
    }
}
