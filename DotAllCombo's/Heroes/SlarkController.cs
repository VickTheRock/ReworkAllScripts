namespace DotaAllCombo.Heroes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
	using SharpDX;
	using SharpDX.Direct3D9;
	using Service;
	using Service.Debug;
	using System.Threading.Tasks;
	internal class SlarkController : Variables, IHeroController
    {
		
		private bool Active;
		private Ability Q, W, R;
		private bool trying = false;
		private Item urn, ethereal, dagon, halberd, mjollnir, abyssal, mom, Shiva, mail, bkb, satanic, blink, armlet, medall, orchid;
		private Font text;
		private Line line;

		public void OnLoadEvent()
		{
            AssemblyExtensions.InitAssembly("Mhoska/Modif by Vick", "0.1b");

            Print.LogMessage.Success("If I'd known I'd end up here, I'd have stayed in Dark Reef Prison.");
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		    menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			menu.AddItem(new MenuItem("keyBind", "Key Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
			menu.AddItem(new MenuItem("healUlt", "Use Ult if me Health % <").SetValue(new Slider(25, 15, 80)));
			menu.AddItem(
			new MenuItem("Skills", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"slark_shadow_dance", true},
			    {"slark_pounce", true},
			    {"slark_dark_pact", true}
			})));
			menu.AddItem(
				new MenuItem("Items", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_blink", true},
				    {"item_orchid", true},
				    {"item_bloodthorn", true},
				    {"item_armlet", true},
				    {"item_mjollnir", true},
				    {"item_urn_of_shadows", true},
				    {"item_ethereal_blade", true},
				    {"item_abyssal_blade", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true},
				    {"item_satanic", true},
				    {"item_medallion_of_courage", true},
				    {"item_solar_crest", true}
				})));
			menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			text = new Font(
				Drawing.Direct3DDevice9,
				new FontDescription
				{
					FaceName = "Segoe UI",
					Height = 17,
					OutputPrecision = FontPrecision.Default,
					Quality = FontQuality.ClearType
				});

			line = new Line(Drawing.Direct3DDevice9);
		}

		public async void Combo()
		{
			if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame) return;
			
			if (!menu.Item("enabled").IsActive()) return;

			e = me.ClosestToMouseTarget(1700);

			if (e == null) return;
			Q = me.Spellbook.SpellQ;

			W = me.Spellbook.SpellW;

			R = me.Spellbook.SpellR;
			mom = me.FindItem("item_mask_of_madness");
			urn = me.FindItem("item_urn_of_shadows");
			dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			ethereal = me.FindItem("item_ethereal_blade");
			halberd = me.FindItem("item_heavens_halberd");
			mjollnir = me.FindItem("item_mjollnir");
			armlet = me.FindItem("item_armlet");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
			abyssal = me.FindItem("item_abyssal_blade");
			mail = me.FindItem("item_blade_mail");
			bkb = me.FindItem("item_black_king_bar");
			blink = me.FindItem("item_blink");
			satanic = me.FindItem("item_satanic");
			medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
			var invisModif = Toolset.invUnit(me);

			

			Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);

			if (invisModif) return;
			if (Active)
			{

				if (e != null && (!e.IsValid || !e.IsVisible || !e.IsAlive))
				{
					e = null;
				}
				if (e != null && e.IsAlive && !e.IsInvul() && !e.IsIllusion)
				{
					if (me.CanCast() && !me.IsChanneling())
					{
						float angle = me.FindAngleBetween(e.Position, true);
						Vector3 pos = new Vector3((float)(e.Position.X - 100 * Math.Cos(angle)), (float)(e.Position.Y - 100 * Math.Sin(angle)), 0);

						if (blink != null
							&& blink.Cooldown <= 0
							&& me.Distance2D(e) <= 1180
							&& me.Distance2D(e) >= 400 &&
							menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
						{
							blink.UseAbility(pos);
						}
						if (W != null && W.CanBeCasted()
						&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
						{
							if (e.NetworkActivity == NetworkActivity.Move)
							{
								var VectorOfMovement = new Vector2((float)Math.Cos(e.RotationRad) * e.MovementSpeed, (float)Math.Sin(e.RotationRad) * e.MovementSpeed);
								var HitPosition = Interception(e.Position, VectorOfMovement, me.Position, 933.33f);
								var HitPosMod = HitPosition + new Vector3(VectorOfMovement.X * (TimeToTurn(me, HitPosition)), VectorOfMovement.Y * (TimeToTurn(me, HitPosition)), 0);
								var HitPosMod2 = HitPosition + new Vector3(VectorOfMovement.X * (TimeToTurn(me, HitPosMod)), VectorOfMovement.Y * (TimeToTurn(me, HitPosMod)), 0);

								if (GetDistance2D(me, HitPosMod2) > (755 + e.HullRadius))
								{
									return;
								}
								if (IsFacing(me, HitPosMod2))
								{
									W.UseAbility();
									trying = true;
									await Task.Delay(400); //Avoid trying to W multiple times.
									trying = false;
								}
								else
								{
									me.Move((HitPosMod2 - me.Position) * 50 / (float)GetDistance2D(HitPosMod2, me) + me.Position);
								}
							}
							else
							{
								if (GetDistance2D(me, e) > (755 + e.HullRadius))
								{
									return;
								}
								if (IsFacing(me, e))
								{
									W.UseAbility();
									trying = true;
									await Task.Delay(400);
									trying = false;
								}
								else
								{
									me.Move((e.Position - me.Position) * 50 / (float)GetDistance2D(e, me) + me.Position);
								}
							}
						}
						if (me.Distance2D(e) <= +me.AttackRange + 250 &&
							(!me.IsAttackImmune()
							|| !e.IsAttackImmune())
							&& (!W.CanBeCasted()
							|| !menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
							&& me.NetworkActivity != NetworkActivity.Attack
							&& me.CanAttack()
							&& !me.IsAttacking()
							&& Utils.SleepCheck("attack")
							)
							{
								me.Attack(e);
								Utils.Sleep(150, "attack");
							}
							else if (
								(!me.CanAttack()
								|| me.Distance2D(e) >= 250)
								&& me.NetworkActivity != NetworkActivity.Attack
								&& (!W.CanBeCasted()
								|| !menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
								&& me.Distance2D(e) <= 2500
								&& !me.IsAttacking()
								&& Utils.SleepCheck("Move")
								)
							{
								me.Move(Prediction.InFront(e, 100));
								Utils.Sleep(400, "Move");
							}
						if (Q != null 
							&& Q.CanBeCasted()
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
							&& me.Distance2D(e) <= 325
							&& Utils.SleepCheck("Q"))
						{
							Q.UseAbility();
							Utils.Sleep(200, "Q");
						}
						if (orchid != null 
							&& orchid.CanBeCasted() 
							&& me.Distance2D(e) <= 400
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) 
							&& Utils.SleepCheck("orchid"))
						{
							orchid.UseAbility(e);
							Utils.Sleep(100, "orchid");
						}
						if (R != null 
							&& R.IsValid 
							&& R.CanBeCasted() 
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
							&& me.Health <= me.MaximumHealth / 100 * menu.Item("healUlt").GetValue<Slider>().Value 
							&& Utils.SleepCheck("R"))
						{
							R.UseAbility();
							Utils.Sleep(200, "R");
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
						if (armlet != null && !armlet.IsToggled &&
							menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(armlet.Name) &&
							Utils.SleepCheck("armlet"))
						{
							armlet.ToggleAbility();
							Utils.Sleep(300, "armlet");
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
						var v = ObjectManager.GetEntities<Hero>().Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
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
			}
		}
		public void DrawFilledBox(float x, float y, float w, float h, Color color)
		{
			var vLine = new Vector2[2];

			line.GLLines = true;
			line.Antialias = false;
			line.Width = w;

			vLine[0].X = x + w / 2;
			vLine[0].Y = y;
			vLine[1].X = x + w / 2;
			vLine[1].Y = y + h;

			line.Begin();
			line.Draw(vLine, color);
			line.End();
		}

		public void DrawBox(float x, float y, float w, float h, float px, Color color)
		{
			DrawFilledBox(x, y + h, w, px, color);
			DrawFilledBox(x - px, y, px, h, color);
			DrawFilledBox(x, y - px, w, px, color);
			DrawFilledBox(x + w, y, px, h, color);
		}

		public void DrawShadowText(string stext, int x, int y, Color color, Font f)
		{
			f.DrawText(null, stext, x + 1, y + 1, Color.Black);
			f.DrawText(null, stext, x, y, color);
		}
		

		private  double GetDistance2D(dynamic A, dynamic B)
		{
			if (!(A is Unit || A is Vector3)) throw new ArgumentException("Not valid parameters, accepts Unit|Vector3 only", "A");
			if (!(B is Unit || B is Vector3)) throw new ArgumentException("Not valid parameters, accepts Unit|Vector3 only", "B");
			if (A is Unit) A = A.Position;
			if (B is Unit) B = B.Position;

			return Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
		}

		private  Vector3 Interception(Vector3 x, Vector2 y, Vector3 z, float s)
		{
			float x1 = x.X - z.X;
			float y1 = x.Y - z.Y;

			float hs = y.X * y.X + y.Y * y.Y - s * s;
			float h1 = x1 * y.X + y1 * y.Y;
			float t;

			if (hs == 0)
			{
				t = -(x1 * x1 + y1 * y1) / 2 * h1;
			}
			else
			{
				float mp = -h1 / hs;
				float d = mp * mp - (x1 * x1 + y1 * y1) / hs;

				float root = (float)Math.Sqrt(d);

				float t1 = mp + root;
				float t2 = mp - root;

				float tMin = Math.Min(t1, t2);
				float tMax = Math.Max(t1, t2);

				t = tMin > 0 ? tMin : tMax;
			}
			return new Vector3(x.X + t * y.X, x.Y + t * y.Y, x.Z);
		}

		private  bool IsFacing(Unit StartUnit, dynamic Enemy)
		{
			if (!(Enemy is Unit || Enemy is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", "Enemy");
			if (Enemy is Unit) Enemy = Enemy.Position;

			float deltaY = StartUnit.Position.Y - Enemy.Y;
			float deltaX = StartUnit.Position.X - Enemy.X;
			float angle = (float)(Math.Atan2(deltaY, deltaX));

			float n1 = (float)Math.Sin(StartUnit.RotationRad - angle);
			float n2 = (float)Math.Cos(StartUnit.RotationRad - angle);

			return (Math.PI - Math.Abs(Math.Atan2(n1, n2))) < 0.1;
		}

		private  float TimeToTurn(Unit StartUnit, dynamic Enemy)
		{
			if (!(Enemy is Unit || Enemy is Vector3)) throw new ArgumentException("TimeToTurn => INVALID PARAMETERS!", "Enemy");
			if (Enemy is Unit) Enemy = Enemy.Position;

			double TurnRate = 0.5; //Game.FindKeyValues(string.Format("{0}/MovementTurnRate", StartUnit.Name), KeyValueSource.Hero).FloatValue; // (Only works in lobby)

			float deltaY = StartUnit.Position.Y - Enemy.Y;
			float deltaX = StartUnit.Position.X - Enemy.X;
			float angle = (float)(Math.Atan2(deltaY, deltaX));

			float n1 = (float)Math.Sin(StartUnit.RotationRad - angle);
			float n2 = (float)Math.Cos(StartUnit.RotationRad - angle);

			float Calc = (float)(Math.PI - Math.Abs(Math.Atan2(n1, n2)));

			if (Calc < 0.1 && Calc > -0.1) return 0;

			return (float)(Calc * (0.03 / TurnRate));
		}
		

		private void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			text.Dispose();
			line.Dispose();
		}

		

		

		public void OnCloseEvent()
		{
			AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
		}
	}
}