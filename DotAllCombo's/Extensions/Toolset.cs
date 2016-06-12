
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



            if (me.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord && me.HasModifier("modifier_troll_warlord_berserkers_rage"))
                AttackRange = 150 + me.HullRadius + 24;
            else if (me.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord && !me.HasModifier("modifier_troll_warlord_berserkers_rage"))
                AttackRange = me.GetAttackRange() + me.HullRadius + 24;
            else
         if (me.ClassID == ClassID.CDOTA_Unit_Hero_TemplarAssassin)
                AttackRange = me.GetAttackRange() + me.HullRadius;
            else
         if (me.ClassID == ClassID.CDOTA_Unit_Hero_DragonKnight && me.HasModifier("modifier_dragon_knight_dragon_form"))
                AttackRange = me.GetAttackRange() + me.HullRadius + 24;
            else
         if (item == null && me.IsRanged)
                AttackRange = me.GetAttackRange() + me.HullRadius + 24;
            else
        if (item != null && me.IsRanged)
                AttackRange = me.GetAttackRange() + me.HullRadius + 24;
            else
                AttackRange = me.GetAttackRange() + me.HullRadius;
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
        public static Unit GetClosestToTarget(List<Unit> units, Unit target)
        {
            Unit closestHero = null;
            foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(target) > v.Distance2D(target)))
            {
                closestHero = v;
            }
            return closestHero;
        }

	    public static void UnAggro(List<Unit> z, Unit v)
	    {
	        var projectiles = ObjectManager.TrackingProjectiles.Where(x => x.Target.Handle == v.Handle).ToList();

	        for (int i = 0; i < projectiles.Count(); ++i)
            {
                var closestCreepUnAgr = GetClosestToTarget(z, v);
                if (v.ClassID == ClassID.CDOTA_Unit_Hero_Axe ||
	                v.ClassID == ClassID.CDOTA_Unit_Hero_Legion_Commander)
	            {
	                if (projectiles[i].Source.ClassID == ClassID.CDOTA_BaseNPC_Tower
	                    || projectiles[i].Source.ClassID == ClassID.CDOTA_Unit_Fountain)
	                {
                        

	                    if (closestCreepUnAgr!=null && closestCreepUnAgr.Distance2D(v) <= 500 & Utils.SleepCheck("UnAgr"))
	                    {
	                        v.Attack(closestCreepUnAgr);
	                        Utils.Sleep(300, "UnAgr");
	                    }
	                }
	            }
	            else if (v.ClassID != ClassID.CDOTA_Unit_Hero_Axe ||
	                     v.ClassID != ClassID.CDOTA_Unit_Hero_Legion_Commander)
	            {
	                if (projectiles[i].Source.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
	                    || projectiles[i].Source.ClassID == ClassID.CDOTA_BaseNPC_Tower
	                    || projectiles[i].Source.ClassID == ClassID.CDOTA_Unit_Fountain
	                    || projectiles[i].Source.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege)

	                {

	                    if (closestCreepUnAgr != null && closestCreepUnAgr.Distance2D(v) <= 500 & Utils.SleepCheck("UnAgr"))
	                    {
	                        v.Attack(closestCreepUnAgr);
	                        Utils.Sleep(500, "UnAgr");
	                    }
	                }
	            }
	        }
	    }


	    public static void UnAggro(Unit v)
        {

            var creepsA = ObjectManager.GetEntities<Unit>().Where(creep =>
                   (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                   || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                   || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                   || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep) &&
                  creep.IsAlive && creep.Team == me.Team && creep.IsVisible && creep.IsSpawned).ToList();
            if (creepsA.Count(x => x.Distance2D(v) <= 500) == 0)
            {
                if (v.ClassID == ClassID.CDOTA_Unit_SpiritBear)
                {
                    creepsA = ObjectManager.GetEntities<Unit>().Where(creep => (
                           creep.HasInventory && creep.Handle != v.Handle)
                           && creep.IsAlive && creep.Team == me.Team 
                           && creep.ClassID!=ClassID.CDOTA_Unit_Hero_LoneDruid
                           && creep.IsVisible && creep.Health >= (creep.MaximumHealth * 0.5)).ToList();
                }
                else
                {
                    creepsA = ObjectManager.GetEntities<Unit>().Where(creep => (
                          creep.HasInventory && creep.Handle != v.Handle)
                          && creep.IsAlive && creep.Team == me.Team && creep.IsVisible && creep.Health >= (creep.MaximumHealth * 0.5)).ToList();
                }
            }
             
            
            UnAggro(creepsA, v);
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
