using System;
using System.Linq;
using System.Collections.Generic;
using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;
using Ensage.Common.Menu;
namespace This_is_your_Mom
{
	internal class Program
	{

		private static bool Combokey;
		private static bool Chasekey;
		private static bool LastHitkey;
		private static readonly Menu Menu = new Menu("This is your Mom", "Broodmother", true, "npc_dota_hero_broodmother", true);
		private static Item mom, abyssal, Soul, orchid, shiva, halberd, mjollnir, satanic, dagon, medall, orhid, sheep, cheese;
		private static Ability Q, W, R;
		private static Hero me;
		private static Hero target;
		private static int spiderDenies = 65;
		private static int spiderDmgStatick = 175;
		private static readonly uint[] spiderQ = { 74, 149, 224, 299 };
		private static readonly uint[] SoulLvl = { 120, 190, 270, 360 };
		private static int spiderDmg;
		//private static bool toggleLasthit = true;
		private static bool useQ;
		private static Font txt;
		private static Font noti;
		private static Line lines;
		//private static Key keyCHASING = Key.Space;
		//private static Key keyCOMBO = Key.D;
		//private static Key toggleKey = Key.D5;
		//private static Key toggleLasthitKey = Key.D7;
		//private static Key UseQ = Key.Q;
		private static readonly Dictionary<string, bool> Skills = new Dictionary<string, bool>
			{
				{"broodmother_spin_web",true},
				{"broodmother_spawn_spiderlings",true},
				{"broodmother_insatiable_hunger",true}

			};

