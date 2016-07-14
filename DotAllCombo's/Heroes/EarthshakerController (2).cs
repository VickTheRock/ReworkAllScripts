namespace DotaAllCombo.Heroes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;

	using Service;
	using Service.Debug;

    internal class EarthshakerController : Variables, IHeroController
    {
        private Ability Q, W, E, R;
        private readonly Menu skills = new Menu("Skills", "Skills");
        private readonly Menu items = new Menu("Items", "Items");
        private readonly Menu ult = new Menu("AutoUlt", "AutoUlt");
        
        private Item orchid, sheep, vail, soul, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost;
		private int[] eDmg;
		private int[] rDmg;
	    private int[] wDmg;
		private int[] qDmg;
		public void Combo()
		{
            e = me.ClosestToMouseTarget(2000);
            if (e == null) return;


            //spell
            Q = me.Spellbook.SpellQ;

            W = me.Spellbook.SpellW;

			E = me.Spellbook.SpellE;

			R = me.Spellbook.SpellR;

            // Item
            ethereal = me.FindItem("item_ethereal_blade");

            sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

            vail = me.FindItem("item_veil_of_discord");

            cheese = me.FindItem("item_cheese");

            ghost = me.FindItem("item_ghost");

            orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");

            atos = me.FindItem("item_rod_of_atos");

            soul = me.FindItem("item_soul_ring");

            arcane = me.FindItem("item_arcane_boots");

            blink = me.FindItem("item_blink");

            shiva = me.FindItem("item_shivas_guard");

            dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;

            
            var modifEther = e.HasModifier("modifier_item_ethereal_blade_slow");
            var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");
			
            if (Active && me.IsAlive && e.IsAlive && Utils.SleepCheck("activated"))
            {
                var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");
                if (e.IsVisible && me.Distance2D(e) <= 2300 && !noBlade)
                {
	                if (me.HasModifier("modifier_earthshaker_enchant_totem") && me.Distance2D(e) <= 300 && Utils.SleepCheck("WMod"))
	                {
						me.Attack(e);
						Utils.Sleep(250, "WMod");
					}
                    if ( // atos Blade
                        atos != null
                        && atos.CanBeCasted()
                        && me.CanCast()
                        && !e.IsLinkensProtected()
                        && !e.IsMagicImmune()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos.Name)
                        && me.Distance2D(e) <= 2000
                        && Utils.SleepCheck("atos")
                        )
                    {
                        atos.UseAbility(e);
                        Utils.Sleep(250, "atos");
                    } // atos Item end

                    if (
                        blink != null
                        && me.CanCast()
                        && blink.CanBeCasted()
                        && me.Distance2D(e) > 400
						&& me.Distance2D(e) <= 1180
						&& !stoneModif
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                        && Utils.SleepCheck("blink")
                        )
                    {
                        blink.UseAbility(e.Position);
                        Utils.Sleep(250, "blink");
                    }
                    if ( // orchid
                        orchid != null
                        && orchid.CanBeCasted()
                        && me.CanCast()
                        && !e.IsLinkensProtected()
                        && !e.IsMagicImmune()
                        && me.Distance2D(e) <= 1400
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
                        && !stoneModif
                        && Utils.SleepCheck("orchid")
                        )
                    {
                        orchid.UseAbility(e);
                        Utils.Sleep(250, "orchid");
                    } // orchid Item end
                    if (!orchid.CanBeCasted() || orchid == null ||
                        !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
                    {
                        if ( // vail
                            vail != null
                            && vail.CanBeCasted()
                            && me.CanCast()
                            && !e.IsMagicImmune()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                            && me.Distance2D(e) <= 1500
                            && Utils.SleepCheck("vail")
                            )
                        {
                            vail.UseAbility(e.Position);
                            Utils.Sleep(250, "vail");
                        } // orchid Item end
                        if (!vail.CanBeCasted() || vail == null ||
                            !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name))
                        {
                            if ( // ethereal
                                ethereal != null
                                && ethereal.CanBeCasted()
                                && me.CanCast()
                                && !e.IsLinkensProtected()
                                && !e.IsMagicImmune()
                                && !stoneModif
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                                && Utils.SleepCheck("ethereal")
                                )
                            {
                                ethereal.UseAbility(e);
                                Utils.Sleep(200, "ethereal");
                            } // ethereal Item end
                            if (!ethereal.CanBeCasted() || ethereal == null ||
                                !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                            {
                                if (
                                    W != null
                                    && W.CanBeCasted()
                                    && me.CanCast()
									&& !me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && me.Distance2D(e) < 2300
									&& me.Distance2D(e)>= 1200
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                    && Utils.SleepCheck("W"))
                                {
                                    W.UseAbility();
                                    Utils.Sleep(200, "W");
                                }
								if (
									W != null
									&& W.CanBeCasted()
									&& me.CanCast()
									&& !me.HasModifier("modifier_earthshaker_enchant_totem")
									&& me.Distance2D(e) < W.GetCastRange()
									&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
									&& Utils.SleepCheck("W"))
								{
									W.UseAbility();
									Utils.Sleep(200, "W");
								}
								if (
                                    Q != null
                                    && Q.CanBeCasted()
                                    && (e.IsLinkensProtected()
                                        || !e.IsLinkensProtected())
                                    && me.CanCast()
                                    && me.Distance2D(e) < 1500
                                    && !stoneModif
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                    && Utils.SleepCheck("Q")
                                    )
                                {
                                    Q.UseAbility(e);
                                    Utils.Sleep(330, "Q");
                                }
                                if ( // SoulRing Item 
                                    soul != null
                                    && soul.CanBeCasted()
                                    && me.CanCast()
                                    && me.Health >= (me.MaximumHealth * 0.6)
                                    && me.Mana <= R.ManaCost
                                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name)
                                    )
                                {
                                    soul.UseAbility();
                                } // SoulRing Item end

                                if ( // Arcane Boots Item
                                    arcane != null
                                    && arcane.CanBeCasted()
                                    && me.CanCast()
                                    && me.Mana <= R.ManaCost
                                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
                                    )
                                {
                                    arcane.UseAbility();
                                } // Arcane Boots Item end

                                if ( //Ghost
                                    ghost != null
                                    && ghost.CanBeCasted()
                                    && me.CanCast()
                                    && ((me.Position.Distance2D(e) < 300
                                         && me.Health <= (me.MaximumHealth * 0.7))
                                        || me.Health <= (me.MaximumHealth * 0.3))
                                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
                                    && Utils.SleepCheck("Ghost"))
                                {
                                    ghost.UseAbility();
                                    Utils.Sleep(250, "Ghost");
                                }


                                if ( // Shiva Item
                                    shiva != null
                                    && shiva.CanBeCasted()
                                    && me.CanCast()
                                    && !e.IsMagicImmune()
                                    && Utils.SleepCheck("shiva")
                                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
                                    && me.Distance2D(e) <= 600
                                    )
                                {
                                    shiva.UseAbility();
                                    Utils.Sleep(250, "shiva");
                                } // Shiva Item end
                                if ( // sheep
                                    sheep != null
                                    && sheep.CanBeCasted()
                                    && me.CanCast()
                                    && !e.IsLinkensProtected()
                                    && !e.IsMagicImmune()
									&& !e.IsRooted()
									&& !e.IsHexed()
									&& !e.IsStunned()
                                    && me.Distance2D(e) <= 1400
                                    && !stoneModif
                                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
                                    && Utils.SleepCheck("sheep")
                                    )
                                {
                                    sheep.UseAbility(e);
                                    Utils.Sleep(250, "sheep");
                                } // sheep Item end

                                if ( // Dagon
                                    me.CanCast()
                                    && dagon != null
                                    && (ethereal == null
                                        || (modifEther
                                            || ethereal.Cooldown < 17))
                                    && !e.IsLinkensProtected()
                                    && dagon.CanBeCasted()
                                    && me.Distance2D(e) <= 1400
                                    && !e.IsMagicImmune() 
									&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
									&& !stoneModif
                                    && Utils.SleepCheck("dagon")
                                    )
                                {
                                    dagon.UseAbility(e);
                                    Utils.Sleep(200, "dagon");
                                } // Dagon Item end

                                if (
                                    // cheese
                                    cheese != null
                                    && cheese.CanBeCasted()
                                    && me.Health <= (me.MaximumHealth * 0.3)
                                    && me.Distance2D(e) <= 700
                                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
                                    && Utils.SleepCheck("cheese")
                                    )
                                {
                                    cheese.UseAbility();
                                    Utils.Sleep(200, "cheese");
                                } // cheese Item end
                            }
                        }
                    }
                    Utils.Sleep(200, "activated");
                }
            }
            A();
        }

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

			Print.LogMessage.Success("Ah these mortals and their futile games. Oh wait! I'm one of them!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

			Menu.AddItem(new MenuItem("keyQ", "Farm Creep Key").SetValue(new KeyBind('F', KeyBindType.Press)));

			skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"earthshaker_echo_slam", true},
			    {"earthshaker_enchant_totem", true},
			    {"earthshaker_fissure", true}
			})));
			items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_dagon", true},
				{"item_orchid", true},
                { "item_bloodthorn", true},
			    {"item_ethereal_blade", true},
			    {"item_veil_of_discord", true},
			    {"item_rod_of_atos", true},
			    {"item_sheepstick", true},
			    {"item_arcane_boots", true},
			    {"item_shivas_guard",true},
			    {"item_blink", true},
			    {"item_soul_ring", true},
			    {"item_ghost", true},
			    {"item_cheese", true}
			})));
			ult.AddItem(new MenuItem("autoUlt", "AutoUlt").SetValue(true));
			ult.AddItem(new MenuItem("AutoUlt", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"earthshaker_fissure", true},
				{"earthshaker_echo_slam", true}
			})));
			items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"zuus_arc_lightning", true}
			})));
			ult.AddItem(new MenuItem("Heel", "Min targets to ult").SetValue(new Slider(2, 1, 5)));
			Menu.AddSubMenu(skills);
			Menu.AddSubMenu(items);
			Menu.AddSubMenu(ult);
		}

		public void OnCloseEvent()
		{
			
		}
		private Unit GetLowestToQ(List<Hero> units, Unit z)
		{

			Q = me.Spellbook.SpellQ;
			qDmg = new[] {0, 110, 160, 210, 260 };
			eDmg = new[] { 0, 50, 75, 100, 125 };
			rDmg =  new[] { 0 , 160, 210, 270 };
			int[] creepsDmg = { 0, 40, 55, 70 };
			//int[] enemyDmg = { 20, 40, 60, 80 };
			int enemiesCount;
			int creepsECount;
			double damage = 0;
			foreach (var v in units.Where(x => !x.IsMagicImmune()))
			{
				creepsECount = ObjectManager.GetEntities<Unit>().Where(creep =>
					(creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
					 || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
					 || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
					 || creep.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling
					 || creep.ClassID == ClassID.CDOTA_Unit_SpiritBear
					 || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
					 || creep.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem
					 || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep
					 ||creep.HasInventory ) &&
					creep.IsAlive && creep.Team != me.Team && creep.IsVisible && me.Distance2D(creep) <= 600+me.HullRadius &&
					creep.IsSpawned).ToList().Count();



				enemiesCount = ObjectManager.GetEntities<Hero>().Where(x =>
					x.Team != me.Team && x.IsAlive && x.IsVisible && me.Distance2D(x) <= 600 + me.HullRadius).ToList().Count();
				/*if (enemiesCount == 0)
				{
					enemiesCount = 0;
				}*/


				if (creepsECount == 0)
				{
					creepsECount = 0;
				}
				damage = ((creepsECount * creepsDmg[R.Level]) +
							 rDmg[R.Level]) * (1 - v.MagicDamageResist);
				if (me.Distance2D(v) < E.CastRange)
					damage = damage + eDmg[E.Level] * (1 - v.MagicDamageResist);
				if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
				{
					damage = ((creepsECount * creepsDmg[R.Level]) +
								 rDmg[R.Level]) *
										   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist);
					if (me.Distance2D(v) < E.CastRange)
						damage = damage + eDmg[E.Level] *
										   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist);
				}
				if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
					v.Spellbook.SpellR.CanBeCasted())
					damage = 0;
				var rum = v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb");
				if (rum) damage = damage * 0.5;
				var mom = v.HasModifier("modifier_item_mask_of_madness_berserk");
				if (mom) damage = damage * 1.3;
				//Console.WriteLine(damage);

				if (damage >= v.Health && z.Distance2D(v) <= Q.CastRange)
					return v;
			}
			return null;
		}
		public void A()
		{
            if (!me.IsAlive)return;
			var enemies =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsIllusion).ToList();
            if(enemies.Count<=0)return;
			if (Menu.Item("autoUlt").GetValue<bool>() && me.IsAlive)
			{
				foreach (var v in enemies)
				{
						rDmg = me.AghanimState()? new[] { 440, 540, 640 }: new[] { 225, 325, 425 };

						qDmg = new[] { 85, 100, 115, 145 };

						wDmg = new [] { 100, 175, 275, 350 };

					var lens = me.HasModifier("modifier_item_aether_lens");
					var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
					var damageR = Math.Floor(rDmg[R.Level - 1] * (1 - v.MagicDamageResist));
					if (me.Distance2D(v) < E.CastRange)
						damageR = damageR + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health * (1 - v.MagicDamageResist);
					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damageR =
							Math.Floor(rDmg[R.Level - 1] *
									   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
						if (me.Distance2D(v) < 1150)
							damageR = damageR + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health * (1 - v.MagicDamageResist);
					}
					if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
						v.Spellbook.SpellR.CanBeCasted())
						damageR = 0;
					if (lens) damageR = damageR * 1.08;
					if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageR = damageR * 0.5;
					if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageR = damageR * 1.3;
					damageR = damageR*spellamplymult;

					var damageW = Math.Floor(wDmg[W.Level - 1] * (1 - v.MagicDamageResist));
					if (me.Distance2D(v) < 1150)
						damageW = damageW + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health * (1 - v.MagicDamageResist);
					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damageW =
							Math.Floor(wDmg[W.Level - 1] *
									   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
						if (me.Distance2D(v) < 1150)
							damageW = damageW + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health * (1 - v.MagicDamageResist);
					}
					
					if (lens) damageW = damageW * 1.08;
					if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageW = damageW * 0.5;
					if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageW = damageW * 1.3;
					damageW = damageW * spellamplymult;

					var damageQ = Math.Floor(qDmg[Q.Level - 1] * (1 - v.MagicDamageResist));
					if (me.Distance2D(v) < 1150)
						damageQ = damageQ + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health * (1 - v.MagicDamageResist);
					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damageQ =
							Math.Floor(qDmg[Q.Level - 1] *
									   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
						if (me.Distance2D(v) < 1150)
							damageQ = damageQ + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health * (1 - v.MagicDamageResist);
					}
					if (lens) damageQ = damageQ * 1.08;
					damageQ = damageQ * spellamplymult;
					if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageQ = damageQ * 0.5;
					if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageQ = damageQ * 1.3;
					if ( // vail
						vail != null
						&& vail.CanBeCasted()
						&& W.CanBeCasted()
						&& v.Health<= damageW * 1.25
						&& v.Health >= damageW
						&& me.CanCast()
						&& !v.HasModifier("modifier_item_veil_of_discord_debuff")
						&& !v.IsMagicImmune()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
						&& me.Distance2D(v) <= W.GetCastRange()
						&& Utils.SleepCheck("vail")
						)
					{
						vail.UseAbility(v.Position);
						Utils.Sleep(250, "vail");
					}
					int etherealdamage = (int)(((me.TotalIntelligence * 2) + 75));
					if ( // vail
					  ethereal != null
					  && ethereal.CanBeCasted()
					  && ((W!=null
					  && W.CanBeCasted()
					  && v.Health <= etherealdamage+ damageW * 1.4
					  && v.Health >= damageW)
					  || (Q!=null 
					  && Q.CanBeCasted()
					  && v.Health <= etherealdamage+(damageQ * 1.4)
					  && v.Health >= damageQ)
					  || (R!=null 
					  && R.CanBeCasted()
					  && v.Health <= etherealdamage + (damageR * 1.4)
					  && enemies.Count(x => x.Health <= (damageR - 20)) >= (Menu.Item("Heel").GetValue<Slider>().Value)
					  && v.Health >= damageR))
					  && me.CanCast()
					  && !v.HasModifier("modifier_item_ethereal_blade_slow")
					  && !v.IsMagicImmune()
					  && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
					  && me.Distance2D(v) <= ethereal.GetCastRange()+50
					  && Utils.SleepCheck("ethereal")
					  )
					{
						ethereal.UseAbility(v);
						Utils.Sleep(250, "ethereal");
					}
					if (
						soul != null
						&& soul.CanBeCasted()
						&& Utils.SleepCheck(v.Handle.ToString())
						&& me.Mana < R.ManaCost
						&& me.Mana + 150 > R.ManaCost
						)
					{
						soul.UseAbility();
						Utils.Sleep(150, v.Handle.ToString());
					}
					if (arcane != null
						&& arcane.CanBeCasted()
						&& Utils.SleepCheck(v.Handle.ToString())
						&& me.Mana < R.ManaCost
						&& me.Mana + 135 > R.ManaCost)
					{
						arcane.UseAbility();
						Utils.Sleep(150, v.Handle.ToString());
					}
					if (arcane != null
						&& soul != null
						&& Utils.SleepCheck(v.Handle.ToString())
						&& arcane.CanBeCasted() && soul.CanBeCasted()
						&& me.Mana < R.ManaCost
						&& me.Mana + 285 > R.ManaCost)
					{
						arcane.UseAbility();
						soul.UseAbility();
						Utils.Sleep(150, v.Handle.ToString());
					}
					if (Q != null && v != null && Q.CanBeCasted()
						&& me.Distance2D(v) <= Q.GetCastRange() + 50
						&& !v.HasModifier("modifier_tusk_snowball_movement")
						&& !v.HasModifier("modifier_snowball_movement_friendly")
						&& !v.HasModifier("modifier_templar_assassin_refraction_absorb")
						&& !v.HasModifier("modifier_ember_spirit_flame_guard")
						&& !v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
						&& !v.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						&& !v.HasModifier("modifier_puck_phase_shift")
						&& !v.HasModifier("modifier_eul_cyclone")
						&& !v.HasModifier("modifier_dazzle_shallow_grave")
						&& !v.HasModifier("modifier_shadow_demon_disruption")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_storm_spirit_ball_lightning")
						&& !v.HasModifier("modifier_ember_spirit_fire_remnant")
						&& !v.HasModifier("modifier_nyx_assassin_spiked_carapace")
						&& !v.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
						&& !v.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
						!v.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
						&& !v.IsMagicImmune()
						&& Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						&& v.Health < damageQ
						&& Utils.SleepCheck(v.Handle.ToString()))
					{
						Q.UseAbility(v);
						Utils.Sleep(150, v.Handle.ToString());
						return;
					}
					if (W != null && v != null && W.CanBeCasted()
						&& me.Distance2D(v)<= W.GetCastRange()+50
						&& !v.HasModifier("modifier_tusk_snowball_movement")
						&& !v.HasModifier("modifier_snowball_movement_friendly")
						&& !v.HasModifier("modifier_templar_assassin_refraction_absorb")
						&& !v.HasModifier("modifier_ember_spirit_flame_guard")
						&& !v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
						&& !v.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						&& !v.HasModifier("modifier_puck_phase_shift")
						&& !v.HasModifier("modifier_eul_cyclone")
						&& !v.HasModifier("modifier_dazzle_shallow_grave")
						&& !v.HasModifier("modifier_shadow_demon_disruption")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_storm_spirit_ball_lightning")
						&& !v.HasModifier("modifier_ember_spirit_fire_remnant")
						&& !v.HasModifier("modifier_nyx_assassin_spiked_carapace")
						&& !v.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
						&& !v.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
						!v.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
						&& !v.IsMagicImmune()
						&& Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(W.Name)
						&& v.Health < damageW
						&& Utils.SleepCheck(v.Handle.ToString()))
					{
						W.UseAbility(v.Position);
						Utils.Sleep(150, v.Handle.ToString());
						return;
					}
					if (R != null && v != null && R.CanBeCasted()
						&& !v.HasModifier("modifier_tusk_snowball_movement")
						&& !v.HasModifier("modifier_snowball_movement_friendly")
						&& !v.HasModifier("modifier_templar_assassin_refraction_absorb")
						&& !v.HasModifier("modifier_ember_spirit_flame_guard")
						&& !v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
						&& !v.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						&& !v.HasModifier("modifier_puck_phase_shift")
						&& !v.HasModifier("modifier_eul_cyclone")
						&& !v.HasModifier("modifier_dazzle_shallow_grave")
						&& !v.HasModifier("modifier_shadow_demon_disruption")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !v.HasModifier("modifier_storm_spirit_ball_lightning")
						&& !v.HasModifier("modifier_ember_spirit_fire_remnant")
						&& !v.HasModifier("modifier_nyx_assassin_spiked_carapace")
						&& !v.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
						&& !v.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
						!v.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
						&& !v.IsMagicImmune()
						&& Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name)
						&& enemies.Count(x => x.Health <= (damageR - 20)) >= (Menu.Item("Heel").GetValue<Slider>().Value)
						&& Utils.SleepCheck(v.Handle.ToString()))
					{
						R.UseAbility();
						Utils.Sleep(150, v.Handle.ToString());
						return;
					}
					
				}
			}
		}
	}
}