using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace ShopItemRevealer.UI.Windows
{
    internal static class ImGuiUtil
    {
        // credit to OtterGui
        public static bool Checkbox(string label, string description, bool current, Action<bool> setter,
            ImGuiHoveredFlags flags = ImGuiHoveredFlags.None)
        {
            var tmp = current;
            var result = ImGui.Checkbox(label, ref tmp);
            HoverTooltip(description, flags);
            if (!result || tmp == current)
                return false;

            setter(tmp);
            return true;
        }

        public static void HoverTooltip(string description, ImGuiHoveredFlags flags)
        {
            if (ImGui.IsItemHovered(flags))
                ImGui.SetTooltip(description);
        }
        public static void HelpMarker(string desc)
        {
            ImGui.SameLine();
            ImGuiComponents.HelpMarker(desc, FontAwesomeIcon.QuestionCircle);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(desc);
        }
        // lifted from Dalamud, modified for sorting
        public static void DrawTable<T>(string label, IEnumerable<T> data, Action<T> drawRow, ImGuiTableFlags flags = ImGuiTableFlags.None, params string[] columnTitles)
        {
            if (columnTitles.Length == 0)
            {
                return;
            }

            using ImRaii.IEndObject endObject = ImRaii.Table(label, columnTitles.Length, flags);
            if (!endObject)
            {
                return;
            }

            foreach (string label2 in columnTitles)
            {
                ImGui.TableNextColumn();
                ImGui.TableHeader(label2);
            }

            foreach (T datum in data)
            {
                ImGui.TableNextRow();
                drawRow(datum);
            }
        }
    }
}
