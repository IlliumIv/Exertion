using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using SharpDX;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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
            if (Settings.MultiThreading)
                return new Job(nameof(Exertion), TickLogic);
            TickLogic();
            return null;
        }

        private List<string> ExertableSkills = new List<string>()
        {
            "infernal_cry",
            "seismic_cry",
            "intimidating_cry",
            "rallying_cry",
            "ancestral_cry",
        };

        private Dictionary<string, DateTime> LastCryTime = new Dictionary<string, DateTime>()
        {
            { "infernal_cry", new DateTime() },
            { "seismic_cry", new DateTime() },
            { "intimidating_cry", new DateTime() },
            { "rallying_cry", new DateTime() },
            { "ancestral_cry", new DateTime() },
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

        private ConcurrentDictionary<string, ExertedSkill> ExertedSkills = new ConcurrentDictionary<string, ExertedSkill>();
        private void TickLogic()
        {
            Core.ParallelRunner.Run(new Coroutine(BuildExertedSkills(), this, "Build Skills"));
        }

        private IEnumerator BuildExertedSkills()
        {
            var buffs = ingameState.Data.LocalPlayer.Buffs.Where(x => x.Name == "display_num_empowered_attacks");
            var actorSkills = ingameState.Data.LocalPlayer.GetComponent<Actor>().ActorSkills;
            foreach (var buff in buffs)
            {
                ushort skillId = ingameState.M.Read<ushort>(buff.Address + 0x38);
                foreach (var skill in actorSkills.Where(x => x.Id == skillId))
                    if (ExertableSkills.Any(x => x.Contains(skill.InternalName)))
                        if (ExertedSkills.ContainsKey(skill.InternalName))
                            ExertedSkills[skill.InternalName].Charges = buff.Charges;
                        else
                            ExertedSkills.TryAdd(skill.InternalName, new ExertedSkill(skill.InternalName, buff.Charges, skill.Id));
            }

            foreach (var skill in ExertedSkills)
                if (skill.Value.Charges > 0 && !buffs.Any(x => skill.Value.Id.Equals(ingameState.M.Read<ushort>(x.Address + 0x38))))
                    ExertedSkills[skill.Key].Charges = 0;

            yield return AutoExert();
        }

        private static IEnumerator UseCry(int CryDelay, int CryPause, Keys key)
        {
            yield return new WaitTime(CryDelay);
            Keyboard.KeyDown(key);
            yield return new WaitTime(20);
            Keyboard.KeyUp(key);
            yield return new WaitTime(CryPause);
        }

        private IEnumerator AutoExert()
        {
            while (GameController.IsLoading || !GameController.InGame)
                yield return new WaitTime(200);
            List<ActorSkill> actorSkills = ingameState.Data.LocalPlayer.GetComponent<Actor>().ActorSkills;
            if (!GameController.Area.CurrentArea.IsHideout && !GameController.Area.CurrentArea.IsTown)
            {
                if (Settings.AutoInfernal && ExertedSkills.TryGetValue("infernal_cry", out ExertedSkill infernal))
                    if (infernal.Charges == 0 && actorSkills.Any(x => x.InternalName == "infernal_cry"))
                        if (LastCryTime.TryGetValue("infernal_cry", out DateTime lastCry))
                        {
                            if ((DateTime.Now - lastCry).TotalMilliseconds > Settings.InfernalCooldown)
                            {
                                yield return UseCry(Settings.CryDelay, Settings.CryPause, Settings.AutoInfernalKey);
                                if (ExertedSkills["infernal_cry"].Charges != 0)
                                    LastCryTime["infernal_cry"] = DateTime.Now;
                            }
                        }

                if (Settings.AutoIntimidating && ExertedSkills.TryGetValue("intimidating_cry", out ExertedSkill intimidating))
                    if (intimidating.Charges == 0 && actorSkills.Any(x => x.InternalName == "intimidating_cry"))
                        if (LastCryTime.TryGetValue("intimidating_cry", out DateTime lastCry))
                        {
                            if ((DateTime.Now - lastCry).TotalMilliseconds > Settings.IntimidatingCooldown)
                            {
                                yield return UseCry(Settings.CryDelay, Settings.CryPause, Settings.AutoIntimidatingKey);
                                if (ExertedSkills["intimidating_cry"].Charges != 0)
                                    LastCryTime["intimidating_cry"] = DateTime.Now;
                            }

                        }

                if (Settings.AutoRallying && ExertedSkills.TryGetValue("rallying_cry", out ExertedSkill rallying))
                    if (rallying.Charges == 0 && actorSkills.Any(x => x.InternalName == "rallying_cry"))
                        if (LastCryTime.TryGetValue("rallying_cry", out DateTime lastCry))
                        {
                            if ((DateTime.Now - lastCry).TotalMilliseconds > Settings.RallyingCooldown)
                            {
                                yield return UseCry(Settings.CryDelay, Settings.CryPause, Settings.AutoRallyingKey);
                                if (ExertedSkills["rallying_cry"].Charges != 0)
                                    LastCryTime["rallying_cry"] = DateTime.Now;
                            }

                        }

                if (Settings.AutoAncestral && ExertedSkills.TryGetValue("ancestral_cry", out ExertedSkill ancestral))
                    if (ancestral.Charges == 0 && actorSkills.Any(x => x.InternalName == "ancestral_cry"))
                        if (LastCryTime.TryGetValue("ancestral_cry", out DateTime lastCry))
                        {
                            if ((DateTime.Now - lastCry).TotalMilliseconds > Settings.AncestralCooldown)
                            {
                                yield return UseCry(Settings.CryDelay, Settings.CryPause, Settings.AutoAncestralKey);
                                if (ExertedSkills["ancestral_cry"].Charges != 0)
                                    LastCryTime["ancestral_cry"] = DateTime.Now;
                            }
                        }

                if (Settings.AutoSeismic && ExertedSkills.TryGetValue("seismic_cry", out ExertedSkill seismic))
                    if (seismic.Charges == 0 && actorSkills.Any(x => x.InternalName == "seismic_cry"))
                        if (LastCryTime.TryGetValue("seismic_cry", out DateTime lastCry))
                            if ((DateTime.Now - lastCry).TotalMilliseconds > Settings.SeismicCooldown)
                            {
                                yield return UseCry(Settings.CryDelay, Settings.CryPause, Settings.AutoSeismicKey);
                                if (ExertedSkills["seismic_cry"].Charges != 0)
                                    LastCryTime["seismic_cry"] = DateTime.Now;
                            }
            }
            yield return new WaitTime(10);
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

        public class Keyboard
        {
            [DllImport("user32.dll")]
            private static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

            private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
            private const int KEYEVENTF_KEYUP = 0x0002;

            [DllImport("user32.dll")]
            public static extern bool BlockInput(bool fBlockIt);

            public static void KeyDown(Keys key)
            {
                keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            }

            public static void KeyUp(Keys key)
            {
                keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0); //0x7F
            }

            [DllImport("USER32.dll")]
            private static extern short GetKeyState(int nVirtKey);

            public static bool IsKeyDown(int nVirtKey)
            {
                return GetKeyState(nVirtKey) < 0;
            }
        }
    }
}
