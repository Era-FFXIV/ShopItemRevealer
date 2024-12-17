using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace ShopItemRevealer.UI.Windows
{
    /// <summary>
    /// Provides utility methods for ImGui operations.
    /// </summary>
    public static class ImGuiUtil
    {
        /// <summary>
        /// Creates a checkbox with a tooltip.
        /// </summary>
        /// <param name="label">The label of the checkbox.</param>
        /// <param name="description">The description for the tooltip.</param>
        /// <param name="current">The current value of the checkbox.</param>
        /// <param name="setter">The action to set the new value.</param>
        /// <param name="flags">The ImGui hovered flags.</param>
        /// <returns>True if the checkbox value changed; otherwise, false.</returns>
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

        /// <summary>
        /// Displays a tooltip when an item is hovered.
        /// </summary>
        /// <param name="description">The description for the tooltip.</param>
        /// <param name="flags">The ImGui hovered flags.</param>
        public static void HoverTooltip(string description, ImGuiHoveredFlags flags)
        {
            if (ImGui.IsItemHovered(flags))
                ImGui.SetTooltip(description);
        }

        /// <summary>
        /// Displays a help marker with a tooltip.
        /// </summary>
        /// <param name="desc">The description for the help marker.</param>
        public static void HelpMarker(string desc)
        {
            ImGui.SameLine();
            ImGuiComponents.HelpMarker(desc, FontAwesomeIcon.QuestionCircle);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(desc);
        }

        /// <summary>
        /// Draws a table with the specified data and columns.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="label">The label of the table.</param>
        /// <param name="data">The data to display in the table.</param>
        /// <param name="drawRow">The action to draw each row.</param>
        /// <param name="flags">The ImGui table flags.</param>
        /// <param name="columnTitles">The titles of the columns.</param>
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
