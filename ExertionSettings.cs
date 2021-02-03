using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;
using System.Windows.Forms;

namespace Exertion
{
    public class ExertionSettings : ISettings
    {
        [Menu("Icons X Coordinate Adjust")] public RangeNode<int> XAdjust { get; set; } = new RangeNode<int>(0, -1280, 1280);
        [Menu("Icons Y Coordinate Adjust")] public RangeNode<int> YAdjust { get; set; } = new RangeNode<int>(0, -1280, 1280);
        [Menu("Ancestral Cry Icon Colour")] public ColorNode AncestralCryColour { get; set; } = new ColorNode(new Color(100, 40, 40, 240));
        [Menu("Infernal Cry Icon Colour")] public ColorNode InfernalCryColour { get; set; } = new ColorNode(new Color(200, 0, 0, 240));
        [Menu("Intimidating Cry Icon Colour")] public ColorNode IntimidatingCryColour { get; set; } = new ColorNode(new Color(255, 140, 40, 240));
        [Menu("Rallying Cry Icon Colour")] public ColorNode RallyingCryColour { get; set; } = new ColorNode(new Color(120, 200, 40, 240));
        [Menu("Seismic Cry Icon Colour")] public ColorNode SeismicCryColour { get; set; } = new ColorNode(new Color(180, 180, 40, 240));
        [Menu("Icon Size Scale")] public RangeNode<float> IconScale { get; set; } = new RangeNode<float>(1.0f, 0f, 5f);
        [Menu("Show exertion count on icons")] public ToggleNode DrawText { get; set; } = new ToggleNode(true);
        public ToggleNode MultiThreading { get; set; } = new ToggleNode(true);
        public ToggleNode Enable { get; set; } = new ToggleNode(true);

    }
}