
namespace DotaAllCombo.Service
{
	using System.Linq;
	using Ensage;
	using Ensage.Common.Extensions;
	using Ensage.Common;
	using System;
	using System.Collections.Generic;
	using Ensage.Common.Menu;
	using SharpDX;
	using Service;
	using Service.Debug;
	class Toolset
	{
		private static Hero me;
        
        public static float RadiansToFace(Unit StartUnit, dynamic Target)
		{
			if (!(Target is Unit || Target is Vector3)) throw new ArgumentException("RadiansToFace -> INVALID PARAMETERS!", "Target");
			if (Target is Unit) Target = Target.Position;

			float deltaY = StartUnit.Position.Y - Target.Y;
			float deltaX = StartUnit.Position.X - Target.X;
			float angle = (float)(Math.Atan2(deltaY, deltaX));

			return (float)(Math.PI - Math.Abs(Math.Atan2(Math.Sin(StartUnit.RotationRad - angle), Math.Cos(StartUnit.RotationRad - angle))));
		}
		public static float AttackRange;
		public static void Range()
		{
            Item item = me.Inventory.Items.FirstOrDefault(x => x != null && x.IsValid && (x.Name.Contains("item_dragon_lance") || x.Name.Contains("item_hurricane_pike")));
            var _q = me.Spellbook.Spell1;

            if (me.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord)
                AttackRange = _q.IsToggled ? 150 + me.HullRadius + 24 : me.GetAttackRange() + me.HullRadius + 24;
            if (me.ClassID == ClassID.CDOTA_Unit_Hero_TemplarAssassin)
                AttackRange = me.GetAttackRange() + me.HullRadius + 24;
            else
            if (item != null && me.IsRanged)
                AttackRange = me.GetAttackRange() + me.HullRadius + 24;
            else
                AttackRange = me.GetAttackRange() + me.HullRadius + 24;
        }
		public static bool checkFace(Ability z, Hero v)
		{
			ObjectManager.GetEntities<Hero>().Where(x => x.Distance2D(v) < z.CastRange).OrderBy(x => RadiansToFace(x, v)).FirstOrDefault();
				
			return false;
		}

		public static void InitToolset(Hero myHero)
		{
			me = myHero;
		}

		public static bool HasStun(Hero x)
		{
			if (x.FindSpell("dragon_knight_dragon_tail") != null &&
			x.FindSpell("dragon_knight_dragon_tail").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("dragon_knight_dragon_tail").CastRange
			||
			x.FindSpell("earthshaker_echo_slam") != null && x.FindSpell("earthshaker_echo_slam").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("earthshaker_echo_slam").CastRange
			||
			x.FindSpell("legion_commander_duel") != null && x.FindSpell("legion_commander_duel").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("legion_commander_duel").CastRange
			||
			x.FindSpell("leshrac_split_earth") != null && x.FindSpell("leshrac_split_earth").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("leshrac_split_earth").CastRange
			||
			x.FindSpell("leoric_hellfire_blast") != null && x.FindSpell("leoric_hellfire_blast").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("leoric_hellfire_blast").CastRange
			||
			x.FindSpell("lina_light_strike_array") != null && x.FindSpell("lina_light_strike_array").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("lina_light_strike_array").CastRange
			||
			x.FindSpell("lion_impale") != null && x.FindSpell("lion_impale").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("lion_impale").CastRange
			||
			x.FindSpell("magnataur_reverse_polarity") != null &&
			x.FindSpell("magnataur_reverse_polarity").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("magnataur_reverse_polarity").CastRange
			||
			x.FindSpell("nyx_assassin_impale") != null && x.FindSpell("nyx_assassin_impale").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("nyx_assassin_impale").CastRange
			||
			x.FindSpell("ogre_magi_fireblast") != null && x.FindSpell("ogre_magi_fireblast").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("ogre_magi_fireblast").CastRange
			||
			x.FindSpell("skeleton_king_hellfire_blast") != null &&
			x.FindSpell("skeleton_king_hellfire_blast").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("skeleton_king_hellfire_blast").CastRange
			||
			x.FindSpell("sven_storm_bolt") != null && x.FindSpell("sven_storm_bolt").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("sven_storm_bolt").CastRange
			||
			x.FindSpell("tiny_avalanche") != null && x.FindSpell("tiny_avalanche").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("tiny_avalanche").CastRange
			||
			x.FindSpell("tusk_walrus_punch") != null && x.FindSpell("tusk_walrus_punch").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("tusk_walrus_punch").CastRange
			||
			x.FindSpell("vengefulspirit_magic_missile") != null &&
			x.FindSpell("vengefulspirit_magic_missile").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("vengefulspirit_magic_missile").CastRange
			||
			x.FindSpell("windrunner_shackleshot") != null && x.FindSpell("windrunner_shackleshot").Cooldown <= 0 &&
			me.Distance2D(x) <= x.FindSpell("windrunner_shackleshot").CastRange
			)
				return true;
			return false;
		}
		public static bool invUnit(Hero z)
		{
			if (z.Modifiers.Any(
			x =>
			(x.Name == "modifier_bounty_hunter_wind_walk" ||
			x.Name == "modifier_riki_permanent_invisibility" ||
			x.Name == "modifier_mirana_moonlight_shadow" || x.Name == "modifier_treant_natures_guise" ||
			x.Name == "modifier_weaver_shukuchi" ||
			x.Name == "modifier_broodmother_spin_web_invisible_applier" ||
			x.Name == "modifier_item_invisibility_edge_windwalk" || x.Name == "modifier_rune_invis" ||
			x.Name == "modifier_clinkz_wind_walk" || x.Name == "modifier_item_shadow_amulet_fade" ||
			x.Name == "modifier_item_silver_edge_windwalk" ||
			x.Name == "modifier_item_edge_windwalk" ||
			x.Name == "modifier_nyx_assassin_vendetta" ||
			x.Name == "modifier_invisible" ||
			x.Name == "modifier_invoker_ghost_walk_enemy")))
				return true;
			return false;
		}
	}
}
