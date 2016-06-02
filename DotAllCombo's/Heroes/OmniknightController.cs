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

	internal class OmniknightController : Variables, IHeroController
	{
		private Ability Q, W, R;

		private Item urn, ethereal, dagon, halberd, mjollnir, orchid, abyssal, mom, Shiva, mail, bkb, satanic, 
            medall, glimmer, manta, pipe, guardian, sphere;

        private bool Active;

		private Menu items = new Menu("Items", "Items");
		private Menu heal = new Menu("Heal", "Heal Items Settings");
		private Menu ult = new Menu("AutoUlt", "AutoUlt");

		public void Combo()
		{
			Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);

			Q = me.Spellbook.SpellQ;
			W = me.Spellbook.SpellW;
			R = me.Spellbook.SpellR;

			mom = me.FindItem("item_mask_of_madness");
			glimmer = me.FindItem("item_glimmer_cape");
			manta = me.FindItem("item_manta");
			pipe = me.FindItem("item_pipe");
			guardian = me.FindItem("item_guardian_greaves") ?? me.FindItem("item_mekansm");
			sphere = me.FindItem("item_sphere");
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
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
					.ToList();
		    e = me.ClosestToMouseTarget(1800);
			if (e == null) return;
			if (Active && me.Distance2D(e) <= 1400 && e.IsAlive && !me.IsInvisible())
			{

                if (
                    me.Distance2D(e) <= me.AttackRange + 100 && (!me.IsAttackImmune() || !e.IsAttackImmune())
                    && me.NetworkActivity != NetworkActivity.Attack && me.CanAttack() && Utils.SleepCheck("attack")
                    )
                {
                    me.Attack(e);
                    Utils.Sleep(150, "attack");
                }
            else if (
					(!me.CanAttack() || me.Distance2D(e) >= 0) && me.NetworkActivity != NetworkActivity.Attack &&
					me.Distance2D(e) <= 600 && Utils.SleepCheck("Move")
					)
				{
					me.Move(e.Predict(300));
					Utils.Sleep(390, "Move");
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
				if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) <= 900 &&
					menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) && Utils.SleepCheck("orchid"))
				{
					orchid.UseAbility(e);
					Utils.Sleep(100, "orchid");
				}

				if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(e) <= 600
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
					&& !e.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					Shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}

				if (ethereal != null && ethereal.CanBeCasted()
					&& me.Distance2D(e) <= 700 && me.Distance2D(e) <= 400
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name) &&
					Utils.SleepCheck("ethereal"))
				{
					ethereal.UseAbility(e);
					Utils.Sleep(100, "ethereal");
				}

				if (dagon != null
					&& dagon.CanBeCasted()
					&& me.Distance2D(e) <= 700
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
				var Ally = ObjectManager.GetEntities<Hero>()
										.Where(x => x.Team == me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion).ToList();

				var countAlly = Ally.Count();
				var countV = v.Count();
				for (int i = 0; i < countAlly; ++i)
				{
					if (
							Q != null && Q.CanBeCasted() 
							&& !me.IsMagicImmune()
							&& me.Health <= (me.MaximumHealth * 0.6)
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
							&& Utils.SleepCheck("Q")
							)
					{
						Q.UseAbility(me);
						Utils.Sleep(200, "Q");
					}
					if (
							Q != null && Q.CanBeCasted()
							&& me.Distance2D(e) <= 255
							&& !me.IsMagicImmune()
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
							&& Utils.SleepCheck("Q")
							)
					{
						Q.UseAbility(me);
						Utils.Sleep(200, "Q");
					}
					
						if (
							W != null 
							&& W.CanBeCasted() 
							&& me.Distance2D(e) <=400
							&& !Q.CanBeCasted()
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
							&& Utils.SleepCheck("W")
							)
					{
						W.UseAbility(me);
						Utils.Sleep(200, "W");
					}
					for (int z = 0; z < countV; ++z)
					{

						if (
							Q != null && Q.CanBeCasted() 
							&& me.Distance2D(Ally[i]) <= Q.CastRange + 50
							&& !Ally[i].IsMagicImmune()
							&& Ally[i].Health <= (Ally[i].MaximumHealth * 0.6)
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
							&& Utils.SleepCheck("Q")
							)
						{
							Q.UseAbility(Ally[i]);
							Utils.Sleep(200, "Q");
						}
						else
						if (
							W != null && W.CanBeCasted() 
							&& !Q.CanBeCasted()
							&& me.Distance2D(Ally[i]) <= W.CastRange + 50
							&& Ally[i].Health <= (Ally[i].MaximumHealth * 0.6) 
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
							&& Utils.SleepCheck("Wq")
							)
						{
							W.UseAbility(Ally[i]);
							Utils.Sleep(200, "Wq");
						}
						if (
							W != null 
							&& W.CanBeCasted() 
							&& me.Distance2D(Ally[i]) <= W.CastRange + 50
							&& !Ally[i].IsMagicImmune()
							&& ((Ally[i].Distance2D(v[z]) <= Ally[i].AttackRange + Ally[i].HullRadius + 10)
							|| (Ally[i].Distance2D(v[z]) <= v[i].AttackRange + Ally[i].HullRadius + 10))
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
							&& Utils.SleepCheck("Ww")
							)
						{
							W.UseAbility(Ally[i]);
							Utils.Sleep(200, "Ww");
						}
						if (
							Q != null && Q.CanBeCasted() && me.Distance2D(Ally[i]) <= Q.CastRange + 50
							&& !Ally[i].IsMagicImmune()
							&& Ally[i].Distance2D(v[z]) <= 250 + Ally[i].HullRadius - 10
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
							&& Utils.SleepCheck("Q")
							)
						{
							Q.UseAbility(Ally[i]);
							Utils.Sleep(200, "Q");
						}
						if (
							R != null && R.CanBeCasted()
							&& me.Distance2D(Ally[i]) <= R.CastRange + 50
							&& (v.Count(x => x.Distance2D(me) <= R.CastRange) >= (menu.Item("UltCountTarget").GetValue<Slider>().Value))
							&& (Ally.Count(x => x.Distance2D(me) <= R.CastRange) >= (menu.Item("UltCountAlly").GetValue<Slider>().Value))
							&& Ally[i].Health <= (Ally[i].MaximumHealth / 100 * (menu.Item("HealhUlt").GetValue<Slider>().Value))
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
							&& Utils.SleepCheck("R")
							)
						{
							R.UseAbility();
							Utils.Sleep(200, "R");
						}
						if (
							guardian != null && guardian.CanBeCasted()
							&& me.Distance2D(Ally[i]) <= guardian.CastRange
							&& (v.Count(x => x.Distance2D(me) <= guardian.CastRange) >= (menu.Item("healsetTarget").GetValue<Slider>().Value))
							&& (Ally.Count(x => x.Distance2D(me) <= guardian.CastRange) >= (menu.Item("healsetAlly").GetValue<Slider>().Value))
							&& Ally[i].Health <= (Ally[i].MaximumHealth / 100 * (menu.Item("HealhHeal").GetValue<Slider>().Value))
							&& menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(guardian.Name)
							&& Utils.SleepCheck("guardian")
							)
						{
							guardian.UseAbility();
							Utils.Sleep(200, "guardian");
						}
						if (
							pipe != null && pipe.CanBeCasted()
							&& me.Distance2D(Ally[i]) <= pipe.CastRange
							&& (v.Count(x => x.Distance2D(me) <= pipe.CastRange) >= (menu.Item("pipesetTarget").GetValue<Slider>().Value))
							&& (Ally.Count(x => x.Distance2D(me) <= pipe.CastRange) >= (menu.Item("pipesetAlly").GetValue<Slider>().Value))
							&& menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(pipe.Name)
							&& Utils.SleepCheck("pipe")
							)
						{
							pipe.UseAbility();
							Utils.Sleep(200, "pipe");
						}

						if (
							sphere != null && sphere.CanBeCasted() && me.Distance2D(Ally[i]) <= sphere.CastRange + 50
							&& !Ally[i].IsMagicImmune()
							&& ((Ally[i].Distance2D(v[z]) <= Ally[i].AttackRange + Ally[i].HullRadius + 10)
							|| (Ally[i].Distance2D(v[z]) <= v[i].AttackRange + Ally[i].HullRadius + 10)
							|| Ally[i].Health <= (me.MaximumHealth * 0.5))
							&& menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(sphere.Name)
							&& Utils.SleepCheck("sphere")
							)
						{
							sphere.UseAbility(Ally[i]);
							Utils.Sleep(200, "sphere");
						}
						if (
							glimmer != null && glimmer.CanBeCasted() && me.Distance2D(Ally[i]) <= glimmer.CastRange + 50
							&& Ally[i].Health <= (me.MaximumHealth * 0.5)
							&& menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)
							&& Utils.SleepCheck("glimmer")
							)
						{
							glimmer.UseAbility(Ally[i]);
							Utils.Sleep(200, "glimmer");
						}
						if (
							manta != null && manta.CanBeCasted()
							&& (me.Distance2D(v[z]) <= me.AttackRange + me.HullRadius + 10)
							|| (me.Distance2D(v[z]) <= v[i].AttackRange + me.HullRadius + 10)
							&& menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(manta.Name)
							&& Utils.SleepCheck("manta")
							)
						{
							manta.UseAbility();
							Utils.Sleep(200, "manta");
						}
					}
				}
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Where piety fails, my hammer falls.");

		    menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"omniknight_guardian_angel", true},
				    {"omniknight_purification", true},
				    {"omniknight_repel", true}
				})));
			items.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true}, {"item_bloodthorn", true},
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
			items.AddItem(
				new MenuItem("ItemsS", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_manta", true},
				    {"item_mekansm", true},
				    {"item_pipe", true},
				    {"item_guardian_greaves", true},
				    {"item_sphere", true},
				    {"item_glimmer_cape", true}
				})));
			menu.AddItem(new MenuItem("Heel", "Min Target's to BKB").SetValue(new Slider(2, 1, 5)));
			menu.AddItem(new MenuItem("Heelm", "Min Target's to BladeMail").SetValue(new Slider(2, 1, 5)));
			ult.AddItem(new MenuItem("UltCountTarget", "Min Target's to ToUlt").SetValue(new Slider(2, 1, 5)));
			ult.AddItem(new MenuItem("HealhUlt", "Min healh Ally % to Ult").SetValue(new Slider(35, 10, 70))); // x/ 10%
			ult.AddItem(new MenuItem("UltCountAlly", "Min Ally to ToUlt").SetValue(new Slider(2, 1, 5)));
			heal.AddItem(new MenuItem("pipesetTarget", "Min Target's to Pipe").SetValue(new Slider(2, 1, 5)));
			heal.AddItem(new MenuItem("pipesetAlly", "Min Ally to Pipe").SetValue(new Slider(2, 1, 5)));
			heal.AddItem(new MenuItem("healsetTarget", "Min Target's to Meka|Guardian").SetValue(new Slider(2, 1, 5)));
			heal.AddItem(new MenuItem("healsetAlly", "Min Ally to Meka|Guardian").SetValue(new Slider(2, 1, 5)));
			heal.AddItem(new MenuItem("HealhHeal", "Min healh % to Heal").SetValue(new Slider(35, 10, 70))); // x/ 10%
			menu.AddSubMenu(items);
			menu.AddSubMenu(heal);
			menu.AddSubMenu(ult);
		}

		public void OnCloseEvent()
		{
			
		}
	}
}