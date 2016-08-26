namespace DotaAllCombo.Heroes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
	using SharpDX;

	using Service;
	using Service.Debug;

	internal class SandKingController : Variables, IHeroController
	{

		private Ability Q, R;

		private Item urn,
			dagon,
			halberd,
			mjollnir,
			orchid,
			abyssal,
			mom,
			Shiva,
			mail,
			bkb,
			ethereal,
			glimmer,
			vail,
			satanic,
			medall,
			blink;

		

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;

			Q = me.Spellbook.SpellQ;
			R = me.Spellbook.SpellR;
			Shiva = me.FindItem("item_shivas_guard");
			mom = me.FindItem("item_mask_of_madness");
			urn = me.FindItem("item_urn_of_shadows");
			dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
			halberd = me.FindItem("item_heavens_halberd");
			mjollnir = me.FindItem("item_mjollnir");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
			abyssal = me.FindItem("item_abyssal_blade");
			mail = me.FindItem("item_blade_mail");
			bkb = me.FindItem("item_black_king_bar");
			ethereal = me.FindItem("item_ethereal_blade");
			glimmer = me.FindItem("item_glimmer_cape");
			vail = me.FindItem("item_veil_of_discord");
			satanic = me.FindItem("item_satanic");
			blink = me.FindItem("item_blink");
			medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
            e = Toolset.ClosestToMouse(me);
            if (e == null) return;

			var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");


			if (me.IsChanneling() || R.IsInAbilityPhase || R.IsChanneling) return;
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
			var modifInv = me.IsInvisible();
			if (Active && Utils.SleepCheck("Combo"))
			{
				if (me.HasModifier("modifier_sandking_sand_storm")) return;
				float angle = me.FindAngleBetween(e.Position, true);

				Vector3 pos = new Vector3((float)(e.Position.X - (Q.GetCastRange() - 100) * Math.Cos(angle)),
					(float)(e.Position.Y - (Q.GetCastRange() - 100) * Math.Sin(angle)), 0);
				uint elsecount = 1;
				if (elsecount == 1 && (blink != null && blink.CanBeCasted() && me.Distance2D(pos) <= 1100 || blink == null && me.Distance2D(e) <= Q.GetCastRange() - 50))
				{
					if (
						R != null && R.CanBeCasted()
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
						&& Utils.SleepCheck("R")
						)
					{
						R.UseAbility();
						Utils.Sleep(200, "R");
						Utils.Sleep(300, "Combo");
					}
				}

				if (!Utils.SleepCheck("Combo") || me.IsChanneling() || R.IsChanneling || R.IsInAbilityPhase) return;

				if (!Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name) || !R.CanBeCasted())
                {
                    if (
                        blink != null
                        && blink.CanBeCasted()
                        && me.Distance2D(e) >= (Q.CanBeCasted() ? Q.GetCastRange() : 450)
                        && me.Distance2D(pos) <= 1190
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                        && Utils.SleepCheck("blink")
                        )
                    {
                        blink.UseAbility(pos);
                        Utils.Sleep(250, "blink");
                    }
                    if (
						blink != null
						&& blink.CanBeCasted()
						&& me.Distance2D(e) < 1180
						&& me.Distance2D(e) > (Q.CanBeCasted() ? Q.GetCastRange() : 450)
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						blink.UseAbility(e.Position);
						Utils.Sleep(250, "blink");
					}

					if (
					Q != null && Q.CanBeCasted() && me.Distance2D(e) <= Q.GetCastRange() + 300
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					&& Utils.SleepCheck("Q")
					)
					{
						Q.UseAbility(e);
						Utils.Sleep(200, "Q");
					}
				}
				if (me.Distance2D(e) <= 2000 && e != null && e.IsAlive && !modifInv && !me.IsChanneling() && (!R.CanBeCasted() || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)))
				{
					if (ethereal != null && ethereal.CanBeCasted() && me.Distance2D(e) <= 700 &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name) &&
					Utils.SleepCheck("ethereal"))
					{
						ethereal.UseAbility(e);
						Utils.Sleep(100, "ethereal");
					}

					if (vail != null && vail.CanBeCasted() && me.Distance2D(e) <= 1100 &&
					   Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name) && Utils.SleepCheck("vail"))
					{
						vail.UseAbility(e.Position);
						Utils.Sleep(130, "vail");
					}


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
					if (glimmer != null
						&& glimmer.CanBeCasted()
						&& me.Distance2D(e) <= 300
						&& Utils.SleepCheck("glimmer"))
					{
						glimmer.UseAbility(me);
						Utils.Sleep(200, "glimmer");
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

					if ( // Dagon
						me.CanCast()
						&& dagon != null
						&& !e.IsLinkensProtected()
						&& dagon.CanBeCasted()
						&& !e.IsMagicImmune()
						&& !stoneModif
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
					if (Menu.Item("logic").IsActive())
					{
						if (mail != null && mail.CanBeCasted() && Toolset.HasStun(e) && !e.IsStunned() &&
							Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) && Utils.SleepCheck("mail"))
						{
							mail.UseAbility();
							Utils.Sleep(100, "mail");
						}
						if (bkb != null && bkb.CanBeCasted() && Toolset.HasStun(e) && !e.IsStunned() &&
							Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Utils.SleepCheck("bkb"))
						{
							bkb.UseAbility();
							Utils.Sleep(100, "bkb");
						}
					}
				}
				if (me.IsChanneling() || R.IsChanneling || R.IsInAbilityPhase) return;
				elsecount++;
				if (elsecount == 2 && e != null && e.IsAlive)
				{

					if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900 && !me.IsChanneling())
					{
						Orbwalking.Orbwalk(e, 0, 1600, true, true);
					}
				}
				Utils.Sleep(200, "Combo");
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

			Print.LogMessage.Success("I am king now of all I survey.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"sandking_burrowstrike", true},
				    //{"sandking_sand_storm", true},
				    {"sandking_epicenter", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_blink", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true}, {"item_bloodthorn", true},
				    {"item_urn_of_shadows", true},
				    {"item_veil_of_discord", true},
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
			Menu.AddItem(new MenuItem("logic", "UseBKB if e have stun").SetValue(true));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}

		public void OnCloseEvent()
		{
			
		}
	}
}