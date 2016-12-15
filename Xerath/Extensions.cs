using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace Xerath
{
    public static class Extensions
    {
        public static bool HasUndyingBuff(this Obj_AI_Hero target)
        {
            // Tryndamere R
            if (target.ChampionName == "Tryndamere" &&
                target.Buffs.Any(b => b.Caster.NetworkId == target.NetworkId && b.IsValidBuff() && b.DisplayName == "Undying Rage"))
            {
                return true;
            }

            // Zilean R
            if (target.Buffs.Any(b => b.IsValidBuff() && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }

            // Kayle R
            if (target.Buffs.Any(b => b.IsValidBuff() && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            // Poppy R
            if (target.ChampionName == "Poppy")
            {
                if (HeroManager.Allies.Any(o =>
                    !o.IsMe &&
                    o.Buffs.Any(b => b.Caster.NetworkId == target.NetworkId && b.IsValidBuff() && b.DisplayName == "PoppyDITarget")))
                {
                    return true;
                }
            }

            return false;
        }

        public static float AttackSpeed(this Obj_AI_Base target)
        {
            return 1 / target.AttackDelay;
        }

        public static bool HasSpellShield(this Obj_AI_Hero target)
        {
            // Various spellshields
            return target.HasBuffOfType(BuffType.SpellShield) || target.HasBuffOfType(BuffType.SpellImmunity);
        }

        public static float TotalShieldHealth(this Obj_AI_Base target)
        {
            return target.Health + target.AllShield + target.PhysicalShield + target.MagicalShield;
        }

        public static float GetStunDuration(this Obj_AI_Base target)
        {
            return (target.Buffs.Where(b => b.IsActive && Game.Time < b.EndTime &&
                (b.Type == BuffType.Charm ||
                b.Type == BuffType.Knockback ||
                b.Type == BuffType.Stun ||
                b.Type == BuffType.Suppression ||
                b.Type == BuffType.Snare)).Aggregate(0f, (current, buff) => Math.Max(current, buff.EndTime)) -
                Game.Time) *1000    ;
        }
    }
}
