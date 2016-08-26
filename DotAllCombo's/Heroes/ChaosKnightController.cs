using SharpDX;

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

	internal class ChaosKnightController : Variables, IHeroController
	{
		private Ability Q, W, R;

		private Item urn, dagon, halberd, mjollnir, abyssal, mom, Shiva, mail, bkb, satanic, blink, armlet, medall, manta;

        

		public void Combo()
		{
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			Q = me.Spellbook.SpellQ;
			W = me.Spellbook.SpellW;
			R = me.Spellbook.SpellR;


			mom = me.FindItem("item_mask_of_madness");
			urn = me.FindItem("item_urn_of_shadows");
			dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			halberd = me.FindItem("item_heavens_halberd");
			mjollnir = me.FindItem("item_mjollnir");
			armlet = me.FindItem("item_armlet");
			abyssal = me.FindItem("item_abyssal_blade");
			mail = me.FindItem("item_blade_mail");
			bkb = me.FindItem("item_black_king_bar");
			blink = me.FindItem("item_blink");
			satanic = me.FindItem("item_satanic");
			medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
			Shiva = me.FindItem("item_shivas_guard");
			manta = me.FindItem("item_manta");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
            e = Toolset.ClosestToMouse(me);
            if (e == null)
				return;
			var a = ObjectManager.GetEntities<Hero>()
						.Where(x => x.Team == me.Team && x.IsAlive && x.IsVisible && x.IsIllusion && x.IsControllable).ToList();
			var illy = a.Count;
			if (illy >= 1)
			{
				for (int i = 0; i < illy; ++i)
				{
					if (a[i].Distance2D(e) <= a[i].GetAttackRange() + a[i].HullRadius && !e.IsAttackImmune()
						&& !a[i].IsAttacking() && a[i].CanAttack() && Utils.SleepCheck(a[i].Handle + "Attack")
						)
					{
						a[i].Attack(e);
						Utils.Sleep(330, a[i].Handle + "Attack");
					}
					else if (a[i].Distance2D(e) <= 1000)
					{
						if ((!a[i].CanAttack() || a[i].Distance2D(e) >= 0)
						   && !a[i].IsAttacking()
						   && a[i].CanMove() && Utils.SleepCheck(a[i].Handle + "Move")
						   )
						{
							a[i].Move(Prediction.InFront(e, 100));
							Utils.Sleep(400, a[i].Handle + "Move");
						}
					}
				}
			}
			if (Active)
			{
				if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
				{
					Orbwalking.Orbwalk(e, 0, 1600, true, true);
				}
			}
			if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !Toolset.invUnit(me))
			{
				float angle = me.FindAngleBetween(e.Position, true);
				Vector3 pos = new Vector3((float) (e.Position.X - 70*Math.Cos(angle)), (float) (e.Position.Y - 70*Math.Sin(angle)),
					0);
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
				if (
					Q != null
					&& Q.CanBeCasted()
					&& me.Distance2D(e) <= 900
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					&& Utils.SleepCheck("Q")
					)
				{
					Q.UseAbility(e);
					Utils.Sleep(200, "Q");
				}
				if (armlet != null
				    && !armlet.IsToggled
				    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(armlet.Name) &&
				    Utils.SleepCheck("armlet"))
				{
					armlet.ToggleAbility();
					Utils.Sleep(300, "armlet");
				}
				if ((manta != null
				     && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name))
				    && manta.CanBeCasted() && me.IsSilenced() && Utils.SleepCheck("manta"))
				{
					manta.UseAbility();
					Utils.Sleep(400, "manta");
				}
				if ((manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name))
				    && manta.CanBeCasted() && (e.Position.Distance2D(me.Position) <= me.GetAttackRange() + me.HullRadius)
				    && Utils.SleepCheck("manta"))
				{
					manta.UseAbility();
					Utils.Sleep(150, "manta");
				}
				if (
					R != null
					&& R.CanBeCasted()
					&& me.Distance2D(e) <= 300
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
					&& (armlet == null || armlet.IsToggled)
					&& Utils.SleepCheck("R")
					)
				{
					R.UseAbility();
					Utils.Sleep(200, "R");
				}
				if (
					W != null
					&& W.CanBeCasted()
					&& (me.Distance2D(e) <= W.GetCastRange() + 300
					    && (me.Distance2D(e) >= me.GetAttackRange()
					        || me.Distance2D(e) <= me.GetAttackRange()
					        && e.NetworkActivity == NetworkActivity.Attack)
						)
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
					&& Utils.SleepCheck("W")
					)
				{
					W.UseAbility(e);
					Utils.Sleep(200, "W");
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

				if (Shiva != null
				    && Shiva.CanBeCasted()
				    && me.Distance2D(e) <= 600
				    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
				    && !e.IsMagicImmune()
				    && Utils.SleepCheck("Shiva"))
				{
					Shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
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
				if (urn != null
				    && urn.CanBeCasted()
				    && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
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
					me.Health <= (me.MaximumHealth*0.3) &&
					satanic.CanBeCasted() &&
					me.Distance2D(e) <= me.GetAttackRange() + me.HullRadius
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

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Rogue Knight at your service!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"chaos_knight_chaos_bolt", true},
				    {"chaos_knight_reality_rift", true},
				    {"chaos_knight_phantasm", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_blink", true},
				    {"item_armlet", true},
				    {"item_mjollnir", true},
				    {"item_urn_of_shadows", true},
				    {"item_abyssal_blade", true},
				    {"item_shivas_guard", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true},
				    {"item_satanic", true},
				    {"item_medallion_of_courage", true},
				    {"item_solar_crest", true},
				    {"item_manta", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}

		public void OnCloseEvent()
		{
			
		}
	}
}