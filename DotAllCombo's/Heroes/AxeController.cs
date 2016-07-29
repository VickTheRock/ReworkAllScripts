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

	internal class AxeController : Variables, IHeroController
	{
		private Ability Q, W, R;

		private Item urn, dagon, mjollnir, abyssal, mom, armlet, Shiva, mail, bkb, satanic, medall, blink;
		private Double rDmg;
		public void Combo()
		{
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
			if (!Menu.Item("enabled").IsActive())
				return;
			Q = me.Spellbook.SpellQ;
			W = me.Spellbook.SpellW;
			R = me.Spellbook.SpellR;
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
					.ToList();
			mom = me.FindItem("item_mask_of_madness");
			urn = me.FindItem("item_urn_of_shadows");
			dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
			mjollnir = me.FindItem("item_mjollnir");
			abyssal = me.FindItem("item_abyssal_blade");
			mail = me.FindItem("item_blade_mail");
			armlet = me.FindItem("item_armlet");
			bkb = me.FindItem("item_black_king_bar");
			satanic = me.FindItem("item_satanic");
			blink = me.FindItem("item_blink");
			medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
			Shiva = me.FindItem("item_shivas_guard");
			if (Menu.Item("kill").IsActive())
			{
				if (R == null || !R.CanBeCasted() || !me.IsAlive) return;
				if(v.Count<=0)return;

				if (W != null && W.CanBeCasted() && Menu.Item("HUNGER").IsActive())
				{
					for (int i = 0; i < v.Count(); ++i)
					{
						if (!v[i].HasModifier("modifier_axe_battle_hunger")
							&& me.Distance2D(v[i]) <= W.GetCastRange()+me.HullRadius
							&& me.Mana >= R.ManaCost+180
							&& Utils.SleepCheck(me.Handle.ToString()))
						{
							W.UseAbility(v[i]);
							Utils.Sleep(400, me.Handle.ToString());
						}
					}
				}
				foreach (Hero vic in v)
				{
					if (vic.Modifiers.Any(x =>
					(x.Name == "modifier_obsidian_destroyer_astral_imprisonment_prison"
					|| x.Name == "modifier_puck_phase_shift"
					|| x.Name == "modifier_eul_cyclone"
					|| x.Name == "modifier_invoker_tornado"
					|| x.Name == "modifier_brewmaster_storm_cyclone"
					|| x.Name == "modifier_shadow_demon_disruption"
					|| x.Name == "modifier_tusk_snowball_movement"
					|| x.Name == "modifier_abaddon_borrowed_time"
					|| x.Name == "modifier_faceless_void_time_walk"
					|| x.Name == "modifier_huskar_life_break_charge"))) return;
					rDmg = R.GetAbilityData("damage") * (1 + (me.TotalIntelligence / 16 / 100));
					if (vic.HasModifier("modifier_item_veil_of_discord_debuff"))
						rDmg = rDmg * 1.25;
					if (vic.NetworkName == "CDOTA_Unit_Hero_Spectre" && vic.Spellbook.Spell3.Level > 0)
					{
						rDmg =
							R.GetAbilityData("damage") * (1 + (me.TotalIntelligence / 16 / 100)) *
									   (1 - (0.10 + vic.Spellbook.Spell3.Level * 0.04));
					}
					if (me.HasModifier("modifier_item_aether_lens")) rDmg = rDmg * 1.08;
					if (vic.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) rDmg = rDmg * 0.5;
					if (vic.HasModifier("modifier_item_mask_of_madness_berserk")) rDmg = rDmg * 1.3;
					
					if (vic.Health <= rDmg && vic.Distance2D(me) <= R.GetCastRange() + me.HullRadius && Utils.SleepCheck(vic.Handle.ToString()))
					{
						R.UseAbility(vic);
						Utils.Sleep(150, vic.Handle.ToString());
					}
					if (blink != null && blink.CanBeCasted() && R != null && R.CanBeCasted() && Menu.Item("blink").IsActive())
					{
						if (me.Distance2D(vic) > R.GetCastRange() + me.HullRadius + 24 && vic.Health < rDmg && Utils.SleepCheck(vic.Handle.ToString()))
						{
							blink.UseAbility(vic.Position);
							Utils.Sleep(150, vic.Handle.ToString());
						}
					}
					if (R.IsInAbilityPhase && v.Where(x => me.Distance2D(x) <= R.GetCastRange() + me.HullRadius + 24).OrderBy(z => z.Health).First().Health > rDmg && vic.Distance2D(me) <= R.GetCastRange() + me.HullRadius + 24 && Utils.SleepCheck(vic.Handle.ToString()))
					{
						me.Stop();
						Utils.Sleep(50, vic.Handle.ToString());
					}
				}
			}
			e = me.ClosestToMouseTarget(1800);
			if (e == null) return;
			var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");

			if(stoneModif)return;
			if (Active && me.Distance2D(e) <= 2000 && e.IsAlive && !me.IsInvisible())
			{
				if (
					blink != null
					&& me.CanCast()
					&& blink.CanBeCasted()
					&& me.Distance2D(e) < 1180
					&& me.Distance2D(e) > 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					blink.UseAbility(e.Position);
					Utils.Sleep(250, "blink");
				}
				if (
					Q != null && Q.CanBeCasted()
					&& me.Distance2D(e) <= Q.GetCastRange() + me.HullRadius
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					&& Utils.SleepCheck("Q")
					)
				{
					Q.UseAbility();
					Utils.Sleep(200, "Q");
				}
				if ( // MOM
					mom != null
					&& mom.CanBeCasted()
					&& me.CanCast()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
					&& Utils.SleepCheck("mom")
					&& me.Distance2D(e) <= 700
					)
				{
					mom.UseAbility();
					Utils.Sleep(250, "mom");
				}
				if (
					W != null && W.CanBeCasted() && me.Distance2D(e) <= W.GetCastRange()+me.HullRadius
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
					&& !e.HasModifier("modifier_axe_battle_hunger")
					&& Utils.SleepCheck("W")
					)
				{
					W.UseAbility(e);
					Utils.Sleep(200, "W");
				}
				if ( // Mjollnir
					mjollnir != null
					&& mjollnir.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
					&& Utils.SleepCheck("mjollnir")
					&& me.Distance2D(e) <= 900
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
				if ( // Abyssal Blade
					abyssal != null
					&& abyssal.CanBeCasted()
					&& me.CanCast()
					&& !e.IsStunned()
					&& !e.IsHexed()
					&& Utils.SleepCheck("abyssal")
					&& me.Distance2D(e) <= 400
					)
				{
					abyssal.UseAbility(e);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end
				if (armlet != null && !armlet.IsToggled &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(armlet.Name) &&
					Utils.SleepCheck("armlet"))
				{
					armlet.ToggleAbility();
					Utils.Sleep(300, "armlet");
				}

				if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(e) <= 600
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
					&& !e.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					Shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}

				if ( // Dagon
					me.CanCast()
					&& dagon != null
					&& !e.IsLinkensProtected()
					&& dagon.CanBeCasted()
					&& !e.IsMagicImmune()
					&& !stoneModif
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
					&& Utils.SleepCheck("dagon")
					)
				{
					dagon.UseAbility(e);
					Utils.Sleep(200, "dagon");
				} // Dagon Item end


				if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
				{
					urn.UseAbility(e);
					Utils.Sleep(240, "urn");
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
				if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
				{
					Orbwalking.Orbwalk(e, 0, 1600, true, true);
				}
				if (Q != null && Q.IsInAbilityPhase && v.Count(x => x.Distance2D(me) <= Q.GetCastRange()+me.HullRadius) == 0 && Utils.SleepCheck("Phase"))
				{
					me.Stop();
					Utils.Sleep(100, "Phase");
				}
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("I came a long way to see you die.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

			Menu.AddItem(
				new MenuItem("Skills", "Skills:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"axe_berserkers_call", true},
					{"axe_battle_hunger", true},
					{"axe_culling_blade", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_dagon", true},
					{"item_blink", true},
					{"item_armlet", true},
					{"item_heavens_halberd", true},
					{"item_urn_of_shadows", true},
					{"item_veil_of_discord", true},
					{"item_abyssal_blade", true},
					{"item_shivas_guard", true},
					{"item_blade_mail", true},
					{"item_black_king_bar", true},
					{"item_satanic", true},
					{"item_medallion_of_courage", true},
					{"item_solar_crest", true}
				})));

			Menu.AddItem(new MenuItem("kill", "AutoUsage").SetValue(true));
			Menu.AddItem(new MenuItem("HUNGER", "Use Auto spells: BATTLE HUNGER(W)").SetValue(true));

			Menu.AddItem(new MenuItem("blink", "Use Auto blink and CULLING BLADE(R)").SetValue(true));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}

		public void OnCloseEvent()
		{
			
		}
	}
}
