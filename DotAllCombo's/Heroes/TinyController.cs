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

	internal class TinyController : Variables, IHeroController
	{
		private Ability Q, W;

		private Item urn, ethereal, dagon, halberd, vail, mjollnir, orchid, abyssal, mom, Shiva, mail, bkb, satanic, medall, blink;

        private bool Active;

		public void Combo()
		{
			if (!menu.Item("enabled").IsActive())
				return;

			e = me.ClosestToMouseTarget(1800);
			if (e == null)
				return;
			Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);

			Q = me.Spellbook.SpellQ;
			W = me.Spellbook.SpellW;

			blink = me.FindItem("item_blink");
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
			vail = me.FindItem("item_veil_of_discord");
            
			var ModifEther = e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
			var stoneModif = e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
			var ModifInv =
				me.IsInvisible();
			if (Active)
            {
                if (
                    me.Distance2D(e) <= Toolset.AttackRange+100 && (!me.IsAttackImmune() || !e.IsAttackImmune())
                    && me.NetworkActivity != NetworkActivity.Attack && me.CanAttack() && Utils.SleepCheck("attack")
                    )
                {
                    me.Attack(e);
                    Utils.Sleep(190, "attack");
                }
                else
				if (
					((!me.CanAttack() && me.Distance2D(e) >= 0) || me.Distance2D(e) >= 300) && me.NetworkActivity != NetworkActivity.Attack &&
					me.Distance2D(e) <= 1500 && Utils.SleepCheck("Move")
					)
				{
					me.Move(e.Predict(350));
					Utils.Sleep(350, "Move");
				}
			}
			if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !ModifInv)
			{
                float angle = me.FindAngleBetween(e.Position, true);
                Vector3 pos = new Vector3((float)(e.Position.X - 100 * Math.Cos(angle)), (float)(e.Position.Y - 100 * Math.Sin(angle)), 0);
                if (
                    blink != null
                    && Q.CanBeCasted()
                    && me.CanCast()
                    && blink.CanBeCasted()
                    && me.Distance2D(e) >= Toolset.AttackRange+150
                    && me.Distance2D(pos) <= 1180
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                    && Utils.SleepCheck("blink")
                    )
                {
                    blink.UseAbility(pos);
                    Utils.Sleep(250, "blink");
                }
                if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) <= 900 &&
					   menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) && Utils.SleepCheck("orchid"))
				{
					orchid.UseAbility(e);
					Utils.Sleep(100, "orchid");
				}
				if ( // vail
				vail != null
				&& vail.CanBeCasted()
				&& me.CanCast()
				&& !e.IsMagicImmune()
				&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
				&& me.Distance2D(e) <= 1500
				&& Utils.SleepCheck("vail")
				)
				{
					vail.UseAbility(e.Position);
					Utils.Sleep(250, "vail");
				} // orchid Item end
				if (ethereal != null && ethereal.CanBeCasted()
					   && me.Distance2D(e) <= 700 && me.Distance2D(e) <= 400
					   && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name) &&
					   Utils.SleepCheck("ethereal"))
				{
					ethereal.UseAbility(e);
					Utils.Sleep(100, "ethereal");
				}

				if ( // Dagon
					me.CanCast()
					&& dagon != null
					&& (ethereal == null
						|| (ModifEther
							|| ethereal.Cooldown < 17))
					&& !e.IsLinkensProtected()
					&& dagon.CanBeCasted()
					&& me.Distance2D(e) <= 1400
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                    && !e.IsMagicImmune()
					&& !stoneModif
					&& Utils.SleepCheck("dagon")
					)
				{
					dagon.UseAbility(e);
					Utils.Sleep(200, "dagon");
				} // Dagon Item end
				if (
					Q != null && Q.CanBeCasted() && me.Distance2D(e) <= 1500
					&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					&& Utils.SleepCheck("Q")
					)
				{
					Q.UseAbility(e.Predict(400));
					Utils.Sleep(200, "Q");
				}
				if (
					W != null && W.CanBeCasted() && me.Distance2D(e) <= 1500
					&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
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
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mom.Name)
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
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
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
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
					&& me.Distance2D(e) <= 700
					)
				{
					medall.UseAbility(e);
					Utils.Sleep(250, "Medall");
				} // Medall Item end


				if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(e) <= 600
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
					&& !e.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					Shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}
				if ( // Abyssal Blade
					abyssal != null
					&& abyssal.CanBeCasted()
					&& me.CanCast()
					&& !e.IsStunned()
					&& !e.IsHexed()
					&& Utils.SleepCheck("abyssal")
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
					&& me.Distance2D(e) <= 400
					)
				{
					abyssal.UseAbility(e);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end
				if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
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
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
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
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
					&& Utils.SleepCheck("satanic")
					)
				{
					satanic.UseAbility();
					Utils.Sleep(240, "satanic");
				} // Satanic Item end
				if (mail != null && mail.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
														   (menu.Item("Heelm").GetValue<Slider>().Value)) &&
					menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) && Utils.SleepCheck("mail"))
				{
					mail.UseAbility();
					Utils.Sleep(100, "mail");
				}
				if (bkb != null && bkb.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
														 (menu.Item("Heel").GetValue<Slider>().Value)) &&
					menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Utils.SleepCheck("bkb"))
				{
					bkb.UseAbility();
					Utils.Sleep(100, "bkb");
				}
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("The road ahead looks rocky, but that's all right with me.");

			menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"tiny_avalanche", true},
				    {"tiny_toss", true}
				})));
			menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_blink", true},
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
                    { "item_bloodthorn", true},
				    {"item_mjollnir", true},
                    {"item_dagon", true},
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
			menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}

		public void OnCloseEvent()
		{
			
		}
	}
}
