using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX;
using SharpDX.Direct3D9;
using System.Timers;
using System.Threading.Tasks;
using Ensage.Common.Menu;
namespace Meepo
{
	internal class Program
	{
		private static Hero e, me, target, initMeepo;
		
		private static bool activated;
		private static bool dodge=true;
		private static Item blink, travel, Travel, shiva, sheep, medall, dagon, cheese, ethereal, vail, atos, orchid, abyssal;
		private static readonly uint[] Qradius = { 0, 400, 650, 800, 1000 };
		private static Font txt;
		private static Font not;

		private static readonly Menu Menu = new Menu("Meepo'S by VickTheRock", "Meepo's Combo's", true, "npc_dota_hero_meepo", true);
		private static Key keyCombo = Key.E;
		private static void Main(string[] args)
		{
			Game.OnUpdate += Game_OnUpdate;
			Console.WriteLine("Meepo combo loaded!");
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("Dodge", "Dodge meepo's").SetValue(new KeyBind('T', KeyBindType.Toggle)));
			Game.PrintMessage("<font face='verdana' color='#f80000'>Alfa Version Meepo's by Vick Loaded.!</font>", MessageType.LogMessage);
			Menu.AddToMainMenu();
			txt = new Font(
				Drawing.Direct3DDevice9,
				new FontDescription
				{
					FaceName = "Tahoma",
					Height = 12,
					OutputPrecision = FontPrecision.Default,
					Quality = FontQuality.Default
				});

			not = new Font(
				Drawing.Direct3DDevice9,
				new FontDescription
				{
					FaceName = "Tahoma",
					Height = 12,
					OutputPrecision = FontPrecision.Default,
					Quality = FontQuality.Default
				});

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}
		private static Hero ClosestToMouse(Hero source, float range = 90000)
		{
			var mousePosition = Game.MousePosition;
			var enemyHeroes =
				ObjectManager.GetEntities<Hero>()
					.Where(
						x =>
							x.Team != me.Team && !x.IsIllusion && x.IsAlive && x.IsVisible
							&& x.Distance2D(mousePosition) <= range);
			Hero[] closestHero = { null };
			foreach (var enemyHero in enemyHeroes.Where(enemyHero => closestHero[0] == null || closestHero[0].Distance2D(mousePosition) > enemyHero.Distance2D(mousePosition)))
			{
				closestHero[0] = enemyHero;
			}
			return closestHero[0];
		}