		static void Main(string[] args)
		{
			Menu.AddItem(new MenuItem("ComboKey", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("ChaseKey", "Chase Key").SetValue(new KeyBind('E', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("LastHit", "LastHitCreeps").SetValue(new KeyBind('F', KeyBindType.Toggle)));
			Menu.AddItem(new MenuItem("useQ", "Kill creep Q Spell").SetValue(true));
			Menu.AddItem(new MenuItem("Skills", "Skills: ").SetValue(new AbilityToggler(Skills)));
			Game.OnUpdate += Game_OnUpdate;
			Game.OnUpdate += Chasing;
			Game.OnUpdate += ChasingAll;

			Menu.AddToMainMenu();
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

		public static void Game_OnUpdate(EventArgs args)
		{
			var me = ObjectManager.LocalHero;
			if ((!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Broodmother))
			{
				return;
			}
			LastHitkey = Menu.Item("LastHit").GetValue<KeyBind>().Active;
			Combokey = Game.IsKeyDown(Menu.Item("ComboKey").GetValue<KeyBind>().Key);
			Chasekey = Game.IsKeyDown(Menu.Item("ChaseKey").GetValue<KeyBind>().Key);
			useQ = Menu.Item("useQ").IsActive();

			if (LastHitkey && !Combokey && !Chasekey && Utils.SleepCheck("combo") && !Game.IsPaused)
			{
				if (Q == null)
					Q = me.Spellbook.SpellQ;


				if (Soul == null)
					Soul = me.FindItem("item_soul_ring");


				var spiderlingsLevel = me.Spellbook.SpellQ.Level - 1;

				var myHero = ObjectManager.LocalHero;

				var enemies = ObjectManager.GetEntities<Hero>().Where(hero => hero.IsAlive && !hero.IsIllusion && hero.IsVisible && hero.Team != me.Team).ToList();

				var creeps = ObjectManager.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
				(creep.ClassID == ClassID.CDOTA_Unit_VisageFamiliar && creep.Team == me.GetEnemyTeam()) || (creep.ClassID == ClassID.CDOTA_Unit_SpiritBear && creep.Team == me.GetEnemyTeam()) || (creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit &&
				creep.Team == me.GetEnemyTeam()) || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep &&
				creep.IsAlive && creep.IsVisible && creep.IsSpawned) && creep.Health <= 259).ToList();

				var creepQ = ObjectManager.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
				creep.ClassID == ClassID.CDOTA_Unit_SpiritBear || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep &&
				creep.IsAlive && creep.IsVisible && creep.IsSpawned)).ToList();

				var Spiderlings = ObjectManager.GetEntities<Unit>().Where(spiderlings => spiderlings.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling).ToList();


				// Creep Q lasthit
				if (useQ)
					foreach (var creep in creepQ)
					{
						if (Q.CanBeCasted() && creep.Health <= Math.Floor((spiderQ[spiderlingsLevel]) * (1 - creep.MagicDamageResist)) && creep.Health > 45 && creep.Team != me.Team)
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
				if (useQ)
					foreach (var creep in creepQ)
					{
						if (me.Mana < Q.ManaCost && creep.Health <= Math.Floor((spiderQ[spiderlingsLevel]) * (1 - creep.MagicDamageResist)) && creep.Health > 55 && creep.Team != me.Team)
						{
							if (creep.Position.Distance2D(me.Position) <= 600 && Utils.SleepCheck("QQQ"))
							{
								if (Soul != null && Soul.CanBeCasted() && me.Health >= 400)
								{
									Soul.UseAbility();
									Utils.Sleep(300, "QQQ");
								}

							}
						}
					}
				// Enemy Q lasthit
				foreach (var enemy in enemies)
				{
					if (Q.CanBeCasted() && enemy.Health <= (spiderQ[spiderlingsLevel] - enemy.MagicDamageResist) && enemy.Health > 0)
					{
						if (enemy.Position.Distance2D(me.Position) <= 600 && Utils.SleepCheck("QQQ"))
						{
							if (Soul != null && Soul.CanBeCasted() && me.Health >= 400)
							{
								Soul.UseAbility();
							}
							else
								Q.UseAbility(target);
						}
						Utils.Sleep(300, "QQQ");
					}
				}

				var Spiderling = ObjectManager.GetEntities<Unit>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling && x.IsAlive && x.IsControllable && x.Team == me.Team).ToList();
				if (Spiderling.Count <= 0)
					return;
				// Autodenies
				for (int s = 0; s < Spiderlings.Count(); s++)
				{
					if (Spiderlings[s].Health > 0 && Spiderlings[s].Health <= spiderDenies)
					{
						for (int z = 0; z < Spiderlings.Count(); z++)
						{
							if (Spiderlings[s].Position.Distance2D(Spiderlings[z].Position) <= 500
								&& Utils.SleepCheck(Spiderlings[z].Handle.ToString() + "Spiderlings"))
							{
								Spiderlings[z].Attack(Spiderlings[s]);
								Utils.Sleep(300, Spiderlings[z].Handle.ToString() + "Spiderlings");

							}
						}
					}
				}

				// Auto spider deny and lasthit

				for (int c = 0; c < creeps.Count(); c++)
				{
					for (int s = 0; s < Spiderlings.Count(); s++)
					{
						if (creeps != null)
						{

							if (creeps[c].Position.Distance2D(Spiderlings[s].Position) <= 500 &&
								creeps[c].Team != me.Team && creeps[c].Health > 0 && creeps[c].Health < Math.Floor(spiderDmgStatick * (1 - creeps[c].DamageResist))
								&& Utils.SleepCheck(Spiderlings[s].Handle.ToString() + "Spiderling"))
							{
								{
									Spiderlings[s].Attack(creeps[c]);
									Utils.Sleep(300, Spiderlings[s].Handle.ToString() + "Spiderling");
								}
							}
							else if (creeps[c].Position.Distance2D(Spiderlings[s].Position) <= 500 &&
								creeps[c].Team == me.Team && creeps[c].Health > 0 && creeps[c].Health < Math.Floor(spiderDmgStatick * (1 - creeps[c].DamageResist))
								&& Utils.SleepCheck(Spiderlings[s].Handle.ToString() + "Spiderlings"))
							{
								Spiderlings[s].Attack(creeps[c]);
								Utils.Sleep(300, Spiderlings[s].Handle.ToString() + "Spiderlings");
							}
						}
					}
				}

				// Auto spider enemy lasthit

				for (int t = 0; t < enemies.Count(); t++)
				{
					for (int s = 0; s < Spiderlings.Count(); s++)
					{
						if (enemies != null)
						{
							spiderDmg = Spiderlings.Count(y => y.Distance2D(enemies[t]) < 800) * Spiderlings[s].MinimumDamage;

							if ((enemies[t].Position.Distance2D(Spiderlings[s].Position)) <= 800 &&
								enemies[t].Team != me.Team && enemies[t].Health > 0 && enemies[t].Health < Math.Floor(spiderDmg * (1 - enemies[t].DamageResist)) && Utils.SleepCheck(Spiderlings[t].Handle.ToString() + "AttackEnemies"))
							{
								Spiderlings[s].Attack(enemies[t]);
								Utils.Sleep(350, Spiderlings[t].Handle.ToString() + "AttackEnemies");
							}
						}
					}
				}
				Utils.Sleep(290, "combo");
			}
		}


