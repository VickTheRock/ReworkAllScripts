namespace Zet
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Ensage;
	using Ensage.Common.Extensions;
	using Ensage.Common;
	using SharpDX;
	using Ensage.Common.Menu;
	internal class Program
	{
		private static Hero me, e, enemy;
		private static readonly Menu Menu = new Menu("Zet", "Zet", true, "npc_dota_hero_arc_warden", true);
		private static readonly Menu skills = new Menu("Skills", "Skills");
		private static readonly Menu items = new Menu("Items", "Items");
		private static readonly Menu ult = new Menu("AutoMagnetic", "AutoMagnetic");
		private static bool keyCombo;
		private static bool keyAutoCombo;

		private static void OnCloseEvent(object sender, EventArgs args)
		{
			Game.OnUpdate -= Game_OnUpdate;
			e = null;
			me = null;
			Menu.RemoveFromMainMenu();
		}

		private static void OnLoadEvent(object sender, EventArgs args)
		{
			if (ObjectManager.LocalHero.ClassID != ClassID.CDOTA_Unit_Hero_ArcWarden) return;

			me = ObjectManager.LocalHero;
			e = me.ClosestToMouseTarget(1900);
			Print.LogMessage.Success("This beginning marks their end!");

			Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("ComboAutoKey", "AutoKillTargetIllusion").SetValue(new KeyBind('F', KeyBindType.Toggle)));
			Menu.AddSubMenu(skills);
			Menu.AddSubMenu(items);
			Menu.AddSubMenu(ult);
			skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"arc_warden_flux",true},
				{"arc_warden_magnetic_field",true},
				{"arc_warden_spark_wraith",true},
				{"arc_warden_tempest_double",true}
			})));
			items.AddItem(new MenuItem("Items", ":").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_necronomicon",true},
				{"item_orchid",true},
				{"item_black_king_bar", true},
				{"item_ethereal_blade",true},
				{"item_veil_of_discord",true},
				{"item_rod_of_atos",true},
				{"item_sheepstick",true},
				{"item_arcane_boots",true},
				{"item_blink",true},
				{"item_soul_ring",true},
				{"item_ghost",true},
				{"item_shivas_guard",true},
				{"item_cheese",true}
			})));
			ult.AddItem(new MenuItem("AutoMagnetic", "AutoMagnetic(W)").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"arc_warden_magnetic_field",true}
			})));
			items.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			items.AddItem(
				new MenuItem("BKB", "Options BKB.").SetValue(new StringList(new[]
				{
					"Me",
					"Clone",
					"All",
					"NoOne"
				}, 2))).SetTooltip("All include ally heroes too");
			items.AddItem(
				new MenuItem("Diff", "Options Diffusal").SetValue(new StringList(new[]
				{
					"ME",
					"CLONE",
					"No One"
				}, 1)));
			items.AddItem(
				new MenuItem("Diffusal.Debuff", "Diffusal.Debuff").SetValue(new StringList(new[]
				{
					"OnlyOneTarget",
					"AllEnemiesInCastRange",
					"No"
				}, 1)));
			items.AddItem(new MenuItem("nec", "Use Necronomicon").SetValue(true));
			//	skills.AddItem(new MenuItem("Link: ", "Auto triggre Linken").SetValue(new AbilityToggler(Link)));
			Menu.AddToMainMenu();
			Game.OnUpdate += Game_OnUpdate;
			Console.WriteLine("Zet combo loaded!");

		}

		private static void Main(string[] args)
		{

			Events.OnLoad += OnLoadEvent;
			Events.OnClose += OnCloseEvent;
		}
		private enum DiffDebuff
		{
			OnlyOneTarget,
			AllEnemiesInCastRange,
			No
		}
		private enum BKBSEL
		{
			Me,
			Clone,
			All,
			Noone
		}
		private enum DiffSEL
		{
			ME,
			CLONE,
			No0ne
		}

		private static void Game_OnUpdate(EventArgs args)
		{
			me = ObjectManager.LocalHero;
			if (me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_ArcWarden || !Game.IsInGame || Game.IsWatchingGame)
				return;

			keyCombo = Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key);
			keyAutoCombo = Menu.Item("ComboAutoKey").GetValue<KeyBind>().Active;
			List<Hero> zet = ObjectManager.GetEntities<Hero>().Where(x => x.IsControllable && x.Team == me.Team && x.IsAlive && x.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden).ToList();
			var enem = ObjectManager.GetEntities<Hero>().Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune()).ToList();


			Ability[] Q = new Ability[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) Q[i] = zet[i].Spellbook.SpellQ;
			Ability[] W = new Ability[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) W[i] = zet[i].Spellbook.SpellW;
			Ability[] E = new Ability[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) E[i] = zet[i].Spellbook.SpellE;
			Ability[] R = new Ability[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) R[i] = zet[i].Spellbook.SpellR;






			/**************************************************DODGE*************************************************************/
			if (Menu.Item("AutoMagnetic").GetValue<AbilityToggler>().IsEnabled(me.Spellbook.SpellW.Name))
			{
				for (int i = 0; i < zet.Count(); ++i)
				{
					for (int v = 0; v < enem.Count(); ++v)
					{
						for (int j = 0; j < zet.Count(); ++j)
						{
							if (
							zet[i] != null
							&& zet[i].Handle != me.Handle
							&& zet[j].Handle == me.Handle
							&& W[i].CanBeCasted()
							&& !me.HasModifier("modifier_arc_warden_magnetic_field")
							&& zet[j].Position.Distance2D(enem[v]) < enem[v].AttackRange + zet[j].HullRadius + 40
							&& zet[j].Position.Distance2D(zet[i]) < W[j].CastRange
							&& enem[v].NetworkActivity == NetworkActivity.Attack
							&& Utils.SleepCheck(zet[i].Handle + "_net_casting"))
							{
								W[i].UseAbility(zet[j].Position);
								Utils.Sleep(W[i].GetCastDelay(zet[i], enem[v], true) + 500, zet[i].Handle + "_net_casting");
							}
							if (
								(zet[i].Handle != me.Handle
								|| zet[i] == null)
								&& zet[j].Handle == me.Handle
								&& (!W[i].CanBeCasted() || zet[i] == null || zet[i].Distance2D(zet[j]) >= W[i].CastRange + 70)
								&& W[j].CanBeCasted()
								&& zet[j].Distance2D(enem[v]) <= enem[v].AttackRange + zet[j].HullRadius + 40
								&& me.Level >= 6
								&& enem[v].NetworkActivity == NetworkActivity.Attack
								&& !me.HasModifier("modifier_arc_warden_magnetic_field")
								&& Utils.SleepCheck(zet[i].Handle + "_net_casting")
								)
							{
								W[j].UseAbility(zet[j].Position);
								Utils.Sleep(W[j].GetCastDelay(zet[j], enem[v], true) + 1500, zet[i].Handle + "_net_casting");
								break;
							}
							if (
								zet.Count == 1
								&& W[j].CanBeCasted()
								&& zet[j].Distance2D(enem[v]) <= enem[v].AttackRange + zet[j].HullRadius + 40
								&& enem[v].NetworkActivity == NetworkActivity.Attack
								&& !me.HasModifier("modifier_arc_warden_magnetic_field")
								&& Utils.SleepCheck(zet[j].Handle + "_net_casting")
								)
							{
								W[j].UseAbility(zet[j].Position);
								Utils.Sleep(W[j].GetCastDelay(zet[j], enem[v], true, true) + 1500, zet[j].Handle + "_net_casting");
								break;
							}
							if (zet[j].Handle != me.Handle && me.Level >= 6 && W[j].CanBeCasted() && zet[j].Position.Distance2D(enem[v]) <= enem[v].AttackRange + zet[j].HullRadius + 40 && !me.HasModifier("modifier_arc_warden_magnetic_field")
								&& enem[v].NetworkActivity == NetworkActivity.Attack
									   && zet[j].Position.Distance2D(zet[i]) > W[j].CastRange + 500 && Utils.SleepCheck(zet[i].Handle + "_net_casting"))
							{
								W[j].UseAbility(zet[j].Position);
								Utils.Sleep(W[j].GetCastDelay(zet[j], enem[v], true, true) + 1500, zet[i].Handle + "_net_casting");
								break;
							}
						}
					}
				}
			}
			/**************************************************DODGE*************************************************************/

			/**************************************************COMBO*************************************************************/


			//TODO: start combo

			Item[] bkb = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) bkb[i] = zet[i].FindItem("item_black_king_bar");
			Item[] ethereal = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) ethereal[i] = zet[i].FindItem("item_ethereal_blade");
			Item[] vail = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) vail[i] = zet[i].FindItem("item_veil_of_discord");
			Item[] cheese = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) cheese[i] = zet[i].FindItem("item_cheese");
			Item[] ghost = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) ghost[i] = zet[i].FindItem("item_ghost");
			Item[] orchid = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) orchid[i] = zet[i].FindItem("item_orchid");
			Item[] atos = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) atos[i] = zet[i].FindItem("item_rod_of_atos");
			Item[] soulring = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) soulring[i] = zet[i].FindItem("item_soul_ring");
			Item[] arcane = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) arcane[i] = zet[i].FindItem("item_arcane_boots");
			Item[] blink = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) blink[i] = zet[i].FindItem("item_blink");
			Item[] shiva = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) shiva[i] = zet[i].FindItem("item_shivas_guard");
			Item[] dagon = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) dagon[i] = zet[i].Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
			Item[] necronomicon = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) necronomicon[i] = zet[i].Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_necronomicon"));
			Item[] diffusal = new Item[zet.Count()];
			for (int i = 0; i < zet.Count(); ++i) diffusal[i] = zet[i].Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_diffusal_blade"));

			if (keyCombo)
			{
				for (int i = 0; i < zet.Count(); ++i)
				{

					e = ClosestToMouse(zet[i]);
					if (e == null) return;
					var ModifEther = e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
					var stoneModif = e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");

					Item[] sheep = new Item[zet.Count()];
					for (int z = 0; z < zet.Count(); z++) sheep[z] = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : zet[z].FindItem("item_sheepstick");
					if (
						blink[i] != null
						&& zet[i].CanCast()
						&& blink[i].CanBeCasted()
						&& zet[i].Distance2D(e) <= 1150
						&& zet[i].Distance2D(e) > 400
						&& !stoneModif
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink[i].Name)
						&& Utils.SleepCheck(zet[i].Handle + "blink[i]")
						)
					{
						blink[i].UseAbility(e.Position);
						Utils.Sleep(250, zet[i].Handle + "blink[i]");
					}
					if ( // sheep
						sheep[i] != null
						&& sheep[i].CanBeCasted()
						&& zet[i].CanCast()
						&& !e.IsLinkensProtected()
						&& !e.IsMagicImmune()
						&& zet[i].Distance2D(e) <= 1400
						&& !stoneModif
						&& !e.IsHexed()
						&& !e.IsStunned()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep[i].Name)
						&& Utils.SleepCheck(zet[i].Handle + "sheep[i]")
						)
					{
						sheep[i].UseAbility(e);
						Utils.Sleep(250, zet[i].Handle + "sheep[i]");
					} // sheep[i] Item end
					if (!sheep[i].CanBeCasted() || e.IsHexed() || e.IsStunned())
					{



						if (
							R[i].CanBeCasted()
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(me.Spellbook.SpellR.Name)
							&& zet[i].Distance2D(e) <= Q[i].CastRange - 50
							&& !e.IsMagicImmune()
							&& Utils.SleepCheck(zet[i].Handle + "R")
							)
						{
							R[i].UseAbility();
							Utils.Sleep(250, zet[i].Handle + "R");
						}
						if (
							Q[i].CanBeCasted()
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(me.Spellbook.SpellQ.Name)
							&& zet[i].Distance2D(e) <= Q[i].CastRange + 150
							&& !e.IsMagicImmune()
							&& Utils.SleepCheck(zet[i].Handle + "Q")
							)
						{
							Q[i].UseAbility(e);
							Utils.Sleep(250, zet[i].Handle + "Q");
						}

						var all = Menu.Item("BKB").GetValue<StringList>().SelectedIndex == (int)BKBSEL.All;
						var main = Menu.Item("BKB").GetValue<StringList>().SelectedIndex == (int)BKBSEL.Me;
						var clone = Menu.Item("BKB").GetValue<StringList>().SelectedIndex == (int)BKBSEL.Clone;

						var OnlyOneTarget = Menu.Item("Diffusal.Debuff").GetValue<StringList>().SelectedIndex == (int)DiffDebuff.OnlyOneTarget;
						var AllEnemiesInCastRange = Menu.Item("Diffusal.Debuff").GetValue<StringList>().SelectedIndex == (int)DiffDebuff.AllEnemiesInCastRange;
						var No = Menu.Item("Diffusal.Debuff").GetValue<StringList>().SelectedIndex == (int)DiffDebuff.No;

						var no = Menu.Item("Diff").GetValue<StringList>().SelectedIndex == (int)DiffSEL.No0ne;
						var ME = Menu.Item("Diff").GetValue<StringList>().SelectedIndex == (int)DiffSEL.ME;
						var CLONE = Menu.Item("Diff").GetValue<StringList>().SelectedIndex == (int)DiffSEL.CLONE;
						if (bkb[i] != null
							&& bkb[i].CanBeCasted()
							&& (enem.Count(x => x.Distance2D(zet[i]) <= 650) >= (Menu.Item("Heel").GetValue<Slider>().Value))
							&& ((zet[i].Handle != me.Handle && clone)
							|| (zet[i].Handle == me.Handle && main)
							|| (all)
							)
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb[i].Name)
							&& Utils.SleepCheck(zet[i].Handle + "bkb[i]"))
						{
							bkb[i].UseAbility();
							Utils.Sleep(250, zet[i].Handle + "bkb[i]");
						}
						for (int v = 0; v < enem.Count(); ++v)
						{
							var mod =
							   enem[v].Modifiers.Any(modifierName => modifierName.Name == "modifier_item_diffusal_blade_slow");
							if (diffusal[i] != null
							&& diffusal[i].CanBeCasted()
							&& enem[v].Distance2D(zet[i]) <= 650
							&& zet[i].Handle != me.Handle && CLONE && AllEnemiesInCastRange
							&& !mod
							&& Utils.SleepCheck(zet[i].Handle + "diffusal[i]"))
							{
								diffusal[i].UseAbility(enem[v]);
								Utils.Sleep(400, zet[i].Handle + "diffusal[i]");
								break;
							}
						}

						var modtarget =
							   e.Modifiers.Any(modifierName => modifierName.Name == "modifier_item_diffusal_blade_slow");
						if (diffusal[i] != null
							&& diffusal[i].CanBeCasted()
							&& !modtarget
							&& e.Distance2D(zet[i]) <= 650 && (ME || CLONE) && OnlyOneTarget
							&& Utils.SleepCheck(zet[i].Handle + "diffusal[i]"))
						{
							diffusal[i].UseAbility(e);
							Utils.Sleep(350, zet[i].Handle + "diffusal[i]");
							break;
						}
						if ( // atos[i] Blade
							atos[i] != null
							&& atos[i].CanBeCasted()
							&& zet[i].CanCast()
							&& !e.IsLinkensProtected()
							&& !e.IsMagicImmune()
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos[i].Name)
							&& zet[i].Distance2D(e) <= 2000
							&& Utils.SleepCheck(zet[i].Handle + "atos[i]")
							)
						{
							atos[i].UseAbility(e);
							Utils.Sleep(250, zet[i].Handle + "atos[i]");
						} // atos[i] Item end


						if ( // orchid[i]
							orchid[i] != null
							&& orchid[i].CanBeCasted()
							&& zet[i].CanCast()
							&& !e.IsLinkensProtected()
							&& !e.IsMagicImmune()
							&& zet[i].Distance2D(e) <= 1400
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid[i].Name)
							&& !stoneModif
							&& Utils.SleepCheck(zet[i].Handle + "orchid[i]")
							)
						{
							orchid[i].UseAbility(e);
							Utils.Sleep(250, zet[i].Handle + "orchid[i]");
						} // orchid[i] Item end
						if (!orchid[i].CanBeCasted() || orchid[i] == null || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid[i].Name))
						{

							if ( // vail
								vail != null
								&& vail[i].CanBeCasted()
								&& zet[i].CanCast()
								&& !e.IsMagicImmune()
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail[i].Name)
								&& zet[i].Distance2D(e) <= 1500
								&& Utils.SleepCheck(zet[i].Handle + "vail[i]")
								)
							{
								vail[i].UseAbility(e.Position);
								Utils.Sleep(250, zet[i].Handle + "vail[i]");
							} // orchid[i] Item end
							if (!vail[i].CanBeCasted() || vail[i] == null || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail[i].Name))
							{
								if (// ethereal[i]
									ethereal[i] != null
									&& ethereal[i].CanBeCasted()
									&& zet[i].CanCast()
									&& !e.IsLinkensProtected()
									&& !e.IsMagicImmune()
									&& !stoneModif
									&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal[i].Name)
									&& Utils.SleepCheck(zet[i].Handle + "ethereal[i]")
									)
								{
									ethereal[i].UseAbility(e);
									Utils.Sleep(250, zet[i].Handle + "ethereal[i]");
								} // ethereal[i] Item end
								if (!ethereal[i].CanBeCasted() || ethereal[i] == null || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal[i].Name))
								{
									if (// SoulRing Item 
										soulring[i] != null
										&& soulring[i].CanBeCasted()
										&& zet[i].CanCast()
										&& zet[i].Health >= (zet[i].MaximumHealth * 0.6)
										&& zet[i].Mana <= E[i].ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soulring[i].Name)
										)
									{
										soulring[i].UseAbility();
									} // SoulRing Item end

									if (// Arcane Boots Item
										arcane[i] != null
										&& arcane[i].CanBeCasted()
										&& zet[i].CanCast()
										&& zet[i].Mana <= E[i].ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane[i].Name)
										)
									{
										arcane[i].UseAbility();
									} // Arcane Boots Item end

									if (//Ghost
										ghost[i] != null
										&& ghost[i].CanBeCasted()
										&& zet[i].CanCast()
										&& ((zet[i].Position.Distance2D(e) < 300
										&& zet[i].Health <= (zet[i].MaximumHealth * 0.7))
										|| zet[i].Health <= (zet[i].MaximumHealth * 0.3))
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ghost[i].Name)
										&& Utils.SleepCheck(zet[i].Handle + "Ghost"))
									{
										ghost[i].UseAbility();
										Utils.Sleep(250, zet[i].Handle + "Ghost");
									}
									if (// Shiva Item
										shiva[i] != null
										&& shiva[i].CanBeCasted()
										&& zet[i].CanCast()
										&& !e.IsMagicImmune()
										& Utils.SleepCheck(zet[i].Handle + "shiva[i]")
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva[i].Name)
										&& zet[i].Distance2D(e) <= 600
										)

									{
										shiva[i].UseAbility();
										Utils.Sleep(250, zet[i].Handle + "shiva[i]");
									} // Shiva Item end

									if (// Dagon
										zet[i].CanCast()
										&& dagon != null
										&& (ethereal[i] == null
										|| (ModifEther
										|| ethereal[i].Cooldown < 17))
										&& !e.IsLinkensProtected()
										&& dagon[i].CanBeCasted()
										&& !e.IsMagicImmune()
										&& !stoneModif
										&& Utils.SleepCheck(zet[i].Handle + "dagon[i]")
									   )
									{
										dagon[i].UseAbility(e);
										Utils.Sleep(250, zet[i].Handle + "dagon[i]");
									} // Dagon Item end

									if (
										 // cheese[i]
										 cheese[i] != null
										 && cheese[i].CanBeCasted()
										 && zet[i].Health <= (zet[i].MaximumHealth * 0.3)
										 && zet[i].Distance2D(e) <= 700
										 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese[i].Name)
										 && Utils.SleepCheck(zet[i].Handle + "cheese[i]")
									 )
									{
										cheese[i].UseAbility();
										Utils.Sleep(250, zet[i].Handle + "cheese[i]");
									} // cheese[i] Item end
									if (
										E[i].CanBeCasted()
										&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(me.Spellbook.SpellE.Name)
										&& zet[i].Distance2D(enemy) <= E[i].CastRange - 300
										&& !enemy.IsMagicImmune()
										)
									{
												var eVector = enemy.NetworkActivity == NetworkActivity.Move
									   ? Prediction.InFront(enemy, (float)(enemy.MovementSpeed * 2.9 + Game.Ping / 1000))
									   : enemy.Position;
										if (Utils.SleepCheck(zet[i].Handle + "E"))
										{
											E[i].UseAbility(eVector);
											Utils.Sleep(250, zet[i].Handle + "E");
										}
									}

									if (
									zet[i].Distance2D(e) >= 0 && zet[i].NetworkActivity != NetworkActivity.Attack &&
									zet[i].Distance2D(e) <= 1200 && Utils.SleepCheck(zet[i].Handle + "move")
										)
									{
										zet[i].Move(e.Predict(450));
										Utils.Sleep(300, zet[i].Handle + "move");
									}
									if (
										zet[i].Distance2D(e) <= zet[i].AttackRange && (!zet[i].IsAttackImmune() || !e.IsAttackImmune())
										&& zet[i].NetworkActivity != NetworkActivity.Attack & Utils.SleepCheck(zet[i].Handle + "attack")
										)
									{
										zet[i].Attack(e);
										Utils.Sleep(zet[i].AttacksPerSecond - 50, zet[i].Handle + "attack");
									}
								}
							}
						}
					}

					if (Menu.Item("nec").IsActive()
						&& zet[i].Distance2D(e) <= me.Spellbook.SpellQ.GetCastRange() + 5
						&& necronomicon[i] != null
						&& necronomicon[i].CanBeCasted()
						&& Utils.SleepCheck(zet[i].Handle + "necronomicon[i]")
						)
					{
						necronomicon[i].UseAbility();
						Utils.Sleep(250, zet[i].Handle + "necronomicon[i]");
					}
					var necro =
					ObjectManager.GetEntities<Unit>()
						.Where(x => x.IsAlive && x.IsControllable && x.Team == me.Team && x.IsSummoned && x.Handle != zet[i].Handle)
						.ToList();
					for (int n = 0; n < necro.Count(); ++n)
					{
						if (necro[n] != null && e.Position.Distance2D(necro[n].Position) <= 1200 && necro[n].Spellbook.SpellQ.CanBeCasted()
							&& Utils.SleepCheck(necro[n].Handle + "qcast"))
						{
							necro[n].Spellbook.SpellQ.UseAbility(e);
							Utils.Sleep(300, necro[n].Handle + "qcast");
						}
						if (
						   necro[n].Distance2D(e) <= necro[n].AttackRange && (!necro[n].IsAttackImmune() || !e.IsAttackImmune())
						   && necro[n].NetworkActivity != NetworkActivity.Attack & Utils.SleepCheck(necro[n].Handle + "attackn")
						   )
						{
							necro[n].Attack(e);
							Utils.Sleep(300, necro[n].Handle + "attackn");
						}
						else if (
								necro[n].Distance2D(e) >= 0 && necro[n].NetworkActivity != NetworkActivity.Attack &&
								necro[n].Distance2D(e) <= 1200 && Utils.SleepCheck(necro[n].Handle + "moven")
								)
						{
							necro[n].Move(e.Predict(450));
							Utils.Sleep(300, necro[n].Handle + "moven");
						}

					}
				}
			}
			//TODO: End combo
			//TODO: StartCopy combo
			if (keyAutoCombo && Utils.SleepCheck("Alt"))
			{
				List<Hero> zetCopy = ObjectManager.GetEntities<Hero>().Where(x => x.IsControllable && x.Team == me.Team && x.IsAlive && x.Handle != me.Handle && x.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden).ToList();
				if (Menu.Item("nec").IsActive())
				{
					var necro =
					ObjectManager.GetEntities<Unit>()
						.Where(x => x.IsAlive && x.IsControllable && x.Team == me.Team && x.IsSummoned)
						.ToList();
					if (necro.Count > 0)
					{
						for (int n = 0; n < necro.Count(); ++n)
						{
							enemy = GetClosestToTarget(enem, necro[n]);
							if (necro[n] != null && enemy.Position.Distance2D(necro[n].Position) <= 1200 &&
								necro[n].Spellbook.SpellQ.CanBeCasted()
								&& Utils.SleepCheck(necro[n].Handle + "qcast"))
							{
								necro[n].Spellbook.SpellQ.UseAbility(enemy);
								Utils.Sleep(300, necro[n].Handle + "qcast");
							}
							if (
								necro[n].Distance2D(enemy) <= necro[n].AttackRange && (!necro[n].IsAttackImmune() || !enemy.IsAttackImmune())
								&& necro[n].NetworkActivity != NetworkActivity.Attack & Utils.SleepCheck(necro[n].Handle + "attackn")
								)
							{
								necro[n].Attack(enemy);
								Utils.Sleep(300, necro[n].Handle + "attackn");
							}
							else if (
								necro[n].Distance2D(enemy) >= 0 && necro[n].NetworkActivity != NetworkActivity.Attack &&
								necro[n].Distance2D(enemy) <= 1200 && Utils.SleepCheck(necro[n].Handle + "moven")
								)
							{
								necro[n].Move(enemy.Predict(450));
								Utils.Sleep(300, necro[n].Handle + "moven");
							}
						}
					}
				}
				if (zetCopy.Count <= 0) return;
				for (int i = 0; i < zetCopy.Count(); ++i)
				{

					enemy = GetClosestToTarget(enem, zetCopy[i]);
					if (enemy == null) return;
					var modifEther = enemy.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
					var stoneModif = enemy.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");

					Item[] sheep = new Item[zetCopy.Count()];
					for (int z = 0; z < zetCopy.Count(); ++z)
						sheep[z] = enemy.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : zetCopy[z].FindItem("item_sheepstick");
					if (
						blink[i] != null
						&& zetCopy[i].CanCast()
						&& blink[i].CanBeCasted()
						&& zetCopy[i].Distance2D(enemy) <= 1150
						&& zetCopy[i].Distance2D(enemy) > 400
						&& !stoneModif
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink[i].Name)
						&& Utils.SleepCheck(zetCopy[i].Handle + "blink[i]")
						)
					{
						blink[i].UseAbility(enemy.Position);
						Utils.Sleep(250, zetCopy[i].Handle + "blink[i]");
					}
					if ( // sheep
						sheep[i] != null
						&& sheep[i].CanBeCasted()
						&& zetCopy[i].CanCast()
						&& !enemy.IsLinkensProtected()
						&& !enemy.IsMagicImmune()
						&& zetCopy[i].Distance2D(enemy) <= 1400
						&& !stoneModif
						&& !enemy.IsHexed()
						&& !enemy.IsStunned()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep[i].Name)
						&& Utils.SleepCheck(zetCopy[i].Handle + "sheep[i]")
						)
					{
						sheep[i].UseAbility(enemy);
						Utils.Sleep(250, zetCopy[i].Handle + "sheep[i]");
					} // sheep[i] Item end
					if (!sheep[i].CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep[i].Name) ||
						enemy.IsHexed() || enemy.IsStunned() || sheep == null)
					{
						if (
							Q[i].CanBeCasted()
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(me.Spellbook.SpellQ.Name)
							&& zetCopy[i].Distance2D(enemy) <= Q[i].CastRange + 150
							&& !enemy.IsMagicImmune()
							&& Utils.SleepCheck(zetCopy[i].Handle + "Q")
							)
						{
							Q[i].UseAbility(enemy);
							Utils.Sleep(250, zetCopy[i].Handle + "Q");
						}

						var all = Menu.Item("BKB").GetValue<StringList>().SelectedIndex == (int)BKBSEL.All;
						var main = Menu.Item("BKB").GetValue<StringList>().SelectedIndex == (int)BKBSEL.Me;
						var clone = Menu.Item("BKB").GetValue<StringList>().SelectedIndex == (int)BKBSEL.Clone;

						var OnlyOneTarget = Menu.Item("Diffusal.Debuff").GetValue<StringList>().SelectedIndex ==
											(int)DiffDebuff.OnlyOneTarget;
						var AllEnemiesInCastRange = Menu.Item("Diffusal.Debuff").GetValue<StringList>().SelectedIndex ==
													(int)DiffDebuff.AllEnemiesInCastRange;
						//var No = Menu.Item("Diffusal.Debuff").GetValue<StringList>().SelectedIndex == (int)DiffDebuff.No;

						//var no = Menu.Item("Diff").GetValue<StringList>().SelectedIndex == (int)DiffSEL.No0ne;
						var ME = Menu.Item("Diff").GetValue<StringList>().SelectedIndex == (int)DiffSEL.ME;
						var CLONE = Menu.Item("Diff").GetValue<StringList>().SelectedIndex == (int)DiffSEL.CLONE;
						if (bkb[i] != null
							&& bkb[i].CanBeCasted()
							&& (enem.Count(x => x.Distance2D(zetCopy[i]) <= 650) >= (Menu.Item("Heel").GetValue<Slider>().Value))
							&& ((zetCopy[i].Handle != me.Handle && clone)
								|| (zetCopy[i].Handle == me.Handle && main)
								|| (all)
								)
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb[i].Name)
							&& Utils.SleepCheck(zetCopy[i].Handle + "bkb[i]"))
						{
							bkb[i].UseAbility();
							Utils.Sleep(250, zetCopy[i].Handle + "bkb[i]");
						}
						for (int v = 0; v < enem.Count(); ++v)
						{
							var mod =
								enem[v].Modifiers.Any(modifierName => modifierName.Name == "modifier_item_diffusal_blade_slow");
							if (diffusal[i] != null
								&& diffusal[i].CanBeCasted()
								&& enem[v].Distance2D(zetCopy[i]) <= 650
								&& zetCopy[i].Handle != me.Handle && CLONE && AllEnemiesInCastRange
								&& !mod
								&& Utils.SleepCheck(zetCopy[i].Handle + "diffusal[i]"))
							{
								diffusal[i].UseAbility(enem[v]);
								Utils.Sleep(400, zetCopy[i].Handle + "diffusal[i]");
								break;
							}
						}

						var modtarget =
							enemy.Modifiers.Any(modifierName => modifierName.Name == "modifier_item_diffusal_blade_slow");
						if (diffusal[i] != null
							&& diffusal[i].CanBeCasted()
							&& !modtarget
							&& enemy.Distance2D(zetCopy[i]) <= 650 && (ME || CLONE) && OnlyOneTarget
							&& Utils.SleepCheck(zetCopy[i].Handle + "diffusal[i]"))
						{
							diffusal[i].UseAbility(enemy);
							Utils.Sleep(350, zetCopy[i].Handle + "diffusal[i]");
							break;
						}
						if ( // atos[i] Blade
							atos[i] != null
							&& atos[i].CanBeCasted()
							&& zetCopy[i].CanCast()
							&& !enemy.IsLinkensProtected()
							&& !enemy.IsMagicImmune()
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos[i].Name)
							&& zetCopy[i].Distance2D(enemy) <= 2000
							&& Utils.SleepCheck(zetCopy[i].Handle + "atos[i]")
							)
						{
							atos[i].UseAbility(enemy);
							Utils.Sleep(250, zetCopy[i].Handle + "atos[i]");
						} // atos[i] Item end


						if ( // orchid[i]
							orchid[i] != null
							&& orchid[i].CanBeCasted()
							&& zetCopy[i].CanCast()
							&& !enemy.IsLinkensProtected()
							&& !enemy.IsMagicImmune()
							&& zetCopy[i].Distance2D(enemy) <= 1400
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid[i].Name)
							&& !stoneModif
							&& Utils.SleepCheck(zetCopy[i].Handle + "orchid[i]")
							)
						{
							orchid[i].UseAbility(enemy);
							Utils.Sleep(250, zetCopy[i].Handle + "orchid[i]");
						} // orchid[i] Item end
						if (!orchid[i].CanBeCasted() || orchid[i] == null ||
							!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid[i].Name))
						{

							if ( // vail
								vail != null
								&& vail[i].CanBeCasted()
								&& zetCopy[i].CanCast()
								&& !enemy.IsMagicImmune()
								&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail[i].Name)
								&& zetCopy[i].Distance2D(enemy) <= 1500
								&& Utils.SleepCheck(zetCopy[i].Handle + "vail[i]")
								)
							{
								vail[i].UseAbility(enemy.Position);
								Utils.Sleep(250, zetCopy[i].Handle + "vail[i]");
							} // orchid[i] Item end
							if (!vail[i].CanBeCasted() || vail[i] == null ||
								!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail[i].Name))
							{
								if ( // ethereal[i]
									ethereal[i] != null
									&& ethereal[i].CanBeCasted()
									&& zetCopy[i].CanCast()
									&& !enemy.IsLinkensProtected()
									&& !enemy.IsMagicImmune()
									&& !stoneModif
									&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal[i].Name)
									&& Utils.SleepCheck(zetCopy[i].Handle + "ethereal[i]")
									)
								{
									ethereal[i].UseAbility(enemy);
									Utils.Sleep(250, zetCopy[i].Handle + "ethereal[i]");
								} // ethereal[i] Item end
								if (!ethereal[i].CanBeCasted() || ethereal[i] == null ||
									!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal[i].Name))
								{
									if ( // SoulRing Item 
										soulring[i] != null
										&& soulring[i].CanBeCasted()
										&& zetCopy[i].CanCast()
										&& zetCopy[i].Health >= (zetCopy[i].MaximumHealth * 0.6)
										&& zetCopy[i].Mana <= E[i].ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soulring[i].Name)
										)
									{
										soulring[i].UseAbility();
									} // SoulRing Item end

									if ( // Arcane Boots Item
										arcane[i] != null
										&& arcane[i].CanBeCasted()
										&& zetCopy[i].CanCast()
										&& zetCopy[i].Mana <= E[i].ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane[i].Name)
										)
									{
										arcane[i].UseAbility();
									} // Arcane Boots Item end
									if (
										E[i].CanBeCasted()
										&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(me.Spellbook.SpellE.Name)
										&& zetCopy[i].Distance2D(enemy) <= E[i].CastRange - 300
										&& !enemy.IsMagicImmune()
										)
									{
										var eVector = enemy.NetworkActivity == NetworkActivity.Move
							   ? Prediction.InFront(enemy, (float)(enemy.MovementSpeed * 2.9 + Game.Ping / 1000))
							   : enemy.Position;
										if (Utils.SleepCheck(zetCopy[i].Handle + "E"))
										{
											E[i].UseAbility(eVector);
											Utils.Sleep(250, zetCopy[i].Handle + "E");
										}
									}
									if ( //Ghost
										ghost[i] != null
										&& ghost[i].CanBeCasted()
										&& zetCopy[i].CanCast()
										&& ((zetCopy[i].Position.Distance2D(enemy) < 300
											 && zetCopy[i].Health <= (zetCopy[i].MaximumHealth * 0.7))
											|| zetCopy[i].Health <= (zetCopy[i].MaximumHealth * 0.3))
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ghost[i].Name)
										&& Utils.SleepCheck(zetCopy[i].Handle + "Ghost"))
									{
										ghost[i].UseAbility();
										Utils.Sleep(250, zetCopy[i].Handle + "Ghost");
									}
									if ( // Shiva Item
										shiva[i] != null
										&& shiva[i].CanBeCasted()
										&& zetCopy[i].CanCast()
										&& !enemy.IsMagicImmune()
										& Utils.SleepCheck(zetCopy[i].Handle + "shiva[i]")
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva[i].Name)
										&& zetCopy[i].Distance2D(enemy) <= 600
										)

									{
										shiva[i].UseAbility();
										Utils.Sleep(250, zetCopy[i].Handle + "shiva[i]");
									} // Shiva Item end

									if ( // Dagon
										zetCopy[i].CanCast()
										&& dagon != null
										&& (ethereal[i] == null
											|| (modifEther
												|| ethereal[i].Cooldown < 17))
										&& !enemy.IsLinkensProtected()
										&& dagon[i].CanBeCasted()
										&& !enemy.IsMagicImmune()
										&& !stoneModif
										&& Utils.SleepCheck(zetCopy[i].Handle + "dagon[i]")
										)
									{
										dagon[i].UseAbility(enemy);
										Utils.Sleep(250, zetCopy[i].Handle + "dagon[i]");
									} // Dagon Item end

									if (
										// cheese[i]
										cheese[i] != null
										&& cheese[i].CanBeCasted()
										&& zetCopy[i].Health <= (zetCopy[i].MaximumHealth * 0.3)
										&& zetCopy[i].Distance2D(enemy) <= 700
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese[i].Name)
										&& Utils.SleepCheck(zetCopy[i].Handle + "cheese[i]")
										)
									{
										cheese[i].UseAbility();
										Utils.Sleep(250, zetCopy[i].Handle + "cheese[i]");
									} // cheese[i] Item end


									if (
										zetCopy[i].Distance2D(enemy) <= zetCopy[i].AttackRange && !enemy.IsAttackImmune()
										&& zetCopy[i].NetworkActivity != NetworkActivity.Attack & Utils.SleepCheck(zetCopy[i].Handle + "attack")
										)
									{
										zetCopy[i].Attack(enemy);
										Utils.Sleep(zetCopy[i].AttacksPerSecond + Game.Ping, zetCopy[i].Handle + "attack");
									}
									else if (
											zetCopy[i].Distance2D(enemy) >= 0 && zetCopy[i].NetworkActivity != NetworkActivity.Attack &&
											zetCopy[i].Distance2D(enemy) <= 1200 && Utils.SleepCheck(zetCopy[i].Handle + "move")
											)
									{
										zetCopy[i].Move(enemy.Predict(450));
										Utils.Sleep(300, zetCopy[i].Handle + "move");
									}
								}
							}
							if (zetCopy[i].Distance2D(enemy) <= me.Spellbook.SpellQ.GetCastRange() + 5
								  && necronomicon[i] != null
								  && necronomicon[i].CanBeCasted()
								  && Utils.SleepCheck(zetCopy[i].Handle + "necronomicon[i]")
								 )
							{
								necronomicon[i].UseAbility();
								Utils.Sleep(250, zetCopy[i].Handle + "necronomicon[i]");
							}
						}
					}
				}
				Utils.Sleep(500, "Alt");
			}
		}

		//TODO: EndCopy combo

		/**************************************************COMBO*************************************************************/
		private static Hero ClosestToMouse(Hero source, float range = 80000)
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
		private static Hero GetClosestToTarget(List<Hero> units, Unit e)
		{
			Hero closestHero = null;
			foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(e) > v.Distance2D(e)))
			{
				closestHero = v;
			}
			return closestHero;
		}

		private static Hero GetClosestToTarget(List<Hero> units, Hero e)
		{
			Hero closestHero = null;
			foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(e) > v.Distance2D(e)))
			{
				closestHero = v;
			}
			return closestHero;
		}
	}
	class Print
	{
		public class LogMessage
		{
			public static void Success(string text, params object[] arguments)
			{
				Game.PrintMessage("<font color='#e0007b'>" + text + "</font>", MessageType.LogMessage);
			}
		} // Console class

		public class ConsoleMessage
		{
			public static void Encolored(string text, ConsoleColor color, params object[] arguments)
			{
				var clr = Console.ForegroundColor;
				Console.ForegroundColor = color;
				Console.WriteLine(text, arguments);
				Console.ForegroundColor = clr;
			}
			public static void Success(string text, params object[] arguments)
			{
				Encolored(text, ConsoleColor.Magenta, arguments);
			}
		} // LogMessage class
	}
}
