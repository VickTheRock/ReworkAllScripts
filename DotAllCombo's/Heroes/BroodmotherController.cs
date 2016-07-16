using System.Diagnostics;

namespace DotaAllCombo.Heroes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using SharpDX.Direct3D9;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using SharpDX;

	using Service;
	using Service.Debug;

	internal class BroodmotherController : Variables, IHeroController
	{
		private bool Combokey;
		private bool Chasekey;
		private bool LastHitkey;
		private Item mom, abyssal, Soul, orchid, shiva, halberd, mjollnir, satanic, mail, bkb, dagon, medall, sheep, cheese;
		private Ability Q, W, R;
		private int spiderDenies = 65;
		private int spiderDmgStatick = 175;
		private readonly uint[] spiderQ = { 74, 149, 224, 299 };
		private int spiderDmg;
		private bool useQ;
		private Font txt;
		private Font noti;
		private Line lines;

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
			me = ObjectManager.LocalHero;
            e = me.ClosestToMouseTarget(1900);
            LastHitkey = Menu.Item("LastHit").GetValue<KeyBind>().Active;
			Combokey = Game.IsKeyDown(Menu.Item("ComboKey").GetValue<KeyBind>().Key);
			Chasekey = Game.IsKeyDown(Menu.Item("ChaseKey").GetValue<KeyBind>().Key);
			useQ = Menu.Item("useQ").IsActive();

			if (LastHitkey && !Combokey && !Chasekey && Utils.SleepCheck("combo") && !Game.IsPaused)
			{

				Q = me.Spellbook.SpellQ;

				Soul = me.FindItem("item_soul_ring");


				var spiderlingsLevel = me.Spellbook.SpellQ.Level - 1;
				

				var enemies = ObjectManager.GetEntities<Hero>().Where(hero => hero.IsAlive && !hero.IsIllusion && hero.IsVisible && hero.Team != me.Team).ToList();

				var creeps = ObjectManager.GetEntities<Creep>().Where(creep => (creep.Name == "npc_dota_creep_lane" || creep.Name == "npc_dota_creep_siege" || creep.Name == "npc_dota_creep_neutral" ||
				(creep.Name == "npc_dota_visage_familiar" && creep.Team != me.Team) || (creep.Name == "npc_dota_lone_druid_bear" && creep.Team != me.Team) || (creep.Name == "npc_dota_invoker_forged_spirit" &&
				creep.Team != me.Team) || creep.Name == "npc_dota_creep" &&
				creep.IsAlive && creep.IsVisible && creep.IsSpawned) && creep.Health <= 259).ToList();

				var creepQ = ObjectManager.GetEntities<Creep>().Where(creep => (creep.Name == "npc_dota_creep_lane" || creep.Name == "npc_dota_creep_siege" || creep.Name == "npc_dota_creep_neutral" ||
				creep.Name == "npc_dota_lone_druid_bear" || creep.Name == "npc_dota_invoker_forged_spirit" || creep.Name == "npc_dota_creep" &&
				creep.IsAlive && creep.IsVisible && creep.IsSpawned)).ToList();

				var Spiderlings = ObjectManager.GetEntities<Unit>().Where(spiderlings => spiderlings.Name == "npc_dota_broodmother_spiderling").ToList();


				// Creep Q lasthit
				if (useQ && Q.CanBeCasted() && me.IsAlive)
					foreach (var creep in creepQ)
					{
						if (creep.Health <= Math.Floor((spiderQ[spiderlingsLevel]) * (1 - creep.MagicDamageResist)) && creep.Health > 45 && creep.Team != me.Team)
						{
							if (creep.Position.Distance2D(me.Position) <= 600 && Utils.SleepCheck("QQQ"))
							{
								if (Soul != null && Soul.CanBeCasted() && me.Health >= 400)
								{
									Soul.UseAbility();
									Utils.Sleep(300, "QQQ");
								}
								else
									Q.UseAbility(creep);
								Utils.Sleep(300, "QQQ");

							}
						}
					}
				// Enemy Q lasthit
				if (Q.CanBeCasted() && me.IsAlive)
				{
					foreach (var enemy in enemies)
					{
						if (enemy.Health <= (spiderQ[spiderlingsLevel] - enemy.MagicDamageResist) && enemy.Health > 0)
						{
							if (enemy.Position.Distance2D(me.Position) <= 600 && Utils.SleepCheck("QQQ"))
							{
								if (Soul != null && Soul.CanBeCasted() && me.Health >= 400)
								{
									Soul.UseAbility();
									Utils.Sleep(300, "QQQ");
								}
								else
									Q.UseAbility(e);
								Utils.Sleep(300, "QQQ");
							}
						}
					}
				}

				var count = Spiderlings.Count();
				if (count <= 0)
					return;
				// Autodenies
				for (int s = 0; s < count; ++s)
				{
					if (Spiderlings[s].Health > 0 && Spiderlings[s].Health <= spiderDenies)
					{
						for (int z = 0; z < count; ++z)
						{
							if (Spiderlings[s].Position.Distance2D(Spiderlings[z].Position) <= 500
								&& Utils.SleepCheck(Spiderlings[z].Handle + "Spiderlings"))
							{
								Spiderlings[z].Attack(Spiderlings[s]);
								Utils.Sleep(350, Spiderlings[z].Handle + "Spiderlings");

							}
						}
					}
				}

				// Auto spider deny and lasthit
				var countcreep = creeps.Count();
				if (countcreep>=1)
				{
					for (int c = 0; c < countcreep; c++)
				{
					for (int s = 0; s < count; s++)
					{
						if (creeps[c].Position.Distance2D(Spiderlings[s].Position) <= 500 &&
								creeps[c].Team != me.Team && creeps[c].Health > 0 && creeps[c].Health < Math.Floor(spiderDmgStatick * (1 - creeps[c].DamageResist))
								&& Utils.SleepCheck(Spiderlings[s].Handle + "Spiderling"))
							{
								{
									Spiderlings[s].Attack(creeps[c]);
									Utils.Sleep(350, Spiderlings[s].Handle + "Spiderling");
								}
							}
							else if (creeps[c].Position.Distance2D(Spiderlings[s].Position) <= 500 &&
								creeps[c].Team == me.Team && creeps[c].Health > 0 && creeps[c].Health < Math.Floor(spiderDmgStatick * (1 - creeps[c].DamageResist))
								&& Utils.SleepCheck(Spiderlings[s].Handle + "Spiderlings"))
							{
								Spiderlings[s].Attack(creeps[c]);
								Utils.Sleep(350, Spiderlings[s].Handle + "Spiderlings");
							}
						}
					}
				}

				// Auto spider enemy lasthit
				var countenemy = enemies.Count();
				if (countenemy>=1)
				{
				for (int t = 0; t < countenemy; ++t)
				{
					for (int s = 0; s < count; ++s)
					{
						
							spiderDmg = Spiderlings.Count(y => y.Distance2D(enemies[t]) < 800) * Spiderlings[s].MinimumDamage;

							if ((enemies[t].Position.Distance2D(Spiderlings[s].Position)) <= 800 &&
								enemies[t].Team != me.Team && enemies[t].Health > 0 && enemies[t].Health < Math.Floor(spiderDmg * (1 - enemies[t].DamageResist)) && Utils.SleepCheck(Spiderlings[t].Handle + "AttackEnemies"))
							{
								Spiderlings[s].Attack(enemies[t]);
								Utils.Sleep(350, Spiderlings[t].Handle + "AttackEnemies");
							}
						}
					}
				}
				Utils.Sleep(290, "combo");
			}
		}


		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "ty Vick");

			Print.LogMessage.Success("Web is World!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));

			var Skills = new Dictionary<string, bool>
			{
				{"broodmother_spin_web",true},
				{"broodmother_spawn_spiderlings",true},
				{"broodmother_insatiable_hunger",true}
			};

			Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(Skills)));
			Menu.AddItem(new MenuItem("ComboKey", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("ChaseKey", "Chase Key").SetValue(new KeyBind('E', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("LastHit", "LastHitCreeps").SetValue(new KeyBind('F', KeyBindType.Toggle)));
			Menu.AddItem(new MenuItem("useQ", "Kill creep Q Spell").SetValue(true));
			Menu.AddItem(new MenuItem("Skills", "Skills: ").SetValue(new AbilityToggler(Skills)));

			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			Game.OnUpdate += Chasing;
			Game.OnUpdate += ChasingAll;

			Console.WriteLine("> This is your Mom# loaded!");

			txt = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Segoe UI",
				   Height = 19,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });

			noti = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Segoe UI",
				   Height = 30,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });

			lines = new Line(Drawing.Direct3DDevice9);

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}

		public void OnCloseEvent()
		{
			AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
			Drawing.OnPreReset -= Drawing_OnPreReset;
			Drawing.OnPostReset -= Drawing_OnPostReset;
			Drawing.OnEndScene -= Drawing_OnEndScene;
			Game.OnUpdate -= ChasingAll;
			Game.OnUpdate -= Chasing;
		}


		public void Chasing(EventArgs args)
		{
			me = ObjectManager.LocalHero;
			if (Chasekey)
			{
				var Spiderlings = ObjectManager.GetEntities<Unit>().Where(spiderlings => spiderlings.Name == "npc_dota_broodmother_spiderling").ToList();
				{
					var count = Spiderlings.Count();
					if (e != null && e.IsAlive && !e.IsIllusion)
					{
						for (int t = 0; t < count; ++t)
						{
							if (Spiderlings[t].Distance2D(e) <= 1000 && Utils.SleepCheck(Spiderlings[t].Handle + "MoveAttack"))
							{
								Spiderlings[t].Attack(e);
								Utils.Sleep(500, Spiderlings[t].Handle + "MoveAttack");
							}
						}
					}
					else
					{
						for (int t = 0; t < count; ++t)
						{
							if (Utils.SleepCheck(Spiderlings[t].Handle + "Move"))
							{
								Spiderlings[t].Move(Game.MousePosition);
								Utils.Sleep(500, Spiderlings[t].Handle + "Move");
							}
						}
					}

				}
			}
		}


		public void ChasingAll(EventArgs args)
		{

			e = me.ClosestToMouseTarget(1900);
		    if (e == null) return;
			if (Combokey  && e.IsAlive && !me.IsVisibleToEnemies)
			{

				if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
				{
					Orbwalking.Orbwalk(e, 0, 1600, true, true);
				}
			}
			if (Combokey && e.IsAlive && me.IsVisibleToEnemies)
			{

				var Spiderlings = ObjectManager.GetEntities<Unit>().Where(spiderlings => spiderlings.Name == "npc_dota_broodmother_spiderling").ToList();
				var count = Spiderlings.Count();
				for (int s = 0; s < count; ++s)
				{
					if (Spiderlings[s].Distance2D(e) <= 1500 && Utils.SleepCheck(Spiderlings[s].Handle + "Spiderlings"))
					{
						Spiderlings[s].Attack(e);
						Utils.Sleep(500, Spiderlings[s].Handle + "Spiderlings");
					}
				}
				for (int s = 0; s < count; ++s)
				{
					if (Spiderlings[s].Distance2D(e) >= 1500 && Utils.SleepCheck(Spiderlings[s].Handle + "Spiderlings"))
					{
						Spiderlings[s].Move(Game.MousePosition);
						Utils.Sleep(500, Spiderlings[s].Handle + "Spiderlings");
					}
				}


			    var linkens = e.IsLinkensProtected();
				if (e.IsAlive && !e.IsIllusion && me.Distance2D(e) <= 1000)
				{
					
						Q = me.Spellbook.SpellQ;
					
						W = me.Spellbook.SpellW;
					
						R = me.Spellbook.SpellR;

					// Item

					sheep = e.Name == "npc_dota_hero_tidehunter" ? null : me.FindItem("item_sheepstick");

					cheese = me.FindItem("item_cheese");

					orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");

					Soul = me.FindItem("item_soul_ring");

					shiva = me.FindItem("item_shivas_guard");

					dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

					mom = me.FindItem("item_mask_of_madness");

					abyssal = me.FindItem("item_abyssal_blade");

					mjollnir = me.FindItem("item_mjollnir");

					halberd = me.FindItem("item_heavens_halberd");

					mail = me.FindItem("item_blade_mail");

					bkb = me.FindItem("item_black_king_bar");

					medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");

					satanic = me.FindItem("item_satanic");

					if ( // Q Skill
						   Q != null &&
						   Q.CanBeCasted() &&
						   me.CanCast()
						&& me.IsVisibleToEnemies &&
						   !e.IsMagicImmune()
						   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						   && me.Distance2D(e) <= 600
						   && Utils.SleepCheck("Q")
						   )

					{
						Q.UseAbility(e);
						Utils.Sleep(250, "Q");
					} // Q Skill end




					if (//R Skill
						R != null &&
						R.CanBeCasted() &&
						me.CanCast() &&
						me.Distance2D(e) <= 350
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
						&& Utils.SleepCheck("R")
						)
					{
						R.UseAbility();
						Utils.Sleep(250, "R");
					} // R Skill end


					if ( // orchid
						orchid != null &&
						orchid.CanBeCasted() &&
						me.CanCast() &&
						!e.IsMagicImmune() &&
						!linkens &&
						Utils.SleepCheck("orchid")
						&& me.IsVisibleToEnemies
						&& me.Distance2D(e) <= 1000
						)
					{
						orchid.UseAbility(e);
						Utils.Sleep(250, "orchid");
					} // orchid Item end

					if ( // sheep
						sheep != null
						&& me.IsVisibleToEnemies &&
						sheep.CanBeCasted() &&
						me.CanCast() &&
						!e.IsMagicImmune() &&
						!linkens &&
						Utils.SleepCheck("sheep") &&
						me.Distance2D(e) <= 600
						)
					{
						sheep.UseAbility(e);
						Utils.Sleep(250, "sheep");
					} // sheep Item end

					if (// Soul Item 
						Soul != null
                        && Q!=null 
                        && Q.CanBeCasted()
                        && me.Health >= (me.MaximumHealth * 0.6)
                        && me.Mana <= Q.ManaCost
                        && Soul.CanBeCasted())
					{
						Soul.UseAbility();
					} // Soul Item end

					if (// Shiva Item
						shiva != null
						&& me.IsVisibleToEnemies &&
						shiva.CanBeCasted() &&
						me.CanCast() &&
						!e.IsMagicImmune() &&
						Utils.SleepCheck("shiva") &&
						me.Distance2D(e) <= 600
						)
					{
						shiva.UseAbility();
						Utils.Sleep(250, "shiva");
					} // Shiva Item end

					if (// MOM
						mom != null &&
						mom.CanBeCasted() &&
						me.CanCast() &&
						Utils.SleepCheck("mom") &&
						me.Distance2D(e) <= 700
						)
					{
						mom.UseAbility();
						Utils.Sleep(250, "mom");
					} // MOM Item end

					if ( // Medall
						medall != null
						&& me.IsVisibleToEnemies &&
						medall.CanBeCasted() &&

						Utils.SleepCheck("Medall") &&
						me.Distance2D(e) <= 500
						)
					{
						medall.UseAbility(e);
						Utils.Sleep(250, "Medall");
					} // Medall Item end

					if ( // Abyssal Blade
						abyssal != null
						&& me.IsVisibleToEnemies &&
						abyssal.CanBeCasted() &&
						me.CanCast() &&
						!e.IsMagicImmune() &&
						Utils.SleepCheck("abyssal") &&
						me.Distance2D(e) <= 400
						)
					{
						abyssal.UseAbility(e);
						Utils.Sleep(250, "abyssal");
					} // Abyssal Item end

					if ( // Hellbard
						halberd != null
						&& me.IsVisibleToEnemies &&
						halberd.CanBeCasted() &&
						me.CanCast() &&
						!e.IsMagicImmune() &&
						Utils.SleepCheck("halberd") &&
						me.Distance2D(e) <= 700
						)
					{
						halberd.UseAbility(e);
						Utils.Sleep(250, "halberd");
					} // Hellbard Item end

					if ( // Mjollnir
						mjollnir != null
						&& me.IsVisibleToEnemies &&
						mjollnir.CanBeCasted() &&
						me.CanCast() &&
						!e.IsMagicImmune() &&
						Utils.SleepCheck("mjollnir") &&
						me.Distance2D(e) <= 600
					   )
					{
						mjollnir.UseAbility(me);
						Utils.Sleep(250, "mjollnir");
					} // Mjollnir Item end
					var v =
			   ObjectManager.GetEntities<Hero>()
				   .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
				   .ToList();
					if (mail != null && mail.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
														   (Menu.Item("Heelm").GetValue<Slider>().Value))
					&& Utils.SleepCheck("mail"))
					{
						mail.UseAbility();
						Utils.Sleep(100, "mail");
					}
					if (bkb != null && bkb.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
															 (Menu.Item("Heel").GetValue<Slider>().Value))
						&& Utils.SleepCheck("bkb"))
					{
						bkb.UseAbility();
						Utils.Sleep(100, "bkb");
					}
					if (// Dagon
						dagon != null &&
						dagon.CanBeCasted() &&
						me.CanCast() &&
                        !e.IsMagicImmune() &&
						Utils.SleepCheck("dagon")
					   )
					{
						dagon.UseAbility(e);
						Utils.Sleep(250, "dagon");
					} // Dagon Item end


					if (// Satanic 
						satanic != null 
                        && me.Health <= (me.MaximumHealth * 0.3)
                        && satanic.CanBeCasted() 
                        && me.Distance2D(e) <= 300
						&& Utils.SleepCheck("Satanic")
						)
					{
						satanic.UseAbility();
						Utils.Sleep(250, "Satanic");
					} // Satanic Item end

					if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
					{
						Orbwalking.Orbwalk(e, 0, 1600, true, true);
					}


					/***************************************WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW**********************************/

					var Web =
				ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_broodmother_web").ToList();
					var SpinWeb = GetClosestToWeb(Web, me);
					if (W != null && W.CanBeCasted() && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
					{
						if ((me.Distance2D(SpinWeb) >= 900 && e.Distance2D(SpinWeb) >= 900) && me.Distance2D(e) <= 800 && Utils.SleepCheck(SpinWeb.Handle + "SpideWeb"))
						{
							W.UseAbility(e.Predict(1100));
							Utils.Sleep(300, SpinWeb.Handle + "SpideWeb");
						}
					}
				}
				/***************************************WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW**********************************/
			}
		}

		private Unit GetClosestToWeb(List<Unit> units, Hero x)
		{
			Unit closestHero = null;
			foreach (var b in units.Where(v => closestHero == null || closestHero.Distance2D(x) > v.Distance2D(x)))
			{
				closestHero = b;
			}
			return closestHero;
		}

		void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			if (!useQ)
			{
				DrawBox(2, 490, 90, 20, 1, new ColorBGRA(0, 0, 90, 70));
				DrawFilledBox(2, 490, 90, 20, new ColorBGRA(0, 0, 0, 90));
				DrawShadowText("  Q DISABLE", 4, 490, Color.Gold, txt);
			}

			if (Combokey)
			{
				DrawBox(2, 530, 132, 20, 1, new ColorBGRA(0, 0, 90, 70));
				DrawFilledBox(2, 530, 132, 20, new ColorBGRA(0, 0, 0, 90));
				DrawShadowText("BroodMother: Active!", 4, 530, Color.Gold, txt);
			}
			if (Chasekey)
			{
				DrawBox(2, 530, 120, 20, 1, new ColorBGRA(0, 0, 30, 70));
				DrawFilledBox(2, 530, 120, 20, new ColorBGRA(0, 0, 0, 90));
				DrawShadowText("Spiderling: Active!", 4, 530, Color.Gold, txt);
			}
			if (LastHitkey)
			{
				DrawBox(2, 510, 120, 20, 1, new ColorBGRA(0, 0, 90, 70));
				DrawFilledBox(2, 510, 120, 20, new ColorBGRA(0, 0, 0, 90));
				DrawShadowText("LastHit Active", 4, 510, Color.Gold, txt);
			}
		}





		void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			noti.Dispose();
			lines.Dispose();
		}



		void Drawing_OnPostReset(EventArgs args)
		{
			txt.OnResetDevice();
			noti.OnResetDevice();
			lines.OnResetDevice();
		}

		void Drawing_OnPreReset(EventArgs args)
		{
			txt.OnLostDevice();
			noti.OnLostDevice();
			lines.OnLostDevice();
		}

		public void DrawFilledBox(float x, float y, float w, float h, Color color)
		{
			var vLine = new Vector2[2];

			lines.GLLines = true;
			lines.Antialias = false;
			lines.Width = w;

			vLine[0].X = x + w / 2;
			vLine[0].Y = y;
			vLine[1].X = x + w / 2;
			vLine[1].Y = y + h;

			lines.Begin();
			lines.Draw(vLine, color);
			lines.End();
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
	}
}


