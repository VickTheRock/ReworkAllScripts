namespace DotaAllCombo.Heroes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using SharpDX;
	using Service;
	using Service.Debug;

	internal class TemplarAssassinController : Variables, IHeroController
	{
		private Ability Q, W, R;
		private Item urn, dagon, phase, cheese, halberd, ethereal,
					mjollnir, orchid, abyssal, stick, mom, Shiva, mail, bkb, satanic, medall, blink, sheep, manta, pike;
		private bool Active;
		private float range_atck;
		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Well that's it. The secret is out!");

			menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"templar_assassin_meld", true},
				    {"templar_assassin_refraction", true},
				    {"templar_assassin_psionic_trap", true}
				})));
			menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_ethereal_blade", true},
				    {"item_blink", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
				    {"item_bloodthorn", true},
				    {"item_urn_of_shadows", true},
				    {"item_abyssal_blade", true},
				    {"item_shivas_guard", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true},
				    {"item_medallion_of_courage", true},
				    {"item_solar_crest", true}
				})));
			menu.AddItem(
			   new MenuItem("Item", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			   {
			       {"item_hurricane_pike", true},
			       {"item_mask_of_madness", true},
			       {"item_sheepstick", true},
			       {"item_cheese", true},
			       {"item_magic_stick", true},
			       {"item_magic_wand", true},
			       {"item_manta", true},
			       {"item_mjollnir", true},
			       {"item_satanic", true},
			       {"item_phase_boots", true}
			   })));
			menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			menu.AddItem(new MenuItem("piKe", "Min targets healh use hurricane pike").SetValue(new Slider(35, 20, 100)));
			menu.AddItem(new MenuItem("piKeMe", "Min me healh use hurricane pike").SetValue(new Slider(45, 20, 100)));
			menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
		}
		public void Combo()
		{
			if (menu.Item("enabled").IsActive() && Utils.SleepCheck("combo"))
			{

				e = me.ClosestToMouseTarget(2500);
				if (e == null)
					return;
				Q = me.Spellbook.SpellQ;
				W = me.Spellbook.SpellW;
				R = me.Spellbook.SpellR;
				Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);

				Shiva = me.FindItem("item_shivas_guard");
				var pike = me.FindItem("item_hurricane_pike");
				var dragon = me.FindItem("item_dragon_lance");
				
				ethereal = me.FindItem("item_ethereal_blade");
				mom = me.FindItem("item_mask_of_madness");
				urn = me.FindItem("item_urn_of_shadows");
				dagon = me.Inventory.Items.FirstOrDefault(
						item => item.Name.Contains("item_dagon"));
				halberd = me.FindItem("item_heavens_halberd");
				mjollnir = me.FindItem("item_mjollnir");
				orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
				abyssal = me.FindItem("item_abyssal_blade");
				mail = me.FindItem("item_blade_mail");
				manta = me.FindItem("item_manta");
				bkb = me.FindItem("item_black_king_bar");
				satanic = me.FindItem("item_satanic");
				blink = me.FindItem("item_blink");
				medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
				sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
				cheese = me.FindItem("item_cheese");
				stick = me.FindItem("item_magic_stick") ?? me.FindItem("item_magic_wand");
				phase = me.FindItem("item_phase_boots");

				var Meld = me.Modifiers.ToList().Exists(y => y.Name == "modifier_templar_assassin_meld");
				var pikeMod = me.Modifiers.ToList().Exists(y => y.Name == "modifier_item_hurricane_pike_range");
			    Toolset.Range();
			    var stoneModif = e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
				var v =
					ObjectManager.GetEntities<Hero>()
						.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
						.ToList();

				var pike_range = me.Modifiers.FirstOrDefault(y => y.Name == "modifier_item_hurricane_pike_range");
				if (pike_range != null)
				{
					if (
						Q != null
						&& Q.CanBeCasted()
						&& !Meld
						&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						Q.UseAbility();
						Utils.Sleep(200, "Q");
					}
					if (
						W != null
						&& Q != null
						&& pike_range.StackCount <= 3
						&& !Q.CanBeCasted()
						&& W.CanBeCasted()
						&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
						&& Utils.SleepCheck("W")
						)
					{
						W.UseAbility();
						me.Attack(e);
						Utils.Sleep(200, "W");
					}
					if (Utils.SleepCheck("attack"))
					{
						me.Attack(e);
						Utils.Sleep(100, "attack");
					}
				}
				if (pike_range != null && pike_range.StackCount > 0) return;
				if (pikeMod) return;
				if (Active)
				{
					if (Meld && me.Distance2D(e) <= 400 + Toolset.AttackRange && Utils.SleepCheck("attack"))
					{
						me.Attack(e);
						Utils.Sleep(100, "attack");
					}
				}

				if (Meld) return;

				if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !me.IsInvisible())
				{
					if (R != null
						&& (pike == null
						|| !pike.CanBeCasted())
                        && me.Distance2D(e)>=Toolset.AttackRange+5
						&& R.CanBeCasted()
						&& !Meld
						&& Utils.SleepCheck("R")
						&& !e.Modifiers.ToList().Exists(x => x.Name == "modifier_templar_assassin_trap_slow")
						&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name))
					{
						R.UseAbility(Prediction.InFront(e, 140));
						Utils.Sleep(150, "R");
					}
					if (
						Q != null && Q.CanBeCasted() 
						&& me.Distance2D(e) <= Toolset.AttackRange+300
						&& !Meld
						&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						Q.UseAbility();
						Utils.Sleep(200, "Q");
					}
					float angle = me.FindAngleBetween(e.Position, true);
					Vector3 pos = new Vector3((float)(e.Position.X + 100 * Math.Cos(angle)), (float)(e.Position.Y + 100 * Math.Sin(angle)), 0);
					if (
						blink != null
						&& me.CanCast()
						&& blink.CanBeCasted()
						&& me.Distance2D(e) >= Toolset.AttackRange
						&& me.Distance2D(pos) <= 1190
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						blink.UseAbility(pos);
						Utils.Sleep(250, "blink");
					}
					if (
						W != null && W.CanBeCasted() && me.Distance2D(e) <= Toolset.AttackRange-10
						&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
						&& Utils.SleepCheck("W")
						)
					{
						W.UseAbility();
						me.Attack(e);
						Utils.Sleep(200, "W");
					}
					if ( // MOM
						mom != null
						&& mom.CanBeCasted()
						&& me.CanCast()
						&& !Meld
						&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(mom.Name)
						&& Utils.SleepCheck("mom")
						&& me.Distance2D(e) <= 700
						)
					{
						mom.UseAbility();
						Utils.Sleep(250, "mom");
					}
					if ( // Hellbard
						halberd != null
						&& halberd.CanBeCasted()
						&& me.CanCast()
						&& !e.IsMagicImmune()
						&& (e.NetworkActivity == NetworkActivity.Attack
						|| e.NetworkActivity == NetworkActivity.Crit
						|| e.NetworkActivity == NetworkActivity.Attack2)
						&& Utils.SleepCheck("halberd")
						&& me.Distance2D(e) <= 700
						&& !Meld
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
						)
					{
						halberd.UseAbility(e);
						Utils.Sleep(250, "halberd");
					}
					if ( // Mjollnir
						mjollnir != null
						&& mjollnir.CanBeCasted()
						&& me.CanCast()
						&& !Meld
						&& !e.IsMagicImmune()
						&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
						&& Utils.SleepCheck("mjollnir")
						&& me.Distance2D(e) <= 900
						)
					{
						mjollnir.UseAbility(me);
						Utils.Sleep(250, "mjollnir");
					} // Mjollnir Item end
					if (
						// cheese
						cheese != null
						&& cheese.CanBeCasted()
						&& !Meld
						&& me.Health <= (me.MaximumHealth * 0.3)
						&& me.Distance2D(e) <= 700
						&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
						&& Utils.SleepCheck("cheese")
						)
					{
						cheese.UseAbility();
						Utils.Sleep(200, "cheese");
					} // cheese Item end
					if ( // Medall
						medall != null
						&& medall.CanBeCasted()
						&& !Meld
						&& Utils.SleepCheck("Medall")
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
						&& me.Distance2D(e) <= 700
						)
					{
						medall.UseAbility(e);
						Utils.Sleep(250, "Medall");
					} // Medall Item end

					if ( // sheep
						sheep != null
						&& sheep.CanBeCasted()
						&& me.CanCast()
						&& !e.IsLinkensProtected()
						&& !e.IsMagicImmune()
						&& !Meld
						&& me.Distance2D(e) <= 1400
						&& !stoneModif
						&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
						&& Utils.SleepCheck("sheep")
						)
					{
						sheep.UseAbility(e);
						Utils.Sleep(250, "sheep");
					} // sheep Item end
					if ( // Abyssal Blade
						abyssal != null
						&& abyssal.CanBeCasted()
						&& me.CanCast()
						&& !Meld
						&& !e.IsStunned()
						&& !e.IsHexed()
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
						&& Utils.SleepCheck("abyssal")
						&& me.Distance2D(e) <= 400
						)
					{
						abyssal.UseAbility(e);
						Utils.Sleep(250, "abyssal");
					} // Abyssal Item end
					if (orchid != null 
						&& orchid.CanBeCasted()
						&& !Meld
						&& me.Distance2D(e) <= 600
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) &&
						Utils.SleepCheck("orchid"))
					{
						orchid.UseAbility(e);
						Utils.Sleep(100, "orchid");
					}

					if (Shiva != null 
						&& Shiva.CanBeCasted() 
						&& me.Distance2D(e) <= 600
						&& !Meld
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
						&& !e.IsMagicImmune() && Utils.SleepCheck("Shiva"))
					{
						Shiva.UseAbility();
						Utils.Sleep(100, "Shiva");
					}
					if ( // ethereal
						ethereal != null
						&& ethereal.CanBeCasted()
						&& me.CanCast()
						&& !Meld
						&& !e.IsLinkensProtected()
						&& !e.IsMagicImmune()
						&& !stoneModif
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
						&& Utils.SleepCheck("ethereal")
						)
					{
						ethereal.UseAbility(e);
						Utils.Sleep(200, "ethereal");
					} // ethereal Item end


					if ( // Dagon
						me.CanCast()
						&& dagon != null
						&& (ethereal == null
						|| (e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow")
						|| ethereal.Cooldown < 17))
						&& !e.IsLinkensProtected()
						&& dagon.CanBeCasted()
						&& !Meld
						&& !e.IsMagicImmune()
						&& !stoneModif
						&& Utils.SleepCheck("dagon")
						)
					{
						dagon.UseAbility(e);
						Utils.Sleep(200, "dagon");
					} // Dagon Item end
					if (phase != null
						&& phase.CanBeCasted()
						&& !Meld
						&& Utils.SleepCheck("phase")
						&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(phase.Name)
						&& !blink.CanBeCasted()
						&& me.Distance2D(e) >= me.AttackRange + 20)
					{
						phase.UseAbility();
						Utils.Sleep(200, "phase");
					}
					if (urn != null 
						&& urn.CanBeCasted() 
						&& urn.CurrentCharges > 0 
						&& me.Distance2D(e) <= 400
						&& !Meld
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) 
						&& Utils.SleepCheck("urn"))
					{
						urn.UseAbility(e);
						Utils.Sleep(240, "urn");
					}
					if (
						stick != null
						&& stick.CanBeCasted()
						&& stick.CurrentCharges != 0
						&& me.Distance2D(e) <= 700
						&& !Meld
						&& (me.Health <= (me.MaximumHealth * 0.5)
							|| me.Mana <= (me.MaximumMana * 0.5))
						&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(stick.Name))
					{
						stick.UseAbility();
						Utils.Sleep(200, "mana_items");
					}
					if (manta != null
						&& manta.CanBeCasted()
						&& !Meld
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name)
						&& me.Distance2D(e) <= Toolset.AttackRange
						&& Utils.SleepCheck("manta"))
					{
						manta.UseAbility();
						Utils.Sleep(100, "manta");
					}
					if ( // Satanic 
						satanic != null &&
						me.Health <= (me.MaximumHealth * 0.3) &&
						satanic.CanBeCasted()
						&& !Meld
						&& me.Distance2D(e) <= me.AttackRange + 50
						&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
						&& Utils.SleepCheck("satanic")
						)
					{
						satanic.UseAbility();
						Utils.Sleep(240, "satanic");
					} // Satanic Item end
					if (mail != null 
						&& mail.CanBeCasted() 
						&& (v.Count(x => x.Distance2D(me) <= 650) 
						>= (menu.Item("Heelm").GetValue<Slider>().Value))
						&& !Meld
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) 
						&& Utils.SleepCheck("mail"))
					{
						mail.UseAbility();
						Utils.Sleep(100, "mail");
					}
					if (bkb != null 
						&& bkb.CanBeCasted()
						&& !Meld
						&& (v.Count(x => x.Distance2D(me) <= 650) 
						>= (menu.Item("Heel").GetValue<Slider>().Value)) &&
						menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) 
						&& Utils.SleepCheck("bkb"))
					{
						bkb.UseAbility();
						Utils.Sleep(100, "bkb");
					}
					
					if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive)
					{
						if (me.Distance2D(e) <= Toolset.AttackRange &&
							(!me.IsAttackImmune() 
							|| !e.IsAttackImmune())
							&& !Meld
							&& me.NetworkActivity != NetworkActivity.Attack
							&& me.CanAttack() 
							&& !me.IsAttacking()
							&& Utils.SleepCheck("attack")
							)
						{
							me.Attack(e);
							Utils.Sleep(150, "attack");
						}
						else if (
							(!me.CanAttack() 
							|| me.Distance2D(e) >= 250) 
							&& me.NetworkActivity != NetworkActivity.Attack 
							&& !Meld
							&& me.Distance2D(e) <= 2500
							&& !me.IsAttacking()
							&& Utils.SleepCheck("Move")
							)
						{
							me.Move(Prediction.InFront(e, 100));
							Utils.Sleep(350, "Move");
						}
					}
					if (pike != null 
						&& pike.CanBeCasted() 
						&& me.IsAttacking()
						&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(pike.Name)
						&& (e.Health <= (e.MaximumHealth / 100 * menu.Item("piKe").GetValue<Slider>().Value)
						||  me.Health <= (me.MaximumHealth / 100 * menu.Item("piKeMe").GetValue<Slider>().Value))
                        && (W == null
						|| !W.CanBeCasted())
						&& !Meld
						&& me.Distance2D(e) <= 450 
						&& Utils.SleepCheck("pike"))
					{
						pike.UseAbility(e);
						if (((pike != null && pike.CanBeCasted()) || IsCasted(pike)) && R.CanBeCasted() && !me.Modifiers.ToList().Exists(y => y.Name == "modifier_templar_assassin_meld") && me.Distance2D(e.NetworkPosition) <= 400 && me.CanCast() && !me.IsSilenced() && !me.IsHexed())
						{
							var a1 = me.Position.ToVector2().FindAngleBetween(e.Position.ToVector2(), true);
							var p1 = new Vector3(
								(e.Position.X + 520 * (float)Math.Cos(a1)),
								(e.Position.Y + 520 * (float)Math.Sin(a1)),
								100);
							R.UseAbility(p1);
						}
						Utils.Sleep(100, "pike");
					}
					var traps = ObjectManager.GetEntities<Unit>().Where(x => x.Name == "npc_dota_templar_assassin_psionic_trap" && x.Team == me.Team
									  && x.Distance2D(me) <= 1700 && x.IsAlive && x.IsValid).ToList();
					foreach (var q in traps)
					{
						if (!HurPikeActived() && e.NetworkPosition.Distance2D(q.Position) < 390 && q.Spellbook.SpellQ.CanBeCasted() && Utils.SleepCheck("traps") && !e.Modifiers.ToList().Exists(x => x.Name == "modifier_templar_assassin_trap_slow"))
						{
							q.Spellbook.SpellQ.UseAbility();
							Utils.Sleep(150, "traps");
						}
					}
				}
				Utils.Sleep(50, "combo");
			}
		}
		
		private static bool IsCasted(Ability ability)
		{
			return ability.Level > 0 && ability.CooldownLength > 0 && Math.Ceiling(ability.CooldownLength).Equals(Math.Ceiling(ability.Cooldown));
		}
		public void OnCloseEvent()
		{

		}
		private bool HurPikeActived()
		{
			return (pike != null && (pike.CooldownLength - pike.Cooldown) < 4.5 && pike.Cooldown > 0);
		}
	}
}