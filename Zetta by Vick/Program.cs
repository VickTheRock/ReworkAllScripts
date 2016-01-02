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
		private static Key KeyCombo = Key.G;
		private static bool loaded;
		private static Boolean ModifEther, stoneModif;
        private static Hero me;
		private static Hero target;
		private static ParticleEffect rangeDisplay;
		static void Main(string[] args)
		{
			Game.OnUpdate += Game_OnUpdate;
			Game.OnUpdate += Game_MidasHero;
			Game.OnUpdate += Game_MidasIllu;
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

			var Illu = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden && x.IsIllusion) && x.IsAlive && x.IsControllable).ToList();



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

					Item necronomicon = me.FindItem("item_necronomicon")?? me.FindItem("item_necronomicon_2") ?? me.FindItem("item_necronomicon_3");

					Item mjollnir = me.FindItem("item_mjollnir");

					Item manta = me.FindItem("item_manta");

					//Boolean linkens = target.IsLinkensProtected();
					Boolean ModifEther = target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
					Boolean stoneModif = target.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");

		


					if (me.Distance2D(target) <= 1200)
					{
						if (R != null
						&& R.CanBeCasted()
						  && me.Position.Distance2D(target.Position) < me.AttackRange
						  && Utils.SleepCheck("R"))
						{
							R.UseAbility();
							Utils.Sleep(500, "R");
						}
						if (
							Q != null
						&& blink != null
						&& Q.CanBeCasted()
							&& blink.CanBeCasted()
							&& me.Position.Distance2D(target.Position) < 1150
							&& me.Position.Distance2D(target.Position) > 400
							&& Utils.SleepCheck("blink"))
						{
							blink.UseAbility(target.Position);
							Utils.Sleep(250, "blink");
						}
						if (Q != null
						    && Q.CanBeCasted()
							&& me.Position.Distance2D(target.Position)
							< 900
							&& Utils.SleepCheck("Q"))
						{
							Q.UseAbility(target);
							Utils.Sleep(150, "Q");
						}
						if (
							W != null
						    && W.CanBeCasted()
							&& me.Position.Distance2D(target.Position) < 400
							&& target.NetworkActivity == NetworkActivity.Attack
							&& me.NetworkActivity != NetworkActivity.Move
							&& Utils.SleepCheck("W"))
						{
							W.UseAbility(me.Position);
							Utils.Sleep(150, "W");
						}
						if (
							E != null
						  && E.CanBeCasted()
						  && me.Position.Distance2D(target.Position) < 1600
						  && Utils.SleepCheck("W"))
						{
							E.UseAbility(target.Predict(3000));
							Utils.Sleep(150, "W");
						}
						if (// ethereal
									   ethereal != null
									   && ethereal.CanBeCasted()
									   && me.CanCast()
									   && !target.IsLinkensProtected()
									   && !target.IsMagicImmune()
									   && !stoneModif
									   && Utils.SleepCheck("ethereal")
									  )
						{
							ethereal.UseAbility(target);
							Utils.Sleep(200, "ethereal");
						} // ethereal Item end

						if (// Dagon
									me.CanCast()
									&& dagon != null
									&& (ethereal == null
									|| (ModifEther
									|| ethereal.Cooldown < 17))
									&& !target.IsLinkensProtected()
									&& dagon.CanBeCasted()
									&& !target.IsMagicImmune()
									&& !stoneModif
									&& Utils.SleepCheck("dagon")
								   )
						{
							dagon.UseAbility(target);
							Utils.Sleep(150, "dagon");
						} // Dagon Item end

						if (
							manta.CanBeCasted()
							&& me.Position.Distance2D(target.Position)
							< me.AttackRange+50
							&& Utils.SleepCheck("manta"))
						{
							manta.UseAbility();
							Utils.Sleep(150, "manta");
						}

						if (// SoulRing Item 
							soulring != null &&
							me.Health >= (me.MaximumHealth * 0.3) &&
							me.Mana <= W.ManaCost + 150 &&
							soulring.CanBeCasted())
						{
							soulring.UseAbility();
						} // SoulRing Item end

						if (// Arcane Boots Item
							arcane != null &&
							me.Mana <= E.ManaCost+150 &&
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

						if (// necronomicon Item
							necronomicon != null &&
							necronomicon.CanBeCasted() &&
							me.CanCast() &&
							!target.IsMagicImmune() &&
							Utils.SleepCheck("necronomicon") &&
							me.Distance2D(target) <= 600
							)
						{
							necronomicon.UseAbility();
							Utils.Sleep(250, "necronomicon");
						} // necronomicon Item end
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
							Utils.SleepCheck("mom") &&
							me.Distance2D(target) <= 700
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

					Item necro = v.FindItem("item_necronomicon")?? v.FindItem("item_necronomicon_2")?? v.FindItem("item_necronomicon_3");

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

					Item manta = v.FindItem("item_manta");


					if (
						rm != null
						&& rm.CanBeCasted()
						&& v.Position.Distance2D(e.Position) < v.AttackRange
						 && Utils.SleepCheck("rm"))
					{
						rm.UseAbility();
						Utils.Sleep(500, "rm");
					}
					if (
						qm != null
						&& bl != null
						&& qm.CanBeCasted()
						&& bl.CanBeCasted()
						&& v.Position.Distance2D(e.Position) < 1150
						&& v.Position.Distance2D(e.Position) > 400
						&& Utils.SleepCheck("bl"))
					{
						bl.UseAbility(e.Position);
						Utils.Sleep(250, "bl");
					}
					if (
						qm != null
						&& qm.CanBeCasted()
						&& v.Position.Distance2D(e.Position)
						< 900
						&& Utils.SleepCheck("qm"))
					{
						qm.UseAbility(e);
						Utils.Sleep(150, "qm");
					}
					if (
						wm != null
					    && wm.CanBeCasted()
						&& v.Position.Distance2D(e.Position) < 400
						&& e.NetworkActivity == NetworkActivity.Attack
						&& v.NetworkActivity != NetworkActivity.Move
						&& Utils.SleepCheck("wm"))
					{
						wm.UseAbility(v.Position);
						Utils.Sleep(150, "wm");
					}
					if (
						em != null
					  && em.CanBeCasted()
					  && v.Position.Distance2D(e.Position) < 1600
					  && Utils.SleepCheck("wm"))
					{
						em.UseAbility(e.Predict(3000));
						Utils.Sleep(150, "wm");
					}
					if (// Dagon
									me.CanCast()
									&& dag != null
									&& (ether == null
									|| (ModifEther
									|| ether.Cooldown < 17))
									&& !target.IsLinkensProtected()
									&& dag.CanBeCasted()
									&& !target.IsMagicImmune()
									&& !stoneModif
									&& Utils.SleepCheck("dagon")
								   )
					{
						dag.UseAbility(e);
						Utils.Sleep(150, "dag");
					} // Dagon Item end


					if (// ethereal
									   ether != null
									   && ether.CanBeCasted()
									   && !ModifEther
									   && me.CanCast()
									   && !target.IsLinkensProtected()
									   && !target.IsMagicImmune()
									   && !stoneModif
									   && Utils.SleepCheck("ethereal")
									  )
					{
						ether.UseAbility(target);
						Utils.Sleep(200, "ethereal");
					} // ethereal Item end

					if (// SoulRing Item 
						soul != null &&
						v.Health >= (v.MaximumHealth * 0.3) &&
						v.Mana <= wm.ManaCost + 150 &&
						soul.CanBeCasted())
					{
						soul.UseAbility();
					} // SoulRing Item end

					if (// Arcane Boots Item
						arc != null &&
						v.Mana <= wm.ManaCost+150 &&
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
					if (
							manta != null
							&& manta.CanBeCasted()
							&& v.Position.Distance2D(e.Position)
							< v.AttackRange + 50
							&& Utils.SleepCheck("manta"))
					{
						manta.UseAbility();
						Utils.Sleep(150, "manta");
					}
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
						Utils.SleepCheck("mask") &&
						v.Distance2D(e) <= 700
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
						Utils.SleepCheck("halber") &&
						v.Distance2D(e) <= 700
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

					if (// necronomicon Item
							necro != null &&
							necro.CanBeCasted() &&
							me.CanCast() &&
							!target.IsMagicImmune() &&
							Utils.SleepCheck("necronomicon") &&
							me.Distance2D(target) <= 600
							)
					{
						necro.UseAbility();
						Utils.Sleep(250, "necronomicon");
					} // necronomicon Item end

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

		private static void Game_MidasHero(EventArgs args)
		{
			var me = ObjectMgr.LocalHero;
			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_ArcWarden)
			{
				return;
			}



			var MyHero = ObjectMgr.GetEntities<Hero>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden) && x.IsAlive && x.IsControllable && !x.IsIllusion).ToList();
			Creep creepHeroPos = ObjectMgr.GetEntities<Creep>()
				  .Where(x => ((x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
				  || x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
				  || x.ClassID == ClassID.CDOTA_BaseNPC_Creep) && x.Team == me.GetEnemyTeam() || (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral && x.Team != me.Team))
				  && x.IsAlive && x.IsVisible && x.IsSpawned)
				  .OrderBy(x => GetDistance2D(x.Position, MyHero.OrderBy(y => GetDistance2D(x.Position, y.Position)).FirstOrDefault().Position))
				  .FirstOrDefault();
			Item midas = me.FindItem("item_hand_of_midas");
			if (MyHero != null)
			{
				foreach (var f in MyHero)
				{
					if (midas.CanBeCasted())
					{
						if (creepHeroPos.Position.Distance2D(f.Position) < 650 && midas.CanBeCasted() &&
							Utils.SleepCheck(f.Handle.ToString()))
						{
							midas.UseAbility(creepHeroPos);
							Utils.Sleep(500, f.Handle.ToString());
						}
						return;
					}
				}
			}
		}

		private static void Game_MidasIllu(EventArgs args)
		{
			var me = ObjectMgr.LocalHero;
			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_ArcWarden)
			{
				return;
			}

			

			var Illu = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden && x.IsIllusion) && x.IsAlive && x.IsControllable);
			Creep creep = ObjectMgr.GetEntities<Creep>()
				  .Where(x => ((x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
				  || x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
				  || x.ClassID == ClassID.CDOTA_BaseNPC_Creep) && x.Team == me.GetEnemyTeam() || (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral && x.Team != me.Team))
				  && x.IsAlive && x.IsVisible)
				  .OrderBy(x => GetDistance2D(x.Position, Illu.OrderBy(y => GetDistance2D(x.Position, y.Position)).FirstOrDefault().Position))
				  .FirstOrDefault();
			if (Illu != null)
			{

				foreach (var f in Illu)
				{
					Item midaS = f.FindItem("item_hand_of_midas");
					if (midaS.CanBeCasted())
					{
						if (creep.Position.Distance2D(f.Position) < 650 &&
							Utils.SleepCheck(f.Handle.ToString()))
						{
							midaS.UseAbility(creep);
							Utils.Sleep(500, f.Handle.ToString());
						}
						return;
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



