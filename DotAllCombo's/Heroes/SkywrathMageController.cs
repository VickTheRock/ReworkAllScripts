namespace DotaAllCombo.Heroes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using SharpDX;

	using Service;
	using Service.Debug;


	internal class SkywrathMageController : Variables, IHeroController
	{
		private readonly Menu skills = new Menu("Skills", "Skills");
		private readonly Menu items = new Menu("Items", "Items");
		private readonly Menu ult = new Menu("AutoAbility", "AutoAbility");
		private readonly Menu healh = new Menu("Healh", "Max Enemy Healh % to Ult");


		private Ability Q, W, E, R;

		private Item orchid, sheep, vail, soul, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost, force, cyclone;
		public void OnLoadEvent()
		{

			AssemblyExtensions.InitAssembly("VickTheRock", "1.0");

			Print.LogMessage.Success("I am sworn to turn the tide where ere I can.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("keyQ", "Spam Q key").SetValue(new KeyBind('Q', KeyBindType.Press)));


			skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"skywrath_mage_arcane_bolt", true},
				{"skywrath_mage_concussive_shot", true},
				{"skywrath_mage_ancient_seal", true},
				{"skywrath_mage_mystic_flare", true}
			})));
			items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_dagon",true},
				{"item_orchid", true},
				{"item_bloodthorn", true},
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
			ult.AddItem(new MenuItem("autoUlt", "AutoAbility").SetValue(true));
			ult.AddItem(new MenuItem("AutoAbility", "AutoAbility").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"skywrath_mage_concussive_shot", true},
				{"skywrath_mage_ancient_seal", true},
				{"skywrath_mage_mystic_flare", true},
				{"item_rod_of_atos", true},
				{"item_cyclone", true},
				{"item_ethereal_blade", true},
				{"item_veil_of_discord", true},

			})));
			items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_force_staff", true},
				{"item_cyclone", true},
				{"item_orchid", true},
				{"item_bloodthorn", true},
				{"item_rod_of_atos", true},
				{"item_dagon", true}
			})));
			healh.AddItem(new MenuItem("Healh", "Min healh % to ult").SetValue(new Slider(35, 10, 70))); // x/ 10%
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(false));
			Menu.AddSubMenu(skills);
			Menu.AddSubMenu(items);
			Menu.AddSubMenu(ult);
			Menu.AddSubMenu(healh);
		} // OnLoadEvent

		public void OnCloseEvent()
		{
			e = null;
		}

		/* Доп. функции скрипта
		-----------------------------------------------------------------------------*/


		public void Combo()
		{
			e = me.ClosestToMouseTarget(2000);
			if (e.HasModifier("modifier_abaddon_borrowed_time")
			|| e.HasModifier("modifier_item_blade_mail_reflect")
			|| e.IsMagicImmune())
			{
				var enemies = ObjectManager.GetEntities<Hero>()
						.Where(x => x.IsAlive && x.Team != me.Team && !x.IsIllusion && !x.IsMagicImmune()
						&& !x.Modifiers.Any(y => y.Name == "modifier_abaddon_borrowed_time"
						|| y.Name == "modifier_item_blade_mail_reflect")
						&& x.Distance2D(e) > 200).ToList();
				e = GetClosestToTarget(enemies, e) ?? null;
				if (Utils.SleepCheck("spam"))
				{
					Utils.Sleep(5000, "spam");
				}
			}
			if (e == null) return;
			//spell
			Q = me.Spellbook.SpellQ;

			W = me.Spellbook.SpellW;

			E = me.Spellbook.SpellE;

			R = me.Spellbook.SpellR;
			// Item
			ethereal = me.FindItem("item_ethereal_blade");

			sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

			vail = me.FindItem("item_veil_of_discord");

			cheese = me.FindItem("item_cheese");

			ghost = me.FindItem("item_ghost");

			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");

			atos = me.FindItem("item_rod_of_atos");

			soul = me.FindItem("item_soul_ring");

			arcane = me.FindItem("item_arcane_boots");

			blink = me.FindItem("item_blink");

			shiva = me.FindItem("item_shivas_guard");

			dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));


			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);


			Push = Game.IsKeyDown(Menu.Item("keyQ").GetValue<KeyBind>().Key);

			var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");
			A();
			if (Push && Q != null && Q.CanBeCasted())
			{
				if (
					Q != null
					&& Q.CanBeCasted()
					&& (e.IsLinkensProtected()
					|| !e.IsLinkensProtected())
					&& me.CanCast()
					&& me.Distance2D(e) < Q.GetCastRange() + me.HullRadius + 24
					&& Utils.SleepCheck("Q")
					)
				{
					Q.UseAbility(e);
					Utils.Sleep(200, "Q");
				}
			}
			if (Active && me.IsAlive && e.IsAlive && Utils.SleepCheck("activated"))
			{
				if (stoneModif) return;
				//var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");
				if (e.IsVisible && me.Distance2D(e) <= 2300)
				{
					var distance = me.IsVisibleToEnemies ? 1400 : E.GetCastRange() + me.HullRadius;
					if (
						Q != null
						&& Q.CanBeCasted()
						&& me.CanCast()
						&& e.IsLinkensProtected()
						&& !e.IsMagicImmune()
						&& me.Distance2D(e) < Q.GetCastRange() + me.HullRadius
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						Q.UseAbility(e);
						Utils.Sleep(200, "Q");
					}

					if (
						E != null
						&& E.CanBeCasted()
						&& me.CanCast()
						&& !e.IsLinkensProtected()
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
						&& me.Position.Distance2D(e) < E.GetCastRange() + me.HullRadius + 500
						&& Utils.SleepCheck("E"))
					{
						E.UseAbility(e);
						Utils.Sleep(200, "E");
					}
					if (
					  Q != null
					  && Q.CanBeCasted()
					  && me.CanCast()
					  && !e.IsMagicImmune()
					  && me.Distance2D(e) < distance
					  && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					  && Utils.SleepCheck("Q")
					  )
					{
						Q.UseAbility(e);
						Utils.Sleep(200, "Q");
					}
					if ( // sheep
						sheep != null
						&& sheep.CanBeCasted()
						&& me.CanCast()
						&& !e.IsLinkensProtected()
						&& !e.IsMagicImmune()
						&& me.Distance2D(e) <= 1400
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
						&& Utils.SleepCheck("sheep")
						)
					{
						sheep.UseAbility(e);
						Utils.Sleep(250, "sheep");
					} // sheep Item end
					if (E == null || !E.CanBeCasted() || me.IsSilenced() || me.Position.Distance2D(e) > E.GetCastRange() + me.HullRadius || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name))
					{
						if (
						   Q != null
						   && Q.CanBeCasted()
						   && me.CanCast()
						   && !e.IsMagicImmune()
						   && me.Distance2D(e) < 1400
						   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						   && Utils.SleepCheck("Q")
					   )
						{
							Q.UseAbility(e);
							Utils.Sleep(200, "Q");
						}
						if ( // atos Blade
							atos != null
							&& atos.CanBeCasted()
							&& me.CanCast()
							&& !e.IsLinkensProtected()
							&& !e.IsMagicImmune()
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos.Name)
							&& me.Distance2D(e) <= distance
							&& Utils.SleepCheck("atos")
							)
						{
							atos.UseAbility(e);

							Utils.Sleep(250 + Game.Ping, "atos");
						} // atos Item end
						if (
							W != null
							&& e.IsVisible
							&& W.CanBeCasted()
							&& me.CanCast()
							&& !e.IsMagicImmune()
							&& me.Distance2D(e) < distance
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
							&& Utils.SleepCheck("W"))
						{
							W.UseAbility();
							Utils.Sleep(300, "W");
						}
						float angle = me.FindAngleBetween(e.Position, true);
						Vector3 pos = new Vector3((float)(e.Position.X - 500 * Math.Cos(angle)), (float)(e.Position.Y - 500 * Math.Sin(angle)), 0);
						if (
							blink != null
							&& Q.CanBeCasted()
							&& me.CanCast()
							&& blink.CanBeCasted()
							&& me.Distance2D(e) >= 490
							&& me.Distance2D(pos) <= 1180
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
							&& Utils.SleepCheck("blink")
							)
						{
							blink.UseAbility(pos);
							Utils.Sleep(250, "blink");
						}
						if ( // orchid
							orchid != null
							&& orchid.CanBeCasted()
							&& me.CanCast()
							&& !e.IsLinkensProtected()
							&& !e.IsMagicImmune()
							&& me.Distance2D(e) <= distance
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
							&& Utils.SleepCheck("orchid")
							)
						{
							orchid.UseAbility(e);
							Utils.Sleep(250, "orchid");
						} // orchid Item end
						if (!orchid.CanBeCasted() || orchid == null || me.IsSilenced() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
						{
							if ( // vail
								vail != null
							   && vail.CanBeCasted()
							   && me.CanCast()
							   && !e.IsMagicImmune()
							   && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
							   && me.Distance2D(e) <= distance
							   && Utils.SleepCheck("vail")
							   )
							{
								vail.UseAbility(e.Position);
								Utils.Sleep(250, "vail");
							} // orchid Item end
							if (vail == null || !vail.CanBeCasted() || me.IsSilenced() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name))
							{
								if (// ethereal
									   ethereal != null
									   && ethereal.CanBeCasted()
									   && me.CanCast()
									   && !e.IsLinkensProtected()
									   && !e.IsMagicImmune()
									   && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
									   && Utils.SleepCheck("ethereal")
									  )
								{
									ethereal.UseAbility(e);
									Utils.Sleep(200, "ethereal");
								} // ethereal Item end
								if (ethereal == null || !ethereal.CanBeCasted() || me.IsSilenced() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
								{
									if (
										 Q != null
										&& Q.CanBeCasted()
										&& me.CanCast()
										&& me.Distance2D(e) < 1400
										 && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
										&& Utils.SleepCheck("Q")
										)
									{
										Q.UseAbility(e);
										Utils.Sleep(200, "Q");
									}


									if (
									   R != null
									   && R.CanBeCasted()
									   && me.CanCast()
									   && !e.HasModifier("modifier_skywrath_mystic_flare_aura_effect")
									   && e.MovementSpeed <= 220
									   && me.Position.Distance2D(e) < 1200
									   && e.Health >= (e.MaximumHealth / 100 * Menu.Item("Healh").GetValue<Slider>().Value)
										  && !me.HasModifier("modifier_pugna_nether_ward_aura")
										  && !e.HasModifier("modifier_item_blade_mail_reflect")
									   && !e.HasModifier("modifier_skywrath_mystic_flare_aura_effect")
									   && !e.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
									   && !e.HasModifier("modifier_puck_phase_shift")
									   && !e.HasModifier("modifier_eul_cyclone")
										  && !e.HasModifier("modifier_dazzle_shallow_grave")
									   && !e.HasModifier("modifier_brewmaster_storm_cyclone")
									   && !e.HasModifier("modifier_spirit_breaker_charge_of_darkness")
									   && !e.HasModifier("modifier_shadow_demon_disruption")
									   && !e.HasModifier("modifier_tusk_snowball_movement")
									   && !e.IsMagicImmune()
									   && (e.FindSpell("abaddon_borrowed_time").Cooldown > 0 && !e.HasModifier("modifier_abaddon_borrowed_time_damage_redirect"))
									   && (e.FindItem("item_cyclone") != null && e.FindItem("item_cyclone").Cooldown > 0
									   || (e.FindItem("item_cyclone") == null || e.IsStunned() || e.IsHexed() || e.IsRooted()))
									   && (e.FindItem("item_force_staff") != null && e.FindItem("item_force_staff").Cooldown > 0
									   || (e.FindItem("item_force_staff") == null || e.IsStunned() || e.IsHexed() || e.IsRooted()))

									   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
									   && Utils.SleepCheck("R"))
									{
										R.UseAbility(Prediction.InFront(e, 100));
										Utils.Sleep(330, "R");
									}

									if (// SoulRing Item 
										soul != null
										&& soul.CanBeCasted()
										&& me.CanCast()
										&& me.Health >= (me.MaximumHealth * 0.5)
										&& me.Mana <= R.ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name)
										)
									{
										soul.UseAbility();
									} // SoulRing Item end

									if (// Arcane Boots Item
										arcane != null
										&& arcane.CanBeCasted()
										&& me.CanCast()
										&& me.Mana <= R.ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
										)
									{
										arcane.UseAbility();
									} // Arcane Boots Item end

									if (//Ghost
										ghost != null
										&& ghost.CanBeCasted()
										&& me.CanCast()
										&& ((me.Position.Distance2D(e) < 300
										&& me.Health <= (me.MaximumHealth * 0.7))
										|| me.Health <= (me.MaximumHealth * 0.3))
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
										&& Utils.SleepCheck("Ghost"))
									{
										ghost.UseAbility();
										Utils.Sleep(250, "Ghost");
									}


									if (// Shiva Item
										shiva != null
										&& shiva.CanBeCasted()
										&& me.CanCast()
										&& !e.IsMagicImmune()
										&& Utils.SleepCheck("shiva")
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
										&& me.Distance2D(e) <= 600
										)

									{
										shiva.UseAbility();
										Utils.Sleep(250, "shiva");
									} // Shiva Item end

									if (// Dagon
										me.CanCast()
										&& dagon != null
										&& (ethereal == null
										|| (e.HasModifier("modifier_item_ethereal_blade_slow")
										|| ethereal.Cooldown < 17))
										&& !e.IsLinkensProtected()
										&& dagon.CanBeCasted()
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
										&& !e.IsMagicImmune()
										&& Utils.SleepCheck("dagon")
									   )
									{
										dagon.UseAbility(e);
										Utils.Sleep(200, "dagon");
									} // Dagon Item end

									if (
										 // cheese
										 cheese != null
										 && cheese.CanBeCasted()
										 && me.Health <= (me.MaximumHealth * 0.3)
										 && me.Distance2D(e) <= 700
										 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
										 && Utils.SleepCheck("cheese")
									 )
									{
										cheese.UseAbility();
										Utils.Sleep(200, "cheese");
									} // cheese Item end
								}
							}
						}
						if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
						{
							Orbwalking.Orbwalk(e, 0, 1600, true, true);
						}
					}
				}
				Utils.Sleep(100, "activated");
			}
		} // Combo


		private Hero GetClosestToTarget(List<Hero> units, Hero z)
		{
			Hero closestHero = null;
			foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(z) > v.Distance2D(z)))
			{
				closestHero = v;
			}
			return closestHero;
		}

		public void A()
		{
			me = ObjectManager.LocalHero;
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.IsValid && x.Team != me.Team && !x.IsIllusion).ToList();

			var ecount = v.Count();
			if (ecount == 0) return;
			if (Menu.Item("autoUlt").GetValue<bool>() && me.IsAlive)
			{
				if (!me.IsAlive) return;
				force = me.FindItem("item_force_staff");
				cyclone = me.FindItem("item_cyclone");
				E = me.Spellbook.SpellE;
				for (int i = 0; i < ecount; ++i)
				{
					var reflect = v[i].HasModifier("modifier_item_blade_mail_reflect");

					if (me.HasModifier("modifier_spirit_breaker_charge_of_darkness_vision"))
					{

						if (v[i].ClassID == ClassID.CDOTA_Unit_Hero_SpiritBreaker)
						{
							float angle = me.FindAngleBetween(v[i].Position, true);
							Vector3 pos = new Vector3((float)(me.Position.X + 100 * Math.Cos(angle)),
								(float)(me.Position.Y + 100 * Math.Sin(angle)), 0);

							if (W != null && W.CanBeCasted() && me.Distance2D(v[i]) <= 900 + Game.Ping
								&& !v[i].IsMagicImmune()
								&& Utils.SleepCheck(v[i].Handle.ToString())
								&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(W.Name)
							   )
								W.UseAbility();

							if (atos != null && R != null && R.CanBeCasted() && atos.CanBeCasted()
							   && me.Distance2D(v[i]) <= 900 + Game.Ping
							   && !v[i].IsMagicImmune()
								&& Utils.SleepCheck(v[i].Handle.ToString())
							   && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(atos.Name)
							   )
								atos.UseAbility(v[i]);

							if (R != null && R.CanBeCasted() && me.Distance2D(v[i]) <= 700 + Game.Ping
							   && !v[i].HasModifier("modifier_item_blade_mail_reflect")
							   && !v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
							   && !v[i].HasModifier("modifier_dazzle_shallow_grave")
							   && !v[i].IsMagicImmune()
								&& Utils.SleepCheck(v[i].Handle.ToString())
							   && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(R.Name)
							   )
								R.UseAbility(pos);
							

							if (cyclone != null && !R.CanBeCasted()
								&& cyclone.CanBeCasted()
								&& me.Distance2D(v[i]) <= 500 + Game.Ping
								&& Utils.SleepCheck(v[i].Handle.ToString())
								&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(cyclone.Name)
								)
								cyclone.UseAbility(me);
							Utils.Sleep(150, v[i].Handle.ToString());

						}

					}

					if (cyclone != null && reflect && cyclone.CanBeCasted() &&
						v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect") &&
						me.Distance2D(v[i]) < cyclone.GetCastRange()
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(cyclone.Name)
						 && Utils.SleepCheck(v[i].Handle.ToString())
						)
						cyclone.UseAbility(me);
					if (R != null && R.CanBeCasted() && me.Distance2D(v[i]) <= R.GetCastRange() + 100
						&& !me.HasModifier("modifier_pugna_nether_ward_aura")
						&&
						(v[i].HasModifier("modifier_meepo_earthbind")
						 || v[i].HasModifier("modifier_pudge_dismember")
						 || v[i].HasModifier("modifier_naga_siren_ensnare")
						 || v[i].HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
						 || (v[i].HasModifier("modifier_legion_commander_duel")
						  && !v[i].AghanimState())
						 || v[i].HasModifier("modifier_kunkka_torrent")
						 || v[i].HasModifier("modifier_ice_blast")
						 || v[i].HasModifier("modifier_crystal_maiden_crystal_nova")
						 || v[i].HasModifier("modifier_enigma_black_hole_pull")
						 || v[i].HasModifier("modifier_ember_spirit_searing_chains")
						 || v[i].HasModifier("modifier_dark_troll_warlord_ensnare")
						 || v[i].HasModifier("modifier_crystal_maiden_frostbite")
						 ||
						 (v[i].FindSpell("rattletrap_power_cogs") != null &&
						 v[i].FindSpell("rattletrap_power_cogs").IsInAbilityPhase)
						 || v[i].HasModifier("modifier_axe_berserkers_call")
						 || v[i].HasModifier("modifier_bane_fiends_grip")
						 || (v[i].HasModifier("modifier_faceless_void_chronosphere_freeze")
						 && v[i].ClassID != ClassID.CDOTA_Unit_Hero_FacelessVoid)
						 || v[i].HasModifier("modifier_storm_spirit_electric_vortex_pull")
						 || (v[i].FindSpell("witch_doctor_death_ward") != null
						 && v[i].FindSpell("witch_doctor_death_ward").IsInAbilityPhase)
						 || (v[i].FindSpell("crystal_maiden_crystal_nova") != null
						 && v[i].FindSpell("crystal_maiden_crystal_nova").IsInAbilityPhase)
						 || v[i].IsStunned())
						 && (v[i].FindItem("item_cyclone") != null && v[i].FindItem("item_cyclone").Cooldown > 0
						 || (v[i].FindItem("item_cyclone") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
						 && (v[i].FindItem("item_force_staff") != null && v[i].FindItem("item_force_staff").Cooldown > 0
						 || (v[i].FindItem("item_force_staff") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
						 && (!v[i].HasModifier("modifier_medusa_stone_gaze_stone")
						 && !v[i].HasModifier("modifier_faceless_void_time_walk")
						 && !v[i].HasModifier("modifier_item_monkey_king_bar")
						 && !v[i].HasModifier("modifier_rattletrap_battery_assault")
						 && !v[i].HasModifier("modifier_item_blade_mail_reflect")
						 && !v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
						 && !v[i].HasModifier("modifier_pudge_meat_hook")
						 && !v[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
						 && !v[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						 && !v[i].HasModifier("modifier_puck_phase_shift")
						 && !v[i].HasModifier("modifier_eul_cyclone")
						 && !v[i].HasModifier("modifier_invoker_tornado")
						 && !v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
						 && !v[i].HasModifier("modifier_dazzle_shallow_grave")
						 && !v[i].HasModifier("modifier_mirana_leap")
						 && !v[i].HasModifier("modifier_abaddon_borrowed_time")
						 && !v[i].HasModifier("modifier_winter_wyvern_winters_curse")
						 && !v[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
						 && !v[i].HasModifier("modifier_brewmaster_storm_cyclone")
						 && !v[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
						 && !v[i].HasModifier("modifier_shadow_demon_disruption")
						 && !v[i].HasModifier("modifier_tusk_snowball_movement")
						 && !v[i].HasModifier("modifier_invoker_tornado")
						 && ((v[i].FindSpell("abaddon_borrowed_time") != null
							  && v[i].FindSpell("abaddon_borrowed_time").Cooldown > 0)
							 || v[i].FindSpell("abaddon_borrowed_time") == null)

						 && v[i].Health >= (v[i].MaximumHealth / 100 * (Menu.Item("Healh").GetValue<Slider>().Value))
						 && !v[i].IsMagicImmune())
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(R.Name)
						 && Utils.SleepCheck(v[i].Handle.ToString())
						)
						R.UseAbility(Prediction.InFront(v[i], 70));

					if (R != null && R.CanBeCasted() && me.Distance2D(v[i]) <= R.GetCastRange() + 100
						&& !me.HasModifier("modifier_pugna_nether_ward_aura")
						&& v[i].MovementSpeed <= 240 
						&& v[i].MagicDamageResist <= 0.07
						&& !v[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
						&& !v[i].HasModifier("modifier_item_blade_mail_reflect")
						&& !v[i].HasModifier("modifier_sniper_headshot")
						&& !v[i].HasModifier("modifier_leshrac_lightning_storm_slow")
						&& !v[i].HasModifier("modifier_razor_unstablecurrent_slow")
						&& !v[i].HasModifier("modifier_pudge_meat_hook")
						&& !v[i].HasModifier("modifier_tusk_snowball_movement")
						&& !v[i].HasModifier("modifier_faceless_void_time_walk")
						&& !v[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						&& !v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
						&& !v[i].HasModifier("modifier_puck_phase_shift")
						&& !v[i].HasModifier("modifier_abaddon_borrowed_time")
						&& !v[i].HasModifier("modifier_winter_wyvern_winters_curse")
						&& !v[i].HasModifier("modifier_eul_cyclone")
						&& !v[i].HasModifier("modifier_dazzle_shallow_grave")
						&& !v[i].HasModifier("modifier_brewmaster_storm_cyclone")
						&& !v[i].HasModifier("modifier_mirana_leap")
						&& !v[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
						&& !v[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
						&& !v[i].HasModifier("modifier_shadow_demon_disruption")
						&& ((v[i].FindSpell("abaddon_borrowed_time") != null
						  && v[i].FindSpell("abaddon_borrowed_time").Cooldown > 0)
						 || v[i].FindSpell("abaddon_borrowed_time") == null)
						 && (v[i].FindItem("item_cyclone") != null && v[i].FindItem("item_cyclone").Cooldown > 0
						 || (v[i].FindItem("item_cyclone") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
						 && (v[i].FindItem("item_force_staff") != null && v[i].FindItem("item_force_staff").Cooldown > 0
						 || (v[i].FindItem("item_force_staff") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
						&& v[i].Health >= (v[i].MaximumHealth / 100 * (Menu.Item("Healh").GetValue<Slider>().Value))
						&& !v[i].IsMagicImmune()
						 && Utils.SleepCheck(v[i].Handle.ToString())
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(R.Name)
						)
						R.UseAbility(Prediction.InFront(v[i], 90));

					if (R != null && R.CanBeCasted() && me.Distance2D(v[i]) <= R.GetCastRange() + 100
						&& !me.HasModifier("modifier_pugna_nether_ward_aura")
						&& v[i].MovementSpeed <= 240
						&& !E.CanBeCasted()
						&& v[i].Health >= (v[i].MaximumHealth / 100 * (Menu.Item("Healh").GetValue<Slider>().Value))
						&& !v[i].HasModifier("modifier_item_blade_mail_reflect")
						&& !v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
						&& !v[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
						&& !v[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						&& !v[i].HasModifier("modifier_puck_phase_shift")
						&& !v[i].HasModifier("modifier_eul_cyclone")
						&& !v[i].HasModifier("modifier_invoker_tornado")
						&& !v[i].HasModifier("modifier_dazzle_shallow_grave")
						&& !v[i].HasModifier("modifier_brewmaster_storm_cyclone")
						&& !v[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
						&& !v[i].HasModifier("modifier_shadow_demon_disruption")
						&& !v[i].HasModifier("modifier_faceless_void_time_walk")
						&& !v[i].HasModifier("modifier_winter_wyvern_winters_curse")
						&& !v[i].HasModifier("modifier_mirana_leap")
						&& !v[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
						&& !v[i].HasModifier("modifier_tusk_snowball_movement")
						&& !v[i].IsMagicImmune()
						&& !v[i].HasModifier("modifier_abaddon_borrowed_time")
						&& ((v[i].FindSpell("abaddon_borrowed_time") != null
						  && v[i].FindSpell("abaddon_borrowed_time").Cooldown > 0)
						 || v[i].FindSpell("abaddon_borrowed_time") == null)
						&& (v[i].FindItem("item_cyclone") != null && v[i].FindItem("item_cyclone").Cooldown > 0
						|| (v[i].FindItem("item_cyclone") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
						&& (v[i].FindItem("item_force_staff") != null && v[i].FindItem("item_force_staff").Cooldown > 0
						|| (v[i].FindItem("item_force_staff") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(R.Name)
						 && Utils.SleepCheck(v[i].Handle.ToString())
						)
						R.UseAbility(Prediction.InFront(v[i], 90));

					if (W != null && W.CanBeCasted() && me.Distance2D(v[i]) <= 1400
						&& ((v[i].MovementSpeed <= 255
						&& !v[i].HasModifier("modifier_phantom_assassin_stiflingdagger"))
						|| (v[i].Distance2D(me) <= me.HullRadius + 24
						&& v[i].NetworkActivity == NetworkActivity.Attack)
						|| v[i].MagicDamageResist <= 0.07)
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(W.Name)
						 && Utils.SleepCheck(v[i].Handle.ToString())
						&& !v[i].IsMagicImmune()
						)
						W.UseAbility();

					if (atos != null && R != null && R.CanBeCasted() && atos.CanBeCasted()
						&& !v[i].IsLinkensProtected()
						&& me.Distance2D(v[i]) <= 1200
						&& v[i].MagicDamageResist <= 0.07
						&& !v[i].IsMagicImmune()
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(atos.Name)
						 && Utils.SleepCheck(v[i].Handle.ToString())
						)
						atos.UseAbility(v[i]);

					if (vail != null && v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
						&& vail.CanBeCasted()
						&& !v[i].IsMagicImmune()
						&& me.Distance2D(v[i]) <= 1200
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(vail.Name)
						 && Utils.SleepCheck(v[i].Handle.ToString())
						)
						vail.UseAbility(v[i].Position);
					if (E != null && !E.CanBeCasted() && !v[i].IsStunned() && !v[i].IsHexed() && !v[i].IsRooted() && (orchid != null && orchid.CanBeCasted() || sheep != null && sheep.CanBeCasted()))
						E = orchid ?? sheep;
					if (E != null && v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
						&& E.CanBeCasted()
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(E.Name)
						&& (v[i].FindItem("item_manta") != null && v[i].FindItem("item_manta").Cooldown > 0
						|| (v[i].FindItem("item_manta") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
						&& me.Distance2D(v[i]) <= 900
						 && Utils.SleepCheck(v[i].Handle.ToString())
						)
						E.UseAbility(v[i]);

					if (ethereal != null &&
						v[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
						&& !v[i].HasModifier("modifier_legion_commander_duel")
						&& ethereal.CanBeCasted()
						&& E.CanBeCasted()
						&& me.Distance2D(v[i]) <= ethereal.GetCastRange() + me.HullRadius
						&& Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
						 && Utils.SleepCheck(v[i].Handle.ToString())
						)
						ethereal.UseAbility(v[i]);

					if (E != null && E.CanBeCasted() && me.Distance2D(v[i]) <= E.GetCastRange()
						&& !v[i].IsLinkensProtected()
						&&
						(v[i].HasModifier("modifier_meepo_earthbind")
						 || v[i].HasModifier("modifier_pudge_dismember")
						 || v[i].HasModifier("modifier_naga_siren_ensnare")
						 || v[i].HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
						 || v[i].HasModifier("modifier_legion_commander_duel")
						 || v[i].HasModifier("modifier_kunkka_torrent")
						 || v[i].HasModifier("modifier_ice_blast")
						 || v[i].HasModifier("modifier_enigma_black_hole_pull")
						 || v[i].HasModifier("modifier_ember_spirit_searing_chains")
						 || v[i].HasModifier("modifier_dark_troll_warlord_ensnare")
						 || v[i].HasModifier("modifier_crystal_maiden_crystal_nova")
						 || v[i].HasModifier("modifier_axe_berserkers_call")
						 || v[i].HasModifier("modifier_bane_fiends_grip")
						 || v[i].HasModifier("modifier_rubick_telekinesis")
						 || v[i].HasModifier("modifier_storm_spirit_electric_vortex_pull")
						 || v[i].HasModifier("modifier_winter_wyvern_cold_embrace")
						 || v[i].HasModifier("modifier_shadow_shaman_shackles")
						 || (v[i].FindSpell("magnataur_reverse_polarity") != null
						 && v[i].FindSpell("magnataur_reverse_polarity").IsInAbilityPhase)
						 || (v[i].FindItem("item_blink") != null && v[i].FindItem("item_blink").Cooldown > 11)
						 || (v[i].FindSpell("queenofpain_blink") != null
						 && v[i].FindSpell("queenofpain_blink").IsInAbilityPhase)
						 || (v[i].FindSpell("antimage_blink") != null && v[i].FindSpell("antimage_blink").IsInAbilityPhase)
						 || (v[i].FindSpell("antimage_mana_void") != null
						 && v[i].FindSpell("antimage_mana_void").IsInAbilityPhase)
						 || (v[i].FindSpell("legion_commander_duel") != null
						 && v[i].FindSpell("legion_commander_duel").Cooldown <= 0)
						 || (v[i].FindSpell("doom_bringer_doom") != null
						 && v[i].FindSpell("doom_bringer_doom").IsInAbilityPhase)
						 || (v[i].HasModifier("modifier_faceless_void_chronosphere_freeze")
						 && v[i].ClassID != ClassID.CDOTA_Unit_Hero_FacelessVoid)
						 || (v[i].FindSpell("witch_doctor_death_ward") != null &&
						 v[i].FindSpell("witch_doctor_death_ward").IsInAbilityPhase)
						 || (v[i].FindSpell("rattletrap_power_cogs") != null &&
						 v[i].FindSpell("rattletrap_power_cogs").IsInAbilityPhase)
						 || (v[i].FindSpell("tidehunter_ravage") != null &&
						 v[i].FindSpell("tidehunter_ravage").IsInAbilityPhase)
						 || (v[i].FindSpell("axe_berserkers_call") != null &&
						 v[i].FindSpell("axe_berserkers_call").IsInAbilityPhase)
						 || (v[i].FindSpell("brewmaster_primal_split") != null &&
						 v[i].FindSpell("brewmaster_primal_split").IsInAbilityPhase)
						 || (v[i].FindSpell("omniknight_guardian_angel") != null &&
						 v[i].FindSpell("omniknight_guardian_angel").IsInAbilityPhase)
						 || (v[i].FindSpell("queenofpain_sonic_wave") != null &&
						 v[i].FindSpell("queenofpain_sonic_wave").IsInAbilityPhase)
						 || (v[i].FindSpell("sandking_epicenter") != null &&
						 v[i].FindSpell("sandking_epicenter").IsInAbilityPhase)
						 || (v[i].FindSpell("slardar_slithereen_crush") != null &&
						 v[i].FindSpell("slardar_slithereen_crush").IsInAbilityPhase)
						 || (v[i].FindSpell("tiny_toss") != null && v[i].FindSpell("tiny_toss").IsInAbilityPhase)
						 || (v[i].FindSpell("tusk_walrus_punch") != null &&
						 v[i].FindSpell("tusk_walrus_punch").IsInAbilityPhase)
						 || (v[i].FindSpell("faceless_void_time_walk") != null &&
						 v[i].FindSpell("faceless_void_time_walk").IsInAbilityPhase)
						 || (v[i].FindSpell("faceless_void_chronosphere") != null
						 && v[i].FindSpell("faceless_void_chronosphere").IsInAbilityPhase)
						 || (v[i].FindSpell("disruptor_static_storm") != null
						 && v[i].FindSpell("disruptor_static_storm").Cooldown <= 0)
						 || (v[i].FindSpell("lion_finger_of_death") != null
						 && v[i].FindSpell("lion_finger_of_death").Cooldown <= 0)
						 || (v[i].FindSpell("luna_eclipse") != null && v[i].FindSpell("luna_eclipse").Cooldown <= 0)
						 || (v[i].FindSpell("lina_laguna_blade") != null && v[i].FindSpell("lina_laguna_blade").Cooldown <= 0)
						 || (v[i].FindSpell("doom_bringer_doom") != null && v[i].FindSpell("doom_bringer_doom").Cooldown <= 0)
						 || (v[i].FindSpell("life_stealer_rage") != null && v[i].FindSpell("life_stealer_rage").Cooldown <= 0
						 && me.Level >= 7)
						 )
						 && (v[i].FindItem("item_manta") != null && v[i].FindItem("item_manta").Cooldown > 0
						 || (v[i].FindItem("item_manta") == null || v[i].IsStunned() || v[i].IsHexed() || v[i].IsRooted()))
						 && !v[i].IsMagicImmune()
						 && Menu.Item("AutoAbility").GetValue<AbilityToggler>().IsEnabled(E.Name)
						 && !v[i].HasModifier("modifier_medusa_stone_gaze_stone")
						 && Utils.SleepCheck(v[i].Handle.ToString())
						 )
						E.UseAbility(v[i]);
					


					if (v[i].IsLinkensProtected() && (me.IsVisibleToEnemies || Active) && Utils.SleepCheck(v[i].Handle.ToString()))
					{
						if (force != null && force.CanBeCasted() && me.Distance2D(v[i]) < force.GetCastRange() &&
							Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(force.Name))
							force.UseAbility(v[i]);
						else if (cyclone != null && cyclone.CanBeCasted() && me.Distance2D(v[i]) < cyclone.GetCastRange() &&
							  Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(cyclone.Name))
							cyclone.UseAbility(v[i]);
						else if (atos != null && atos.CanBeCasted() && me.Distance2D(v[i]) < atos.GetCastRange() - 400 &&
							  Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(atos.Name))
							atos.UseAbility(v[i]);
						else if (dagon != null && dagon.CanBeCasted() && me.Distance2D(v[i]) < dagon.GetCastRange() &&
							  Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
							dagon.UseAbility(v[i]);
						else if (orchid != null && orchid.CanBeCasted() && me.Distance2D(v[i]) < orchid.GetCastRange() &&
							  Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
							orchid.UseAbility(v[i]);
						Utils.Sleep(350, v[i].Handle.ToString());
					}
					Utils.Sleep(250, v[i].Handle.ToString());
				}
			}
		} // SkywrathMage class
	}
}