		public static void Chasing(EventArgs args)
		{
			var me = ObjectManager.LocalHero;
			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Broodmother || me == null)
			{
				return;
			}
			if (Chasekey)
			{
				var target = me.ClosestToMouseTarget(1500);
				var Spiderlings = ObjectManager.GetEntities<Unit>().Where(spiderlings => spiderlings.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling).ToList();
				{
					if (target != null && target.IsAlive && !target.IsIllusion)
					{
						for (int t = 0; t < Spiderlings.Count(); t++)
						{
							if (Spiderlings[t].Distance2D(target) <= 1000 && Utils.SleepCheck(Spiderlings[t].Handle.ToString() + "MoveAttack"))
							{
								Spiderlings[t].Attack(target);
								Utils.Sleep(350, Spiderlings[t].Handle.ToString() + "MoveAttack");
							}
						}
					}
					else
					{
						for (int t = 0; t < Spiderlings.Count(); t++)
						{
							if (Utils.SleepCheck(Spiderlings[t].Handle.ToString() + "Move"))
							{
								Spiderlings[t].Move(Game.MousePosition);
								Utils.Sleep(350, Spiderlings[t].Handle.ToString() + "Move");
							}
						}
					}

				}
			}
		}


		public static void ChasingAll(EventArgs args)
		{
			var me = ObjectManager.LocalHero;
			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Broodmother || me == null)
			{
				return;
			}
			var target = me.ClosestToMouseTarget(1900);
			if (Combokey && target != null && target.IsAlive && !me.IsVisibleToEnemies)
			{

				if (
					me.Distance2D(target) <= 1100 && (!me.IsAttackImmune() || !target.IsAttackImmune())
					&& me.NetworkActivity != NetworkActivity.Attack && me.CanAttack() && Utils.SleepCheck("attack")
					)
				{
					me.Attack(target);
					Utils.Sleep(150, "attack");
				}
			}
			if (Combokey && target != null && target.IsAlive && me.IsVisibleToEnemies)
			{

				var Spiderlings = ObjectManager.GetEntities<Unit>().Where(spiderlings => spiderlings.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling).ToList();

				for (int s = 0; s < Spiderlings.Count(); s++)
				{
					if (Spiderlings[s].Distance2D(target) <= 1500 && Utils.SleepCheck(Spiderlings[s].Handle.ToString() + "Spiderlings"))
					{
						Spiderlings[s].Attack(target);
						Utils.Sleep(350, Spiderlings[s].Handle.ToString() + "Spiderlings");
					}
				}
				for (int s = 0; s < Spiderlings.Count(); s++)
				{
					if (Spiderlings[s].Distance2D(target) >= 1500 && Utils.SleepCheck(Spiderlings[s].Handle.ToString() + "Spiderlings"))
					{
						Spiderlings[s].Move(Game.MousePosition);
						Utils.Sleep(350, Spiderlings[s].Handle.ToString() + "Spiderlings");
					}
				}


				var linkens = target.Modifiers.Any(x => x.Name == "modifier_item_spheretarget") || target.Inventory.Items.Any(x => x.Name == "item_sphere");

				var enemies = ObjectManager.GetEntities<Hero>().Where(hero => hero.IsAlive && !hero.IsIllusion && hero.IsVisible && hero.Team != me.Team).ToList();
				if (target != null && target.IsAlive && !target.IsIllusion && me.Distance2D(target) <= 1000)
				{


					if (Q == null)
						Q = me.Spellbook.SpellQ;

					if (W == null) ///////It will be added later//////////
						W = me.Spellbook.SpellW;

					if (R == null)
						R = me.Spellbook.SpellR;

					// Item

					if (sheep == null)
						sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");


					if (cheese == null)
						cheese = me.FindItem("item_cheese");

					if (orchid == null)
						orchid = me.FindItem("item_orchid");

					if (Soul == null)
						Soul = me.FindItem("item_soul_ring");

					if (shiva == null)
						shiva = me.FindItem("item_shivas_guard");
					
						dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

					if (mom == null)
						mom = me.FindItem("item_mask_of_madness");

					if (abyssal == null)
						abyssal = me.FindItem("item_abyssal_blade");

					if (mjollnir == null)
						mjollnir = me.FindItem("item_mjollnir");

					if (halberd == null)
						halberd = me.FindItem("item_heavens_halberd");
					
						medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");

					if (satanic == null)
						satanic = me.FindItem("item_satanic");

					if ( // Q Skill
						   Q != null &&
						   Q.CanBeCasted() &&
						   me.CanCast() &&
						   !target.IsMagicImmune()
						   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						   && me.Distance2D(target) <= 600
						   && Utils.SleepCheck("Q")
						   )

					{
						Q.UseAbility(target);
						Utils.Sleep(250, "Q");
					} // Q Skill end




					if (//R Skill
						R != null &&
						R.CanBeCasted() &&
						me.CanCast() &&
						me.Distance2D(target) <= 350
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
						!target.IsMagicImmune() &&
						!linkens &&
						Utils.SleepCheck("orchid") &&
						me.Distance2D(target) <= 1000
						)
					{
						orchid.UseAbility(target);
						Utils.Sleep(250, "orchid");
					} // orchid Item end

					if ( // sheep
						sheep != null &&
						sheep.CanBeCasted() &&
						me.CanCast() &&
						!target.IsMagicImmune() &&
						!linkens &&
						Utils.SleepCheck("sheep") &&
						me.Distance2D(target) <= 600
						)
					{
						sheep.UseAbility(target);
						Utils.Sleep(250, "sheep");
					} // sheep Item end

					if (// Soul Item 
						Soul != null &&
						me.Health / me.MaximumHealth <= 0.5 &&

						me.Mana <= Q.ManaCost &&
						Soul.CanBeCasted())
					{
						Soul.UseAbility();
					} // Soul Item end

					if (// Shiva Item
						shiva != null &&
						shiva.CanBeCasted() &&
						me.CanCast() &&
						!target.IsMagicImmune() &&
						Utils.SleepCheck("shiva") &&
						me.Distance2D(target) <= 600
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
						me.Distance2D(target) <= 700
						)
					{
						mom.UseAbility();
						Utils.Sleep(250, "mom");
					} // MOM Item end

					if ( // Medall
						medall != null &&
						medall.CanBeCasted() &&

						Utils.SleepCheck("Medall") &&
						me.Distance2D(target) <= 500
						)
					{
						medall.UseAbility(target);
						Utils.Sleep(250, "Medall");
					} // Medall Item end

					if ( // Abyssal Blade
						abyssal != null &&
						abyssal.CanBeCasted() &&
						me.CanCast() &&
						!target.IsMagicImmune() &&
						Utils.SleepCheck("abyssal") &&
						me.Distance2D(target) <= 400
						)
					{
						abyssal.UseAbility(target);
						Utils.Sleep(250, "abyssal");
					} // Abyssal Item end

					if ( // Hellbard
						halberd != null &&
						halberd.CanBeCasted() &&
						me.CanCast() &&
						!target.IsMagicImmune() &&
						Utils.SleepCheck("halberd") &&
						me.Distance2D(target) <= 700
						)
					{
						halberd.UseAbility(target);
						Utils.Sleep(250, "halberd");
					} // Hellbard Item end

					if ( // Mjollnir
						mjollnir != null &&
						mjollnir.CanBeCasted() &&
						me.CanCast() &&
						!target.IsMagicImmune() &&
						Utils.SleepCheck("mjollnir") &&
						me.Distance2D(target) <= 600
					   )
					{
						mjollnir.UseAbility(me);
						Utils.Sleep(250, "mjollnir");
					} // Mjollnir Item end

					if (// Dagon
						dagon != null &&
						dagon.CanBeCasted() &&
						me.CanCast() &&
						!target.IsMagicImmune() &&
						Utils.SleepCheck("dagon")
					   )
					{
						dagon.UseAbility(target);
						Utils.Sleep(250, "dagon");
					} // Dagon Item end


					if (// Satanic 
						satanic != null &&
						me.Health / me.MaximumHealth <= 0.4 &&
						satanic.CanBeCasted() &&
						me.Distance2D(target) <= 300
						 &&
						Utils.SleepCheck("Satanic")
						)
					{
						satanic.UseAbility();
						Utils.Sleep(250, "Satanic");
					} // Satanic Item end

					if (
						 (!me.CanAttack() || me.Distance2D(target) >= 0) && me.NetworkActivity != NetworkActivity.Attack &&
							me.Distance2D(target) <= 600 && Utils.SleepCheck("Move")
						)
					{
						me.Move(target.Predict(500));
						Utils.Sleep(390, "Move");
					}
					if (
						me.Distance2D(target) <= me.AttackRange + 100 && (!me.IsAttackImmune() || !target.IsAttackImmune())
						&& me.NetworkActivity != NetworkActivity.Attack && me.CanAttack() && Utils.SleepCheck("attack")
						)
					{
						me.Attack(target);
						Utils.Sleep(160, "attack");
					}


					/***************************************WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW**********************************/

					var Web =
				ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_broodmother_web").ToList();
					var SpinWeb = GetClosestToWeb(Web, me);
					if (W != null && W.CanBeCasted() && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
					{
						if ((me.Distance2D(SpinWeb) >= 900) && me.Distance2D(target) <= 800 && Utils.SleepCheck(SpinWeb.Handle.ToString() + "SpideWeb"))
						{
							W.UseAbility(target.Predict(1100));
							Utils.Sleep(300, SpinWeb.Handle.ToString() + "SpideWeb");
						}
					}
				}
				/***************************************WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW**********************************/
			}
		}

