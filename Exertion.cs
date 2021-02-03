using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using nuVector2 = System.Numerics.Vector2;
using nuVector4 = System.Numerics.Vector4;

namespace Exertion
{
    public class Exertion : BaseSettingsPlugin<ExertionSettings>
    {
        private IngameState ingameState;
        private RectangleF windowArea;

        public override bool Initialise()
        {
            base.Initialise();
            Name = "Exertion";
            ingameState = GameController.Game.IngameState;
            windowArea = GameController.Window.GetWindowRectangle();
            return true;
        }

        public override void OnLoad()
        {
            CanUseMultiThreading = true;
        }

        public override Job Tick()
        {
            if (Settings.MultiThreading)    //custom setting ToggleNode
            {
                return new Job(nameof(Exertion), TickLogic);
                //return GameController.MultiThreadManager.AddJob(TickLogic, nameof(HealthBars));   //another way to enable multiprocessing
            }

            TickLogic();
            return null;
        }

        private List<string> ExertableSkills = new List<string>()
        {
            "infernal_cry",
            "seismic_cry",
            "intimidating_cry",
            "rallying_cry",
            "ancestral_cry"
        };

        private class ExertedSkill
        {
            public ExertedSkill(string name, int charges, int id)
            {
                Name = name;
                Charges = charges;
                Id = id;
            }
            public string Name;
            public int Charges;
            public int Id;
        }

        private Dictionary<string, ExertedSkill> ExertedSkills = new Dictionary<string, ExertedSkill>();
        private void TickLogic()
        {
            var buffs = ingameState.Data.LocalPlayer.Buffs;
            var actorSkills = ingameState.Data.LocalPlayer.GetComponent<Actor>().ActorSkills;
            foreach (var buff in buffs)
            {
                ushort skillId = ingameState.M.Read<ushort>(buff.Address + 0x38);
                foreach (var skill in actorSkills.Where(x => x.Id == skillId))
                {
                    if (ExertableSkills.Contains(skill.InternalName))
                    {
                        if (ExertedSkills.ContainsKey(skill.InternalName))
                        {
                            ExertedSkills[skill.InternalName] = new ExertedSkill(skill.InternalName, buff.Charges, skill.Id);
                        }
                        else
                        {
                            ExertedSkills.Add(skill.InternalName, new ExertedSkill(skill.InternalName, buff.Charges, skill.Id));
                        }
                    }
                }
            }
            foreach (var skill in ExertedSkills)
                if (skill.Value.Charges > 0 && !buffs.Any(x => skill.Value.Id.Equals(ingameState.M.Read<ushort>(x.Address + 0x38))))
                    ExertedSkills[skill.Key].Charges = 0;
        }

        public override void Render()
        {
            float scl = Settings.IconScale;
            int iconPad = (int)(12 * Settings.IconScale);
            var exertedSkills = ExertedSkills.Where(x => x.Value.Charges > 0);
            int skillCount = exertedSkills.Count() - 1;
            int originStart = 0;
            if (skillCount > 0) originStart -= iconPad * skillCount;
            var origin = windowArea.Center.Translate((float)Settings.XAdjust, (float)Settings.YAdjust);
            if (ExertedSkills.Count > 0)
                foreach (var skill in exertedSkills)
                {
                    Color iconColour = Color.White;
                    switch (skill.Value.Name)
                    {
                        case "infernal_cry":
                            iconColour = Settings.InfernalCryColour;
                            break;
                        case "seismic_cry":
                            iconColour = Settings.SeismicCryColour;
                            break;
                        case "intimidating_cry":
                            iconColour = Settings.IntimidatingCryColour;
                            break;
                        case "rallying_cry":
                            iconColour = Settings.RallyingCryColour;
                            break;
                        case "ancestral_cry":
                            iconColour = Settings.AncestralCryColour;
                            break;
                    }
                    RectangleF iconRect = new RectangleF(origin.X + originStart - iconPad, origin.Y, iconPad * 2, iconPad * 2);
                    if (Settings.DrawText)
                    {
                        Graphics.DrawText($"{skill.Value.Charges}", iconRect.Center.Translate(1, -7), Color.Black, FontAlign.Center);
                        Graphics.DrawText($"{skill.Value.Charges}", iconRect.Center.Translate(0, -8), Color.White, FontAlign.Center);
                    }
                    Graphics.DrawImage("menu-colors.png", iconRect, iconColour);
                    originStart += (int)(24 * scl);
                }

        }
    }
}
