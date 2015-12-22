//ONLY LOVE MazaiPC ;) 
using System;
using System.Linq;
using System.Collections.Generic;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;

namespace Zetta
{
	internal class Program
	{
		private static bool activated;
		private static Font txt;
		private static Font not;
		private static Key KeyCombo = Key.D;
		private static bool loaded;
		private static Hero me;
		private static Hero target;
		private static ParticleEffect rangeDisplay;
		static void Main(string[] args)
		{
			Game.OnUpdate += Game_OnUpdate;
			Game.OnWndProc += Game_OnWndProc;
			Console.WriteLine("> Zetta# loaded!");

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
				   Height = 20,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.Default
			   });

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}

		public static void Game_OnUpdate(EventArgs args)
		{
			var me = ObjectMgr.LocalHero;
			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_ArcWarden)
			{
				return;
			}

			if (activated)
			{
				var target = me.ClosestToMouseTarget(2000);
				if (target == null)
				{
					return;
				}
				if (target.IsAlive && !target.IsInvul())
				{


					//spell
					
					Ability Q = me.Spellbook.SpellQ;
					
					Ability W = me.Spellbook.SpellW;
					
					Ability E = me.Spellbook.SpellE;
				
					Ability R = me.Spellbook.SpellR;
					// item 

					Item satanic = me.FindItem("item_satanic");

					Item shiva = me.FindItem("item_shivas_guard");
					Item dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

					
						Item arcane = me.FindItem("item_arcane_boots");
				
					Item mom = me.FindItem("item_mask_of_madness");
					
					Item medall = me.FindItem("item_shivas_guard") ?? me.FindItem("item_solar_crest");
				
					Item ethereal = me.FindItem("item_ethereal_blade");

					Item blink = me.FindItem("item_blink");
					
					Item soulring = me.FindItem("item_soul_ring");
				
					Item cheese = me.FindItem("item_cheese");
					Item halberd = me.FindItem("item_heavens_halberd");
					
					Item abyssal = me.FindItem("item_abyssal_blade");
					
					Item mjollnir = me.FindItem("item_mjollnir");

					var linkens = target.Modifiers.Any(x => x.Name == "modifier_item_spheretarget") || target.Inventory.Items.Any(x => x.Name == "item_sphere");
					var ModifEther = target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");





					if (me.Distance2D(target) <= 1200)
					{
						if (R.CanBeCasted()
						  && me.Position.Distance2D(target.Position) < me.AttackRange
						  && Utils.SleepCheck("R"))
						{
							R.UseAbility();
							Utils.Sleep(500, "R");
						}
						if (
							Q.CanBeCasted()
							&& blink.CanBeCasted()
							&& me.Position.Distance2D(target.Position) < 1150
							&& me.Position.Distance2D(target.Position) > 400
							&& Utils.SleepCheck("blink"))
						{
							blink.UseAbility(target.Position);
							Utils.Sleep(250, "blink");
						}
						if (
							Q.CanBeCasted()
							&& me.Position.Distance2D(target.Position)
							< 900
							&& Utils.SleepCheck("Q"))
						{
							Q.UseAbility(target);
							Utils.Sleep(150, "Q");
						}
						if (
							W.CanBeCasted()
							&& me.Position.Distance2D(target.Position) < 400
							&& target.NetworkActivity == NetworkActivity.Attack
							&& me.NetworkActivity != NetworkActivity.Move
							&& Utils.SleepCheck("W"))
						{
							W.UseAbility(me.Position);
							Utils.Sleep(150, "W");
						}
						if (E.CanBeCasted()
						  && me.Position.Distance2D(target.Position) < 1600
						  && Utils.SleepCheck("W"))
						{
							E.UseAbility(target.Predict(4000));
							Utils.Sleep(150, "W");
						}


						if (// Dagon
						dagon != null
						&& dagon.CanBeCasted() &&
						me.CanCast() &&
						!target.IsMagicImmune() &&
						Utils.SleepCheck("dagon")
					   )
						{
							dagon.UseAbility(target);
							Utils.Sleep(150, "dagon");
						} // Dagon Item end

						if (// SoulRing Item 
							soulring != null &&
							me.Health >= (me.MaximumHealth * 0.3) &&
							me.Mana <= dagon.ManaCost &&
							soulring.CanBeCasted())
						{
							soulring.UseAbility();
						} // SoulRing Item end

						if (// Arcane Boots Item
							arcane != null &&
							me.Mana <= Q.ManaCost &&
							arcane.CanBeCasted())
						{
							arcane.UseAbility();
						} // Arcane Boots Item end

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

						if ( // Medall
					   medall != null &&
					   medall.CanBeCasted() &&
					   me.CanCast() &&
					   Utils.SleepCheck("Medall") &&
					   me.Distance2D(target) <= 700
					   )
						{
							medall.UseAbility(target);
							Utils.Sleep(250, "Medall");
						} // Medall Item end

						if (// MOM
							mom != null &&
							mom.CanBeCasted() &&
							me.CanCast() &&
							(mom.CanBeCasted() &&
							Utils.SleepCheck("mom") &&
							me.Distance2D(target) <= 700)
							)
						{
							mom.UseAbility();
							Utils.Sleep(250, "mom");
						} // MOM Item end


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
							(halberd.CanBeCasted() &&
							Utils.SleepCheck("halberd") &&
							me.Distance2D(target) <= 700)
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
							me.Distance2D(target) <= 900
						   )
						{
							mjollnir.UseAbility(me);
							Utils.Sleep(250, "mjollnir");
						} // Mjollnir Item end

						if (// Satanic 
							satanic != null
							&& me.Health <= (me.MaximumHealth * 0.4)
							&& satanic.CanBeCasted()
							&& me.Distance2D(target) <= 700
							&& Utils.SleepCheck("Satanic")
							)
						{
							satanic.UseAbility();
							Utils.Sleep(350, "Satanic");
						} // Satanic Item end

						if (
							me.CanAttack()
							&& me.Distance2D(target) <= 1200
							&& Utils.SleepCheck("Attack")
							)
						{
							me.Attack(target);
							Utils.Sleep(350, "Attack");
						}



					}

				}

				var Illu = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden && x.IsIllusion)
				&& x.IsAlive && x.IsControllable);
				if (Illu == null)
				{
					return;
				}
				Hero e = ObjectMgr.GetEntities<Hero>()
				  .Where(x => x.IsAlive && x.Team != me.Team && !x.IsIllusion)
				  .OrderBy(x => GetDistance2D(x.Position, Illu.OrderBy(y => GetDistance2D(x.Position, y.Position)).FirstOrDefault().Position))
				  .FirstOrDefault();
				foreach (var v in Illu)
				{




					Ability qm = v.Spellbook.SpellQ;

					Ability wm = v.Spellbook.SpellW;

					Ability em = v.Spellbook.SpellE;

					Ability rm = v.Spellbook.SpellR;
					// item 

					Item sat = v.FindItem("item_satanic");

					Item shiv = v.FindItem("item_shivas_guard");

					Item dag = v.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));


					Item arc = v.FindItem("item_arcane_boots");

					Item mask = v.FindItem("item_mask_of_madness");

					Item medal = v.FindItem("item_shivas_guard") ?? v.FindItem("item_solar_crest");

					Item ether = v.FindItem("item_ethereal_blade");

					Item bl = v.FindItem("item_blink");

					Item soul = v.FindItem("item_soul_ring");

					Item chees = v.FindItem("item_cheese");

					Item halber = v.FindItem("item_heavens_halberd");

					Item abys = v.FindItem("item_abyssal_blade");

					Item mjoll = v.FindItem("item_mjollnir");




					if (rm.CanBeCasted()
								  && v.Position.Distance2D(e.Position) < v.AttackRange
								  && Utils.SleepCheck("rm"))
					{
						rm.UseAbility();
						Utils.Sleep(500, "rm");
					}
					if (
						qm.CanBeCasted()
						&& bl.CanBeCasted()
						&& v.Position.Distance2D(e.Position) < 1150
						&& v.Position.Distance2D(e.Position) > 400
						&& Utils.SleepCheck("bl"))
					{
						bl.UseAbility(e.Position);
						Utils.Sleep(250, "bl");
					}
					if (
						qm.CanBeCasted()
						&& v.Position.Distance2D(e.Position)
						< 900
						&& Utils.SleepCheck("qm"))
					{
						qm.UseAbility(e);
						Utils.Sleep(150, "qm");
					}
					if (
						wm.CanBeCasted()
						&& v.Position.Distance2D(e.Position) < 400
						&& e.NetworkActivity == NetworkActivity.Attack
						&& v.NetworkActivity != NetworkActivity.Move
						&& Utils.SleepCheck("wm"))
					{
						wm.UseAbility(v.Position);
						Utils.Sleep(150, "wm");
					}
					if (em.CanBeCasted()
					  && v.Position.Distance2D(e.Position) < 1600
					  && Utils.SleepCheck("wm"))
					{
						em.UseAbility(e.Predict(4000));
						Utils.Sleep(150, "wm");
					}


					if (// Dagon
					dag != null
					&& dag.CanBeCasted() &&
					v.CanCast() &&
					!e.IsMagicImmune() &&
					Utils.SleepCheck("dag")
				   )
					{
						dag.UseAbility(e);
						Utils.Sleep(150, "dag");
					} // Dagon Item end

					if (// SoulRing Item 
						soul != null &&
						v.Health >= (v.MaximumHealth * 0.3) &&
						v.Mana <= dag.ManaCost &&
						soul.CanBeCasted())
					{
						soul.UseAbility();
					} // SoulRing Item end

					if (// Arcane Boots Item
						arc != null &&
						v.Mana <= qm.ManaCost &&
						arc.CanBeCasted())
					{
						arc.UseAbility();
					} // Arcane Boots Item end

					if (// Shiva Item
						shiv != null &&
						shiv.CanBeCasted() &&
						v.CanCast() &&
						!e.IsMagicImmune() &&
						Utils.SleepCheck("shiv") &&
						v.Distance2D(e) <= 600
						)
					{
						shiv.UseAbility();
						Utils.Sleep(250, "shiv");
					} // Shiva Item end

					if ( // Medall
				   medal != null &&
				   medal.CanBeCasted() &&
				   v.CanCast() &&
				   Utils.SleepCheck("Medall") &&
				   v.Distance2D(e) <= 700
				   )
					{
						medal.UseAbility(e);
						Utils.Sleep(250, "Medall");
					} // Medall Item end

					if (// MOM
						mask != null &&
						mask.CanBeCasted() &&
						v.CanCast() &&
						(mask.CanBeCasted() &&
						Utils.SleepCheck("mask") &&
						v.Distance2D(e) <= 700)
						)
					{
						mask.UseAbility();
						Utils.Sleep(250, "mask");
					} // MOM Item end


					if ( // Abyssal Blade
						abys != null &&
						abys.CanBeCasted() &&
						v.CanCast() &&
						!e.IsMagicImmune() &&
						Utils.SleepCheck("abys") &&
						v.Distance2D(e) <= 400
						)
					{
						abys.UseAbility(e);
						Utils.Sleep(250, "abys");
					} // Abyssal Item end

					if ( // Hellbard
						halber != null &&
						halber.CanBeCasted() &&
						v.CanCast() &&
						!e.IsMagicImmune() &&
						(halber.CanBeCasted() &&
						Utils.SleepCheck("halber") &&
						v.Distance2D(e) <= 700)
						)
					{
						halber.UseAbility(e);
						Utils.Sleep(250, "halber");
					} // Hellbard Item end

					if ( // Mjollnir
						mjoll != null &&
						mjoll.CanBeCasted() &&
						v.CanCast() &&
						!e.IsMagicImmune() &&
						Utils.SleepCheck("mjoll") &&
						v.Distance2D(e) <= 900
					   )
					{
						mjoll.UseAbility(v);
						Utils.Sleep(250, "mjoll");
					} // Mjollnir Item end

					if (// Satanic 
						sat != null
						&& v.Health <= (v.MaximumHealth * 0.4)
						&& sat.CanBeCasted()
						&& v.Distance2D(e) <= 700
						&& Utils.SleepCheck("Satanic")
						)
					{
						sat.UseAbility();
						Utils.Sleep(350, "Satanic");
					} // Satanic Item end

					if (
						v.CanAttack()
						&& v.Distance2D(e) <= 1200
						&& Utils.SleepCheck("Attack")
						)
					{
						v.Attack(e);
						Utils.Sleep(350, "Attack");
					}

					if (e.Position.Distance2D(v.Position) < 1550 &&
						Utils.SleepCheck(v.Handle.ToString()))
					{
						v.Attack(e);
						Utils.Sleep(400, v.Handle.ToString());
					}
				}
			}
		}
		


		static double GetDistance2D(Vector3 A, Vector3 B)
		{
			return Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
		}


		private static void Game_OnWndProc(WndEventArgs args)
		{
			if (!Game.IsChatOpen)
			{
				if (Game.IsKeyDown(KeyCombo))
				{
					activated = true;
				}
				else
				{
					activated = false;
				}



			}
		}

		static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			not.Dispose();
		}

		static void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectMgr.LocalPlayer;
			var me = ObjectMgr.LocalHero;
			if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_ArcWarden)
				return;

			if (activated)
			{
				txt.DrawText(null, "Zetta#: Comboing!", 4, 150, Color.Green);
			}

			if (!activated)
			{
				txt.DrawText(null, "Zetta#: go combo  [" + KeyCombo + "] for toggle combo", 4, 150, Color.Aqua);
			}


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



