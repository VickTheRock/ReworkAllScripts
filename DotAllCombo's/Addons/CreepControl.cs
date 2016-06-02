using System.Security.Permissions;

namespace DotaAllCombo.Addons
{
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using SharpDX;
	using System;
	using System.Linq;
	using SharpDX.Direct3D9;
	using Service;

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    public class CreepControl : IAddon
	{
        private Item midas, abyssal, mjollnir, boots, medall, mom;
		private Font txt;
		private Font not;
		private Hero me, target;

		public void Load()
		{
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

			OnLoadMessage();
		}

		public void Unload()
		{
			AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
			Drawing.OnEndScene -= Drawing_OnEndScene;
			Drawing.OnPostReset -= Drawing_OnPostReset;
			Drawing.OnPreReset -= Drawing_OnPreReset;
		}
		
		public void RunScript()
		{

			me = ObjectManager.LocalHero;
			if (!MainMenu.CCMenu.Item("controll").IsActive() || !Game.IsInGame || me == null || Game.IsPaused ||
				Game.IsChatOpen) return;


			target = me.ClosestToMouseTarget(10000);

			var Units = ObjectManager.GetEntities<Unit>().Where(creep =>
				(creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
				|| creep.ClassID == ClassID.CDOTA_BaseNPC_Additive
				|| creep.ClassID == ClassID.CDOTA_BaseNPC_Tusk_Sigil
				|| creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
				|| creep.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem
				|| creep.ClassID == ClassID.CDOTA_BaseNPC_Creep
				|| creep.ClassID == ClassID.CDOTA_Unit_VisageFamiliar
				|| creep.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalEarth
				|| creep.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalStorm
				|| creep.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalFire
				|| creep.ClassID == ClassID.CDOTA_NPC_WitchDoctor_Ward
				|| creep.ClassID == ClassID.CDOTA_Unit_Hero_Beastmaster_Boar
				|| creep.ClassID == ClassID.CDOTA_Unit_SpiritBear
				|| creep.ClassID == ClassID.CDOTA_BaseNPC_Venomancer_PlagueWard
				|| creep.ClassID == ClassID.CDOTA_BaseNPC_ShadowShaman_SerpentWard
				|| creep.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling
				)
				&& creep.IsAlive
				&& creep.Team == me.Team
				&& creep.IsControllable).ToList();

			if (Utils.SleepCheck("delay"))
			{
				if (me.IsAlive)
				{
					var count = Units.Count();
                    if(count==0)return;
					for (int i = 0; i < count; ++i)
					{
						var v = ObjectManager.GetEntities<Hero>()
										  .Where(x => x.Team == me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion).ToList();
						if (me.ClassID == ClassID.CDOTA_Unit_Hero_Juggernaut)
						{
							if (Units[i].Name == "npc_dota_juggernaut_healing_ward")

							{
								if (me.Position.Distance2D(Units[i].Position) > 5 && Utils.SleepCheck(Units[i].Handle.ToString()))
								{
									Units[i].Move(me.Position);
									Utils.Sleep(50, Units[i].Handle.ToString());
								}
							}
						}
						else
						if (Units[i].Name == "npc_dota_neutral_ogre_magi")
						{
							for (int z = 0; z < v.Count(); z++)
							{
								var armor = Units[i].Spellbook.SpellQ;

								if ((!v[z].HasModifier("modifier_ogre_magi_frost_armor") || !me.HasModifier("modifier_ogre_magi_frost_armor")) && armor.CanBeCasted() && Units[i].Position.Distance2D(v[z]) <= 900
									&& Utils.SleepCheck(Units[i].Handle.ToString()))
								{
									armor.UseAbility(v[z]);
									Utils.Sleep(400, Units[i].Handle.ToString());
								}
							}
						}
						else
						if (Units[i].Name == "npc_dota_neutral_forest_troll_high_priest")
						{


							if (Units[i].Spellbook.SpellQ.CanBeCasted())
							{
								for (int z = 0; z < v.Count(); z++)
								{
									if (v[z].Health <= (v[z].MaximumHealth * 0.9) && Units[i].Position.Distance2D(v[z]) <= 900
									&& Utils.SleepCheck(Units[i].Handle.ToString() + "high_priest"))
									{
										Units[i].Spellbook.SpellQ.UseAbility(v[z]);
										Utils.Sleep(350, Units[i].Handle.ToString() + "high_priest");
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
							if (Units[i].Name == "npc_dota_neutral_dark_troll_warlord")
							{
								if (target.Position.Distance2D(Units[i].Position) < 550 && (!CheckSetka || !CheckStun || !target.IsHexed() || !target.IsStunned()) && Units[i].Spellbook.SpellQ.CanBeCasted() &&
										 Utils.SleepCheck(Units[i].Handle.ToString() + "warlord"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(target);
									Utils.Sleep(450, Units[i].Handle.ToString() + "warlord");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_big_thunder_lizard")
							{
								if (target.Position.Distance2D(Units[i].Position) < 250 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle.ToString() + "lizard"))
								{
									Units[i].Spellbook.SpellQ.UseAbility();
									Utils.Sleep(450, Units[i].Handle.ToString() + "lizard");
								}
								if (target.Position.Distance2D(Units[i].Position) < 550 && Units[i].Spellbook.SpellW.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle.ToString() + "lizard"))
								{
									Units[i].Spellbook.SpellW.UseAbility();
									Utils.Sleep(450, Units[i].Handle.ToString() + "lizard");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_centaur_khan")
							{
								if (target.Position.Distance2D(Units[i].Position) < 200 && (!CheckSetka || !CheckStun || !target.IsHexed() || !target.IsStunned()) && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle.ToString() + "centaur"))
								{
									Units[i].Spellbook.SpellQ.UseAbility();
									Utils.Sleep(450, Units[i].Handle.ToString() + "centaur");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_satyr_hellcaller")
							{
								if (target.Position.Distance2D(Units[i].Position) < 850 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle.ToString() + "satyr"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(target);
									Utils.Sleep(350, Units[i].Handle.ToString() + "satyr");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_satyr_trickster")
							{
								if (target.Position.Distance2D(Units[i].Position) < 850 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle.ToString() + "satyr_trickster"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(target);
									Utils.Sleep(350, Units[i].Handle.ToString() + "satyr_trickster");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_satyr_soulstealer")
							{
								if (target.Position.Distance2D(Units[i].Position) < 850 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle.ToString() + "satyrsoulstealer"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(target);
									Utils.Sleep(350, Units[i].Handle.ToString() + "satyrsoulstealer");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_black_dragon")
							{
								if (target.Position.Distance2D(Units[i].Position) < 700 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle.ToString() + "dragonspawn"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(target.Predict(600));
									Utils.Sleep(350, Units[i].Handle.ToString() + "dragonspawn");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_big_thunder_lizard")
							{
								if (target.Position.Distance2D(Units[i].Position) < 200 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle.ToString() + "lizard"))
								{
									Units[i].Spellbook.SpellQ.UseAbility();
									Utils.Sleep(350, Units[i].Handle.ToString() + "lizard");
								}

								for (int z = 0; z < v.Count(); z++)
								{
									if (Units[i].Spellbook.SpellW.CanBeCasted() && Units[i].Position.Distance2D(v[z]) <= 900)
									{
										if (target.Position.Distance2D(v[z]) < v[z].AttackRange + 150 &&
										Utils.SleepCheck(Units[i].Handle.ToString() + "lizard"))
										{
											Units[i].Spellbook.SpellW.UseAbility(v[z]);
											Utils.Sleep(350, Units[i].Handle.ToString() + "lizard");
										}
									}
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_mud_golem")
							{
								if (target.Position.Distance2D(Units[i].Position) < 850 && (!CheckSetka || !CheckStun || !target.IsHexed() || !target.IsStunned())
									&& Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle.ToString() + "golem"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(target);
									Utils.Sleep(350, Units[i].Handle.ToString() + "golem");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_polar_furbolg_ursa_warrior")
							{
								if (target.Position.Distance2D(Units[i].Position) < 240 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle.ToString() + "ursa"))
								{
									Units[i].Spellbook.SpellQ.UseAbility();
									Utils.Sleep(350, Units[i].Handle.ToString() + "ursa");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_harpy_storm")
							{
								if (target.Position.Distance2D(Units[i].Position) < 900 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
										Utils.SleepCheck(Units[i].Handle.ToString() + "harpy"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(target);
									Utils.Sleep(350, Units[i].Handle.ToString() + "harpy");
								}
							}
							else
							if (me.ClassID == ClassID.CDOTA_Unit_Hero_Tusk)
							{
								if (Units[i].ClassID == ClassID.CDOTA_BaseNPC_Tusk_Sigil)
								{
									if (target.Position.Distance2D(Units[i].Position) < 1550 &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
									{
										Units[i].Move(target.Predict(1500));
										Utils.Sleep(700, Units[i].Handle.ToString());
									}
								}
							}
							else
							if (Units[i].ClassID == ClassID.CDOTA_BaseNPC_Creep)
							{
								if (Units[i].Name == "npc_dota_necronomicon_archer")
								{
									if (target.Position.Distance2D(Units[i].Position) <= 700 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
										Utils.SleepCheck(Units[i].Handle.ToString()))

									{
										Units[i].Spellbook.SpellQ.UseAbility(target);
										Utils.Sleep(300, Units[i].Handle.ToString());
									}
								}
							}
							else
							if (me.ClassID == ClassID.CDOTA_Unit_Hero_Visage)
							{
								if (Units[i].ClassID == ClassID.CDOTA_Unit_VisageFamiliar)
								{
									var damageModif = Units[i].Modifiers.FirstOrDefault(x => x.Name == "modifier_visage_summon_familiars_damage_charge");


									if (target.Position.Distance2D(Units[i].Position) < 1550 && Units[i].Health < 6 && Units[i].Spellbook.Spell1.CanBeCasted() &&
										Utils.SleepCheck(Units[i].Handle.ToString()))
									{
										Units[i].Spellbook.Spell1.UseAbility();
										Utils.Sleep(200, Units[i].Handle.ToString());
									}

									if (target.Position.Distance2D(Units[i].Position) < 340 && ((damageModif.StackCount < 1) && !target.IsStunned()) && Units[i].Spellbook.Spell1.CanBeCasted() &&
										Utils.SleepCheck(Units[i].Handle.ToString()))
									{
										Units[i].Spellbook.Spell1.UseAbility();
										Utils.Sleep(350, Units[i].Handle.ToString());
									}
								}
							}
							else
							if (me.ClassID == ClassID.CDOTA_Unit_Hero_Brewmaster)
							{
								if (Units[i].ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalEarth)
								{
									if (target.Position.Distance2D(Units[i].Position) < 1300 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
										Utils.SleepCheck(Units[i].Handle.ToString()))
									{
										Units[i].Spellbook.SpellQ.UseAbility(target);
										Utils.Sleep(400, Units[i].Handle.ToString());
									}
									if (target.Position.Distance2D(Units[i].Position) < 340 && Units[i].Spellbook.SpellR.CanBeCasted() &&
									   Utils.SleepCheck(Units[i].Handle.ToString()))
									{
										Units[i].Spellbook.SpellR.UseAbility();
										Utils.Sleep(400, Units[i].Handle.ToString());
									}
								}
								else
								if (Units[i].ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalStorm)
								{
									if (Units != null)
									{
										if (target.Position.Distance2D(Units[i].Position) < 700 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											Units[i].Spellbook.SpellQ.UseAbility(target.Position);
											Utils.Sleep(400, Units[i].Handle.ToString());
										}
										if (target.Position.Distance2D(Units[i].Position) < 900 && Units[i].Spellbook.SpellE.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											Units[i].Spellbook.SpellE.UseAbility();
											Utils.Sleep(400, Units[i].Handle.ToString());
										}
										if (target.Position.Distance2D(Units[i].Position) < 850 && Units[i].Spellbook.SpellR.CanBeCasted() &&
											   Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											Units[i].Spellbook.SpellR.UseAbility(target);
											Utils.Sleep(400, Units[i].Handle.ToString());
										}
									}
								}
							}
							else
							if (me.ClassID == ClassID.CDOTA_Unit_Hero_LoneDruid)
							{
								if (Units[i].ClassID == ClassID.CDOTA_Unit_SpiritBear)
								{
									if ((!me.AghanimState() && me.Position.Distance2D(Units[i]) <= 1200) || me.AghanimState())
									{
										abyssal = Units[i].FindItem("item_abyssal_blade");

										mjollnir = Units[i].FindItem("item_mjollnir");

										boots = Units[i].FindItem("item_phase_boots");

										midas = Units[i].FindItem("item_hand_of_midas");

										mom = Units[i].FindItem("item_mask_of_madness");

										medall = Units[i].FindItem("item_medallion_of_courage") ?? Units[i].FindItem("item_solar_crest");


										if (boots != null && target.Position.Distance2D(Units[i].Position) < 1550 && boots.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											boots.UseAbility();
											Utils.Sleep(350, Units[i].Handle.ToString());
										}
										if (mjollnir != null && target.Position.Distance2D(Units[i].Position) < 525 && mjollnir.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											mjollnir.UseAbility(Units[i]);
											Utils.Sleep(350, Units[i].Handle.ToString());
										}
										if (medall != null && target.Position.Distance2D(Units[i].Position) < 525 && medall.CanBeCasted() &&
										   Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											medall.UseAbility(target);
											Utils.Sleep(350, Units[i].Handle.ToString());
										}

										if (mom != null && target.Position.Distance2D(Units[i].Position) < 525 && mom.CanBeCasted() &&
										   Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											mom.UseAbility();
											Utils.Sleep(350, Units[i].Handle.ToString());
										}
										if (abyssal != null && target.Position.Distance2D(Units[i].Position) < 500 && abyssal.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											abyssal.UseAbility(target);
											Utils.Sleep(350, Units[i].Handle.ToString());
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
									}
								}
							}
							else
							if (me.ClassID == ClassID.CDOTA_Unit_Hero_TemplarAssassin)
							{
								if (Units[i].ClassID == ClassID.CDOTA_BaseNPC_Additive)
								{
									if (Units[i].Name == "npc_dota_templar_assassin_psionic_trap")
									{

										if (target.Position.Distance2D(Units[i].Position) < 250
											&& Units[i].Spellbook.Spell1.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											Units[i].Spellbook.Spell1.UseAbility();
											Utils.Sleep(250, Units[i].Handle.ToString());
										}
									}
								}
							}
							
							if (Units[i].Distance2D(target) <= Units[i].AttackRange + 100 && (!Units[i].IsAttackImmune() || !target.IsAttackImmune())
							&& Units[i].NetworkActivity != NetworkActivity.Attack && Units[i].CanAttack() && Utils.SleepCheck(Units[i].Handle.ToString() + "Attack")
							)
							{
								Units[i].Attack(target);
								Utils.Sleep(250, Units[i].Handle.ToString() + "Attack");
							}
							else if ((!Units[i].CanAttack() || Units[i].Distance2D(target) >= 0) && Units[i].NetworkActivity != NetworkActivity.Attack
								&& Units[i].CanMove() &&
									Units[i].Distance2D(target) <= 600 && Utils.SleepCheck(Units[i].Handle.ToString() + "Move")
								)
							{
								Units[i].Move(target.Predict(300));
								Utils.Sleep(350, Units[i].Handle.ToString() + "Move");
							}
						}
						Utils.Sleep(500, "delay");
					}
				}
			}
		}


		void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			not.Dispose();
		}

		void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;
            
			if (MainMenu.CCMenu.Item("controll").IsActive())
			{
				txt.DrawText(null, "Creep Control On", 1200, 27, Color.Coral);
			}

			if (!MainMenu.CCMenu.Item("controll").IsActive())
			{
				txt.DrawText(null, "Creep Control Off", 1200, 27, Color.Coral);
			}
		}



		void Drawing_OnPostReset(EventArgs args)
		{
			txt.OnResetDevice();
			not.OnResetDevice();
		}

		void Drawing_OnPreReset(EventArgs args)
		{
			txt.OnLostDevice();
			not.OnLostDevice();
		}

		private void OnLoadMessage()
		{
			Game.PrintMessage("<font face='verdana' color='#ffa420'>@addon CreepControl is Loaded!</font>", MessageType.LogMessage);
			Service.Debug.Print.ConsoleMessage.Encolored("@addon CreepControl is Loaded!", ConsoleColor.Yellow);
		}
	}
}