		private static Unit GetClosestToWeb(List<Unit> units, Hero x)
		{
			Unit closestHero = null;
			foreach (var b in units.Where(v => closestHero == null || closestHero.Distance2D(x) > v.Distance2D(x)))
			{
				closestHero = b;
			}
			return closestHero;
		}

		static void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectManager.LocalPlayer;
			var me = ObjectManager.LocalHero;
			if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Broodmother)
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





		static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			noti.Dispose();
			lines.Dispose();
		}



		static void Drawing_OnPostReset(EventArgs args)
		{
			txt.OnResetDevice();
			noti.OnResetDevice();
			lines.OnResetDevice();
		}

		static void Drawing_OnPreReset(EventArgs args)
		{
			txt.OnLostDevice();
			noti.OnLostDevice();
			lines.OnLostDevice();
		}

		public static void DrawFilledBox(float x, float y, float w, float h, Color color)
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

		public static void DrawBox(float x, float y, float w, float h, float px, Color color)
		{
			DrawFilledBox(x, y + h, w, px, color);
			DrawFilledBox(x - px, y, px, h, color);
			DrawFilledBox(x, y - px, w, px, color);
			DrawFilledBox(x + w, y, px, h, color);
		}

		public static void DrawShadowText(string stext, int x, int y, Color color, Font f)
		{
			f.DrawText(null, stext, x + 1, y + 1, Color.Black);
			f.DrawText(null, stext, x, y, color);
		}
	}
}


