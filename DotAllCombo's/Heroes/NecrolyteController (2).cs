using Ensage.Common;

namespace DotaAllCombo.Heroes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using SharpDX;

    internal class NecrolyteController : Variables, IHeroController
    {
		//private Ability Q, R;
		//private Item urn, ethereal, dagon, glimmer, vail, orchid, soul, leans, Shiva, mail, sheep, abyssal, bkb, lotus, arcane;
		
		//private double[] rDmg;
	    //private int[] qDmg;
		private Menu skills = new Menu("Skills", "Skills");
		private Menu items = new Menu("Items", "Items");
		
		public void OnLoadEvent()
		{
			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			skills.AddItem(new MenuItem("Skills", "Skills:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"necrolyte_death_pulse",true},
				{"necrolyte_reapers_scythe",false}
			})));
			items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_dagon", true},
				{"item_abyssal_blade",true},
				{"item_sheepstick",true},
				{"item_bloodthorn", true},
				{"item_lotus_orb", true},
				{"item_orchid",true},
				{"item_urn_of_shadows",true},
				{"item_ethereal_blade",true},
				{"item_veil_of_discord",true},
				{"item_rod_of_atos",true},
				{"item_shivas_guard",true},
				{"item_blade_mail",true},
				{"item_black_king_bar",true}
			})));
			//Menu.AddItem(new MenuItem("x", "Cancelling Hook").SetValue(new Slider(65, 1, 65)));
			//Menu.AddItem(new MenuItem("z", "Min Cancelling Hook Time").SetValue(new Slider(250, 100, 300)));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB|BladeMail").SetValue(new Slider(2, 1, 5)));
			Menu.AddSubMenu(skills);
			Menu.AddSubMenu(items);
		} // OnLoad
		public void OnCloseEvent()
		{
		} // OnClose
		

		public void Combo()
		{
			/*me = ObjectManager.LocalHero;
			

			Q = me.Spellbook.SpellQ;
			R = me.Spellbook.SpellR;

			leans = me.FindItem("item_aether_lens");
			urn = me.FindItem("item_urn_of_shadows");
			dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			ethereal = me.FindItem("item_ethereal_blade");
			Shiva = me.FindItem("item_shivas_guard");
			glimmer = me.FindItem("item_glimmer_cape");
			vail = me.FindItem("item_veil_of_discord");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
			abyssal = me.FindItem("item_abyssal_blade");
			bkb = me.FindItem("item_black_king_bar");
			mail = me.FindItem("item_blade_mail");
			lotus = me.FindItem("item_lotus_orb");
			soul = me.FindItem("item_soul_ring");

			arcane = me.FindItem("item_arcane_boots");
			var v = ObjectManager.GetEntities<Hero>().Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion).ToList();
			Active = Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key);
			
			e = me.ClosestToMouseTarget(1800);
			if (e == null) return;
			sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
			/*if (Active && me.Distance2D(e) <= 1400 && e.IsAlive)
			{
				uint countElse = 0;
				countElse += 1;
				if (vail != null && vail.CanBeCasted() && me.Distance2D(e) <= 1100 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name) && Utils.SleepCheck("vail"))
				{
					vail.UseAbility(e.Position);
					Utils.Sleep(130, "vail");
				}
				else countElse += 1;
				if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) <= 900 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) && Utils.SleepCheck("orchid"))
				{
					orchid.UseAbility(e);
					Utils.Sleep(100, "orchid");
				}
				else countElse += 1;
				if (lotus != null && lotus.CanBeCasted() && me.Distance2D(e) <= 600 && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(lotus.Name) && Utils.SleepCheck("lotus"))
				{
					lotus.UseAbility(me);
					Utils.Sleep(100, "lotus");
				}
				else countElse += 1;
				if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(e) <= 600 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name) && !e.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					Shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}
				else countElse += 1;
				if (ethereal != null && ethereal.CanBeCasted() && me.Distance2D(e) <= 700 && me.Distance2D(e) <= 400 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name) && Utils.SleepCheck("ethereal"))
				{
					ethereal.UseAbility(e);
					Utils.Sleep(100, "ethereal");
				}
				else countElse += 1;
				if (// Dagon
					me.CanCast()
					&& dagon != null && (ethereal == null || (e.HasModifier("modifier_item_ethereal_blade_slow") || ethereal.Cooldown < 17))
					&& !e.IsLinkensProtected() && dagon.CanBeCasted()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
					&& !e.IsMagicImmune()
					&& Utils.SleepCheck("dagon")
					)
				{
					dagon.UseAbility(e);
					Utils.Sleep(200, "dagon");
				} // Dagon Item end
				else countElse += 1;
				if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
				{
					urn.UseAbility(e);
					Utils.Sleep(240, "urn");
				}
				else countElse += 1;
				if (glimmer != null && glimmer.CanBeCasted() && me.Distance2D(e) <= 300 && Utils.SleepCheck("glimmer"))
				{
					glimmer.UseAbility(me);
					Utils.Sleep(200, "glimmer");
				}
				else countElse += 1;
				if (mail != null && mail.CanBeCasted() && v.Count(x => x.Distance2D(me) <= 650) >=
														 (Menu.Item("Heel").GetValue<Slider>().Value)
														 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name)
														 && Utils.SleepCheck("mail"))
				{
					mail.UseAbility();
					Utils.Sleep(100, "mail");
				}
				else countElse += 1;
				if (bkb != null && bkb.CanBeCasted() && v.Count(x => x.Distance2D(me) <= 650) >=
														 (Menu.Item("Heel").GetValue<Slider>().Value)
														 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name)
														 && Utils.SleepCheck("bkb"))
				{
					bkb.UseAbility();
					Utils.Sleep(100, "bkb");
				}
				else countElse += 1;
				if (
					countElse == 11 &&
					R != null && R.CanBeCasted() && me.Distance2D(e) <= R.CastRange + 150 && (!urn.CanBeCasted() || urn.CurrentCharges <= 0 || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name)) && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
					&& Utils.SleepCheck("R")
					)
				{
					R.UseAbility(e);
					Utils.Sleep(150, "R");
				}
				else countElse += 1;
				if (countElse == 12 && abyssal != null && !R.CanBeCasted() && abyssal.CanBeCasted() && !e.IsStunned() && !e.IsHexed()
					&& me.Distance2D(e) <= 300 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name) && Utils.SleepCheck("abyssal"))
				{
					abyssal.UseAbility(e);
					Utils.Sleep(200, "abyssal");
				}
				else countElse += 1;
				if (countElse == 13 && sheep != null && !R.CanBeCasted() && sheep.CanBeCasted() && !e.IsStunned() && !e.IsHexed()
					&& me.Distance2D(e) <= 900 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name) && Utils.SleepCheck("sheep"))
				{
					sheep.UseAbility(e);
					Utils.Sleep(200, "sheep");
				}
				if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e)<= 1900)
						{
							Orbwalking.Orbwalk(e,0,1600,true,true);
						}
			}*/
			//A();
		}
		/*
		public void A()
		{
			if (!me.IsAlive) return;
			var enemies = ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsIllusion).ToList();
			if (enemies.Count <= 0) return;
			if (Menu.Item("autoUlt").GetValue<bool>() && me.IsAlive)
			{
				R = me.Spellbook.SpellR;
				foreach (var v in enemies)
				{

					rDmg = me.AghanimState() ? new[] {0.6, 0.9, 1.2} : new[] {0.4, 0.6, 0.9};
					

					qDmg = new[] { 125, 175, 225, 275 };
					var dDamage = new [] { 400, 500, 600, 700, 800 };
					double damageR = 0;
					if (R.Level > 0 && R.CanBeCasted()&& Menu.Item("Abilities: ").GetValue<AbilityToggler>().IsEnabled(R.Name))
						damageR = (int)Math.Floor(((rDmg[R.Level - 1] / (1 + rDmg[R.Level - 1])) * v.MaximumHealth)) * (1 - v.MagicDamageResist);
					else
						damageR = 0;
					var lens = me.HasModifier("modifier_item_aether_lens");
					var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damageR =
							(int)Math.Floor(((rDmg[R.Level - 1] / (1 + rDmg[R.Level - 1])) * v.MaximumHealth)) *
									   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist);
					}
					Console.WriteLine("damage1"+damageR);
					if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
						v.Spellbook.SpellR.CanBeCasted())
						damageR = 0;
					if (lens) damageR = damageR * 1.08;
					if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageR = damageR * 0.5;
					if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageR = damageR * 1.3;
					damageR = damageR * spellamplymult;

					Console.WriteLine("damage2" + damageR);
					var damageQ = Math.Floor(qDmg[Q.Level - 1] * (1 - v.MagicDamageResist));
					
					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damageQ =
							Math.Floor(qDmg[Q.Level - 1] *
									   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
					}
					if (lens) damageQ = damageQ * 1.08;
					damageQ = damageQ * spellamplymult;
					if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageQ = damageQ * 0.5;
					if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageQ = damageQ * 1.3;

					Console.WriteLine("damage3" + damageR);
					if (vail != null && vail.CanBeCasted()
					    && !v.HasModifier("modifier_item_veil_of_discord_debuff") && R != null && R.CanBeCasted())
						damageR = damageR*1.25;
					int etherealdamage = (int)(((me.TotalIntelligence * 2) + 75));
					if (ethereal != null && ethereal.CanBeCasted() && !v.HasModifier("modifier_item_ethereal_blade_slow") && R != null && R.CanBeCasted())
						damageR = etherealdamage + (damageR * 1.4);
					if (dagon != null && dagon.CanBeCasted() && R != null && R.CanBeCasted())
						damageR = damageR + (dDamage[dagon.Level] * (1 - v.MagicDamageResist));
					if(Q!=null && Q.CanBeCasted() && me.Distance2D(v)<= Q.GetCastRange()&& R!=null && R.CanBeCasted())
						damageR= damageR + (damageQ);

					Console.WriteLine("damage4" + damageR);
					uint elsecount = 0;
					elsecount += 1;
					if ( // vail
						vail != null
						&& vail.CanBeCasted()
						&& R.CanBeCasted()
						&& v.Health <= damageR * 1.25
						&& v.Health >= damageR
						&& me.CanCast()
						&& !v.HasModifier("modifier_item_veil_of_discord_debuff")
						&& !v.IsMagicImmune()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
						&& me.Distance2D(v) <= R.GetCastRange()
						&& Utils.SleepCheck("vail")
						)
					{
						vail.UseAbility(v.Position);
						Utils.Sleep(250, "vail");
					}
					else elsecount += 1;
					if ( // vail
					   ethereal != null
					   && ethereal.CanBeCasted()
					   && (R != null
					   && R.CanBeCasted()
					   && v.Health <= etherealdamage + (damageR * 1.4)
					   && v.Health >= damageR)
					   && me.CanCast()
					   && !v.HasModifier("modifier_item_ethereal_blade_slow")
					   && !v.IsMagicImmune()
					   && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
					   && me.Distance2D(v) <= ethereal.GetCastRange() + 50
					   && Utils.SleepCheck("ethereal")
					   )
					{
						ethereal.UseAbility(v);
						Utils.Sleep(250, "ethereal");
					}
					else elsecount += 1;
					Console.WriteLine(damageR);
					if ( // vail
					 dagon != null
					 && dagon.CanBeCasted()
					 && R != null
					 && R.CanBeCasted()
					 && v.Health <= damageR
					 && me.CanCast()
					 && !v.HasModifier("modifier_item_ethereal_blade_slow")
					 && !v.IsMagicImmune()
					 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
					 && me.Distance2D(v) <= dagon.GetCastRange() + 50
					 && Utils.SleepCheck("dagon")
					 )
					{
						dagon.UseAbility(v);
						Utils.Sleep(250, "dagon");
					}
					else elsecount += 1;
					if (
						soul != null
						&& soul.CanBeCasted()
						&& Utils.SleepCheck(v.Handle.ToString())
						&& me.Mana < R.ManaCost
						&& me.Mana + 150 > R.ManaCost
						)
					{
						soul.UseAbility();
						Utils.Sleep(150, v.Handle.ToString());
					}
					if (arcane != null
						&& arcane.CanBeCasted()
						&& Utils.SleepCheck(v.Handle.ToString())
						&& me.Mana < R.ManaCost
						&& me.Mana + 135 > R.ManaCost)
					{
						arcane.UseAbility();
						Utils.Sleep(150, v.Handle.ToString());
					}
					if (arcane != null
						&& soul != null
						&& Utils.SleepCheck(v.Handle.ToString())
						&& arcane.CanBeCasted() && soul.CanBeCasted()
						&& me.Mana < R.ManaCost
						&& me.Mana + 285 > R.ManaCost)
					{
						arcane.UseAbility();
						soul.UseAbility();
						Utils.Sleep(150, v.Handle.ToString());
					}
					else elsecount += 1;
					if (elsecount==5 && R != null && v != null && R.CanBeCasted()
						&& !v.HasModifier("modifier_tusk_snowball_movement")
						&& !v.HasModifier("modifier_snowball_movement_friendly")
						&& !v.HasModifier("modifier_templar_assassin_refraction_absorb")
						&& !v.HasModifier("modifier_ember_spirit_flame_guard")
						&& !v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
						&& !v.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						&& !v.HasModifier("modifier_puck_phase_shift")
						&& !v.HasModifier("modifier_eul_cyclone")
						&& !v.HasModifier("modifier_dazzle_shallow_grave")
						&& !v.HasModifier("modifier_shadow_demon_disruption")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_storm_spirit_ball_lightning")
						&& !v.HasModifier("modifier_ember_spirit_fire_remnant")
						&& !v.HasModifier("modifier_nyx_assassin_spiked_carapace")
						&& !v.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
						&& !v.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
						!v.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
						&& !v.IsMagicImmune()
						&& v.Health < damageR
						&& Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name)
						&& Utils.SleepCheck(v.Handle.ToString()))
					{
						R.UseAbility(v);
						Utils.Sleep(150, v.Handle.ToString());
						return;
					}

					if (Q != null && Q.CanBeCasted()
						&& me.Distance2D(v) <= Q.GetCastRange()
						&& !v.HasModifier("modifier_tusk_snowball_movement")
						&& !v.HasModifier("modifier_snowball_movement_friendly")
						&& !v.HasModifier("modifier_templar_assassin_refraction_absorb")
						&& !v.HasModifier("modifier_ember_spirit_flame_guard")
						&& !v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
						&& !v.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						&& !v.HasModifier("modifier_puck_phase_shift")
						&& !v.HasModifier("modifier_eul_cyclone")
						&& !v.HasModifier("modifier_dazzle_shallow_grave")
						&& !v.HasModifier("modifier_shadow_demon_disruption")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_storm_spirit_ball_lightning")
						&& !v.HasModifier("modifier_ember_spirit_fire_remnant")
						&& !v.HasModifier("modifier_nyx_assassin_spiked_carapace")
						&& !v.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
						&& !v.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
						!v.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
						&& !v.IsMagicImmune()
						&& Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						&& v.Health < damageQ
						&& Utils.SleepCheck(v.Handle.ToString()))
					{
						Q.UseAbility();
						Utils.Sleep(150, v.Handle.ToString());
						return;
					}
					/*
					if (v.IsLinkensProtected() && (me.IsVisibleToEnemies && !me.IsInvisible() || Active))
					{
						if (Q != null && Q.CanBeCasted() && me.Distance2D(v) < Q.CastRange &&
							Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(Q.Name) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							Q.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
					}
				}
			}
		}*/
	}
}