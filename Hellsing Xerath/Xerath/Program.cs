using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace Xerath
{
    public class Program
    {
        private Items.Item
        FarsightOrb = new Items.Item(3342, 4000f),
        ScryingOrb = new Items.Item(3363, 3500f);
        private static string News = "Welcome, HotshotGG";

        public Vector3 Rtarget;
        public const string CHAMP_NAME = "Xerath";
        private static readonly Obj_AI_Hero player = ObjectManager.Player;

        public static bool HasIgnite { get; private set; }

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            // Validate champ
            if (player.ChampionName != CHAMP_NAME)
                return;
            Game.PrintChat("<font size='24'>Xerath by</font><font size='24'> <font color='#ff0000'>Hellsing</font>");
            // Clear the console
            Utils.ClearConsole();

            // Initialize classes
            SpellManager.Initialize();
            Config.Initialize();

            // Check if the player has ignite
            HasIgnite = player.GetSpellSlot("SummonerDot") != SpellSlot.Unknown;

            // Initialize damage indicator
            Utility.HpBarDamageIndicator.DamageToUnit = Damages.GetTotalDamage;
            Utility.HpBarDamageIndicator.Color = System.Drawing.Color.Aqua;
            Utility.HpBarDamageIndicator.Enabled = true;

            // Listend to some other events
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            //Spellbook.OnCastSpell += Spellbook_OnCastSpell;

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            // Always active stuff, ignite and stuff :P
            ActiveModes.OnPermaActive();

            if (Config.KeyLinks["comboActive"].Value.Active)
                ActiveModes.OnCombo();
            if (Config.KeyLinks["harassActive"].Value.Active)
                ActiveModes.OnHarass();
        }

                
            
        

        private static void Drawing_OnDraw(EventArgs args)
        {
            // Draw all circles except for R
            foreach (var circleLink in Config.CircleLinks)
            {
                if (circleLink.Value.Value.Active && circleLink.Key != "drawRangeR")
                    Render.Circle.DrawCircle(player.Position, circleLink.Value.Value.Radius, circleLink.Value.Value.Color);
            }

            // Draw R range
            if (Config.CircleLinks["drawRangeR"].Value.Active && SpellManager.R.Level > 0) 
                Render.Circle.DrawCircle(player.Position, SpellManager.R.Range, Config.CircleLinks["drawRangeR"].Value.Color);
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            // Draw R on minimap
            if (Config.CircleLinks["drawRangeR"].Value.Active && SpellManager.R.Level > 0)
                Utility.DrawCircle(player.Position, SpellManager.R.Range, Config.CircleLinks["drawRangeR"].Value.Color, 5, 30, true);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.BoolLinks["miscGapcloseE"].Value && SpellManager.E.IsReady() && SpellManager.E.IsInRange(gapcloser.End))
            {
                if (ObjectManager.Player.Distance(gapcloser.Sender.Position) < SpellManager.E.Range)
                {
                    SpellManager.E.Cast(gapcloser.Sender);
                }
            }
        }

        private static void OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel == Interrupter2.DangerLevel.High && Config.BoolLinks["miscInterruptE"].Value && SpellManager.E.IsReady() && SpellManager.E.IsInRange(sender))
            {
                if (ObjectManager.Player.Distance(sender) < SpellManager.E.Range)
                {
                    SpellManager.E.Cast(sender);
                }
            }
        }
        public void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R)
            {
                var t = TargetSelector.GetTarget(SpellManager.R.Range, TargetSelector.DamageType.Magical);
                Rtarget = SpellManager.R.GetPrediction(t).CastPosition;
                if (args.Slot == SpellSlot.R)
                {
                    if ((Config.BoolLinks["itemsOrb"].Value) && !IsCastingR)
                    {
                        if (ObjectManager.Player.Level < 9)
                            ScryingOrb.Range = 2500;
                        else
                            ScryingOrb.Range = 3500;

                        if (ScryingOrb.IsReady())
                            ScryingOrb.Cast(Rtarget);
                        if (FarsightOrb.IsReady())
                            FarsightOrb.Cast(Rtarget);
                    }
                }
            }
        }
        public bool IsCastingR
        {
            get
            {
                return ObjectManager.Player.HasBuff("XerathLocusOfPower2", true) ||
                       (ObjectManager.Player.LastCastedSpellName() == "XerathLocusOfPower2" &&
                        Utils.TickCount - ObjectManager.Player.LastCastedSpellT() < 500);
            }
        }
    }
}