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


        [Menu("Delay before cry use")] public RangeNode<int> CryDelay { get; set; } = new RangeNode<int>(0, 0, 1500);
        [Menu("Delay after cry use")] public RangeNode<int> CryPause { get; set; } = new RangeNode<int>(0, 0, 1500);
        [Menu("Automatically use Infernal Cry")] public ToggleNode AutoInfernal { get; set; } = new ToggleNode(true);
        [Menu("Infernal Cry keybind")] public HotkeyNode AutoInfernalKey { get; set; } = new HotkeyNode(Keys.Q);
        [Menu("Infernal Cry cooldown")] public RangeNode<int> InfernalCooldown { get; set; } = new RangeNode<int>(8020, 0, 15000);
        [Menu("Automatically use Seismic Cry")] public ToggleNode AutoSeismic { get; set; } = new ToggleNode(true);
        [Menu("Seismic Cry Bind")] public HotkeyNode AutoSeismicKey { get; set; } = new HotkeyNode(Keys.W);
        [Menu("Seismic Cry cooldown")] public RangeNode<int> SeismicCooldown { get; set; } = new RangeNode<int>(8020, 0, 15000);
        [Menu("Automatically use Rallying Cry ")] public ToggleNode AutoRallying { get; set; } = new ToggleNode(true);
        [Menu("Rallying Cry Bind")] public HotkeyNode AutoRallyingKey { get; set; } = new HotkeyNode(Keys.E);
        [Menu("Rallying Cry cooldown")] public RangeNode<int> RallyingCooldown { get; set; } = new RangeNode<int>(8020, 0, 15000);
        [Menu("Automatically use Intimidating Cry")] public ToggleNode AutoIntimidating { get; set; } = new ToggleNode(true);
        [Menu("Intimidating Cry Bind")] public HotkeyNode AutoIntimidatingKey { get; set; } = new HotkeyNode(Keys.R);
        [Menu("Intimidating Cry cooldown")] public RangeNode<int> IntimidatingCooldown { get; set; } = new RangeNode<int>(8020, 0, 15000);
        [Menu("Automatically use Ancestral Cry")] public ToggleNode AutoAncestral { get; set; } = new ToggleNode(true);
        [Menu("Ancestral Cry Bind")] public HotkeyNode AutoAncestralKey { get; set; } = new HotkeyNode(Keys.T);
        [Menu("Ancestral Cry cooldown")] public RangeNode<int> AncestralCooldown { get; set; } = new RangeNode<int>(8020, 0, 15000);

        public ToggleNode MultiThreading { get; set; } = new ToggleNode(true);
        public ToggleNode Enable { get; set; } = new ToggleNode(true);

    }
}