using System;
using System.Linq;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;
using Ensage.Common.Menu;
namespace ControlCreep_By_Vick
{
	internal class Program
	{

		private static bool activated;
		private static Item midas, abyssal, mjollnir, boots, medall, mom;
		private static Font txt;
		private static Font not;
		private static Hero me, target;
		private static readonly Menu Menu = new Menu("ControlCreep's", "ControlCreep's", true, "item_helm_of_the_dominator", true);

		static void Main(string[] args)
		{
			Game.OnUpdate += Game_OnUpdate;
			Console.WriteLine("> ControlCreep By Vick# loaded!");

			txt = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Tahoma",
				   Height = 11,
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
			Menu.AddItem(new MenuItem("controll", "controll").SetValue(new KeyBind('L', KeyBindType.Toggle)));
			Menu.AddToMainMenu();
		}




		public static void Game_OnUpdate(EventArgs args)
		{

			me = ObjectManager.LocalHero;
			if (!Menu.Item("controll").GetValue<KeyBind>().Active || !Game.IsInGame || me == null || Game.IsPaused ||
				Game.IsChatOpen) return;


			target = me.ClosestToMouseTarget(10000);

			if (Utils.SleepCheck("delay"))
			{


				if (me.IsAlive)
				{

					var Additive = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Additive)
					&& x.IsAlive && x.IsControllable && x.Team == me.Team).ToList();
					if (me.ClassID == ClassID.CDOTA_Unit_Hero_Juggernaut)
					{
						for (int i = 0; i < Additive.Count(); i++)
						{
							if (Additive[i].Name == "npc_dota_juggernaut_healing_ward")

							{

								if (me.Position.Distance2D(Additive[i].Position) > 5 && Utils.SleepCheck(Additive[i].Handle.ToString()))
								{
									Additive[i].Move(me.Position);
									Utils.Sleep(50, Additive[i].Handle.ToString());
								}
							}
						}
					}

				}
				var Neutral = ObjectManager.GetEntities<Creep>().Where(creep => creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral &&
									 creep.IsAlive && creep.Team == me.Team && creep.IsControllable).ToList();

				for (int i = 0; i < Neutral.Count(); i++)
				{
					var v =
							   ObjectManager.GetEntities<Hero>()
								   .Where(x => x.Team == me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion).ToList();

					if (Neutral[i].Name == "npc_dota_neutral_ogre_magi")
					{


						for (int z = 0; z < v.Count(); z++)
						{
							var armor = Neutral[i].Spellbook.SpellQ;

							if (!v[z].Modifiers.Any(y => y.Name == "modifier_ogre_magi_frost_armor") && armor.CanBeCasted() && Neutral[i].Position.Distance2D(v[z]) <= 900
								&& Utils.SleepCheck(Neutral[i].Handle.ToString()))
							{
								armor.UseAbility(v[z]);
								Utils.Sleep(400, Neutral[i].Handle.ToString());
							}
						}
					}
					if (Neutral[i].Name == "npc_dota_neutral_forest_troll_high_priest")
					{

						for (int z = 0; z < v.Count(); z++)
						{
							if (Neutral[i].Spellbook.SpellQ.CanBeCasted() && Neutral[i].Position.Distance2D(v[z]) <= 900)
							{
								if (v[z].Health <= (v[z].MaximumHealth * 0.9)
									&& Utils.SleepCheck(Neutral[i].Handle.ToString() + "high_priest"))
								{
									Neutral[i].Spellbook.SpellQ.UseAbility(v[z]);
									Utils.Sleep(350, Neutral[i].Handle.ToString() + "high_priest");
								}
							}
						}
					}
				}

				if (target == null)
					return;




