using Dalamud.Interface.Animation.EasingFunctions;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
