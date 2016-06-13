namespace DotaAllCombo.Heroes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using SharpDX;

	using Service;
	using Service.Debug;
	

	internal class PuckController : Variables, IHeroController
	{
		private Ability Q, W, E, D, R;
		private Item Blink, dagon, ethereal, euls, forcestaff, Shiva, orchid, sheep, vail;
		//private int stage;

		public void Combo()
		{
			target = me.ClosestToMouseTarget(2000);


			if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || Game.IsChatOpen)
				return;

			if (me == null || target == null)
				return;
			// skills
				Q = me.Spellbook.SpellQ;
				W = me.Spellbook.SpellW;
				E = me.Spellbook.SpellE;
				D = me.Spellbook.SpellD;
				R = me.Spellbook.SpellR;
			// itens
			Blink = me.FindItem("item_blink");
			dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			ethereal = me.FindItem("item_ethereal_blade");
			euls = me.FindItem("item_cyclone");
			Shiva = me.FindItem("item_shivas_guard");
			orchid = me.FindItem("item_orchid");
			vail = me.FindItem("item_veil_of_discord");
			forcestaff = me.FindItem("item_force_staff");
			sheep = me.FindItem("item_sheepstick");

			//Starting Combo


			var target2 =
				ObjectManager.GetEntities<Hero>()
					.Where(
						hero =>
							hero.IsAlive && hero.Player.Hero.Handle != target.Handle && !hero.IsIllusion && hero.IsVisible &&
							hero.Team != me.Team)
					.ToList();
			var _Is_in_Advantage = (target.HasModifier("modifier_item_blade_mail_reflect") ||
									target.HasModifier("modifier_item_lotus_orb_active") ||
									target.HasModifier("modifier_nyx_assassin_spiked_carapace") ||
									target.HasModifier("modifier_templar_assassin_refraction_damage") ||
									target.HasModifier("modifier_ursa_enrage") ||
									target.HasModifier("modifier_abaddon_borrowed_time") ||
									(target.HasModifier("modifier_dazzle_shallow_grave")));
			if (Game.IsKeyDown(menu.Item("Full").GetValue<KeyBind>().Key) && Utils.SleepCheck("combo"))
			{
				if (me.CanCast() && !me.IsChanneling() && me.Distance2D(target) <= 1200 &&
				 target.IsVisible && target.IsAlive && !target.IsMagicImmune() &&
				 !_Is_in_Advantage)
				{
					if (target.IsLinkensProtected())
					{
						if (euls != null 
							&& euls.CanBeCasted()  
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(euls.Name))
						{
							euls.UseAbility(target);
						}
						else if (forcestaff != null 
							&& forcestaff.CanBeCasted() 
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(forcestaff.Name))
						{
							forcestaff.UseAbility(target);
						}
						else if (dagon != null 
							&& dagon.CanBeCasted() 
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
						{
							dagon.UseAbility(target);
						}
						else if (ethereal != null 
							&& ethereal.CanBeCasted() 
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
						{
							ethereal.UseAbility(target);
						}
					}
					if ( // vail
						vail != null
						&& vail.CanBeCasted()
						&& me.CanCast()
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
						&& !target.IsMagicImmune()
						&& Utils.SleepCheck("vail")
						&& me.Distance2D(target) <= 1500
						)
					{
						vail.UseAbility(target.Position);
						Utils.Sleep(250, "vail");
					}
					if (Blink != null
						&& Blink.CanBeCasted()
						&& me.Distance2D(target.Position) > 300
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Blink.Name)
						&& Utils.SleepCheck("b"))
					{
						Blink.UseAbility(target.Position);
						Utils.Sleep(300, "b");
					}
					if (ethereal != null
					  && ethereal.CanBeCasted()
					  && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
					{
						ethereal.UseAbility(target);
					}
					if (!ethereal.CanBeCasted() || ethereal == null || me.IsSilenced() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
					{
						

						if (orchid != null 
							&& orchid.CanBeCasted() 
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
						{
							orchid.UseAbility(target);
						}
						if (sheep != null 
							&& sheep.CanBeCasted() 
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
						{
							sheep.UseAbility(target);
						}
						if (ethereal != null 
							&& ethereal.CanBeCasted() 
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
						{
							ethereal.UseAbility(target);
						}

						if (Q!=null 
							&& Q.CanBeCasted() 
							&& me.Distance2D(target) < Q.CastRange 
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
						{
								Q.CastSkillShot(target);
						}
						if (W!=null
							&& W.CanBeCasted()
							&& me.Distance2D(target) < 390 
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
						{
							W.UseAbility();
						}
						foreach (var t in target2)
						{
							if (target.Distance2D(t.Player.Hero) < 350 
								&& target.Distance2D(t.Player.Hero) > 10 &&
								R!=null&& R.CanBeCasted() &&
								menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name))
							{
								R.UseAbility(target.Position);
							}
						}

						if (// Dagon
							me.CanCast()
							&& dagon != null
							&& (ethereal == null
							|| (target.HasModifier("modifier_item_ethereal_blade_slow")
							|| ethereal.Cooldown < 17))
							&& !target.IsLinkensProtected()
							&& dagon.CanBeCasted()
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
							&& !target.IsMagicImmune()
							&& Utils.SleepCheck("dagon")
							)
						{
							dagon.UseAbility(target);
							Utils.Sleep(200, "dagon");
						} // Dagon Item end
						if (Shiva != null 
							&& Shiva.CanBeCasted() 
							&& me.Distance2D(target)<=600
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name))
						{
							Shiva.UseAbility();
						}
						
						/*
						if ((dagon == null || !dagon.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
						 && (Q == null || !Q.CanBeCasted() || !menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
						 && (Shiva == null || !Shiva.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name))
						 && (ethereal == null || !ethereal.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
						 && (R == null || !R.CanBeCasted() || !menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name))
						 && (orchid == null || !orchid.CanBeCasted()) || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
						{
							stage = 0;
						}*/
					}
				}
				Utils.Sleep(250, "combo");
			}
			//Escape-combo
			if (Game.IsKeyDown(menu.Item("Escape").GetValue<KeyBind>().Key) && me.Distance2D(target) <= 1200 &&
				target.IsVisible && target.IsAlive && !target.IsMagicImmune() &&
				Utils.SleepCheck("combo2"))
			{
				if (me.CanCast())
				{
					var X = me.Position.X;
					var Y = me.Position.Y;
					var Pos = new Vector3(X, Y, me.Position.Z);
					if (target.IsLinkensProtected())
					{

						if (me.Position.X < 0)
						{
							X = me.Position.X + 100;
						}
						else
						{
							X = me.Position.X - 100;
						}
						if (me.Position.Y < 0)
						{
							Y = me.Position.Y + 100;
						}
						else
						{
							Y = me.Position.Y - 100;
						}
						if (Q != null
							&& Q.CanBeCasted()
							&& me.Distance2D(target) < Q.CastRange
							&&
							menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
						{
							Q.UseAbility(Pos);
						}

						if (Blink != null && Blink.CanBeCasted() && me.Distance2D(target.Position) > 300 &&
							menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Blink.Name) &&
							Utils.SleepCheck("b2"))
						{
							Blink.UseAbility(target.Position);
						}
						if (euls != null && euls.CanBeCasted() && target.IsLinkensProtected() &&
							menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(euls.Name))
						{
							euls.UseAbility(target);
						}
						else if (forcestaff != null && forcestaff.CanBeCasted() && target.IsLinkensProtected() &&
								 menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(forcestaff.Name))
						{
							forcestaff.UseAbility(target);
						}
						else if (dagon != null && dagon.CanBeCasted() &&
								 menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
						{
							dagon.UseAbility(target);
						}
					}
					if ( // vail
						vail != null
						&& vail.CanBeCasted()
						&& me.CanCast()
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
						&& !target.IsMagicImmune()
						&& Utils.SleepCheck("vail")
						&& me.Distance2D(target) <= 1500
						)
					{
						vail.UseAbility(target.Position);
					}
					if (ethereal != null
						&& ethereal.CanBeCasted()
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
					{
						ethereal.UseAbility(target);
					}
					if (!ethereal.CanBeCasted() || ethereal == null || me.IsSilenced() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
					{
						

						if (me.Position.X < 0)
						{
							X = me.Position.X + 100;
						}
						else
						{
							X = me.Position.X - 100;
						}
						if (me.Position.Y < 0)
						{
							Y = me.Position.Y + 100;
						}
						else
						{
							Y = me.Position.Y - 100;
						}
						if (Q != null 
							&& Q.CanBeCasted() 
							&& ( Blink == null 
							|| !Blink.CanBeCasted())
							&& me.Distance2D(target) < Q.CastRange 
							&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
						{
							Q.UseAbility(Pos);
						}

						if (Blink != null && Blink.CanBeCasted() && me.Distance2D(target.Position) > 300 &&
							menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Blink.Name))
						{
							Blink.UseAbility(target.Position);
						}
						if (orchid != null && orchid.CanBeCasted() &&
							menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
						{
							orchid.UseAbility(target);
						}
						if (sheep != null && sheep.CanBeCasted() &&
							menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
						{
							sheep.UseAbility(target);
						}
						if (ethereal != null && ethereal.CanBeCasted() &&
							menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
						{
							ethereal.UseAbility(target);
						}
						if (W != null && W.CanBeCasted() && me.Distance2D(target) < 390 &&
							menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
						{
							W.UseAbility();
						}
						if ( // Dagon
							me.CanCast()
							&& dagon != null
							&& !target.IsLinkensProtected()
							&& dagon.CanBeCasted()
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
							&& !target.IsMagicImmune()
							&& Utils.SleepCheck("dagon")
							)
						{
							dagon.UseAbility(target);
							Utils.Sleep(200, "dagon");
						} // Dagon Item end
						if (Shiva != null
							&& Shiva.CanBeCasted()
							&& me.Distance2D(target) <= 600
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name))
						{
							Shiva.UseAbility();
						}
						if (
							D != null
							&& D.CanBeCasted()
							&& (sheep == null || !sheep.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
							&& (Shiva == null || !Shiva.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name))
							&& (dagon == null || !dagon.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
							&& (ethereal == null || !ethereal.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
							&& (orchid == null || !orchid.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
							&& (Blink == null || !Blink.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Blink.Name))
							&& (W == null || !W.CanBeCasted() || me.Distance2D(target) >= 400 || !menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
							&& Utils.SleepCheck("12"))
						{
							var baseDota =
								ObjectManager.GetEntities<Unit>()
									.Where(unit => unit.Name == "npc_dota_base" && unit.Team == me.Team)
									.ToList();

							if (baseDota != null)
							{
								for (int i = 0; i < baseDota.Count(); ++i)
								{
									if (baseDota[i].Distance2D(me) >= 1000)
									{
										D.UseAbility();
									}
								}
							}
							Utils.Sleep(200, "12");
						}
						if (E != null
							&& E.CanBeCasted()
							&& D != null
							&& D.CanBeCasted()
							&&
							((sheep == null || !sheep.CanBeCasted() ||  !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
							 && (Shiva == null || !Shiva.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name))
							 && (dagon == null || !dagon.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
							 && (ethereal == null || !ethereal.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
							 && (orchid == null || !orchid.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
							 && (Blink == null || !Blink.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Blink.Name))
							 && (W == null || !W.CanBeCasted() || me.Distance2D(target) >= 400 || !menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
							&& Utils.SleepCheck("1")))
						{
							E.UseAbility();
							Utils.Sleep(250, "1");
						}
					}
				}
				Utils.Sleep(300, "combo2");
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("Slon|Modif by Vick", "0.1");

			Print.LogMessage.Success("I find myself strangely drawn to this odd configuration of activity.");

			menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			menu.AddItem(new MenuItem("Full", "Full Combo(Please use only if you have Blink)").SetValue(new KeyBind('A', KeyBindType.Press)));
			menu.AddItem(new MenuItem("Escape", "Escape Combo(Please use only if you have Blink)").SetValue(new KeyBind('D', KeyBindType.Press)));

			menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"puck_dream_coil", true},
					{"puck_ethereal_jaunt", true},
					{"puck_phase_shift", true},
					{"puck_waning_rift", true},
					{"puck_illusory_orb", true}
				})));

			menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_veil_of_discord", true},
					{"item_blink", true},
					{"item_dagon", true},
					{"item_cyclone", true},
					{"item_ethereal_blade", true},
					{"item_shivas_guard", true},
					{"item_orchid", true},
					{"item_bloodthorn", true},
					{"item_sheepstick", true}
				})));
		}

		public void OnCloseEvent()
		{
			
		}
	}
}