				if (target.IsAlive && !target.IsInvul() && (Game.MousePosition.Distance2D(target) <= 1000 || target.Distance2D(me) <= 600))
				{

					//spell
					var CheckStun = target.HasModifier("modifier_centaur_hoof_stomp");
					var CheckSetka = target.HasModifier("modifier_dark_troll_warlord_ensnare");


					for (int i = 0; i < Neutral.Count(); i++)
					{


						if (Neutral[i].Name == "npc_dota_neutral_dark_troll_warlord")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 550 && (!CheckSetka || !CheckStun || !target.IsHexed() || !target.IsStunned()) && Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
									 Utils.SleepCheck(Neutral[i].Handle.ToString() + "warlord"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility(target);
								Utils.Sleep(450, Neutral[i].Handle.ToString() + "warlord");
							}
						}
						else
						if (Neutral[i].Name == "npc_dota_neutral_big_thunder_lizard")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 250 && Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(Neutral[i].Handle.ToString() + "lizard"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility();
								Utils.Sleep(450, Neutral[i].Handle.ToString() + "lizard");
							}
							if (target.Position.Distance2D(Neutral[i].Position) < 550 && Neutral[i].Spellbook.SpellW.CanBeCasted() &&
								Utils.SleepCheck(Neutral[i].Handle.ToString() + "lizard"))
							{
								Neutral[i].Spellbook.SpellW.UseAbility();
								Utils.Sleep(450, Neutral[i].Handle.ToString() + "lizard");
							}
						}
						else
						if (Neutral[i].Name == "npc_dota_neutral_centaur_khan")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 200 && (!CheckSetka || !CheckStun || !target.IsHexed() || !target.IsStunned()) && Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(Neutral[i].Handle.ToString() + "centaur"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility();
								Utils.Sleep(450, Neutral[i].Handle.ToString() + "centaur");
							}
						}
						else
						if (Neutral[i].Name == "npc_dota_neutral_satyr_hellcaller")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 850 && Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(Neutral[i].Handle.ToString() + "satyr"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility(target);
								Utils.Sleep(350, Neutral[i].Handle.ToString() + "satyr");
							}
						}
						else
						if (Neutral[i].Name == "npc_dota_neutral_satyr_trickster")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 850 && Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(Neutral[i].Handle.ToString() + "satyr_trickster"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility(target);
								Utils.Sleep(350, Neutral[i].Handle.ToString() + "satyr_trickster");
							}
						}
						else
						if (Neutral[i].Name == "npc_dota_neutral_satyr_soulstealer")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 850 && Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(Neutral[i].Handle.ToString() + "satyrsoulstealer"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility(target);
								Utils.Sleep(350, Neutral[i].Handle.ToString() + "satyrsoulstealer");
							}
						}
						else
						if (Neutral[i].Name == "npc_dota_neutral_black_dragon")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 700 && Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(Neutral[i].Handle.ToString() + "dragonspawn"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility(target.Predict(600));
								Utils.Sleep(350, Neutral[i].Handle.ToString() + "dragonspawn");
							}
						}
						else
						if (Neutral[i].Name == "npc_dota_neutral_big_thunder_lizard")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 200 && Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(Neutral[i].Handle.ToString() + "lizard"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility();
								Utils.Sleep(350, Neutral[i].Handle.ToString() + "lizard");
							}
							var v =
								ObjectManager.GetEntities<Hero>()
									.Where(x => x.Team == me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion).ToList();
							for (int z = 0; z < v.Count(); z++)
							{
								if (Neutral[i].Spellbook.SpellW.CanBeCasted() && Neutral[i].Position.Distance2D(v[z]) <= 900)
								{
									if (target.Position.Distance2D(v[z]) < v[z].AttackRange + 150 &&
									Utils.SleepCheck(Neutral[i].Handle.ToString() + "lizard"))
									{
										Neutral[i].Spellbook.SpellW.UseAbility(v[z]);
										Utils.Sleep(350, Neutral[i].Handle.ToString() + "lizard");
									}
								}
							}
						}
						else
						if (Neutral[i].Name == "npc_dota_neutral_mud_golem")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 850 && (!CheckSetka || !CheckStun || !target.IsHexed() || !target.IsStunned())
								&& Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(Neutral[i].Handle.ToString() + "golem"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility(target);
								Utils.Sleep(350, Neutral[i].Handle.ToString() + "golem");
							}
						}
						else
						if (Neutral[i].Name == "npc_dota_neutral_polar_furbolg_ursa_warrior")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 240 && Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(Neutral[i].Handle.ToString() + "ursa"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility();
								Utils.Sleep(350, Neutral[i].Handle.ToString() + "ursa");
							}
						}
						else
						if (Neutral[i].Name == "npc_dota_neutral_harpy_storm")
						{
							if (target.Position.Distance2D(Neutral[i].Position) < 900 && Neutral[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Neutral[i].Handle.ToString() + "harpy"))
							{
								Neutral[i].Spellbook.SpellQ.UseAbility(target);
								Utils.Sleep(350, Neutral[i].Handle.ToString() + "harpy");
							}
						}

						if (Neutral[i].Distance2D(target) <= Neutral[i].AttackRange + 100 && (!Neutral[i].IsAttackImmune() || !target.IsAttackImmune())
						&& Neutral[i].NetworkActivity != NetworkActivity.Attack && Neutral[i].CanAttack() && Utils.SleepCheck(Neutral[i].Handle.ToString() + "Attack")
						)
						{
							Neutral[i].Attack(target);
							Utils.Sleep(150, Neutral[i].Handle.ToString() + "Attack");
						}
						else if ((!Neutral[i].CanAttack() || Neutral[i].Distance2D(target) >= 0) && Neutral[i].NetworkActivity != NetworkActivity.Attack &&
								Neutral[i].Distance2D(target) <= 600 && Utils.SleepCheck(Neutral[i].Handle.ToString() + "Move")
							)
						{
							Neutral[i].Move(target.Predict(300));
							Utils.Sleep(250, Neutral[i].Handle.ToString() + "Move");
						}

					}
					if (me.ClassID == ClassID.CDOTA_Unit_Hero_Tusk)
					{
						var Sigl = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Tusk_Sigil)
							&& x.IsAlive && x.IsControllable).ToList();
						for (int i = 0; i < Sigl.Count(); i++)
						{

							if (target.Position.Distance2D(Sigl[i].Position) < 1550 &&
									Utils.SleepCheck(Sigl[i].Handle.ToString()))
							{
								Sigl[i].Move(target.Predict(1500));
								Utils.Sleep(700, Sigl[i].Handle.ToString());
							}
						}
					}

					if (me.ClassID == ClassID.CDOTA_Unit_Hero_Invoker)
					{
						var InvForgeds = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit)
							&& x.IsAlive && x.IsControllable).ToList();
						for (int i = 0; i < InvForgeds.Count(); i++)
						{
							if (target.Position.Distance2D(InvForgeds[i].Position) < 1550 &&
									Utils.SleepCheck(InvForgeds[i].Handle.ToString()))
							{
								InvForgeds[i].Attack(target);
								Utils.Sleep(300, InvForgeds[i].Handle.ToString());
							}
						}
					}

					if (me.ClassID == ClassID.CDOTA_Unit_Hero_Warlock)
					{
						var WarlockGolem = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem)
							&& x.IsAlive && x.IsControllable).ToList();
						for (int i = 0; i < WarlockGolem.Count(); i++)
						{

							if (target.Position.Distance2D(WarlockGolem[i].Position) < 1550 &&
								   Utils.SleepCheck(WarlockGolem[i].Handle.ToString() + "WarlockGolem"))
							{
								WarlockGolem[i].Attack(target);
								Utils.Sleep(450, WarlockGolem[i].Handle.ToString() + "WarlockGolem");
							}
						}
					}
					var BaseNPC = ObjectManager.GetEntities<Creep>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Creep)
							&& x.IsAlive && x.IsControllable).ToList();
					for (int i = 0; i < BaseNPC.Count(); i++)
					{
						if (BaseNPC[i].Name == "npc_dota_necronomicon_archer")
						{
							if (target.Position.Distance2D(BaseNPC[i].Position) <= 700 && BaseNPC[i].Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(BaseNPC[i].Handle.ToString()))

							{
								BaseNPC[i].Spellbook.SpellQ.UseAbility(target);
								Utils.Sleep(300, BaseNPC[i].Handle.ToString());
							}
						}
						if (target.Position.Distance2D(BaseNPC[i].Position) < 1550 &&
						   Utils.SleepCheck(BaseNPC[i].Handle.ToString()))
						{
							BaseNPC[i].Attack(target);
							Utils.Sleep(300, BaseNPC[i].Handle.ToString());
						}
					}


					if (me.ClassID == ClassID.CDOTA_Unit_Hero_Visage)
					{
						var Familliar = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_VisageFamiliar && x.Team == me.Team)
					  && x.IsAlive && x.IsControllable).ToList();

						for (int i = 0; i < Familliar.Count(); i++)
						{
							var damageModif = Familliar[i].Modifiers.FirstOrDefault(x => x.Name == "modifier_visage_summon_familiars_damage_charge");


							if (target.Position.Distance2D(Familliar[i].Position) < 1550 && Familliar[i].Health < 6 && Familliar[i].Spellbook.Spell1.CanBeCasted() &&
								Utils.SleepCheck(Familliar[i].Handle.ToString()))
							{
								Familliar[i].Spellbook.Spell1.UseAbility();
								Utils.Sleep(200, Familliar[i].Handle.ToString());
							}

							if (target.Position.Distance2D(Familliar[i].Position) < 340 && ((damageModif.StackCount < 1) && !target.IsStunned()) && Familliar[i].Spellbook.Spell1.CanBeCasted() &&
								Utils.SleepCheck(Familliar[i].Handle.ToString()))
							{
								Familliar[i].Spellbook.Spell1.UseAbility();
								Utils.Sleep(350, Familliar[i].Handle.ToString());
							}
							if (Familliar[i].Distance2D(target) <= Familliar[i].AttackRange + 100 && (!Familliar[i].IsAttackImmune() || !target.IsAttackImmune())
							&& Familliar[i].NetworkActivity != NetworkActivity.Attack && Familliar[i].CanAttack() && Utils.SleepCheck(Familliar[i].Handle.ToString() + "Attack")
							)
							{
								Familliar[i].Attack(target);
								Utils.Sleep(150, Familliar[i].Handle.ToString() + "Attack");
							}
							else
								if ((!Familliar[i].CanAttack() || Familliar[i].Distance2D(target) >= 0) && Familliar[i].NetworkActivity != NetworkActivity.Attack &&
									Familliar[i].Distance2D(target) <= 600 && Utils.SleepCheck(Familliar[i].Handle.ToString() + "Move")
								)
							{
								Familliar[i].Move(target.Predict(300));
								Utils.Sleep(250, Familliar[i].Handle.ToString() + "Move");
							}

						}
					}

					if (me.ClassID == ClassID.CDOTA_Unit_Hero_Brewmaster)
					{
						var primalearth = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => (x.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalEarth && x.Team == me.Team)
							&& x.IsAlive && x.IsControllable);

						if (primalearth != null)
						{
							if (target.Position.Distance2D(primalearth.Position) < 1300 && primalearth.Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(primalearth.Handle.ToString()))
							{
								primalearth.Spellbook.SpellQ.UseAbility(target);
								Utils.Sleep(400, primalearth.Handle.ToString());
							}
							if (target.Position.Distance2D(primalearth.Position) < 340 && primalearth.Spellbook.SpellR.CanBeCasted() &&
							   Utils.SleepCheck(primalearth.Handle.ToString()))
							{
								primalearth.Spellbook.SpellR.UseAbility();
								Utils.Sleep(400, primalearth.Handle.ToString());
							}
							if (primalearth.Distance2D(target) <= primalearth.AttackRange + 100 && (!primalearth.IsAttackImmune() || !target.IsAttackImmune())
							&& primalearth.NetworkActivity != NetworkActivity.Attack && primalearth.CanAttack() && Utils.SleepCheck(primalearth.Handle.ToString() + "Attack")
							)
							{
								primalearth.Attack(target);
								Utils.Sleep(150, primalearth.Handle.ToString() + "Attack");
							}
							else
								if ((!primalearth.CanAttack() || primalearth.Distance2D(target) >= 0) && primalearth.NetworkActivity != NetworkActivity.Attack &&
									primalearth.Distance2D(target) <= 600 && Utils.SleepCheck(primalearth.Handle.ToString() + "Move")
								)
							{
								primalearth.Move(target.Predict(300));
								Utils.Sleep(250, primalearth.Handle.ToString() + "Move");
							}
						}


						var primalstorm = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => (x.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalStorm && x.Team == me.Team)
							   && x.IsAlive && x.IsControllable);
						if (primalstorm != null)
						{
							if (target.Position.Distance2D(primalstorm.Position) < 700 && primalstorm.Spellbook.SpellQ.CanBeCasted() &&
								Utils.SleepCheck(primalstorm.Handle.ToString()))
							{
								primalstorm.Spellbook.SpellQ.UseAbility(target.Position);
								Utils.Sleep(400, primalstorm.Handle.ToString());
							}
							if (target.Position.Distance2D(primalstorm.Position) < 900 && primalstorm.Spellbook.SpellE.CanBeCasted() &&
								Utils.SleepCheck(primalstorm.Handle.ToString()))
							{
								primalstorm.Spellbook.SpellE.UseAbility();
								Utils.Sleep(400, primalstorm.Handle.ToString());
							}
							if (target.Position.Distance2D(primalstorm.Position) < 850 && primalstorm.Spellbook.SpellR.CanBeCasted() &&
								   Utils.SleepCheck(primalstorm.Handle.ToString()))
							{
								primalstorm.Spellbook.SpellR.UseAbility(target);
								Utils.Sleep(400, primalstorm.Handle.ToString());
							}
							if (primalstorm.Distance2D(target) <= primalstorm.AttackRange + 100 && (!primalstorm.IsAttackImmune() || !target.IsAttackImmune())
							&& primalstorm.NetworkActivity != NetworkActivity.Attack && primalstorm.CanAttack() && Utils.SleepCheck(primalstorm.Handle.ToString() + "Attack")
							)
							{
								primalstorm.Attack(target);
								Utils.Sleep(150, primalstorm.Handle.ToString() + "Attack");
							}
							else
								if ((!primalstorm.CanAttack() || primalstorm.Distance2D(target) >= 0) && primalstorm.NetworkActivity != NetworkActivity.Attack &&
									primalstorm.Distance2D(target) <= 600 && Utils.SleepCheck(primalstorm.Handle.ToString() + "Move")
								)
							{
								primalstorm.Move(target.Predict(300));
								Utils.Sleep(250, primalstorm.Handle.ToString() + "Move");
							}
						}


						var primalfire = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => (x.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalFire && x.Team == me.Team)
							   && x.IsAlive && x.IsControllable);
						if (primalfire != null)
						{
							if (primalfire.Distance2D(target) <= primalfire.AttackRange + 100 && (!primalfire.IsAttackImmune() || !target.IsAttackImmune())
							&& primalfire.NetworkActivity != NetworkActivity.Attack && primalfire.CanAttack() && Utils.SleepCheck(primalfire.Handle.ToString() + "Attack")
							)
							{
								primalfire.Attack(target);
								Utils.Sleep(150, primalfire.Handle.ToString() + "Attack");
							}
							else
								if ((!primalfire.CanAttack() || primalfire.Distance2D(target) >= 0) && primalfire.NetworkActivity != NetworkActivity.Attack &&
									primalfire.Distance2D(target) <= 600 && Utils.SleepCheck(primalfire.Handle.ToString() + "Move")
								)
							{
								primalfire.Move(target.Predict(300));
								Utils.Sleep(250, primalfire.Handle.ToString() + "Move");
							}
						}
					}





					if (me.ClassID == ClassID.CDOTA_Unit_Hero_Beastmaster)
					{
						var boar = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_beastmaster_boar" && unit.IsAlive && unit.IsControllable).ToList();
						if (boar != null)
						{
							for (int i = 0; i < boar.Count(); i++)
							{

								if (target.Position.Distance2D(boar[i].Position) < 1550 &&
									Utils.SleepCheck(boar[i].Handle.ToString()))
								{
									boar[i].Attack(target);
									Utils.Sleep(700, boar[i].Handle.ToString());
								}
							}
						}
					}
					if (me.ClassID == ClassID.CDOTA_Unit_Hero_WitchDoctor)
					{
						var Ward = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => (x.ClassID == ClassID.CDOTA_NPC_WitchDoctor_Ward)
							   && x.IsAlive && x.IsControllable);
						if (Ward != null)
						{
							if (target.Position.Distance2D(Ward.Position) < 700 &&
								Utils.SleepCheck(Ward.Handle.ToString()))
							{
								Ward.Attack(target);
								Utils.Sleep(350, Ward.Handle.ToString());
							}
						}
					}
					if (me.ClassID == ClassID.CDOTA_Unit_Hero_ShadowShaman)
					{
						var SerpentWard = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_ShadowShaman_SerpentWard)
						&& x.IsAlive && x.IsControllable).ToList();
						if (SerpentWard != null)
						{
							for (int i = 0; i < SerpentWard.Count(); i++)
							{
								if (target.Position.Distance2D(SerpentWard[i].Position) < 650 &&
									Utils.SleepCheck(SerpentWard[i].Handle.ToString()))
								{
									SerpentWard[i].Attack(target);
									Utils.Sleep(700, SerpentWard[i].Handle.ToString());
								}
							}
						}
					}

					if (me.ClassID == ClassID.CDOTA_Unit_Hero_LoneDruid)
					{
						var spiritbear = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => (x.ClassID == ClassID.CDOTA_Unit_SpiritBear)
						   && x.IsAlive && x.IsControllable);
						if (spiritbear != null)
						{
							if ((!me.AghanimState() && me.Position.Distance2D(spiritbear) <= 1200) || me.AghanimState())
							{
								abyssal = spiritbear.FindItem("item_abyssal_blade");

								mjollnir = spiritbear.FindItem("item_mjollnir");

								boots = spiritbear.FindItem("item_phase_boots");

								midas = spiritbear.FindItem("item_hand_of_midas");

								mom = spiritbear.FindItem("item_mask_of_madness");

								medall = spiritbear.FindItem("item_medallion_of_courage") ?? spiritbear.FindItem("item_solar_crest");


								if (boots != null && target.Position.Distance2D(spiritbear.Position) < 1550 && boots.CanBeCasted() &&
									Utils.SleepCheck(spiritbear.Handle.ToString()))
								{
									boots.UseAbility();
									Utils.Sleep(350, spiritbear.Handle.ToString());
								}
								if (mjollnir != null && target.Position.Distance2D(spiritbear.Position) < 525 && mjollnir.CanBeCasted() &&
									Utils.SleepCheck(spiritbear.Handle.ToString()))
								{
									mjollnir.UseAbility(spiritbear);
									Utils.Sleep(350, spiritbear.Handle.ToString());
								}

								if (medall != null && target.Position.Distance2D(spiritbear.Position) < 525 && medall.CanBeCasted() &&
								   Utils.SleepCheck(spiritbear.Handle.ToString()))
								{
									medall.UseAbility(target);
									Utils.Sleep(350, spiritbear.Handle.ToString());
								}

								if (mom != null && target.Position.Distance2D(spiritbear.Position) < 525 && mom.CanBeCasted() &&
								   Utils.SleepCheck(spiritbear.Handle.ToString()))
								{
									mom.UseAbility();
									Utils.Sleep(350, spiritbear.Handle.ToString());
								}
								if (abyssal != null && target.Position.Distance2D(spiritbear.Position) < 500 && abyssal.CanBeCasted() &&
									Utils.SleepCheck(spiritbear.Handle.ToString()))
								{
									abyssal.UseAbility(target);
									Utils.Sleep(350, spiritbear.Handle.ToString());
								}
								if (midas != null)
								{
									var Neutrals = ObjectManager.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep &&
										creep.IsAlive && creep.IsVisible && creep.IsSpawned) && creep.Team != me.Team).ToList();

									foreach (var f in Neutrals)
									{
										if (target.Position.Distance2D(f.Position) < 650 && midas.CanBeCasted() &&
											Utils.SleepCheck(f.Handle.ToString()))
										{
											midas.UseAbility(f);
											Utils.Sleep(350, f.Handle.ToString());
										}
									}
								}
								if (target.Position.Distance2D(spiritbear.Position) < 1550 &&
								   Utils.SleepCheck(spiritbear.Handle.ToString()))
								{
									spiritbear.Attack(target);
									Utils.Sleep(350, spiritbear.Handle.ToString());
								}
							}
						}
					}

					if (me.ClassID == ClassID.CDOTA_Unit_Hero_TemplarAssassin)
					{
						var Additive = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Additive)
					   && x.IsAlive && x.IsControllable && x.Team == me.Team).ToList();

						for (int i = 0; i < Additive.Count(); i++)
						{
							if (Additive[i].Name == "npc_dota_templar_assassin_psionic_trap")
							{

								if (target.Position.Distance2D(Additive[i].Position) < 250
									&& Additive[i].Spellbook.Spell1.CanBeCasted() &&
									Utils.SleepCheck(Additive[i].Handle.ToString()))
								{
									Additive[i].Spellbook.Spell1.UseAbility();
									Utils.Sleep(250, Additive[i].Handle.ToString());
								}
							}
						}
					}
				}
				Utils.Sleep(500, "delay");
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

			var player = ObjectManager.LocalPlayer;


			if (Menu.Item("controll").GetValue<KeyBind>().Active)
			{
				txt.DrawText(null, "Creep Control On", 1200, 27, Color.Gold);
			}

			if (!Menu.Item("controll").GetValue<KeyBind>().Active)
			{
				txt.DrawText(null, "Creep Control Off", 1200, 27, Color.Gold);
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