		private static Hero GetClosestToTarget(List<Hero> units, Hero target)
		{
			Hero closestHero = null;
			foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(target) > v.Distance2D(target)))
			{
				closestHero = v;
			}
			return closestHero;
		}

		public static void Game_OnUpdate(EventArgs args)
		{

			me = ObjectManager.LocalHero;
			if (me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_Meepo || !Game.IsInGame)
			{
				return;
			}
			if (!me.IsAlive)
				return;
			activated = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
			var meepos = ObjectManager.GetEntities<Hero>().Where(x => x.IsControllable && x.IsAlive && x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo).ToList();

			Ability[] q = new Ability[meepos.Count()];
			for (int i = 0; i < meepos.Count(); i++) q[i] = meepos[i].Spellbook.SpellQ;
			Ability[] w = new Ability[meepos.Count()];
			for (int i = 0; i < meepos.Count(); i++) w[i] = meepos[i].Spellbook.SpellW;
			List<Unit> fount = ObjectManager.GetEntities<Unit>().Where(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain).ToList();
			//blink = me.FindItem("item_blink");

			dodge = Menu.Item("Dodge").GetValue<KeyBind>().Active;


			e = ObjectManager.GetEntities<Hero>()
						.Where(x => x.IsAlive && x.Team != me.Team && !x.IsIllusion)
						.OrderBy(x => GetDistance2D(x.Position, meepos.OrderBy(y => GetDistance2D(x.Position, y.Position)).FirstOrDefault().Position))
						.FirstOrDefault();
			

			/**************************************************DODGE*************************************************************/
			
			var f = ObjectManager.GetEntities<Hero>()
						.Where(x => x.IsAlive && x.Team == me.Team && !x.IsIllusion && x.IsControllable && x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo)
						.OrderBy(x => GetDistance2D(x.Position, fount.OrderBy(y => GetDistance2D(x.Position, y.Position)).FirstOrDefault().Position))
						.FirstOrDefault();
			if (dodge && me.IsAlive)
			{
				var baseDota =
				  ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_base" && unit.Team != me.Team).ToList();
				if (baseDota != null)
				{
					for (int t = 0; t < baseDota.Count(); t++)
					{
						for (int i = 0; i < meepos.Count(); i++)
						{
							float angle = meepos[i].FindAngleBetween(baseDota[t].Position, true);
							Vector3 pos = new Vector3((float)(baseDota[t].Position.X - 710 * Math.Cos(angle)), (float)(baseDota[t].Position.Y - 710 * Math.Sin(angle)), 0);
							if (meepos[i].Distance2D(baseDota[t]) <= 700 && Utils.SleepCheck(meepos[i].Handle.ToString() + "MoveDodge"))
							{
								meepos[i].Move(pos);
								Utils.Sleep(120, meepos[i].Handle.ToString() + "MoveDodge");
								//	Console.WriteLine("Name: " + baseDota[t].Name);
								//	Console.WriteLine("Speed: " + baseDota[t].Speed);
								//	Console.WriteLine("ClassID: " + baseDota[t].ClassID);
								//	Console.WriteLine("Handle: " + baseDota[t].Handle);
								//	Console.WriteLine("UnitState: " + baseDota[t].UnitState);
							}
						}
					}
				}

				var thinker =
				   ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_thinker" && unit.Team != me.Team).ToList();
				if (thinker != null)
				{
					for (int i = 0; i < thinker.Count(); i++)
					{
						for (int j = 0; j < meepos.Count(); j++)
						{
							float angle = meepos[j].FindAngleBetween(thinker[i].Position, true);
							Vector3 pos = new Vector3((float)(thinker[i].Position.X - 360 * Math.Cos(angle)), (float)(thinker[i].Position.Y - 360 * Math.Sin(angle)), 0);
							if (meepos[j].Distance2D(thinker[i]) <= 350)
							{

								if (Utils.SleepCheck(meepos[j].Handle.ToString() + "MoveDodge"))
								{
									meepos[j].Move(pos);
									Utils.Sleep(350, meepos[j].Handle.ToString() + "MoveDodge");
								}

							}
						}
					}
				}
				foreach (var v in meepos)
				{
					if (Utils.SleepCheck(v.Handle.ToString() + "_move") && v.Health <= v.MaximumHealth * 0.58
						&& v.Distance2D(fount.First().Position) >= 1000
						)
					{
						v.Move(fount.First().Position);
						Utils.Sleep(300, v.Handle.ToString() + "_move");
					}
					if (activated)
					{
						float angle = v.FindAngleBetween(fount.First().Position, true);
						Vector3 pos = new Vector3((float)(fount.First().Position.X - 500 * Math.Cos(angle)), (float)(fount.First().Position.Y - 500 * Math.Sin(angle)), 0);
						if (
							v.Health >= v.MaximumHealth * 0.58
							&& v.Distance2D(fount.First()) <= 400
							&& me.Team == Team.Radiant
							&& Utils.SleepCheck(v.Handle.ToString() + "RadMove")
							)
						{
							v.Move(pos);
							Utils.Sleep(400, v.Handle.ToString() + "RadMove");
						}
						if (
							v.Health >= v.MaximumHealth * 0.58
							&& v.Distance2D(fount.First()) <= 400
							&& me.Team == Team.Dire
							&& Utils.SleepCheck(v.Handle.ToString() + "DireMove")
							)
						{
							v.Move(pos);
							Utils.Sleep(400, v.Handle.ToString() + "DireMove");
						}
					}
				}

				for (int i = 0; i < meepos.Count(); i++)
				{
					travel = meepos[i].FindItem("item_travel_boots") ?? meepos[i].FindItem("item_travel_boots_2");

					if (meepos[i].Health <= meepos[i].MaximumHealth * 0.58 && q[i].CanBeCasted() && !e.Modifiers.Any(y => y.Name == "modifier_meepo_earthbind") && meepos[i].Distance2D(e) <= q[i].CastRange - 200 && Utils.SleepCheck(meepos[i].Handle.ToString() + "_net_casting"))
					{
						q[i].CastSkillShot(e);
						Utils.Sleep(q[i].GetCastDelay(meepos[i], e, true, true) + 500, meepos[i].Handle.ToString() + "_net_casting");
					}
					else if (!q[i].CanBeCasted() && meepos[i].Health <= meepos[i].MaximumHealth * 0.58)
					{
						for (int j = 0; j < meepos.Count(); j++)
						{
							if (meepos[j] != meepos[i] && meepos[j].Position.Distance2D(e) < q[i].CastRange && !e.Modifiers.Any(y => y.Name == "modifier_meepo_earthbind")
								&& meepos[j].Position.Distance2D(meepos[i]) < q[j].CastRange - 100 && Utils.SleepCheck(meepos[i].Handle.ToString() + "_net_casting"))
							{
								q[j].CastSkillShot(e);
								Utils.Sleep(q[j].GetCastDelay(meepos[j], e, true, true) + 1500, meepos[i].Handle.ToString() + "_net_casting");
								break;
							}
						}
					}
					if ((w[i].CanBeCasted() && meepos[i].Health <= meepos[i].MaximumHealth * 0.58 && meepos[i] != f && meepos[i].Distance2D(f) >= 700)
						&& (meepos[i].Distance2D(e) >= (e.AttackRange + 60) || meepos[i].MovementSpeed <= 290) && (q == null || (!q[i].CanBeCasted() || e.Modifiers.Any(y => y.Name == "modifier_meepo_earthbind")) || meepos[i].Distance2D(e) >= 1000)
						&& meepos[i].Distance2D(fount.First().Position) >= 1100
						&& Utils.SleepCheck(meepos[i].Handle.ToString() + "W"))
					{
						w[i].UseAbility(f);
						Utils.Sleep(1000, meepos[i].Handle.ToString() + "W");
					}
					else if (travel.CanBeCasted() && meepos[i].Health <= meepos[i].MaximumHealth * 0.58
						&& (!w[i].CanBeCasted() || meepos[i].Position.Distance2D(f) >= 1000 || (w[i].CanBeCasted() && f.Distance2D(fount.First()) >= 2000))
						&& (meepos[i].Distance2D(e) >= (e.AttackRange + 60) || (meepos[i].IsSilenced() || meepos[i].MovementSpeed <= 290))
						&& meepos[i].Distance2D(fount.First().Position) >= 1100
						&& Utils.SleepCheck(meepos[i].Handle.ToString() + "travel"))
					{
						travel.UseAbility(fount.First().Position);
						Utils.Sleep(1000, meepos[i].Handle.ToString() + "travel");
					}
				}
			}
			/**************************************************DODGE*************************************************************/


			/**************************************************COMBO*************************************************************/
			if (activated)
			{
				
				for (int i = 0; i < meepos.Count(); i++)
				{

					for (int j = 0; j < meepos.Count(); j++)
					{
						if (
						w[i] != null
						&& meepos[i].CanCast()
						&& meepos[j].Modifiers.Any(y => y.Name == "modifier_fountain_aura")
						&& (
							meepos[i].Handle != f.Handle && f.Modifiers.Any(y => y.Name == "modifier_fountain_aura_buff")
							|| meepos[i].Handle == f.Handle && !f.Modifiers.Any(y => y.Name == "modifier_fountain_aura_buff")
							)
						&& meepos.Count(x => x.Distance2D(meepos[j]) <= 1000) >1
						&& meepos[i].Health >= meepos[i].MaximumHealth * 0.8
						&& w[i].CanBeCasted()
						&& Utils.SleepCheck(meepos[i].Handle + "poof")
						)
						{
							w[i].UseAbility(target.Position);
							Utils.Sleep(250, meepos[i].Handle + "poof");
						}
						var W = me.Spellbook.SpellW;
						if (W!=null
							&& me.Modifiers.Any(y => y.Name == "modifier_fountain_aura_buff")
							&& me.Health >= me.MaximumHealth * 0.8
							&& meepos.Count(x => x.Distance2D(me) <= 1100) >= 2
							&& W.CanBeCasted()
							&& initMeepo.Distance2D(target)<= 1180
							&& Utils.SleepCheck(me.Handle + "pooff")
							)
						{
							W.UseAbility(target.Position);
							Utils.Sleep(250, me.Handle + "pooff");
						}
					}
					if (meepos[i].Health <= meepos[i].MaximumHealth * 0.58)
						return;
					/*int[] cool;
					var core = me.FindItem("item_octarine_core");
					if (core !=null)
						cool = new int[4] { 20, 16, 12, 8 };
					else
						cool = new int[4] { 15, 12, 9, 6 };*/
					target = ClosestToMouse(meepos[i]);
					if (target == null) return;
					initMeepo = GetClosestToTarget(meepos, target);
					blink = meepos[i].FindItem("item_blink");

					sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
					var ModifEther = target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
					if ( // sheep
						   sheep != null
						   && sheep.CanBeCasted()
						   && me.CanCast()
						   && !target.IsLinkensProtected()
						   && !target.IsMagicImmune()
						   && me.Distance2D(target) <= 1100
						   && Utils.SleepCheck("sheep")
						   )
					{
						sheep.UseAbility(target);
						Utils.Sleep(250, "sheep");
					} // sheep Item end
					if (// Dagon
						me.CanCast()
						&& dagon != null
						&& (ethereal == null
						|| (ModifEther
						|| ethereal.Cooldown < 17))
						&& !target.IsLinkensProtected()
						&& dagon.CanBeCasted()
						&& !target.IsMagicImmune()
						&& Utils.SleepCheck("dagon")
						)
					{
						dagon.UseAbility(target);
						Utils.Sleep(200, "dagon");
					} // Dagon Item end
					if (Utils.SleepCheck("Q") && !target.Modifiers.Any(y => y.Name == "modifier_meepo_earthbind"))
						if (meepos[i].Health >= meepos[i].MaximumHealth * 0.58 && q[i].CanBeCasted()
							&& (blink == null || !blink.CanBeCasted() || meepos[i].Distance2D(target) <= 400) && !meepos[i].IsChanneling()  && meepos[i].Distance2D(target) <= q[i].CastRange - 200 && Utils.SleepCheck(meepos[i].Handle.ToString() + "_net_casting"))
						{
							q[i].CastSkillShot(e);
							Utils.Sleep(q[i].GetCastDelay(meepos[i], e, true, true) + 1500, meepos[i].Handle.ToString() + "_net_casting");
							Utils.Sleep(2000, "Q");
						}

					if ((w[i].CanBeCasted() && meepos[i].Health >= meepos[i].MaximumHealth * 0.58)
						&& (meepos[i].Distance2D(target) <= 290) && (!q[i].CanBeCasted() || q == null || target.Modifiers.Any(y => y.Name == "modifier_meepo_earthbind"))
						&& ((meepos[i].Handle != f.Handle && f.Modifiers.Any(y => y.Name == "modifier_fountain_aura")) || !f.Modifiers.Any(y => y.Name == "modifier_fountain_aura"))
						&& Utils.SleepCheck(meepos[i].Handle.ToString() + "W"))
					{
						w[i].UseAbility(target.Position);
						Utils.Sleep(200, meepos[i].Handle.ToString() + "W");
					}
					if (
						blink != null
						&& me.CanCast()
						&& blink.CanBeCasted()
						&& me.Distance2D(target) >= 350
						&& me.Distance2D(target) <= 1150
						)
					{
						for (int j = 0; j < meepos.Count(); j++)
						{
							if (
							w[j] != null && meepos[j].Handle != me.Handle
							&& meepos[j].CanCast()
							&& (
							meepos[j].Handle != f.Handle && f.Modifiers.Any(y => y.Name == "modifier_fountain_aura_buff")
							|| !f.Modifiers.Any(y => y.Name == "modifier_fountain_aura_buff")
							)
							&& w[j].CanBeCasted()
							&& Utils.SleepCheck(meepos[j].Handle + "poof")
							)
							{
								w[j].UseAbility(target.Position);
								Utils.Sleep(250, meepos[j].Handle + "poof");
							}
						}


						var delay = Task.Delay(1370 - (int)Game.Ping).ContinueWith(_ =>
						{
							if (blink.CanBeCasted() && Utils.SleepCheck("12"))
							{
								blink.UseAbility(target.Position);
								Utils.Sleep(200, "12");
							}
						});
					}

					if (meepos[i].IsAttacking())
						return;
					if (initMeepo.Distance2D(meepos[i]) > 500 && (initMeepo.Distance2D(target) <= 350

						|| (blink != null && blink.CanBeCasted() && me.Distance2D(target) <= 1150))
						&& (
							meepos[i].Handle != f.Handle && f.Modifiers.Any(y => y.Name == "modifier_fountain_aura_buff")
							|| !f.Modifiers.Any(y => y.Name == "modifier_fountain_aura_buff")
							)
						&& w[i].CanBeCasted() && Utils.SleepCheck(meepos[i].Handle.ToString() + "W"))
					{
						w[i].UseAbility(target.Position);
						Utils.Sleep(200, meepos[i].Handle.ToString() + "W");
					}
					if (((
				   (!meepos[i].CanAttack() 
				   || meepos[i].Distance2D(target) >= 0) 
				   && meepos[i].NetworkActivity != NetworkActivity.Attack
				   && meepos[i].Distance2D(target) <= 1300)) 
				   && (meepos[i].Handle !=me.Handle && (blink.CanBeCasted() 
				   || me.Distance2D(target)<=350) || (meepos[i].Handle == me.Handle 
				   && !blink.CanBeCasted())) && Utils.SleepCheck(meepos[i].Handle.ToString() + "Move"))
					{
						meepos[i].Move(target.Predict(450));
						Utils.Sleep(250, meepos[i].Handle.ToString() + "Move");
					}
					else if (
					   meepos[i].Distance2D(target) <= 200 && (!meepos[i].IsAttackImmune() || !target.IsAttackImmune())
					   && meepos[i].NetworkActivity != NetworkActivity.Attack && meepos[i].CanAttack() && Utils.SleepCheck(meepos[i].Handle.ToString() + "Attack")
					   )
					{
						meepos[i].Attack(target);
						Utils.Sleep(200, meepos[i].Handle.ToString() + "Attack");
					}

				}

				if (shiva == null)
					shiva = me.FindItem("item_shivas_guard");

				dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

				if (vail == null)
					vail = me.FindItem("item_veil_of_discord");
				
					medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");

				if (ethereal == null)
					ethereal = me.FindItem("item_ethereal_blade");

				if (blink == null)
					blink = me.FindItem("item_blink");


				if (cheese == null)
					cheese = me.FindItem("item_cheese");


				if (abyssal == null)
					abyssal = me.FindItem("item_abyssal_blade");
				
				if (// Shiva Item
						shiva != null
						&& shiva.CanBeCasted()
						&& me.CanCast()
						&& !target.IsMagicImmune()
						&& Utils.SleepCheck("shiva")
						&& me.Distance2D(target) <= 600
						)

				{
					shiva.UseAbility();
					Utils.Sleep(250, "shiva");
				} // Shiva Item end
				if (
						// cheese
						cheese != null
						&& cheese.CanBeCasted()
						&& me.Health <= (me.MaximumHealth * 0.3)
						&& me.Distance2D(target) <= 700
						&& Utils.SleepCheck("cheese")
						)
				{
					cheese.UseAbility();
					Utils.Sleep(200, "cheese");
				} // cheese Item end
				if ( // vail
					vail != null
					&& vail.CanBeCasted()
					&& me.CanCast()
					&& !target.IsMagicImmune()
					&& me.Distance2D(target) <= 1100
					&& Utils.SleepCheck("vail")
					)
				{
					vail.UseAbility(target.Position);
					Utils.Sleep(250, "vail");
				} // orchid Item end
				if ( // atos Blade
						atos != null
						&& atos.CanBeCasted()
						&& me.CanCast()
						&& !target.IsLinkensProtected()
						&& !target.IsMagicImmune()
						&& me.Distance2D(target) <= 2000
						&& Utils.SleepCheck("atos")
						)
				{
					atos.UseAbility(target);
					Utils.Sleep(250, "atos");
				} // atos Item end
				if ( // Medall
					medall != null
					&& medall.CanBeCasted()
					&& Utils.SleepCheck("Medall")
					&& me.Distance2D(e) <= 700
					)
				{
					medall.UseAbility(e);
					Utils.Sleep(250, "Medall");
				} // Medall Item end
				if (me.Distance2D(target) <= 400 || !blink.CanBeCasted() || blink == null)
				{
					
					if ( // orchid
						orchid != null
						&& orchid.CanBeCasted()
						&& me.CanCast()
						&& !target.IsLinkensProtected()
						&& !target.IsMagicImmune()
						&& me.Distance2D(target) <= 900
						&& Utils.SleepCheck("orchid")
						)
					{
						orchid.UseAbility(target);
						Utils.Sleep(250, "orchid");
					} // orchid Item end

					

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
				}

			}
			/**************************************************COMBO*************************************************************/
		}


		/**************************************************Farm*************************************************************/



		/**************************************************Farm*************************************************************/









		/**************************************************Push*************************************************************/
		/**************************************************Push*************************************************************/

		static double GetDistance2D(Vector3 A, Vector3 B)
		{
			return Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
		}

		
		static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			not.Dispose();
		}
		
		
		private static void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectManager.LocalPlayer;
			if (player == null || player.Team == Team.Observer || me == null)
				return;

			if (me.ClassID != ClassID.CDOTA_Unit_Hero_Meepo)
				return;

			if (activated)
			{
				txt.DrawText(null, "Combo meepo's active", 1200, 12, Color.Green);
			}
			
            if (!dodge)
            {
                txt.DrawText(null, "Warning! Dodge unActive", 1200, 22, Color.DarkRed);
            }
			/*
			if (safe)
			{
				txt.DrawText(null, "Safe Meepo On", 1200, 30, Color.Green);
			}

			if (!safe)
			{
				txt.DrawText(null, "Safe Meepo Off  [" + safeMEEPO + "] ", 1200, 30, Color.DarkRed);
			}
			if (farm)
			{
				txt.DrawText(null, "Farm Meepo On", 1200, 32, Color.Green);
			}

			if (!farm)
			{
				txt.DrawText(null, "Farm Meepo Off", 1200, 32, Color.DarkRed);
			}
			if (push)
			{
				txt.DrawText(null, "Push Meepo On", 1200, 42, Color.Green);
			}

			if (!push)
			{
				txt.DrawText(null, "Push Meepo Off", 1200, 42, Color.DarkRed);
			}
			*/
		}

		static void Drawing_OnPostReset(EventArgs args)
		{
			txt.OnResetDevice();
			not.OnResetDevice();
		}

		static void Drawing_OnPreReset(EventArgs args)
		{
			txt.OnLostDevice();
			not.OnLostDevice();
		}
	}
}
