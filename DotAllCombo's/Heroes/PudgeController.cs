

namespace DotaAllCombo.Heroes
{
	using Ensage.Common;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Timers;
	using Ensage;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using SharpDX;
    using Service;
	internal class PudgeController : Variables, IHeroController
	{
		public Ability Q, W, R;
		public Item urn, ethereal, dagon, glimmer, vail, orchid, atos, leans, Shiva, mail, sheep, abyssal, bkb, lotus;
		public float eMoveSpeed , minRangeHook;
		public Timer time;
		public readonly Menu skills = new Menu("Skills", "Skills");
		public readonly Menu items = new Menu("Items", "Items");
		public float CastRange;
		public Vector3 HookPosition, CastPos;
		public double TimeTurn;

		public void OnLoadEvent()
		{
			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddSubMenu(skills);
			Menu.AddSubMenu(items);
			skills.AddItem(new MenuItem("Skills", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{   {"pudge_meat_hook",true},
				{"pudge_rot",true},
				{"pudge_dismember",true}
			})));
			items.AddItem(new MenuItem("Items", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_abyssal_blade",true},
				{"item_sheepstick",true},
				{"item_bloodthorn", true},
				{"item_dagon",true},
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
			Menu.AddItem(new MenuItem("z", "Toggle Cancelling Hook Time").SetValue(new Slider(100, 200, 280)));
			Menu.AddItem(new MenuItem("x", "Cancelling Hook").SetValue(new Slider(70, 1, 75)));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB|BladeMail").SetValue(new Slider(2, 1, 5)));
			time = new Timer(Menu.Item("z").GetValue<Slider>().Value);
			time.Elapsed += CancelHook;
		} // OnLoad
		public void OnCloseEvent()
		{
			time.Elapsed -= CancelHook;
			time = null;
		} // OnClose


		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive() || Game.IsChatOpen || time.Enabled)
				return;
			me = ObjectManager.LocalHero;

