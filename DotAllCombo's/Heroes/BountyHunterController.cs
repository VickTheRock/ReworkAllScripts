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

	internal class BountyHunterController : Variables, IHeroController
    {
        private Ability Q, R;

        private Item urn,
            ethereal,
            dagon,
            halberd,
            mjollnir,
            orchid,
            abyssal,
            mom,
            Shiva,
            mail,
            bkb,
            satanic,
            medall;

        

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			Push = Menu.Item("KeyR").GetValue<KeyBind>().Active;
			Q = me.Spellbook.SpellQ;
			R = me.Spellbook.SpellR;

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
			var enemies =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
			
			if (Active)
            {
                e = Toolset.ClosestToMouse(me);
                if (e == null)
					return;
				var track = e.HasModifier("modifier_bounty_hunter_track");
				if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
				{
					Orbwalking.Orbwalk(e, 0, 1600, true, true);
				}
				if (me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !me.IsInvisible())
				{
					if (
						Q != null && Q.CanBeCasted() && me.Distance2D(e) <= 1500
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						Q.UseAbility(e);
						Utils.Sleep(200, "Q");
					}
					if (
						R != null && R.CanBeCasted() && me.Distance2D(e) <= 1500
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
						&& !track
						&& !me.IsChanneling()
						&& Utils.SleepCheck("R")
						)
					{
						R.UseAbility(e);
						Utils.Sleep(200, "R");
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
					if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) <= 900 &&
						Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) && Utils.SleepCheck("orchid"))
					{
						orchid.UseAbility(e);
						Utils.Sleep(100, "orchid");
					}

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
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
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
					if (mail != null && mail.CanBeCasted() && (enemies.Count(x => x.Distance2D(me) <= 650) >=
															   (Menu.Item("Heelm").GetValue<Slider>().Value)) &&
						Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) && Utils.SleepCheck("mail"))
					{
						mail.UseAbility();
						Utils.Sleep(100, "mail");
					}
					if (bkb != null && bkb.CanBeCasted() && (enemies.Count(x => x.Distance2D(me) <= 650) >=
															 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
						Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Utils.SleepCheck("bkb"))
					{
						bkb.UseAbility();
						Utils.Sleep(100, "bkb");
					}
				}
			}

			if (Push && me.IsAlive && R != null && R.CanBeCasted())
				if (!me.HasModifier("modifier_bounty_hunter_wind_walk") || me.IsVisibleToEnemies)
					foreach (var v in enemies)
					{
						var CheckMod = v.Modifiers.Where(y => y.Name == "modifier_bounty_hunter_track").DefaultIfEmpty(null).FirstOrDefault();
						var invItem = v.FindItem("item_glimmer_cape") ?? v.FindItem("item_invis_sword") ?? v.FindItem("item_silver_edge") ?? v.FindItem("item_glimmer_cape");
						if (
							((v.ClassID == ClassID.CDOTA_Unit_Hero_Riki || v.ClassID == ClassID.CDOTA_Unit_Hero_Broodmother
							|| v.ClassID == ClassID.CDOTA_Unit_Hero_Clinkz || v.ClassID == ClassID.CDOTA_Unit_Hero_Invoker
							|| v.ClassID == ClassID.CDOTA_Unit_Hero_SandKing || v.ClassID == ClassID.CDOTA_Unit_Hero_TemplarAssassin
							|| v.ClassID == ClassID.CDOTA_Unit_Hero_Treant || v.ClassID == ClassID.CDOTA_Unit_Hero_PhantomLancer
							)
							|| (
								v.Modifiers.Any(x =>
								(x.Name == "modifier_riki_permanent_invisibility"
								|| x.Name == "modifier_mirana_moonlight_shadow"
								|| x.Name == "modifier_treant_natures_guise"
								|| x.Name == "modifier_weaver_shukuchi"
								|| x.Name == "modifier_broodmother_spin_web_invisible_applier"
								|| x.Name == "modifier_item_invisibility_edge_windwalk"
								|| x.Name == "modifier_rune_invis"
								|| x.Name == "modifier_clinkz_wind_walk"
								|| x.Name == "modifier_item_shadow_amulet_fade"
								|| x.Name == "modifier_item_silver_edge_windwalk"
								|| x.Name == "modifier_item_edge_windwalk"
								|| x.Name == "modifier_nyx_assassin_vendetta"
								|| x.Name == "modifier_invisible"
								|| x.Name == "modifier_invoker_ghost_walk_enemy")))
							|| (invItem != null && invItem.Cooldown <= 0)
							|| v.Health <= (v.MaximumHealth * 0.5))
							&& me.Distance2D(v) <= R.GetCastRange() + me.HullRadius
							&& (!v.HasModifier("modifier_bounty_hunter_track") || CheckMod != null && CheckMod.RemainingTime <= 2)
							&& Utils.SleepCheck("R"))
						{
							R.UseAbility(v);
							Utils.Sleep(300, "R");
						}
					}
	}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Need to Creet!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("KeyR", "Use Auto Track").SetValue(new KeyBind('R', KeyBindType.Toggle)));

			Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"bounty_hunter_track", true},
				    {"bounty_hunter_shuriken_toss", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_mjollnir", true},
				    {"item_orchid", true}, {"item_bloodthorn", true},
				    {"item_urn_of_shadows", true},
				    {"item_ethereal_blade", true},
				    {"item_veil_of_discord", true},
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
		}

		public void OnCloseEvent()
		{
			
		}
	}
}