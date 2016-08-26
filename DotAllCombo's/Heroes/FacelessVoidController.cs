namespace DotaAllCombo.Heroes
{
	using System.Threading.Tasks;
	using SharpDX;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;

	using Service;
	using Service.Debug;

	internal class FacelessVoidController : Variables, IHeroController
	{
		private Ability Q, W, R;

		private Item urn, orchid, ethereal, dagon, halberd, blink, mjollnir, abyssal, mom, Shiva, mail, bkb, satanic, medall;

		private readonly Menu ult = new Menu("Auto Time Walk", "Auto Time Walk");
		private float health;
		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("The threads of fate are mine to weave.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

			Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"faceless_void_chronosphere", true},
					{"faceless_void_time_dilation", true},
					{"faceless_void_time_walk", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_mask_of_madness", true},
					{"item_heavens_halberd", true},
					{"item_orchid", true},
					{"item_bloodthorn", true},
					{"item_blink", true},
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
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
            Menu.AddItem(new MenuItem("time_dilation", "Min targets to TimeDilation").SetValue(new Slider(2, 1, 5))).SetTooltip("TODO UPDATE LOGIC");
            Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("v", "Min targets in Ult").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("ally", "Max ally in Ult").SetValue(new Slider(1, 0, 5)));
			ult.AddItem(new MenuItem("ultDraw", "Show me Lost Health").SetValue(true));
			ult.AddItem(new MenuItem("MomentDownHealth", "Min Health Down To Ult").SetValue(new Slider(450, 200, 2000)));
			Menu.AddSubMenu(ult);
			Drawing.OnDraw += DrawUltiDamage;
		}

		public void Combo()
		{
			if (!Menu.Item("enabled").GetValue<bool>()) return;

			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
			Q = me.Spellbook.SpellQ;
			W = me.Spellbook.SpellW;
			R = me.Spellbook.SpellR;

			mom = me.FindItem("item_mask_of_madness");
			urn = me.FindItem("item_urn_of_shadows");
			dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			ethereal = me.FindItem("item_ethereal_blade");
			halberd = me.FindItem("item_heavens_halberd");
			mjollnir = me.FindItem("item_mjollnir");
			blink = me.FindItem("item_blink");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
			abyssal = me.FindItem("item_abyssal_blade");
			mail = me.FindItem("item_blade_mail");
			bkb = me.FindItem("item_black_king_bar");
			satanic = me.FindItem("item_satanic");
			medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
			Shiva = me.FindItem("item_shivas_guard");
			var v = ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
					.ToList();
            e = Toolset.ClosestToMouse(me);
            if (e == null) return;
			if (Active)
			{
				if (Menu.Item("orbwalk").GetValue<bool>())
				{
					Orbwalking.Orbwalk(e, 0, 1600, true, true);
				}
			}
			if (Active && me.Distance2D(e) <= 1400 && e.IsAlive && !me.IsInvisible())
            {
                if (Menu.Item("orbwalk").GetValue<bool>())
                {
                    Orbwalking.Orbwalk(e, 0, 1600, true, true);
                }
                if (
                    W != null && W.CanBeCasted() && v.Count(x => x.Distance2D(me) <= 725+me.HullRadius) 
                    >= (Menu.Item("time_dilation").GetValue<Slider>().Value)
                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                    && (R== null||!R.CanBeCasted() || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name) || v.Count(x => x.Distance2D(me) <= 425 + me.HullRadius) 
                    < Menu.Item("v").GetValue<Slider>().Value)
					&& Utils.SleepCheck("W")
                    && (R == null || !R.IsInAbilityPhase)
                    && !e.HasModifier("modifier_faceless_void_chronosphere_freeze")
					)
				{
					W.UseAbility();
					Utils.Sleep(100, "W");
				}
				if (
					blink != null
					&& me.CanCast()
					&& blink.CanBeCasted()
					&& me.Distance2D(e) < 1190
					&& v.Count(x => x.Distance2D(e) <= 525) <= 1
					&& me.Distance2D(e) > me.AttackRange + 150
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					blink.UseAbility(e.Position);
					Utils.Sleep(250, "blink");
				}
				if (
					Q != null && Q.CanBeCasted() 
					&& me.Distance2D(e) <= Q.GetCastRange()+me.HullRadius+24
					&& me.Distance2D(e) >= 450
					&& me.CanAttack()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					&& Utils.SleepCheck("Q")
					)
				{
					Q.UseAbility(e.Position);
					Utils.Sleep(100, "Q");
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
				if ( // orchid
					orchid != null
					&& orchid.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& me.Distance2D(e) <= me.AttackRange + 40
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
			OnTimedEvent();
			if (Active)
			{
				//TODO test
				var ally = ObjectManager.GetEntities<Hero>()
											 .Where(x => x.Team == me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.Equals(me)).ToList();
				for (int i = 0; i < v.Count; ++i)
				{
				    if ((v.Count(x => x.Distance2D(v[i]) <= 425 + me.HullRadius) >=
				         (Menu.Item("v").GetValue<Slider>().Value))
				        && (ally.Count(x => x.Distance2D(me) <= 425 + me.HullRadius) <=
				            (Menu.Item("ally").GetValue<Slider>().Value)))
				    {
                        if (blink != null && blink.CanBeCasted() && me.Distance2D(v[i]) <= blink.GetCastRange() && me.Distance2D(v[i]) > R.GetCastRange() + me.HullRadius
                             && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name) && Utils.SleepCheck("blink"))
                        {
                            blink.UseAbility(v[i].Position);
                            Utils.Sleep(100, "blink");
                        }
                        if (Q != null && Q.CanBeCasted() && me.Distance2D(v[i]) <= Q.GetCastRange() + me.HullRadius && me.Distance2D(v[i]) > R.GetCastRange() + me.HullRadius
                             && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name) && Utils.SleepCheck("Q"))
                        {
                            Q.UseAbility(v[i].Position);
                            Utils.Sleep(100, "Q");
                        }
                        if (R != null && R.CanBeCasted() && me.Distance2D(v[i]) <= R.GetCastRange() + me.HullRadius
                            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name) && Utils.SleepCheck("Q"))
                        {
                            R.UseAbility(v[i].Position);
                            Utils.Sleep(100, "Q");
                        }
                    }
				}
			}
		}


		public void OnCloseEvent()
		{

			Drawing.OnDraw -= DrawUltiDamage;
		}

		private void OnTimedEvent()
		{
			
			if (Game.IsPaused || Q == null) return;
			if (Q != null)
			{
				if (Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
				{
					//TODO test
					float now = me.Health;
					Task.Delay(2000 - (int)Game.Ping).ContinueWith(_ =>
					{
						float back4 = me.Health;
						if ((now - back4) >= Menu.Item("MomentDownHealth").GetValue<Slider>().Value)
						{
							if (Q.CanBeCasted() && Utils.SleepCheck("Q"))
							{
								Q.UseAbility(Prediction.InFront(me, 150));
								Utils.Sleep(250, "Q");
							}
						}
					});
				}
			}
		}
		private bool OnScreen(Vector3 v)
		{
			return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
		}
		private void DrawUltiDamage(EventArgs args)
		{
			if (!Game.IsInGame || Game.IsPaused || !me.IsAlive || Game.IsWatchingGame)
			{
				return;
			}
			if (Menu.Item("ultDraw").GetValue<bool>())
			{
				float now = me.Health;
				Task.Delay(2000-(int)Game.Ping).ContinueWith(_ =>
				{
					float back4 = me.Health;
					health = (now - back4);
					if (health < 0)
						health = 0;
				});
				var screenPos = HUDInfo.GetHPbarPosition(me);
				if (!OnScreen(me.Position)) return;
				//TODO test
				var text =  Math.Floor(health).ToString();
				var size = new Vector2(18, 18);
				var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
				var position = new Vector2(screenPos.X - textSize.X - 1, screenPos.Y + 1);
				Drawing.DrawText(
					text,
					position,
					size,
					(Color.LawnGreen),
					FontFlags.AntiAlias);
				Drawing.DrawText(
					text,
					new Vector2(screenPos.X - textSize.X - 0, screenPos.Y + 0),
					size,
					(Color.Black),
					FontFlags.AntiAlias);
			}
		}
	}
}

