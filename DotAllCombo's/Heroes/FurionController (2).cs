namespace DotaAllCombo.Heroes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;

	using Service;
	using Service.Debug;


	internal class FurionController : Variables, IHeroController
	{
		private Ability Q, W, R;

		private Item urn, orchid,
			ethereal,
			dagon,
			halberd,
			mjollnir,
			abyssal,
			mom,
			Shiva,
			mail,
			bkb,
			satanic,
            medall, arcane, soul, vail;

		private int  [] rDmg;
		public void Combo()
		{
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			Q = me.Spellbook.SpellQ;
			R = me.Spellbook.SpellR;



			soul = me.FindItem("item_soul_ring");
			vail = me.FindItem("item_veil_of_discord");
			arcane = me.FindItem("item_arcane_boots");

			mom = me.FindItem("item_mask_of_madness");
			urn = me.FindItem("item_urn_of_shadows");
			dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			ethereal = me.FindItem("item_ethereal_blade");
			halberd = me.FindItem("item_heavens_halberd");
			mjollnir = me.FindItem("item_mjollnir");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
			abyssal = me.FindItem("item_abyssal_blade");
			mail = me.FindItem("item_blade_mail");
			bkb = me.FindItem("item_black_king_bar");
			satanic = me.FindItem("item_satanic");
			medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
			Shiva = me.FindItem("item_shivas_guard");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
			var modifInv = Toolset.invUnit(me);
			e = me.ClosestToMouseTarget(1800);
			if (e == null) return;
			if (Active)
			{
				if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
				{
					Orbwalking.Orbwalk(e, 0, 1600, true, true);
				}
			}
			if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !modifInv)
			{
				if (
					Q != null && Q.CanBeCasted() && me.Distance2D(e) <= Q.CastRange
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					&& Utils.SleepCheck("Q")
					)
				{
					Q.UseAbility(e);
					Utils.Sleep(150, "Q");
				}
				
				if ( // MOM
				mom != null
				&& mom.CanBeCasted()
				&& me.CanCast()
				&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mom.Name)
				&& Utils.SleepCheck("mom")
				&& me.Distance2D(e) <= 700
				)
				{
					mom.UseAbility();
					Utils.Sleep(250, "mom");
				}
				if ( // Mjollnir
					mjollnir != null
					&& mjollnir.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
					&& Utils.SleepCheck("mjollnir")
					&& me.Distance2D(e) <= 700
					)
				{
					mjollnir.UseAbility(me);
					Utils.Sleep(250, "mjollnir");
				} // Mjollnir Item end
				if ( // Medall
					medall != null
					&& medall.CanBeCasted()
					&& Utils.SleepCheck("Medall")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
					&& me.Distance2D(e) <= 700
					)
				{
					medall.UseAbility(e);
					Utils.Sleep(250, "Medall");
				} // Medall Item end
				if ( // orchid
					orchid != null
					&& orchid.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& me.Distance2D(e) <= me.GetAttackRange() + me.HullRadius
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
					&& Utils.SleepCheck("orchid")
					)
				{
					orchid.UseAbility(e);
					Utils.Sleep(250, "orchid");
				} // orchid Item end

				if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(e) <= 600
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
					&& !e.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					Shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}

				if (ethereal != null && ethereal.CanBeCasted()
					&& me.Distance2D(e) <= 700 && me.Distance2D(e) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name) &&
					Utils.SleepCheck("ethereal"))
				{
					ethereal.UseAbility(e);
					Utils.Sleep(100, "ethereal");
				}

				if (dagon != null
					&& dagon.CanBeCasted()
					&& me.Distance2D(e) <= 500
					&& Utils.SleepCheck("dagon"))
				{
					dagon.UseAbility(e);
					Utils.Sleep(100, "dagon");
				}
				if ( // Abyssal Blade
					abyssal != null
					&& abyssal.CanBeCasted()
					&& me.CanCast()
					&& !e.IsStunned()
					&& !e.IsHexed()
					&& Utils.SleepCheck("abyssal")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
					&& me.Distance2D(e) <= 400
					)
				{
					abyssal.UseAbility(e);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end
				if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
				{
					urn.UseAbility(e);
					Utils.Sleep(240, "urn");
				}
				if ( // Hellbard
					halberd != null
					&& halberd.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& (e.NetworkActivity == NetworkActivity.Attack
						|| e.NetworkActivity == NetworkActivity.Crit
						|| e.NetworkActivity == NetworkActivity.Attack2)
					&& Utils.SleepCheck("halberd")
					&& me.Distance2D(e) <= 700
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
					)
				{
					halberd.UseAbility(e);
					Utils.Sleep(250, "halberd");
				}
				if ( // Satanic 
					satanic != null &&
					me.Health <= (me.MaximumHealth * 0.3) &&
					satanic.CanBeCasted() &&
					me.Distance2D(e) <= me.AttackRange + 50
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
					&& Utils.SleepCheck("satanic")
					)
				{
					satanic.UseAbility();
					Utils.Sleep(240, "satanic");
				} // Satanic Item end
				if (mail != null && mail.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
														   (Menu.Item("Heelm").GetValue<Slider>().Value)) &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) && Utils.SleepCheck("mail"))
				{
					mail.UseAbility();
					Utils.Sleep(100, "mail");
				}
				if (bkb != null && bkb.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
														 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Utils.SleepCheck("bkb"))
				{
					bkb.UseAbility();
					Utils.Sleep(100, "bkb");
				}
			}
		}
		public void A()
		{
			if (!me.IsAlive) return;
			var enemies =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsIllusion).ToList();
			if (enemies.Count <= 0) return;
			if (Menu.Item("autoUlt").GetValue<bool>() && me.IsAlive)
			{
				foreach (var v in enemies)
				{
					if (v == null) return;
					if (me.HasItem(ClassID.CDOTA_Item_UltimateScepter))
						rDmg = new[] { 440, 540, 640 };
					else
						rDmg = new[] { 225, 325, 425 };
					
					var lens = me.HasModifier("modifier_item_aether_lens");
					var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
					var damageR = Math.Floor(rDmg[R.Level - 1] * (1 - v.MagicDamageResist));
					
					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damageR =
							Math.Floor(rDmg[R.Level - 1] *
									   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
					}
					if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
						v.Spellbook.SpellR.CanBeCasted())
						damageR = 0;
					if (lens) damageR = damageR * 1.08;
					if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageR = damageR * 0.5;
					if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageR = damageR * 1.3;
					damageR = damageR * spellamplymult;

					if ( // vail
						vail != null
						&& vail.CanBeCasted()
						&& me.Distance2D(v)<= vail.CastRange
						&& R.CanBeCasted()
						&& v.Health <= damageR * 1.25
						&& v.Health >= damageR
						&& me.CanCast()
						&& !v.HasModifier("modifier_item_veil_of_discord_debuff")
						&& !v.IsMagicImmune()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
						&& Utils.SleepCheck("vail")
						)
					{
						vail.UseAbility(v.Position);
						Utils.Sleep(250, "vail");
					}
					int etherealdamage = (int)(((me.TotalIntelligence * 2) + 75));
					if ( // vail
					  ethereal != null
					  && ethereal.CanBeCasted()
					  && me.Distance2D(v)<= ethereal.CastRange
					  && R != null
					  && R.CanBeCasted()
					  && v.Health <= etherealdamage + (damageR * 1.4)
					  && v.Health >= damageR
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
					if (R != null && v != null && R.CanBeCasted()
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
						&& Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name)
						&& enemies.Count(x => x.Health <= (damageR - 20)) >= (Menu.Item("Heel").GetValue<Slider>().Value)
						&& Utils.SleepCheck(v.Handle.ToString()))
					{
						R.UseAbility();
						Utils.Sleep(150, v.Handle.ToString());
					}
				}
			}
		}
		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Go Rampage!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"furion_force_of_nature", true},
				    {"furion_sprout", true},
				    {"furion_wrath_of_nature", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
                    {"item_bloodthorn", true},
				    {"item_mjollnir", true},
				    {"item_urn_of_shadows", true},
				    {"item_ethereal_blade", true},
				    {"item_abyssal_blade", true},
				    {"item_shivas_guard", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true},
				    {"item_satanic", true},
				    {"item_medallion_of_courage", true},
				    {"item_solar_crest", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));

			Menu.AddItem(new MenuItem("autoUlt", "Auto Steal Ult").SetValue(true));
		}
		
		public void OnCloseEvent()
		{
			
		}
	}
}