			Q = me.Spellbook.SpellQ;
			W = me.Spellbook.SpellW;
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
			atos = me.FindItem("item_rod_of_atos");
			bkb = me.FindItem("item_black_king_bar");
			mail = me.FindItem("item_blade_mail");
			lotus = me.FindItem("item_lotus_orb");
			Active = Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key);
			var v = ObjectManager.GetEntities<Hero>().Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune()).ToList();

			if (Active && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
			{
				if (me.HasModifier("modifier_pudge_rot") && v.Count(x => x.Distance2D(me) <= W.GetCastRange()+me.HullRadius) == 0)
				{
					W.ToggleAbility();
					time.Start();
				}
				else if(!me.HasModifier("modifier_pudge_rot") && v.Count(x => x.Distance2D(me) <= W.GetCastRange() + me.HullRadius) > 0)
				{
					W.ToggleAbility();
					time.Start();
				}
			}

            e = Toolset.ClosestToMouse(me);
            if (e == null || !me.IsAlive) return;
			sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
			if (R.IsInAbilityPhase || R.IsChanneling || me.IsChanneling()) return;
			if (Active)
			{
				minRangeHook = e.HullRadius + 27;
				CastRange = leans != null ? (Q.CastRange + 200 + e.HullRadius) : (Q.CastRange + e.HullRadius);
				eMoveSpeed = e.HasModifier("modifier_spirit_breaker_charge_of_darkness") ? 550 + ((int)e.Spellbook.Spell1.Level * 50) : e.MovementSpeed;

				Vector2 vector = new Vector2((float)Math.Cos(e.RotationRad) * eMoveSpeed, (float)Math.Sin(e.RotationRad) * eMoveSpeed);
				Vector3 start = new Vector3((float)((0.3 + (Game.Ping / 1000)) * Math.Cos(e.RotationRad) * eMoveSpeed + e.Position.X),
											(float)((0.3 + (Game.Ping / 1000)) * Math.Sin(e.RotationRad) * eMoveSpeed + e.NetworkPosition.Y), e.NetworkPosition.Z);
				Vector3 specialPosition = new Vector3((float)(minRangeHook * Math.Cos(e.RotationRad) + e.NetworkPosition.X),
													(float)(minRangeHook * Math.Sin(e.RotationRad) + e.NetworkPosition.Y),
													e.NetworkPosition.Z);
				HookPosition = Interception(start, vector, me.Position, 1600);
				if (
					atos != null && atos.CanBeCasted() && me.CanCast() && !e.IsMagicImmune() && !e.HasModifier("modifier_spirit_breaker_charge_of_darkness")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos.Name) && me.Distance2D(e) <= 1500 && Utils.SleepCheck("a")
					)
				{
					atos.UseAbility(e);
					Utils.Sleep(250, "a");
				}
				else if (Q.CanBeCasted() && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
				{
					if (e.NetworkActivity == NetworkActivity.Move || e.HasModifier("modifier_spirit_breaker_charge_of_darkness"))
					{
						for (double i = 0.03; i <= 0.135; i += 0.03)
						{
							Vector3 estimated = new Vector3((float)(i * Math.Cos(e.RotationRad) * eMoveSpeed + HookPosition.X),
															(float)(i * Math.Sin(e.RotationRad) * eMoveSpeed + HookPosition.Y), e.NetworkPosition.Z);
							if (GetTimeToTurn(estimated) <= i)
							{
								HookPosition = estimated;
								TimeTurn = i;
								break;
							}
						}
						CastPos = (HookPosition - me.Position) * ((Q.GetCastRange()+me.HullRadius) / HookPosition.Distance2D(me.Position)) + me.Position;
						if (me.Position.Distance2D(HookPosition) < CastRange)
						{
							Q.UseAbility(CastPos); time.Interval = 150 + TimeTurn * 1000;
							time.Start();
						}
					}
					else
					{
						CastPos = (specialPosition - me.Position) * ((Q.GetCastRange() + me.HullRadius) / specialPosition.Distance2D(me.Position)) + me.Position;
						if (me.Position.Distance2D(e.NetworkPosition) < CastRange)
						{
							Q.UseAbility(CastPos);
							time.Start();
						}
					}
				}
				else
				{
					if (R.IsInAbilityPhase || R.IsChanneling)
						return;
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
					if (dagon != null && dagon.CanBeCasted() && me.Distance2D(e) <= dagon.GetCastRange() && Utils.SleepCheck("dagon"))
					{
						dagon.UseAbility(e);
						Utils.Sleep(100, "dagon");
					}
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
						Console.WriteLine(countElse.ToString());
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
					if (lotus != null && lotus.CanBeCasted() && v.Count(x => x.Distance2D(me) <= 650) >= 2
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(lotus.Name)
						&& Utils.SleepCheck("lotus"))
					{
						lotus.UseAbility(me);
						Utils.Sleep(100, "lotus");
					}
					else countElse += 1;
					if (countElse == 11 && R != null && R.CanBeCasted() && me.Distance2D(e) <= R.GetCastRange() + 150 && (!urn.CanBeCasted() || urn.CurrentCharges <= 0) && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
						&& Utils.SleepCheck("R")
						)
					{
						R.UseAbility(e);
						Utils.Sleep(150, "R");
					}
					else countElse += 1;
					if (abyssal != null && !R.CanBeCasted() && abyssal.CanBeCasted() && !e.IsStunned() && !e.IsHexed()
						&& me.Distance2D(e) <= 300 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name) && Utils.SleepCheck("abyssal"))
					{
						abyssal.UseAbility(e);
						Utils.Sleep(200, "abyssal");
					}
					else countElse += 1;
					if (sheep != null && !R.CanBeCasted() && sheep.CanBeCasted() && !e.IsStunned() && !e.IsHexed()
						&& me.Distance2D(e) <= 900 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name) && Utils.SleepCheck("sheep"))
					{
						sheep.UseAbility(e);
						Utils.Sleep(200, "sheep");
					}
					else countElse += 1;
					if (countElse == 14 && me.Distance2D(e) <= 300 && mail != null
						&& mail.CanBeCasted() && (e.NetworkActivity == NetworkActivity.Attack || e.Spellbook.Spells.All(x => x.IsInAbilityPhase))
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name)
							&& Utils.SleepCheck("mail"))
					{
						mail.UseAbility();
						Utils.Sleep(100, "mail");
					}
					else countElse += 1;
					if (countElse == 15 && lotus != null && lotus.CanBeCasted() && me.Distance2D(e) <= 600
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(lotus.Name)
						&& Utils.SleepCheck("lotus"))
					{
						lotus.UseAbility(me);
						Utils.Sleep(100, "lotus");
					}
					if ((R == null || !R.CanBeCasted() || !Q.CanBeCasted() && me.Distance2D(e) >= R.GetCastRange() + me.HullRadius && !e.HasModifier("pudge_meat_hook")) && !e.IsAttackImmune())
					{
						if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
						{
							Orbwalking.Orbwalk(e, 0, 1600, true, true);
						}
					}
				}
			}
		}
		
		public Vector3 Interception(Vector3 a, Vector2 v, Vector3 b, float s)
		{
			float ox = a.X - b.X;
			float oy = a.Y - b.Y;

			float h1 = v.X * v.X + v.Y * v.Y - s * s;
			float h2 = ox * v.X + oy * v.Y;
			float t;

			if (h1 == 0)
			{
				t = -(ox * ox + oy * oy) / 2 * h2;
			}
			else
			{
				float minusPHalf = -h2 / h1;
				float discriminant = minusPHalf * minusPHalf - (ox * ox + oy * oy) / h1;

				float root = (float)Math.Sqrt(discriminant);

				float t1 = minusPHalf + root;
				float t2 = minusPHalf - root;

				float tMin = Math.Min(t1, t2);
				float tMax = Math.Max(t1, t2);

				t = tMin > 0 ? tMin : tMax;
			}
			return new Vector3(a.X + t * v.X, a.Y + t * v.Y, a.Z);
		}

		public float GetTimeToTurn(Vector3 x)
		{
			Hero m = ObjectManager.LocalHero;
			float deltaY = m.Position.Y - x.Y;
			float deltaX = m.Position.X - x.X;
			float angle = (float)(Math.Atan2(deltaY, deltaX));

			return (float)((Math.PI - Math.Abs(Math.Atan2(Math.Sin(m.RotationRad - angle), Math.Cos(m.RotationRad - angle)))) * (0.03 / 0.7));
		}

		public void CancelHook(object s, ElapsedEventArgs args)
        {
            e = Toolset.ClosestToMouse(me);
            if (e == null) return;
			if (e.HasModifier("modifier_spirit_breaker_charge_of_darkness")) return;

			double travelTime = HookPosition.Distance2D(me.Position) / 1600;
			Vector3 ePosition = new Vector3((float)((travelTime) * Math.Cos(e.RotationRad) * e.MovementSpeed + e.NetworkPosition.X),
										   (float)((travelTime) * Math.Sin(e.RotationRad) * e.MovementSpeed + e.NetworkPosition.Y), 0);
			if (e != null && e.NetworkActivity == NetworkActivity.Move && ePosition.Distance2D(HookPosition) > minRangeHook + Menu.Item("x").GetValue<Slider>().Value)
			{
				me.Stop();
				time.Stop();
			}
			else
			{
				if (Q!=null)
					time.Stop();
			}
		}
	}
}