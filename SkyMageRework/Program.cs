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
		private static Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost, core, force, cyclone;
		private static Ability Q, W, E, R;
		private static Font txt;
		private static Font noti;
		private static Line lines;
		private static readonly Menu skills = new Menu("Skills", "Skills");
		private static readonly Menu items = new Menu("Items", "Items");
		private static readonly Menu link = new Menu("Link", "Link");
		private static readonly Menu ult = new Menu("AutoUlt", "Auto");
		private static readonly Menu healh = new Menu("Healh", "Min % Enemy Healh to Ult"); 
		private static readonly Dictionary<string, bool> Skills = new Dictionary<string, bool>
			{
				{"skywrath_mage_arcane_bolt",true},
				{"skywrath_mage_concussive_shot",true},
				{"skywrath_mage_mystic_flare",true}
			};
		private static readonly Dictionary<string, bool> AutoUlt = new Dictionary<string, bool>
			{
				{"skywrath_mage_mystic_flare",true}
			};
		private static readonly Dictionary<string, bool> Link = new Dictionary<string, bool>
			{
				{"item_force_staff",true},
				{"item_cyclone",true},
				{"item_orchid",true},
				{"item_rod_of_atos",true},
			    {"item_dagon",true}
			};
		private static readonly Dictionary<string, bool> Items = new Dictionary<string, bool>
			{
				{"item_orchid",true},
				{"item_ethereal_blade",true},
				{"item_veil_of_discord",true},
				{"item_rod_of_atos",true},
				{"item_sheepstick",true},
				{"item_arcane_boots",true},
				{"item_blink",true},
				{"item_soul_ring",true},
				{"item_ghost",true},
				{"item_cheese",true}
			};


		


		static void Main(string[] args)
		{
			Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddSubMenu(skills);
			Menu.AddSubMenu(items);
			Menu.AddSubMenu(ult);
			Menu.AddSubMenu(healh);
			skills.AddItem(new MenuItem("Skills: ", "Skills: ").SetValue(new AbilityToggler(Skills)));
			items.AddItem(new MenuItem("Items: ", "Items:").SetValue(new AbilityToggler(Items)));
			ult.AddItem(new MenuItem("autoUlt", "AutoSpell and Items").SetValue(true));
			ult.AddItem(new MenuItem("AutoUlt: ", "AutoUlt").SetValue(new AbilityToggler(AutoUlt)));
			items.AddItem(new MenuItem("Link: ", "Auto triggre Linken").SetValue(new AbilityToggler(Link)));
			healh.AddItem(new MenuItem("Healh", "Min healh % to ult").SetValue(new Slider(35, 10, 70))); // x/ 10%
			Menu.AddToMainMenu();
			Game.OnUpdate += Game_OnUpdate;
			Game.OnUpdate += A;
		//	Game.OnWndProc += Game_OnWndProc;
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

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}

		public static void Game_OnUpdate(EventArgs args)
		{
			var me = ObjectMgr.LocalHero;

			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage || me == null || Game.IsWatchingGame)
			{
				return;
			}

			var target = me.ClosestToMouseTarget(2000);
			if (target == null)
			{
				return;
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




			
			var ModifW = target.Modifiers.Any(y => y.Name == "modifier_skywrath_mage_concussive_shot_slow");
			var ModifR = target.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect");
			var ModifE = target.Modifiers.Any(y => y.Name == "modifier_skywrath_mage_ancient_seal");
			var ModifRod = target.Modifiers.Any(y => y.Name == "modifier_rod_of_atos_debuff");
			var ModifEther = target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
			var ModifVail = target.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff");
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
					&& !stoneModif
					&& Utils.SleepCheck("Q")
					)
				{
					Q.UseAbility(target);
					Utils.Sleep(200, "Q");
				}
			}
			if (Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key) && me.IsAlive && target.IsAlive && Utils.SleepCheck("activated"))
			{
				var noBlade = target.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect");
				if (target.IsVisible && me.Distance2D(target) <= 2300 && !noBlade)
				{
					if (
									  Q != null
									  && Q.CanBeCasted()
									  && (target.IsLinkensProtected()
									  || !target.IsLinkensProtected())
									  && me.CanCast()
									  && me.Distance2D(target) < 1400
									  && !stoneModif
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
						&& me.Distance2D(target) < 900
                        && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(W.Name)
                        && Utils.SleepCheck("W"))
					{
						W.UseAbility();
						Utils.Sleep(300, "W");
					}
					if (
                                  blink != null
								  && Q.CanBeCasted()
								  && me.CanCast()
								  && blink.CanBeCasted()
								  && me.Distance2D(target) > 1000
								  && !stoneModif
								  && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(blink.Name)
								  && Utils.SleepCheck("blink")
						)
					{
						blink.UseAbility(target.Position);

						Utils.Sleep(250, "blink");
					}
					if (
						   E != null
						   && E.CanBeCasted()
						   && me.CanCast() 
                           && !target.IsLinkensProtected()
						   && me.Position.Distance2D(target) < 1400
						   && !stoneModif
						   && Utils.SleepCheck("E"))
					{
						E.UseAbility(target);
						Utils.Sleep(200, "E");
					}
					if(!E.CanBeCasted() || E == null)
					{
						if ( // orchid
								  orchid != null
								  && orchid.CanBeCasted()
								  && me.CanCast()
								  && !target.IsLinkensProtected()
								  && !target.IsMagicImmune()
								  && me.Distance2D(target) <= 1400
								  && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
								  && !stoneModif
								  && Utils.SleepCheck("orchid")
							)
						{
							orchid.UseAbility(target);
							Utils.Sleep(250, "orchid");
						} // orchid Item end
						if (!orchid.CanBeCasted() || orchid == null || !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
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
							if (!vail.CanBeCasted() || vail == null || !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(vail.Name))
							{
								if (// ethereal
									   ethereal != null
									   && ethereal.CanBeCasted()
									   && me.CanCast()
									   && !target.IsLinkensProtected()
									   && !target.IsMagicImmune()
									   && !stoneModif
									   && Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
									   && Utils.SleepCheck("ethereal")
									  )
								{
									ethereal.UseAbility(target);
									Utils.Sleep(200, "ethereal");
								} // ethereal Item end
								if (!ethereal.CanBeCasted() || ethereal == null || !Menu.Item("Items: ").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
								{
									if (
										 Q != null
										&& Q.CanBeCasted()
										&& me.CanCast()
										&& me.Distance2D(target) < 1400
										&& !stoneModif
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
									   && !ModifR
									   && (ModifW
									   || ModifEther
									   || ModifRod)
									   && me.Position.Distance2D(target) < 1200
									   && target.Health >= (target.MaximumHealth / 100 * Menu.Item("Healh").GetValue<Slider>().Value)
									   && !stoneModif
									   && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(R.Name)
									   && Utils.SleepCheck("R"))
									{
										R.UseAbility(target.Predict(500));
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
										&& !stoneModif
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
										|| (ModifEther
										|| ethereal.Cooldown < 17))
										&& !target.IsLinkensProtected()
										&& dagon.CanBeCasted()
										&& !target.IsMagicImmune()
										&& !stoneModif
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
					}
				}
				Utils.Sleep(200, "activated");
			}
		}
		public static void A(EventArgs args)
		{
			var me = ObjectMgr.LocalHero;

			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage || me == null)
			{
				return;
			}


			var enemies =ObjectMgr.GetEntities<Hero>().Where(x => x.IsVisible && x.IsAlive && x.Team == me.GetEnemyTeam() && !x.IsIllusion);
			if (Menu.Item("autoUlt").GetValue<bool>())
			{
				 core = me.FindItem("item_octarine_core");
				 force = me.FindItem("item_force_staff");
				 cyclone = me.FindItem("item_cyclone");
				 orchid = me.FindItem("item_orchid");
				 atos = me.FindItem("item_rod_of_atos");
				
				foreach (var e in enemies)
				{
					if (e == null)
						return;
					bool reflect = e.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect");
					{

						if (cyclone != null && reflect && cyclone.CanBeCasted() && me.Distance2D(e) < cyclone.CastRange && Utils.SleepCheck(e.Handle.ToString()))
						{
							cyclone.UseAbility(me);
							Utils.Sleep(500, e.Handle.ToString());
						}
						if ((Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key) && e.MovementSpeed <= 200 && e.Modifiers.Any(y => y.Name == "modifier_skywrath_mage_ancient_seal") || ((E.Cooldown <= 14 && core == null) || E.Cooldown <= 10 && core != null || e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow"))))
						{
							if (R != null && e != null && R.CanBeCasted() && me.Distance2D(e) <= 1200
								&& e.MovementSpeed <= 200
								&& !e.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect")
								&& !e.Modifiers.Any(y => y.Name == "modifier_sniper_headshot")
								&& !e.Modifiers.Any(y => y.Name == "modifier_leshrac_lightning_storm_slow")
								&& !e.Modifiers.Any(y => y.Name == "modifier_razor_unstablecurrent_slow")
								&& !e.Modifiers.Any(y => y.Name == "modifier_pudge_meat_hook")
								&& !e.Modifiers.Any(y => y.Name == "modifier_tusk_snowball_movement")
								&& !e.Modifiers.Any(y => y.Name == "modifier_obsidian_destroyer_astral_imprisonment_prison")
								&& !e.Modifiers.Any(y => y.Name == "modifier_puck_phase_shift")
								&& !e.Modifiers.Any(y => y.Name == "modifier_eul_cyclone")
								&& !e.Modifiers.Any(y => y.Name == "modifier_dazzle_shallow_grave")
								&& !e.Modifiers.Any(y => y.Name == "modifier_brewmaster_storm_cyclone")
								&& !e.Modifiers.Any(y => y.Name == "modifier_spirit_breaker_charge_of_darkness")
								&& !e.Modifiers.Any(y => y.Name == "modifier_shadow_demon_disruption")
								&& (!e.FindSpell("abaddon_borrowed_time").CanBeCasted() && !e.Modifiers.Any(y => y.Name == "modifier_abaddon_borrowed_time_damage_redirect"))
								&& e.Health >= (e.MaximumHealth / 100 * (Menu.Item("Healh").GetValue<Slider>().Value))
								&& !e.IsMagicImmune()
								&& Menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name)
								&& Utils.SleepCheck(e.Handle.ToString()))
							{
								R.UseAbility(e.Predict(500));
								Utils.Sleep(300, e.Handle.ToString());
							}
							if (R != null && e != null && R.CanBeCasted() && me.Distance2D(e) <= 1200
							   &&
							   (
								  e.Modifiers.Any(y => y.Name == "modifier_meepo_earthbind")
							   || e.Modifiers.Any(y => y.Name == "modifier_pudge_dismember")
							   || e.Modifiers.Any(y => y.Name == "modifier_naga_siren_ensnare")
							   || e.Modifiers.Any(y => y.Name == "modifier_lone_druid_spirit_bear_entangle_effect")
							   || (e.Modifiers.Any(y => y.Name == "modifier_legion_commander_duel") && !e.AghanimState())
							   || e.Modifiers.Any(y => y.Name == "modifier_kunkka_torrent")
							   || e.Modifiers.Any(y => y.Name == "modifier_ice_blast")
							   || e.Modifiers.Any(y => y.Name == "modifier_enigma_black_hole_pull")
							   || e.Modifiers.Any(y => y.Name == "modifier_ember_spirit_searing_chains")
							   || e.Modifiers.Any(y => y.Name == "modifier_dark_troll_warlord_ensnare")
							   || e.Modifiers.Any(y => y.Name == "modifier_crystal_maiden_frostbite")
							   || e.ClassID == ClassID.CDOTA_Unit_Hero_Rattletrap && e.FindSpell("rattletrap_power_cogs").IsInAbilityPhase
							   || e.Modifiers.Any(y => y.Name == "modifier_axe_berserkers_call")
							   || e.Modifiers.Any(y => y.Name == "modifier_bane_fiends_grip")
							   || e.Modifiers.Any(y => y.Name == "modifier_faceless_void_chronosphere_freeze") && e.ClassID != ClassID.CDOTA_Unit_Hero_FacelessVoid
							   || e.Modifiers.Any(y => y.Name == "modifier_storm_spirit_electric_vortex_pull")
							   || (e.ClassID == ClassID.CDOTA_Unit_Hero_WitchDoctor && e.FindSpell("witch_doctor_death_ward").IsInAbilityPhase)
							   || (e.ClassID == ClassID.CDOTA_Unit_Hero_CrystalMaiden && e.FindSpell("crystal_maiden_crystal_nova").IsInAbilityPhase)
							   || e.IsStunned()
							   )
							   && (!e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone")
							   && !e.Modifiers.Any(y => y.Name == "modifier_item_monkey_king_bar")
							   && !e.FindSpell("abaddon_borrowed_time").CanBeCasted() && !e.Modifiers.Any(y => y.Name == "modifier_abaddon_borrowed_time_damage_redirect")
							   && !e.Modifiers.Any(y => y.Name == "modifier_rattletrap_battery_assault")
							   && !e.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect")
							   && !e.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect")
							   && !e.Modifiers.Any(y => y.Name == "modifier_pudge_meat_hook")
							   && !e.Modifiers.Any(y => y.Name == "modifier_obsidian_destroyer_astral_imprisonment_prison")
							   && !e.Modifiers.Any(y => y.Name == "modifier_puck_phase_shift")
							   && !e.Modifiers.Any(y => y.Name == "modifier_eul_cyclone")
							   && !e.Modifiers.Any(y => y.Name == "modifier_dazzle_shallow_grave")
							   && !e.Modifiers.Any(y => y.Name == "modifier_brewmaster_storm_cyclone")
							   && !e.Modifiers.Any(y => y.Name == "modifier_spirit_breaker_charge_of_darkness")
							   && !e.Modifiers.Any(y => y.Name == "modifier_shadow_demon_disruption")
							   && !e.Modifiers.Any(y => y.Name == "modifier_tusk_snowball_movement")
							   && (!e.FindSpell("abaddon_borrowed_time").CanBeCasted() && !e.Modifiers.Any(y => y.Name == "modifier_abaddon_borrowed_time_damage_redirect"))
							   && e.Health >= (e.MaximumHealth / 100 * (Menu.Item("Healh").GetValue<Slider>().Value))
							   && !e.IsMagicImmune())
							   && Menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name)
							   && Utils.SleepCheck(e.Handle.ToString()))
							{
								R.UseAbility(e.Predict(500));
								Utils.Sleep(300, e.Handle.ToString());
							}
							if (R != null && e != null && R.CanBeCasted() && me.Distance2D(e) <= 1200
							   && e.MovementSpeed <= 200
							   && e.MagicDamageResist <= 0.07
							   && e.Health >= (e.MaximumHealth/100 * (Menu.Item("Healh").GetValue<Slider>().Value))
							   && !e.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect")
							   && !e.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect")
							   && !e.Modifiers.Any(y => y.Name == "modifier_obsidian_destroyer_astral_imprisonment_prison")
							   && !e.Modifiers.Any(y => y.Name == "modifier_puck_phase_shift")
							   && !e.Modifiers.Any(y => y.Name == "modifier_eul_cyclone")
							   && !e.Modifiers.Any(y => y.Name == "modifier_dazzle_shallow_grave")
							   && !e.Modifiers.Any(y => y.Name == "modifier_brewmaster_storm_cyclone")
							   && !e.Modifiers.Any(y => y.Name == "modifier_spirit_breaker_charge_of_darkness")
							   && !e.Modifiers.Any(y => y.Name == "modifier_shadow_demon_disruption")
							   && !e.Modifiers.Any(y => y.Name == "modifier_tusk_snowball_movement")
							   && !e.IsMagicImmune()
							   && (!e.FindSpell("abaddon_borrowed_time").CanBeCasted() && !e.Modifiers.Any(y => y.Name == "modifier_abaddon_borrowed_time_damage_redirect"))
							   && Menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name)
							   && Utils.SleepCheck(e.Handle.ToString()))
							{
								R.UseAbility(e.Predict(500));
								Utils.Sleep(500, e.Handle.ToString()); goto leave;
							}
						}
						if (E != null && e != null && E.CanBeCasted() && me.Distance2D(e) <= 730
							&& !e.IsLinkensProtected()
						&&
						(
						   e.Modifiers.Any(y => y.Name == "modifier_meepo_earthbind")
						|| e.Modifiers.Any(y => y.Name == "modifier_pudge_dismember")
						|| e.Modifiers.Any(y => y.Name == "modifier_naga_siren_ensnare")
						|| e.Modifiers.Any(y => y.Name == "modifier_lone_druid_spirit_bear_entangle_effect")
						|| e.Modifiers.Any(y => y.Name == "modifier_legion_commander_duel")
						|| e.Modifiers.Any(y => y.Name == "modifier_kunkka_torrent")
						|| e.Modifiers.Any(y => y.Name == "modifier_ice_blast")
						|| e.Modifiers.Any(y => y.Name == "modifier_enigma_black_hole_pull")
						|| e.Modifiers.Any(y => y.Name == "modifier_ember_spirit_searing_chains")
						|| e.Modifiers.Any(y => y.Name == "modifier_dark_troll_warlord_ensnare")
						|| e.Modifiers.Any(y => y.Name == "modifier_crystal_maiden_frostbite")
						|| e.Modifiers.Any(y => y.Name == "modifier_axe_berserkers_call")
						|| e.Modifiers.Any(y => y.Name == "modifier_bane_fiends_grip")
						|| e.ClassID == ClassID.CDOTA_Unit_Hero_Magnataur && e.FindSpell("magnataur_reverse_polarity").IsInAbilityPhase
						|| e.FindItem("item_blink").IsInAbilityPhase
						|| e.ClassID == ClassID.CDOTA_Unit_Hero_QueenOfPain && e.FindSpell("queenofpain_blink").IsInAbilityPhase
						|| e.ClassID == ClassID.CDOTA_Unit_Hero_AntiMage && e.FindSpell("antimage_blink").IsInAbilityPhase
						|| e.ClassID == ClassID.CDOTA_Unit_Hero_AntiMage && e.FindSpell("antimage_mana_void").IsInAbilityPhase
						|| e.ClassID == ClassID.CDOTA_Unit_Hero_DoomBringer && e.FindSpell("doom_bringer_doom").IsInAbilityPhase
						|| e.Modifiers.Any(y => y.Name == "modifier_rubick_telekinesis")
						|| e.Modifiers.Any(y => y.Name == "modifier_storm_spirit_electric_vortex_pull")
						|| e.Modifiers.Any(y => y.Name == "modifier_winter_wyvern_cold_embrace")
						|| e.Modifiers.Any(y => y.Name == "modifier_winter_wyvern_winters_curse")
						|| e.Modifiers.Any(y => y.Name == "modifier_shadow_shaman_shackles")
						|| e.Modifiers.Any(y => y.Name == "modifier_faceless_void_chronosphere_freeze") && e.ClassID != ClassID.CDOTA_Unit_Hero_FacelessVoid
						|| e.ClassID == ClassID.CDOTA_Unit_Hero_WitchDoctor && e.FindSpell("witch_doctor_death_ward").IsInAbilityPhase
						|| e.ClassID == ClassID.CDOTA_Unit_Hero_Rattletrap && e.FindSpell("rattletrap_power_cogs").IsInAbilityPhase
						|| e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter && e.FindSpell("tidehunter_ravage").IsInAbilityPhase
						|| e.IsStunned()
						&& !e.IsMagicImmune()
						)
						&& !e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone")
						&& Utils.SleepCheck(e.Handle.ToString()))
						{
							E.UseAbility(e);
							Utils.Sleep(250, e.Handle.ToString()); goto leave;
						}
						 if (W != null && e != null && W.CanBeCasted() && me.Distance2D(e) <= 1400
							&& e.MovementSpeed <= 255
							&& !e.IsMagicImmune()
							&& Utils.SleepCheck(e.Handle.ToString()))
						{
							W.UseAbility();
							Utils.Sleep(500, e.Handle.ToString()); goto leave;
						}
						 if (atos != null && e != null && R != null && R.CanBeCasted() && atos.CanBeCasted()
							&& !e.IsLinkensProtected()
							&& me.Distance2D(e) <= 1200
							&& e.MagicDamageResist <= 0.07
							&& !e.IsMagicImmune()
							&& Utils.SleepCheck(e.Handle.ToString()))
						{
							atos.UseAbility(e);
							Utils.Sleep(500, e.Handle.ToString()); goto leave;
						}
						 if (vail != null && e.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect")
						   && vail.CanBeCasted() 
						   && me.Distance2D(e) <= 1200
						   && Utils.SleepCheck(e.Handle.ToString())
						  )
						{
							vail.UseAbility(e.Position);
							Utils.Sleep(500, e.Handle.ToString()); goto leave;
						}
						 if (E != null && e.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect")
							&& E.CanBeCasted()
							&& me.Distance2D(e) <= 900
							&& Utils.SleepCheck(e.Handle.ToString())
							)
						{
							E.UseAbility(e);
							Utils.Sleep(500, e.Handle.ToString()); goto leave;
						}
						 if (ethereal != null && e.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect")
							&& !e.Modifiers.Any(y => y.Name == "modifier_legion_commander_duel")
							&& ethereal.CanBeCasted()
							&& E.CanBeCasted()
							&& me.Distance2D(e) <= 1000
							&& Utils.SleepCheck(e.Handle.ToString())
							)
						{
							ethereal.UseAbility(e);
							Utils.Sleep(500, e.Handle.ToString()); goto leave;
						}
					}
					if(e.IsLinkensProtected() && (me.IsVisibleToEnemies || Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)))
					{
						if(force !=null && force.CanBeCasted() && me.Distance2D(e) < force.CastRange && Menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(force.Name) && Utils.SleepCheck(e.Handle.ToString()))
						{
							force.UseAbility(e);
							Utils.Sleep(500, e.Handle.ToString());
						}
						else if (cyclone != null && cyclone.CanBeCasted() && me.Distance2D(e) < cyclone.CastRange && Menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(cyclone.Name) && Utils.SleepCheck(e.Handle.ToString()))
						{
							cyclone.UseAbility(e);
							Utils.Sleep(500, e.Handle.ToString());
						}
						else if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) < orchid.CastRange && Menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(orchid.Name) && Utils.SleepCheck(e.Handle.ToString()))
						{
							orchid.UseAbility(e);
							Utils.Sleep(500, e.Handle.ToString());
						}
						else if (atos != null && atos.CanBeCasted() && me.Distance2D(e) < atos.CastRange-400 && Menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(atos.Name) && Utils.SleepCheck(e.Handle.ToString()))
						{
							atos.UseAbility(e);
							Utils.Sleep(500, e.Handle.ToString());
						}
						else if (dagon != null && dagon.CanBeCasted() && me.Distance2D(e) < dagon.CastRange && Menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(dagon.Name) && Utils.SleepCheck(e.Handle.ToString()))
						{
							dagon.UseAbility(e);
							Utils.Sleep(500, e.Handle.ToString());
						}
					}
				}
				leave:;
			}
		}
	
		static void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectMgr.LocalPlayer;
			var me = ObjectMgr.LocalHero;
			if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage)
				return;

			if (Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key))
            {
				DrawBox(2, 510, 125, 20, 1, new ColorBGRA(0, 0, 90, 90));
				DrawFilledBox(2, 510, 125, 20, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("SkyWrath#: Comboing!", 4, 510, Color.DeepPink, txt);
			}
			if (Menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name) && !Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key))
            {
				DrawBox(2, 510, 125, 20, 1, new ColorBGRA(0, 0, 90, 90));
				DrawFilledBox(2, 510, 125, 20, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("  Auto ult Enable", 4, 510, Color.DeepPink, txt);
			}
			if (!Menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name) && !Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key))
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


