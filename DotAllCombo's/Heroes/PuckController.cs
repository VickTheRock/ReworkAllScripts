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
		private Item blink, dagon, ethereal, euls, forcestaff, Shiva, orchid, sheep, vail;
		//private int stage;

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
            e = Toolset.ClosestToMouse(me);


            if (me == null || e == null)
				return;
			// skills
				Q = me.Spellbook.SpellQ;
				W = me.Spellbook.SpellW;
				E = me.Spellbook.SpellE;
				D = me.Spellbook.SpellD;
				R = me.Spellbook.SpellR;
			// itens
			blink = me.FindItem("item_blink");
			dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			ethereal = me.FindItem("item_ethereal_blade");
			euls = me.FindItem("item_cyclone");
			Shiva = me.FindItem("item_shivas_guard");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
			vail = me.FindItem("item_veil_of_discord");
			forcestaff = me.FindItem("item_force_staff");
			sheep = me.FindItem("item_sheepstick");

			//Starting Combo
            
			var _Is_in_Advantage = (e.HasModifier("modifier_item_blade_mail_reflect") ||
									e.HasModifier("modifier_item_lotus_orb_active") ||
									e.HasModifier("modifier_nyx_assassin_spiked_carapace") ||
									e.HasModifier("modifier_templar_assassin_refraction_damage") ||
									e.HasModifier("modifier_ursa_enrage") ||
									e.HasModifier("modifier_abaddon_borrowed_time") ||
									(e.HasModifier("modifier_dazzle_shallow_grave")));
			if (Game.IsKeyDown(Menu.Item("Full").GetValue<KeyBind>().Key) && Utils.SleepCheck("combo"))
			{
				if (me.CanCast() && !me.IsChanneling() && me.Distance2D(e) <= 1200 &&
				 e.IsVisible && e.IsAlive && !e.IsMagicImmune() &&
				 !_Is_in_Advantage)
				{
                    if (me.CanCast())
                    {
                        var v =
                      ObjectManager.GetEntities<Hero>()
                          .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
                          .ToList();
                        var X = me.Position.X;
                        var Y = me.Position.Y;
                        var pos = new Vector3(X, Y, me.Position.Z);
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
                        uint elsecount = 0;
                        if (blink != null
                            && blink.CanBeCasted()
                            && me.Distance2D(e.Position) > 300
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
                        {
                            blink.UseAbility(e.Position);
                        }
                        elsecount++;
                        if ( // vail
                            vail != null
                            && vail.CanBeCasted()
                            && me.CanCast()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                            && !e.IsMagicImmune()
                            && Utils.SleepCheck("vail")
                            && me.Distance2D(e) <= 1500
                            )
                        {
                            vail.UseAbility(e.Position);
                        }
                        else elsecount++;
                        if (orchid != null && orchid.CanBeCasted() &&
                                Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
                        {
                            orchid.UseAbility(e);
                        }
                        else elsecount++;
                        if (sheep != null && sheep.CanBeCasted() &&
                                      Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
                        {
                            sheep.UseAbility(e);
                        }
                        else elsecount++;
                        if (ethereal != null
                            && ethereal.CanBeCasted()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                        {
                            ethereal.UseAbility(e);
                        }
                        else elsecount++;
                        if (Q != null
                            && Q.CanBeCasted()
                            && (blink == null
                            || !blink.CanBeCasted())
                            && me.Distance2D(e) < Q.GetCastRange()
                            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
                        {
                            Q.UseAbility(pos);
                        }
                        else elsecount++;

                        if (W != null && W.CanBeCasted() && me.Distance2D(e) < 390 &&
                                Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
                        {
                            W.UseAbility();
                        }
                        else elsecount++;
                        if (// Dagon
                             elsecount == 7 &&
                                me.CanCast()
                                && dagon != null
                                && (ethereal == null
                                || (e.HasModifier("modifier_item_ethereal_blade_slow")
                                || ethereal.Cooldown < 17))
                                && !e.IsLinkensProtected()
                                && dagon.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                && !e.IsMagicImmune()
                                && Utils.SleepCheck("dagon")
                                )
                        {
                            dagon.UseAbility(e);
                            Utils.Sleep(200, "dagon");
                        }
                        else elsecount++;  // Dagon Item end
                        if (elsecount == 8 && Shiva != null
                                && Shiva.CanBeCasted()
                                && me.Distance2D(e) <= 600
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name))
                        {
                            Shiva.UseAbility();
                        }
                        else elsecount++;
                        if (elsecount == 9 && R != null && R.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
                                                                 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
                            Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name) && Utils.SleepCheck("R"))
                        {
                            R.UseAbility(e.Position);
                            Utils.Sleep(100, "R");
                        }
                    }
					if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
					{
						Orbwalking.Orbwalk(e, 0, 1600, true, true);
					}
				}
				Utils.Sleep(250, "combo");
			}
			//Escape-combo
			if (Game.IsKeyDown(Menu.Item("Escape").GetValue<KeyBind>().Key) && me.Distance2D(e) <= 1200 &&
				e.IsVisible && e.IsAlive && !e.IsMagicImmune() &&
				Utils.SleepCheck("combo2"))
			{
			   

                if (me.CanCast())
				{
					var X = me.Position.X;
					var Y = me.Position.Y;
					var pos = new Vector3(X, Y, me.Position.Z);
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
                    uint elsecount = 0;
                    if (blink != null 
                        && blink.CanBeCasted() 
                        && me.Distance2D(e.Position) > 300 
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
                    {
                        blink.UseAbility(e.Position);
                    }
                    elsecount++;
                    if ( // vail
						vail != null
						&& vail.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
						&& !e.IsMagicImmune()
						&& Utils.SleepCheck("vail")
						&& me.Distance2D(e) <= 1500
						)
					{
						vail.UseAbility(e.Position);
                    }
                    else elsecount++;
                    if (orchid != null && orchid.CanBeCasted() &&
                            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
                    {
                        orchid.UseAbility(e);
                    }
                    else elsecount++;
                    if (sheep != null && sheep.CanBeCasted() &&
                                  Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
                    {
                        sheep.UseAbility(e);
                    }
                    else elsecount++;
                    if (ethereal != null
						&& ethereal.CanBeCasted()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
					{
						ethereal.UseAbility(e);
					}
                    else elsecount++;
                        if (Q != null 
							&& Q.CanBeCasted() 
							&& ( blink == null 
							|| !blink.CanBeCasted())
							&& me.Distance2D(e) < Q.GetCastRange() 
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
						{
							Q.UseAbility(pos);
                    }
                    else elsecount++;

                    if (W != null && W.CanBeCasted() && me.Distance2D(e) < 390 &&
							Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
						{
							W.UseAbility();
                    }
                    else elsecount++;
                    if (// Dagon
                         elsecount == 7 &&
                            me.CanCast()
                            && dagon != null
                            && (ethereal == null
                            || (e.HasModifier("modifier_item_ethereal_blade_slow")
                            || ethereal.Cooldown < 17))
                            && !e.IsLinkensProtected()
                            && dagon.CanBeCasted()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                            && !e.IsMagicImmune()
                            && Utils.SleepCheck("dagon")
                            )
                        {
                            dagon.UseAbility(e);
                            Utils.Sleep(200, "dagon");
                    }
                    else elsecount++;  // Dagon Item end
                    if (Shiva != null
							&& Shiva.CanBeCasted()
							&& me.Distance2D(e) <= 600
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name))
						{
							Shiva.UseAbility();
                    }
                    else elsecount++;
                    if (elsecount ==9 && E != null
                            && E.CanBeCasted()
                            && D != null
                            && D.CanBeCasted()
                            &&
                            ((sheep == null || !sheep.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
                             && (Shiva == null || !Shiva.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name))
                             && (dagon == null || !dagon.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                             && (ethereal == null || !ethereal.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                             && (orchid == null || !orchid.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
                             && (blink == null || !blink.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
                             && (W == null || !W.CanBeCasted() || me.Distance2D(e) >= 400 || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
                            && Utils.SleepCheck("1")))
                    {
                        E.UseAbility();
                        Utils.Sleep(250, "1");
                    }
                    if (
							D != null
							&& D.CanBeCasted()
							&& (sheep == null || !sheep.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
							&& (Shiva == null || !Shiva.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name))
							&& (dagon == null || !dagon.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
							&& (ethereal == null || !ethereal.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
							&& (orchid == null || !orchid.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
							&& (blink == null || !blink.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
							&& (W == null || !W.CanBeCasted() || me.Distance2D(e) >= 400 || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
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
									if (baseDota[i].Distance2D(me) >= 1200)
									{
										D.UseAbility();
									}
								}
							}
							Utils.Sleep(200, "12");
                    }
				}
				if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
				{
					Orbwalking.Orbwalk(e, 0, 1600, true, true);
				}
				Utils.Sleep(300, "combo2");
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("Slon|Modif by Vick", "0.1");

			Print.LogMessage.Success("I find myself strangely drawn to this odd configuration of activity.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("Full", "Full Combo(Please use only if you have blink)").SetValue(new KeyBind('A', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("Escape", "Escape Combo(Please use only if you have blink)").SetValue(new KeyBind('D', KeyBindType.Press)));

            Menu.AddItem(new MenuItem("Heel", "Min targets to Ult(Full Combo)").SetValue(new Slider(2, 1, 5)));
            Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"puck_dream_coil", true},
					{"puck_ethereal_jaunt", true},
					{"puck_phase_shift", true},
					{"puck_waning_rift", true},
					{"puck_illusory_orb", true}
				})));

			Menu.AddItem(
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