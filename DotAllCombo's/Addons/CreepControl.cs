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
		private Hero me, e;

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


			e = me.ClosestToMouseTarget(10000);

			var Units = ObjectManager.GetEntities<Unit>().Where(creep =>
				(creep.NetworkName == "CDOTA_BaseNPC_Creep_Neutral"
				|| creep.Name == "npc_dota_base_additive"
				|| creep.Name == "npc_dota_tusk_frozen_sigil"
				|| creep.Name == "npc_dota_invoker_forged_spirit"
				|| creep.Name == "npc_dota_warlock_golem"
				|| creep.NetworkName == "CDOTA_BaseNPC_Creep"
				|| creep.Name == "npc_dota_visage_familiar"
				|| creep.Name == "npc_dota_brewmaster_earth"
				|| creep.Name == "npc_dota_brewmaster_storm"
				|| creep.Name == "npc_dota_brewmaster_fire"
				|| creep.Name == "npc_dota_witch_doctor_death_ward"
				|| creep.Name == "npc_dota_beastmaster_boar"
				|| creep.Name == "npc_dota_lone_druid_bear"
				|| creep.Name == "npc_dota_venomancer_plagueward"
				|| creep.Name == "npc_dota_shadowshaman_serpentward"
				|| creep.Name == "npc_dota_broodmother_spiderling"
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
						if (me.Name == "npc_dota_hero_juggernaut")
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
							for (int z = 0; z < v.Count(); ++z)
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
								for (int z = 0; z < v.Count(); ++z)
								{
									if (v[z].Health <= (v[z].MaximumHealth * 0.9) && Units[i].Position.Distance2D(v[z]) <= 900
									&& Utils.SleepCheck(Units[i].Handle+"high_priest"))
									{
										Units[i].Spellbook.SpellQ.UseAbility(v[z]);
										Utils.Sleep(350, Units[i].Handle+"high_priest");
									}
								}
							}
						}


						if (e == null)
							return;




						if (e.IsAlive && !e.IsInvul() && (Game.MousePosition.Distance2D(e) <= 1000 || e.Distance2D(me) <= 600))
						{

							//spell
							var CheckStun = e.HasModifier("modifier_centaur_hoof_stomp");
							var CheckSetka = e.HasModifier("modifier_dark_troll_warlord_ensnare");
							if (Units[i].Name == "npc_dota_neutral_dark_troll_warlord")
							{
								if (e.Position.Distance2D(Units[i].Position) < 550 && (!CheckSetka || !CheckStun || !e.IsHexed() || !e.IsStunned()) && Units[i].Spellbook.SpellQ.CanBeCasted() &&
										 Utils.SleepCheck(Units[i].Handle+"warlord"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(e);
									Utils.Sleep(450, Units[i].Handle+"warlord");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_big_thunder_lizard")
							{
								if (e.Position.Distance2D(Units[i].Position) < 250 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle+"lizard"))
								{
									Units[i].Spellbook.SpellQ.UseAbility();
									Utils.Sleep(450, Units[i].Handle+"lizard");
								}
								if (e.Position.Distance2D(Units[i].Position) < 550 && Units[i].Spellbook.SpellW.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle+"lizard"))
								{
									Units[i].Spellbook.SpellW.UseAbility();
									Utils.Sleep(450, Units[i].Handle+"lizard");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_centaur_khan")
							{
								if (e.Position.Distance2D(Units[i].Position) < 200 && (!CheckSetka || !CheckStun || !e.IsHexed() || !e.IsStunned()) && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle+"centaur"))
								{
									Units[i].Spellbook.SpellQ.UseAbility();
									Utils.Sleep(450, Units[i].Handle+"centaur");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_satyr_hellcaller")
							{
								if (e.Position.Distance2D(Units[i].Position) < 850 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle+"satyr"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(e);
									Utils.Sleep(350, Units[i].Handle+"satyr");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_satyr_trickster")
							{
								if (e.Position.Distance2D(Units[i].Position) < 850 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle+"satyr_trickster"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(e);
									Utils.Sleep(350, Units[i].Handle+"satyr_trickster");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_satyr_soulstealer")
							{
								if (e.Position.Distance2D(Units[i].Position) < 850 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle+"satyrsoulstealer"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(e);
									Utils.Sleep(350, Units[i].Handle+"satyrsoulstealer");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_black_dragon")
							{
								if (e.Position.Distance2D(Units[i].Position) < 700 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle+"dragonspawn"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(e.Predict(600));
									Utils.Sleep(350, Units[i].Handle+"dragonspawn");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_big_thunder_lizard")
							{
								if (e.Position.Distance2D(Units[i].Position) < 200 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle+"lizard"))
								{
									Units[i].Spellbook.SpellQ.UseAbility();
									Utils.Sleep(350, Units[i].Handle+"lizard");
								}

								for (int z = 0; z < v.Count(); z++)
								{
									if (Units[i].Spellbook.SpellW.CanBeCasted() && Units[i].Position.Distance2D(v[z]) <= 900)
									{
										if (e.Position.Distance2D(v[z]) < v[z].AttackRange + 150 &&
										Utils.SleepCheck(Units[i].Handle+"lizard"))
										{
											Units[i].Spellbook.SpellW.UseAbility(v[z]);
											Utils.Sleep(350, Units[i].Handle+"lizard");
										}
									}
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_mud_golem")
							{
								if (e.Position.Distance2D(Units[i].Position) < 850 && (!CheckSetka || !CheckStun || !e.IsHexed() || !e.IsStunned())
									&& Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle+"golem"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(e);
									Utils.Sleep(350, Units[i].Handle+"golem");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_polar_furbolg_ursa_warrior")
							{
								if (e.Position.Distance2D(Units[i].Position) < 240 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
									Utils.SleepCheck(Units[i].Handle+"ursa"))
								{
									Units[i].Spellbook.SpellQ.UseAbility();
									Utils.Sleep(350, Units[i].Handle+"ursa");
								}
							}
							else
								if (Units[i].Name == "npc_dota_neutral_harpy_storm")
							{
								if (e.Position.Distance2D(Units[i].Position) < 900 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
										Utils.SleepCheck(Units[i].Handle+"harpy"))
								{
									Units[i].Spellbook.SpellQ.UseAbility(e);
									Utils.Sleep(350, Units[i].Handle+"harpy");
								}
							}
							else
							if (me.Name == "npc_dota_hero_tusk")
							{
								if (Units[i].Name == "npc_dota_tusk_frozen_sigil")
								{
									if (e.Position.Distance2D(Units[i].Position) < 1550 &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
									{
										Units[i].Move(e.Predict(1500));
										Utils.Sleep(700, Units[i].Handle.ToString());
									}
								}
							}
							else
							if (Units[i].Name == "npc_dota_necronomicon_archer")
								{
									if (e.Position.Distance2D(Units[i].Position) <= 700 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
										Utils.SleepCheck(Units[i].Handle.ToString()))

									{
										Units[i].Spellbook.SpellQ.UseAbility(e);
										Utils.Sleep(300, Units[i].Handle.ToString());
									}
							}
							else
							if (me.Name == "npc_dota_hero_visage")
							{
								if (Units[i].Name == "npc_dota_visage_familiar")
								{
									var damageModif = Units[i].Modifiers.FirstOrDefault(x => x.Name == "modifier_visage_summon_familiars_damage_charge");


									if (e.Position.Distance2D(Units[i].Position) < 1550 && Units[i].Health < 6 && Units[i].Spellbook.Spell1.CanBeCasted() &&
										Utils.SleepCheck(Units[i].Handle.ToString()))
									{
										Units[i].Spellbook.Spell1.UseAbility();
										Utils.Sleep(200, Units[i].Handle.ToString());
									}

									if (e.Position.Distance2D(Units[i].Position) < 340 && ((damageModif.StackCount < 1) && !e.IsStunned()) && Units[i].Spellbook.Spell1.CanBeCasted() &&
										Utils.SleepCheck(Units[i].Handle.ToString()))
									{
										Units[i].Spellbook.Spell1.UseAbility();
										Utils.Sleep(350, Units[i].Handle.ToString());
									}
								}
							}
							else
							if (me.Name == "npc_dota_hero_brewmaster")
							{
								if (Units[i].Name == "npc_dota_brewmaster_earth")
								{
									if (e.Position.Distance2D(Units[i].Position) < 1300 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
										Utils.SleepCheck(Units[i].Handle.ToString()))
									{
										Units[i].Spellbook.SpellQ.UseAbility(e);
										Utils.Sleep(400, Units[i].Handle.ToString());
									}
									if (e.Position.Distance2D(Units[i].Position) < 340 && Units[i].Spellbook.SpellR.CanBeCasted() &&
									   Utils.SleepCheck(Units[i].Handle.ToString()))
									{
										Units[i].Spellbook.SpellR.UseAbility();
										Utils.Sleep(400, Units[i].Handle.ToString());
									}
								}
								else
								if (Units[i].Name == "npc_dota_brewmaster_storm")
								{
									if (Units != null)
									{
										if (e.Position.Distance2D(Units[i].Position) < 700 && Units[i].Spellbook.SpellQ.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											Units[i].Spellbook.SpellQ.UseAbility(e.Position);
											Utils.Sleep(400, Units[i].Handle.ToString());
										}
										if (e.Position.Distance2D(Units[i].Position) < 900 && Units[i].Spellbook.SpellE.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											Units[i].Spellbook.SpellE.UseAbility();
											Utils.Sleep(400, Units[i].Handle.ToString());
										}
										if (e.Position.Distance2D(Units[i].Position) < 850 && Units[i].Spellbook.SpellR.CanBeCasted() &&
											   Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											Units[i].Spellbook.SpellR.UseAbility(e);
											Utils.Sleep(400, Units[i].Handle.ToString());
										}
									}
								}
							}
							else
							if (me.Name == "npc_dota_hero_lone_druid")
							{
								if (Units[i].Name == "npc_dota_lone_druid_bear")
								{
									if ((!me.AghanimState() && me.Position.Distance2D(Units[i]) <= 1200) || me.AghanimState())
									{
										abyssal = Units[i].FindItem("item_abyssal_blade");

										mjollnir = Units[i].FindItem("item_mjollnir");

										boots = Units[i].FindItem("item_phase_boots");

										midas = Units[i].FindItem("item_hand_of_midas");

										mom = Units[i].FindItem("item_mask_of_madness");

										medall = Units[i].FindItem("item_medallion_of_courage") ?? Units[i].FindItem("item_solar_crest");


										if (boots != null && e.Position.Distance2D(Units[i].Position) < 1550 && boots.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											boots.UseAbility();
											Utils.Sleep(350, Units[i].Handle.ToString());
										}
										if (mjollnir != null && e.Position.Distance2D(Units[i].Position) < 525 && mjollnir.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											mjollnir.UseAbility(Units[i]);
											Utils.Sleep(350, Units[i].Handle.ToString());
										}
										if (medall != null && e.Position.Distance2D(Units[i].Position) < 525 && medall.CanBeCasted() &&
										   Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											medall.UseAbility(e);
											Utils.Sleep(350, Units[i].Handle.ToString());
										}

										if (mom != null && e.Position.Distance2D(Units[i].Position) < 525 && mom.CanBeCasted() &&
										   Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											mom.UseAbility();
											Utils.Sleep(350, Units[i].Handle.ToString());
										}
										if (abyssal != null && e.Position.Distance2D(Units[i].Position) < 500 && abyssal.CanBeCasted() &&
											Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											abyssal.UseAbility(e);
											Utils.Sleep(350, Units[i].Handle.ToString());
										}
										if (midas != null)
										{
											var neutrals = ObjectManager.GetEntities<Creep>().Where(creep => (creep.NetworkName == "CDOTA_BaseNPC_Creep_Lane" || creep.NetworkName == "CDOTA_BaseNPC_Creep_Siege" || creep.NetworkName == "CDOTA_BaseNPC_Creep_Neutral" || creep.Name == "npc_dota_invoker_forged_spirit" || creep.NetworkName == "CDOTA_BaseNPC_Creep" &&
												creep.IsAlive && creep.IsVisible && creep.IsSpawned) && creep.Team != me.Team).ToList();

											foreach (var f in neutrals)
											{
												if (e.Position.Distance2D(f.Position) < 650 && midas.CanBeCasted() &&
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
							if (me.Name == "npc_dota_hero_templar_assassin")
							{
								if (Units[i].Name == "npc_dota_templar_assassin_psionic_trap")
									{

										if (e.Position.Distance2D(Units[i].Position) < 250
											&& Units[i].Spellbook.Spell1.CanBeCasted()
											&& e.Distance2D(Game.MousePosition)<=1000
											&& Utils.SleepCheck(Units[i].Handle.ToString()))
										{
											Units[i].Spellbook.Spell1.UseAbility();
											Utils.Sleep(250, Units[i].Handle.ToString());
										}
									}
							}
							
							if (Units[i].Distance2D(e) <= Units[i].AttackRange + 100 && (!Units[i].IsAttackImmune() || !e.IsAttackImmune())
							&& Units[i].NetworkActivity != NetworkActivity.Attack && Units[i].CanAttack() && Utils.SleepCheck(Units[i].Handle+"Attack")
							)
							{
								Units[i].Attack(e);
								Utils.Sleep(250, Units[i].Handle+"Attack");
							}
							else if ((!Units[i].CanAttack() || Units[i].Distance2D(e) >= 0) && Units[i].NetworkActivity != NetworkActivity.Attack
								&& Units[i].CanMove() &&
									Units[i].Distance2D(e) <= 600 && Utils.SleepCheck(Units[i].Handle+"Move")
								)
							{
								Units[i].Move(e.Predict(300));
								Utils.Sleep(350, Units[i].Handle+"Move");
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
