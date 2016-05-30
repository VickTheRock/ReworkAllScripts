using System;
using System.Linq;
using System.Collections.Generic;
using Ensage;
using Ensage.Common.Menu;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;

namespace SkyMageRework
{
	internal class Program
	{

		//private static bool activated
		//private static bool autoUlt;
		private static readonly Menu Menu = new Menu("SkyWrath", "SkyWrath by Vick", true, "npc_dota_hero_skywrath_mage", true);
		private static Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost, force, cyclone;
		private static Ability Q, W, E, R;
		private static Font txt;
		private static Font noti;
		private static Line lines;
		private static Hero me, target;
		private static bool Active;

		private static readonly Menu skills = new Menu("Skills", "Skills");
		private static readonly Menu items = new Menu("Items", "Items");
		private static readonly Menu ult = new Menu("AutoUlt", "Auto");
		private static readonly Menu healh = new Menu("Healh", "Min % Enemy Healh to Ult");

		private static void Main(string[] args)
		{
			Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddSubMenu(skills);
			Menu.AddSubMenu(items);
			Menu.AddSubMenu(ult);
			Menu.AddSubMenu(healh);
			skills.AddItem(new MenuItem("Skills: ", "Skills: ").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"skywrath_mage_arcane_bolt",true},
				{"skywrath_mage_concussive_shot",true},
				{"skywrath_mage_mystic_flare",false}
			})));
			items.AddItem(new MenuItem("Items: ", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_orchid", true}, {"item_bloodthorn", true},
				{"item_ethereal_blade", true},
				{"item_veil_of_discord", true},
				{"item_rod_of_atos", true},
				{"item_sheepstick", true},
				{"item_arcane_boots", true},
				{"item_shivas_guard",true},
				{"item_blink", true},
				{"item_soul_ring", true},
				{"item_ghost", true},
				{"item_cheese", true}
			})));
			ult.AddItem(new MenuItem("autoUlt", "AutoSpell and Items").SetValue(true));
			ult.AddItem(new MenuItem("AutoUlt: ", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"skywrath_mage_mystic_flare",true}
			})));
			items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_force_staff", true},
				{"item_cyclone", true},
				{"item_orchid", true},
				{"item_rod_of_atos", true},
				{"item_dagon", true}
			})));
			healh.AddItem(new MenuItem("Healh", "Min healh % to ult").SetValue(new Slider(35, 10, 70))); // x/ 10%
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(false));
			Menu.AddToMainMenu();
			Console.WriteLine("> SkyWrath# loaded!");

			txt = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Segoe UI",
				   Height = 17,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });

			noti = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Segoe UI",
				   Height = 30,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });

			lines = new Line(Drawing.Direct3DDevice9);

			Game.OnUpdate += Game_OnUpdate;
			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}




		public static void Game_OnUpdate(EventArgs args)
		{
			me = ObjectManager.LocalHero;

			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage || me == null || Game.IsWatchingGame)
			{
				return;
			}
			target = me.ClosestToMouseTarget(2000);
			if (target == null) return;
			if (target.Modifiers.Any(y => y.Name == "modifier_abaddon_borrowed_time"
			|| y.Name == "modifier_item_blade_mail_reflect")
			|| target.IsMagicImmune())
			{
				var enemies = ObjectManager.GetEntities<Hero>()
						.Where(x => x.IsAlive && x.Team != me.Team && !x.IsIllusion && !x.IsMagicImmune()
						&& !x.Modifiers.Any(y => y.Name == "modifier_abaddon_borrowed_time"
						|| y.Name == "modifier_item_blade_mail_reflect")
						&& x.Distance2D(target) > 200).ToList();
				target = GetClosestToTarget(enemies, target) ?? null;
				if (Utils.SleepCheck("spam"))
				{
					Utils.Sleep(5000, "spam");
				}
			}
			//spell
			Q = me.Spellbook.SpellQ;

			W = me.Spellbook.SpellW;

			E = me.Spellbook.SpellE;

			R = me.Spellbook.SpellR;

			// Item
			ethereal = me.FindItem("item_ethereal_blade");

			sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

			vail = me.FindItem("item_veil_of_discord");

			cheese = me.FindItem("item_cheese");

			ghost = me.FindItem("item_ghost");

			orchid = me.FindItem("item_orchid");

			atos = me.FindItem("item_rod_of_atos");

			soulring = me.FindItem("item_soul_ring");

			arcane = me.FindItem("item_arcane_boots");

			blink = me.FindItem("item_blink");

			shiva = me.FindItem("item_shivas_guard");

			dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));


			Active = Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key);


			var stoneModif = target.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");

			if (Game.IsKeyDown(keyCode: 70) && Q.CanBeCasted() && target != null)
			{
				if (
					Q != null
					&& Q.CanBeCasted()
					&& (target.IsLinkensProtected()
					|| !target.IsLinkensProtected())
					&& me.CanCast()
					&& me.Distance2D(target) < 900
					&& Utils.SleepCheck("Q")
					)
				{
					Q.UseAbility(target);
					Utils.Sleep(200, "Q");
				}
			}
			if (Active && me.IsAlive && target.IsAlive && Utils.SleepCheck("activated"))
			{
				if (stoneModif) return;
				//var noBlade = target.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect");
				if (target.IsVisible && me.Distance2D(target) <= 2300)
				{
					if (
						Q != null
						&& Q.CanBeCasted()
						&& me.CanCast()
						&& !target.IsMagicImmune()
						&& me.Distance2D(target) < 1400
						&& Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						Q.UseAbility(target);
						Utils.Sleep(200, "Q");
					}
					if ( // atos Blade
						atos != null
						&& atos.CanBeCasted()
						&& me.CanCast()
						&& !target.IsLinkensProtected()
						&& !target.IsMagicImmune()
						&& Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(atos.Name)
						&& me.Distance2D(target) <= 2000
						&& Utils.SleepCheck("atos")
						)
					{
						atos.UseAbility(target);

						Utils.Sleep(250 + Game.Ping, "atos");
					} // atos Item end
					if (
						W != null
						&& target.IsVisible
						&& W.CanBeCasted()
						&& me.CanCast()
						&& !target.IsMagicImmune()
						&& me.Distance2D(target) < 900
						&& Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(W.Name)
						&& Utils.SleepCheck("W"))
					{
						W.UseAbility();
						Utils.Sleep(300, "W");
					}
					float angle = me.FindAngleBetween(target.Position, true);
					Vector3 pos = new Vector3((float)(target.Position.X - 500 * Math.Cos(angle)), (float)(target.Position.Y - 500 * Math.Sin(angle)), 0);
					if (
						blink != null
						&& Q.CanBeCasted()
						&& me.CanCast()
						&& blink.CanBeCasted()
						&& me.Distance2D(target) >= 490
						&& me.Distance2D(pos) <= 1180
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						blink.UseAbility(pos);
						Utils.Sleep(250, "blink");
					}
					if (
						   E != null
						   && E.CanBeCasted()
						   && me.CanCast()
						   && !target.IsLinkensProtected()
						   && me.Position.Distance2D(target) < 1400
						   && Utils.SleepCheck("E"))
					{
						E.UseAbility(target);
						Utils.Sleep(200, "E");
					}
					if (!E.CanBeCasted() || E == null || me.IsSilenced())
					{
						if ( // orchid
							orchid != null
							&& orchid.CanBeCasted()
							&& me.CanCast()
							&& !target.IsLinkensProtected()
							&& !target.IsMagicImmune()
							&& me.Distance2D(target) <= 1400
							&& Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
							&& Utils.SleepCheck("orchid")
							)
						{
							orchid.UseAbility(target);
							Utils.Sleep(250, "orchid");
						} // orchid Item end
						if (!orchid.CanBeCasted() || orchid == null || me.IsSilenced() || !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
						{
							if ( // vail
								   vail != null
								  && vail.CanBeCasted()
								  && me.CanCast()
								  && !target.IsMagicImmune()
								  && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(vail.Name)
								  && me.Distance2D(target) <= 1500
								  && Utils.SleepCheck("vail")
								  )
							{
								vail.UseAbility(target.Position);
								Utils.Sleep(250, "vail");
							} // orchid Item end
							if (!vail.CanBeCasted() || vail == null || me.IsSilenced() || !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(vail.Name))
							{
								if (// ethereal
									   ethereal != null
									   && ethereal.CanBeCasted()
									   && me.CanCast()
									   && !target.IsLinkensProtected()
									   && !target.IsMagicImmune()
									   && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
									   && Utils.SleepCheck("ethereal")
									  )
								{
									ethereal.UseAbility(target);
									Utils.Sleep(200, "ethereal");
								} // ethereal Item end
								if (!ethereal.CanBeCasted() || ethereal == null || me.IsSilenced() || !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
								{
									if (
										 Q != null
										&& Q.CanBeCasted()
										&& me.CanCast()
										&& me.Distance2D(target) < 1400
										 && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(Q.Name)
										&& Utils.SleepCheck("Q")
										)
									{
										Q.UseAbility(target);
										Utils.Sleep(200, "Q");
									}


									if (
									   R != null
									   && R.CanBeCasted()
									   && me.CanCast()
									   && !target.HasModifier("modifier_skywrath_mystic_flare_aura_effect")
									   && target.MovementSpeed <= 240
									   && me.Position.Distance2D(target) < 1200
									   && target.Health >= (target.MaximumHealth / 100 * Menu.Item("Healh").GetValue<Slider>().Value)

									   && !target.HasModifier("modifier_item_blade_mail_reflect")
									   && !target.HasModifier("modifier_skywrath_mystic_flare_aura_effect")
									   && !target.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
									   && !target.HasModifier("modifier_puck_phase_shift")
									   && !target.HasModifier("modifier_eul_cyclone")
									   && !target.HasModifier("modifier_dazzle_shallow_grave")
									   && !target.HasModifier("modifier_brewmaster_storm_cyclone")
									   && !target.HasModifier("modifier_spirit_breaker_charge_of_darkness")
									   && !target.HasModifier("modifier_shadow_demon_disruption")
									   && !target.HasModifier("modifier_tusk_snowball_movement")
									   && !target.IsMagicImmune()
									   && (!target.FindSpell("abaddon_borrowed_time").CanBeCasted() && !target.HasModifier("modifier_abaddon_borrowed_time_damage_redirect"))

									   && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(R.Name)
									   && Utils.SleepCheck("R"))
									{
										R.UseAbility(Prediction.InFront(target, 100));
										Utils.Sleep(330, "R");
									}

									if (// SoulRing Item 
										soulring != null
										&& soulring.CanBeCasted()
										&& me.CanCast()
										&& me.Health >= (me.MaximumHealth * 0.5)
										&& me.Mana <= R.ManaCost
										&& Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(soulring.Name)
										)
									{
										soulring.UseAbility();
									} // SoulRing Item end

									if (// Arcane Boots Item
										arcane != null
										&& arcane.CanBeCasted()
										&& me.CanCast()
										&& me.Mana <= R.ManaCost
										&& Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
										)
									{
										arcane.UseAbility();
									} // Arcane Boots Item end

									if (//Ghost
										ghost != null
										&& ghost.CanBeCasted()
										&& me.CanCast()
										&& ((me.Position.Distance2D(target) < 300
										&& me.Health <= (me.MaximumHealth * 0.7))
										|| me.Health <= (me.MaximumHealth * 0.3))
										&& Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
										&& Utils.SleepCheck("Ghost"))
									{
										ghost.UseAbility();
										Utils.Sleep(250, "Ghost");
									}


									if (// Shiva Item
										shiva != null
										&& shiva.CanBeCasted()
										&& me.CanCast()
										&& !target.IsMagicImmune()
										&& Utils.SleepCheck("shiva")
										&& Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
										&& me.Distance2D(target) <= 600
										)

									{
										shiva.UseAbility();
										Utils.Sleep(250, "shiva");
									} // Shiva Item end
									if ( // sheep
										sheep != null
										&& sheep.CanBeCasted()
										&& me.CanCast()
										&& !target.IsLinkensProtected()
										&& !target.IsMagicImmune()
										&& me.Distance2D(target) <= 1400
										&& Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
										&& Utils.SleepCheck("sheep")
										)
									{
										sheep.UseAbility(target);
										Utils.Sleep(250, "sheep");
									} // sheep Item end

									if (// Dagon
										me.CanCast()
										&& dagon != null
										&& (ethereal == null
										|| (target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow")
										|| ethereal.Cooldown < 17))
										&& !target.IsLinkensProtected()
										&& dagon.CanBeCasted()
										&& !target.IsMagicImmune()
										&& Utils.SleepCheck("dagon")
									   )
									{
										dagon.UseAbility(target);
										Utils.Sleep(200, "dagon");
									} // Dagon Item end

									if (
										 // cheese
										 cheese != null
										 && cheese.CanBeCasted()
										 && me.Health <= (me.MaximumHealth * 0.3)
										 && me.Distance2D(target) <= 700
										 && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
										 && Utils.SleepCheck("cheese")
									 )
									{
										cheese.UseAbility();
										Utils.Sleep(200, "cheese");
									} // cheese Item end
								}
							}
						}
						if (Menu.Item("orbwalk").GetValue<bool>())
						{
							if (me.Distance2D(target) <= me.AttackRange + 5 &&
							(!me.IsAttackImmune()
							|| !target.IsAttackImmune()
							)
							&& me.NetworkActivity != NetworkActivity.Attack
							&& me.CanAttack()
							&& !me.IsAttacking()
							&& Utils.SleepCheck("attack")
							)
							{
								me.Attack(target);
								Utils.Sleep(180, "attack");
							}
							else if (
								(!me.CanAttack()
								|| me.Distance2D(target) >= 450)
								&& me.NetworkActivity != NetworkActivity.Attack
								&& me.Distance2D(target) <= 2500
								&& !me.IsAttacking()
								&& Utils.SleepCheck("Move")
								)
							{
								me.Move(Prediction.InFront(target, 100));
								Utils.Sleep(400, "Move");
							}
						}
					}
				}
				Utils.Sleep(100, "activated");
			}
			A();
		}
		private static Hero GetClosestToTarget(List<Hero> units, Hero target)
		{
			Hero closestHero = null;
			foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(target) > v.Distance2D(target)))
			{
				closestHero = v;
			}
			return closestHero;
		}

		private static void A()
		{
			me = ObjectManager.LocalHero;
			var e =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.IsValid && x.Team != me.Team && !x.IsIllusion).ToList();

			if (e.Count == 0) return;
			if (Menu.Item("autoUlt").GetValue<bool>() && me.IsAlive)
			{
				force = me.FindItem("item_force_staff");
				cyclone = me.FindItem("item_cyclone");
				orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
				atos = me.FindItem("item_rod_of_atos");
				var ecount = e.Count();

				for (int i = 0; i < ecount; ++i)
				{
					var reflect = e[i].HasModifier("modifier_item_blade_mail_reflect");
					{
						if (cyclone != null && reflect && cyclone.CanBeCasted() && e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect") && me.Distance2D(e[i]) < cyclone.CastRange &&
							Utils.SleepCheck(e[i].Handle.ToString()))
						{
							cyclone.UseAbility(me);
							Utils.Sleep(500, e[i].Handle.ToString());
						}
						if (((Active && !E.CanBeCasted()) || e[i].HasModifier("modifier_item_ethereal_blade_slow")) || !Active)
						{
							if (R != null && R.CanBeCasted() && me.Distance2D(e[i]) <= R.CastRange + 100
								&& e[i].MovementSpeed <= 220
								&& !e[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
								&& !e[i].HasModifier("modifier_item_blade_mail_reflect")
								&& !e[i].HasModifier("modifier_sniper_headshot")
								&& !e[i].HasModifier("modifier_leshrac_lightning_storm_slow")
								&& !e[i].HasModifier("modifier_razor_unstablecurrent_slow")
								&& !e[i].HasModifier("modifier_pudge_meat_hook")
								&& (e[i].FindSpell("phantom_assassin_phantom_strike") != null
								&& !e[i].FindSpell("phantom_assassin_phantom_strike").IsInAbilityPhase)
								&& !e[i].HasModifier("modifier_tusk_snowball_movement")
								&& !e[i].HasModifier("modifier_faceless_void_time_walk")
								&& !e[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
								&& !e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
								&& !e[i].HasModifier("modifier_puck_phase_shift")
								&& !e[i].HasModifier("modifier_winter_wyvern_winters_curse")
								&& !e[i].HasModifier("modifier_eul_cyclone")
								&& !e[i].HasModifier("modifier_dazzle_shallow_grave")
								&& !e[i].HasModifier("modifier_brewmaster_storm_cyclone")
								&& !e[i].HasModifier("modifier_mirana_leap")
								&& !e[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
								&& !e[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
								&& !e[i].HasModifier("modifier_shadow_demon_disruption")
								&& (e[i].FindSpell("abaddon_borrowed_time") != null
								&& e[i].FindSpell("abaddon_borrowed_time").Cooldown > 0
								&& !e[i].HasModifier("modifier_abaddon_borrowed_time"))
								&& e[i].Health >= (e[i].MaximumHealth / 100 * (Menu.Item("Healh").GetValue<Slider>().Value))
								&& !e[i].IsMagicImmune()
								&& Menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name)
								&& Utils.SleepCheck(e[i].Handle.ToString()))
							{
								R.UseAbility(Prediction.InFront(e[i], 90));
								Utils.Sleep(300, e[i].Handle.ToString());
							}
							if (R != null && R.CanBeCasted() && me.Distance2D(e[i]) <= R.CastRange + 100
								&&
								(e[i].HasModifier("modifier_meepo_earthbind")
									|| e[i].HasModifier("modifier_pudge_dismember")
									|| e[i].HasModifier("modifier_naga_siren_ensnare")
									|| e[i].HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
									|| (e[i].HasModifier("modifier_legion_commander_duel")
									&& !e[i].AghanimState())
									|| e[i].HasModifier("modifier_kunkka_torrent")
									|| e[i].HasModifier("modifier_ice_blast")
									|| e[i].HasModifier("modifier_crystal_maiden_crystal_nova")
									|| e[i].HasModifier("modifier_enigma_black_hole_pull")
									|| e[i].HasModifier("modifier_ember_spirit_searing_chains")
									|| e[i].HasModifier("modifier_dark_troll_warlord_ensnare")
									|| e[i].HasModifier("modifier_crystal_maiden_frostbite")
									|| (e[i].FindSpell("rattletrap_power_cogs") != null && e[i].FindSpell("rattletrap_power_cogs").IsInAbilityPhase)
									|| e[i].HasModifier("modifier_axe_berserkers_call")
									|| e[i].HasModifier("modifier_bane_fiends_grip")
									|| (e[i].HasModifier("modifier_faceless_void_chronosphere_freeze")
									&& e[i].ClassID != ClassID.CDOTA_Unit_Hero_FacelessVoid)
									|| e[i].HasModifier("modifier_storm_spirit_electric_vortex_pull")
									|| (e[i].FindSpell("witch_doctor_death_ward") != null
									&& e[i].FindSpell("witch_doctor_death_ward").IsInAbilityPhase)
									|| (e[i].FindSpell("crystal_maiden_crystal_nova") != null
									&& e[i].FindSpell("crystal_maiden_crystal_nova").IsInAbilityPhase)
									|| e[i].IsStunned())
									&& (!e[i].HasModifier("modifier_medusa_stone_gaze_stone")
									&& !e[i].HasModifier("modifier_faceless_void_time_walk")
									&& !e[i].HasModifier("modifier_item_monkey_king_bar")
									&& !e[i].HasModifier("modifier_rattletrap_battery_assault")
									&& !e[i].HasModifier("modifier_item_blade_mail_reflect")
									&& !e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
									&& !e[i].HasModifier("modifier_pudge_meat_hook")
									&& !e[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
									&& !e[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
									&& !e[i].HasModifier("modifier_puck_phase_shift")
									&& !e[i].HasModifier("modifier_eul_cyclone")
									&& (e[i].FindSpell("phantom_assassin_phantom_strike") != null
									&& !e[i].FindSpell("phantom_assassin_phantom_strike").IsInAbilityPhase)
									&& !e[i].HasModifier("modifier_invoker_tornado")
									&& !e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
									&& !e[i].HasModifier("modifier_dazzle_shallow_grave")
									&& !e[i].HasModifier("modifier_mirana_leap")
									&& !e[i].HasModifier("modifier_winter_wyvern_winters_curse")
									&& !e[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
									&& !e[i].HasModifier("modifier_brewmaster_storm_cyclone")
									&& !e[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
									&& !e[i].HasModifier("modifier_shadow_demon_disruption")
									&& !e[i].HasModifier("modifier_tusk_snowball_movement")
									&& !e[i].HasModifier("modifier_invoker_tornado")
									&& (e[i].FindSpell("abaddon_borrowed_time") != null
									&& e[i].FindSpell("abaddon_borrowed_time").Cooldown > 0
									&& !e[i].HasModifier("modifier_abaddon_borrowed_time"))
									&& e[i].Health >= (e[i].MaximumHealth / 100 * (Menu.Item("Healh").GetValue<Slider>().Value))
									&& !e[i].IsMagicImmune())
								&& Menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name)
								&& Utils.SleepCheck(e[i].Handle.ToString()))
							{
								R.UseAbility(Prediction.InFront(e[i], 70));
								Utils.Sleep(500, e[i].Handle.ToString());
							}

							if (R != null && R.CanBeCasted() && me.Distance2D(e[i]) <= R.CastRange + 100
								&& e[i].MovementSpeed <= 220
								&& e[i].MagicDamageResist <= 0.07
								&& e[i].Health >= (e[i].MaximumHealth / 100 * (Menu.Item("Healh").GetValue<Slider>().Value))
								&& !e[i].HasModifier("modifier_item_blade_mail_reflect")
								&& !e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
								&& !e[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
								&& !e[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
								&& !e[i].HasModifier("modifier_puck_phase_shift")
								&& !e[i].HasModifier("modifier_eul_cyclone")
								&& !e[i].HasModifier("modifier_invoker_tornado")
								&& !e[i].HasModifier("modifier_dazzle_shallow_grave")
								&& !e[i].HasModifier("modifier_brewmaster_storm_cyclone")
								&& !e[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
								&& !e[i].HasModifier("modifier_shadow_demon_disruption")
								&& !e[i].HasModifier("modifier_faceless_void_time_walk")
								&& !e[i].HasModifier("modifier_winter_wyvern_winters_curse")
								&& !e[i].HasModifier("modifier_mirana_leap")
								&& !e[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
								&& !e[i].HasModifier("modifier_tusk_snowball_movement")
								&& !e[i].IsMagicImmune()
								&& (e[i].FindSpell("phantom_assassin_phantom_strike") != null
								&& !e[i].FindSpell("phantom_assassin_phantom_strike").IsInAbilityPhase)
								&& (e[i].FindSpell("abaddon_borrowed_time") != null
								&& e[i].FindSpell("abaddon_borrowed_time").Cooldown > 0
								&& !e[i].HasModifier("modifier_abaddon_borrowed_time"))
								&& Menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name)
								&& Utils.SleepCheck(e[i].Handle.ToString()))
							{
								R.UseAbility(Prediction.InFront(e[i], 90));
								Utils.Sleep(500, e[i].Handle.ToString());
							}
						}
						if (W != null && W.CanBeCasted() && me.Distance2D(e[i]) <= 1400
						   && (e[i].MovementSpeed <= 255
						   || (e[i].Distance2D(me)<= me.HullRadius+10 
						   && e[i].NetworkActivity==NetworkActivity.Attack)
						   || e[i].MagicDamageResist <= 0.07)
						   && !e[i].IsMagicImmune()
						   && Utils.SleepCheck(e[i].Handle.ToString()))
						{
							W.UseAbility();
							Utils.Sleep(500, e[i].Handle.ToString());
						}
						if (atos != null && R != null && R.CanBeCasted() && atos.CanBeCasted()
							&& !e[i].IsLinkensProtected()
							&& me.Distance2D(e[i]) <= 1200
							&& e[i].MagicDamageResist <= 0.07
							&& !e[i].IsMagicImmune()
							&& Utils.SleepCheck(e[i].Handle.ToString()))
						{
							atos.UseAbility(e[i]);
							Utils.Sleep(500, e[i].Handle.ToString());
						}
						if (vail != null && e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
							&& vail.CanBeCasted()
							&& me.Distance2D(e[i]) <= 1200
							&& Utils.SleepCheck(e[i].Handle.ToString())
							)
						{
							vail.UseAbility(e[i].Position);
							Utils.Sleep(500, e[i].Handle.ToString());
						}
						if (E != null && e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
							&& E.CanBeCasted()
							&& me.Distance2D(e[i]) <= 900
							&& Utils.SleepCheck(e[i].Handle.ToString())
							)
						{
							E.UseAbility(e[i]);
							Utils.Sleep(500, e[i].Handle.ToString());
						}
						if (ethereal != null &&
							e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
							&& !e[i].HasModifier("modifier_legion_commander_duel")
							&& ethereal.CanBeCasted()
							&& E.CanBeCasted()
							&& me.Distance2D(e[i]) <= 1000
							&& Utils.SleepCheck(e[i].Handle.ToString())
							)
						{
							ethereal.UseAbility(e[i]);
							Utils.Sleep(500, e[i].Handle.ToString());
						}
						if (E != null && E.CanBeCasted() && me.Distance2D(e[i]) <= E.CastRange
							&& !e[i].IsLinkensProtected()
							&&
							(e[i].HasModifier("modifier_meepo_earthbind")
								|| e[i].HasModifier("modifier_pudge_dismember")
								|| e[i].HasModifier("modifier_naga_siren_ensnare")
								|| e[i].HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
								|| e[i].HasModifier("modifier_legion_commander_duel")
								|| e[i].HasModifier("modifier_kunkka_torrent")
								|| e[i].HasModifier("modifier_ice_blast")
								|| e[i].HasModifier("modifier_enigma_black_hole_pull")
								|| e[i].HasModifier("modifier_ember_spirit_searing_chains")
								|| e[i].HasModifier("modifier_dark_troll_warlord_ensnare")
								|| e[i].HasModifier("modifier_crystal_maiden_crystal_nova")
								|| e[i].HasModifier("modifier_axe_berserkers_call")
								|| e[i].HasModifier("modifier_bane_fiends_grip")
								|| e[i].HasModifier("modifier_rubick_telekinesis")
								|| e[i].HasModifier("modifier_storm_spirit_electric_vortex_pull")
								|| e[i].HasModifier("modifier_winter_wyvern_cold_embrace")
								|| e[i].HasModifier("modifier_shadow_shaman_shackles")
								|| (e[i].FindSpell("magnataur_reverse_polarity") != null && e[i].FindSpell("magnataur_reverse_polarity").IsInAbilityPhase)
								|| (e[i].FindItem("item_blink") != null && e[i].FindItem("item_blink").Cooldown > 11)
								|| (e[i].FindSpell("queenofpain_blink") != null && e[i].FindSpell("queenofpain_blink").IsInAbilityPhase)
								|| (e[i].FindSpell("antimage_blink") != null && e[i].FindSpell("antimage_blink").IsInAbilityPhase)
								|| (e[i].FindSpell("antimage_mana_void") != null && e[i].FindSpell("antimage_mana_void").IsInAbilityPhase)
								|| (e[i].FindSpell("legion_commander_duel") != null && e[i].FindSpell("legion_commander_duel").Cooldown <= 0)
								|| (e[i].FindSpell("doom_bringer_doom") != null && e[i].FindSpell("doom_bringer_doom").IsInAbilityPhase)
								|| (e[i].HasModifier("modifier_faceless_void_chronosphere_freeze") && e[i].ClassID != ClassID.CDOTA_Unit_Hero_FacelessVoid)
								|| (e[i].FindSpell("witch_doctor_death_ward") != null && e[i].FindSpell("witch_doctor_death_ward").IsInAbilityPhase)
								|| (e[i].FindSpell("rattletrap_power_cogs") != null && e[i].FindSpell("rattletrap_power_cogs").IsInAbilityPhase)
								|| (e[i].FindSpell("tidehunter_ravage") != null && e[i].FindSpell("tidehunter_ravage").IsInAbilityPhase)
								|| (e[i].FindSpell("axe_berserkers_call") != null && e[i].FindSpell("axe_berserkers_call").IsInAbilityPhase)
								|| (e[i].FindSpell("brewmaster_primal_split") != null && e[i].FindSpell("brewmaster_primal_split").IsInAbilityPhase)
								|| (e[i].FindSpell("omniknight_guardian_angel") != null && e[i].FindSpell("omniknight_guardian_angel").IsInAbilityPhase)
								|| (e[i].FindSpell("queenofpain_sonic_wave") != null && e[i].FindSpell("queenofpain_sonic_wave").IsInAbilityPhase)
								|| (e[i].FindSpell("sandking_epicenter") != null && e[i].FindSpell("sandking_epicenter").IsInAbilityPhase)
								|| (e[i].FindSpell("slardar_slithereen_crush") != null && e[i].FindSpell("slardar_slithereen_crush").IsInAbilityPhase)
								|| (e[i].FindSpell("tiny_toss") != null && e[i].FindSpell("tiny_toss").IsInAbilityPhase)
								|| (e[i].FindSpell("tusk_walrus_punch") != null && e[i].FindSpell("tusk_walrus_punch").IsInAbilityPhase)
								|| (e[i].FindSpell("faceless_void_time_walk") != null && e[i].FindSpell("faceless_void_time_walk").IsInAbilityPhase)
								|| (e[i].FindSpell("faceless_void_chronosphere") != null && e[i].FindSpell("faceless_void_chronosphere").IsInAbilityPhase)
								|| (e[i].FindSpell("disruptor_static_storm") != null && e[i].FindSpell("disruptor_static_storm").Cooldown <= 0)
								|| (e[i].FindSpell("lion_finger_of_death") != null && e[i].FindSpell("lion_finger_of_death").Cooldown <= 0)
								|| (e[i].FindSpell("luna_eclipse") != null && e[i].FindSpell("luna_eclipse").Cooldown <= 0)
								|| (e[i].FindSpell("lina_laguna_blade") != null && e[i].FindSpell("lina_laguna_blade").Cooldown <= 0)
								|| (e[i].FindSpell("doom_bringer_doom") != null && e[i].FindSpell("doom_bringer_doom").Cooldown <= 0)
								|| (e[i].FindSpell("life_stealer_rage") != null && e[i].FindSpell("life_stealer_rage").Cooldown <= 0 && me.Level >= 7)
								|| e[i].IsStunned()
								)
							&& !e[i].IsMagicImmune()
							&& !e[i].HasModifier("modifier_medusa_stone_gaze_stone")
							&& Utils.SleepCheck(e[i].Handle.ToString()))
						{
							E.UseAbility(e[i]);
							Utils.Sleep(250, e[i].Handle.ToString());
						}
					}
					if (e[i].IsLinkensProtected() && (me.IsVisibleToEnemies || Active))
					{
						if (force != null && force.CanBeCasted() && me.Distance2D(e[i]) < force.CastRange &&
							Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(force.Name) &&
							Utils.SleepCheck(e[i].Handle.ToString()))
						{
							force.UseAbility(e[i]);
							Utils.Sleep(500, e[i].Handle.ToString());
						}
						else if (cyclone != null && cyclone.CanBeCasted() && me.Distance2D(e[i]) < cyclone.CastRange &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(cyclone.Name) &&
								 Utils.SleepCheck(e[i].Handle.ToString()))
						{
							cyclone.UseAbility(e[i]);
							Utils.Sleep(500, e[i].Handle.ToString());
						}
						else if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e[i]) < orchid.CastRange &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(orchid.Name) &&
								 Utils.SleepCheck(e[i].Handle.ToString()))
						{
							orchid.UseAbility(e[i]);
							Utils.Sleep(500, e[i].Handle.ToString());
						}
						else if (atos != null && atos.CanBeCasted() && me.Distance2D(e[i]) < atos.CastRange - 400 &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(atos.Name) &&
								 Utils.SleepCheck(e[i].Handle.ToString()))
						{
							atos.UseAbility(e[i]);
							Utils.Sleep(500, e[i].Handle.ToString());
						}
						else if (dagon != null && dagon.CanBeCasted() && me.Distance2D(e[i]) < dagon.CastRange &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(dagon.Name) &&
								 Utils.SleepCheck(e[i].Handle.ToString()))
						{
							dagon.UseAbility(e[i]);
							Utils.Sleep(500, e[i].Handle.ToString());
						}
					}
				}
			}
		} // SkywrathMage class

		static void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectManager.LocalPlayer;
			var me = ObjectManager.LocalHero;
			if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage)
				return;
			R = me.Spellbook.SpellR;
			if (Active)
			{
				DrawBox(2, 510, 125, 20, 1, new ColorBGRA(0, 0, 90, 90));
				DrawFilledBox(2, 510, 125, 20, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("SkyWrath#: Comboing!", 4, 510, Color.DeepPink, txt);
			}
			if (!Menu.Item("autoUlt").GetValue<bool>())
			{
				DrawBox(2, 530, 125, 20, 1, new ColorBGRA(0, 0, 90, 90));
				DrawFilledBox(2, 530, 125, 20, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("  Auto items Disable", 4, 530, Color.Crimson, txt);
			}
			if (Menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name) && !Active)
			{
				DrawBox(2, 510, 125, 20, 1, new ColorBGRA(0, 0, 90, 90));
				DrawFilledBox(2, 510, 125, 20, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("  Auto ult Enable", 4, 510, Color.DeepPink, txt);
			}
			if (!Menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name) && !Active)
			{
				DrawBox(2, 510, 125, 20, 1, new ColorBGRA(0, 0, 90, 90));
				DrawFilledBox(2, 510, 125, 20, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("  Auto ult Disable", 4, 510, Color.Crimson, txt);
			}
		}

		static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			noti.Dispose();
			lines.Dispose();
		}



		static void Drawing_OnPostReset(EventArgs args)
		{
			txt.OnResetDevice();
			noti.OnResetDevice();
			lines.OnResetDevice();
		}

		static void Drawing_OnPreReset(EventArgs args)
		{
			txt.OnLostDevice();
			noti.OnLostDevice();
			lines.OnLostDevice();
		}

		public static void DrawFilledBox(float x, float y, float w, float h, Color color)
		{
			var vLine = new Vector2[2];

			lines.GLLines = true;
			lines.Antialias = false;
			lines.Width = w;

			vLine[0].X = x + w / 2;
			vLine[0].Y = y;
			vLine[1].X = x + w / 2;
			vLine[1].Y = y + h;

			lines.Begin();
			lines.Draw(vLine, color);
			lines.End();
		}

		public static void DrawBox(float x, float y, float w, float h, float px, Color color)
		{
			DrawFilledBox(x, y + h, w, px, color);
			DrawFilledBox(x - px, y, px, h, color);
			DrawFilledBox(x, y - px, w, px, color);
			DrawFilledBox(x + w, y, px, h, color);
		}

		public static void DrawShadowText(string stext, int x, int y, Color color, Font f)
		{
			f.DrawText(null, stext, x + 1, y + 1, Color.Black);
			f.DrawText(null, stext, x, y, color);
		}
	}
}
