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
	using SharpDX.Direct3D9;
	using Service;
	using Service.Debug;

	internal class LegionCommanderController : Variables, IHeroController
    {
        private readonly Menu items = new Menu("Items", "Items");
        private readonly Menu skills = new Menu("Skills", "Skills");
        private Ability R, W, Q;
		private double damage;

		private Font txt;
		private Font noti;
		private Line lines;
		private Item blink, armlet, blademail, bkb, abyssal, mjollnir, halberd, medallion, madness, urn, 
            satanic, solar, dust, sentry, mango, arcane, buckler, crimson, lotusorb, cheese, stick, 
            soul, force, cyclone, sheep, orchid;
		private Unit GetLowestToQ(List<Hero> units, Unit z)
		{

			Q = me.Spellbook.SpellQ;
			int[] qDmg = { 40, 80, 120, 160 };
			int[] creepsDmg = { 14, 16, 18, 20 };
			int[] enemyDmg = { 20, 40, 60, 80 };
			int enemiesCount;
			int creepsECount;

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
					 || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep) &&
					creep.IsAlive && creep.Team != me.Team && creep.IsVisible && v.Distance2D(creep) <= 330 &&
					creep.IsSpawned).ToList().Count();
				enemiesCount = ObjectManager.GetEntities<Hero>().Where(x =>
					x.Team != me.Team && x.IsAlive && x.IsVisible && v.Distance2D(x) <= 330).ToList().Count();
				if (enemiesCount == 0)
				{
					enemiesCount = 0;
				}
				if (creepsECount == 0)	
				{
					creepsECount = 0;
				}
				damage = ((creepsECount * creepsDmg[Q.Level - 1] + enemiesCount * enemyDmg[Q.Level - 1]) +
							 qDmg[Q.Level - 1]) * (1 - v.MagicDamageResist);
				
				if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
				{
					damage =
						Math.Floor((((creepsECount * creepsDmg[Q.Level - 1] + enemiesCount * enemyDmg[Q.Level - 1]) +
							 qDmg[Q.Level - 1]) *
								   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04))) * (1 - v.MagicDamageResist));
				}
				if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
					v.Spellbook.SpellR.CanBeCasted())
					damage = 0;
				var rum = v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb");
				if (rum) damage = damage * 0.5;
				var mom = v.HasModifier("modifier_item_mask_of_madness_berserk");
				if (mom) damage = damage * 1.3;
				//Console.WriteLine(damage);

				if (damage >= v.Health && z.Distance2D(v) <= Q.GetCastRange())
					return v;
			}
			return null;
		}
		public void Combo()
		{
			if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
				return;
			Q = me.Spellbook.SpellQ;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
			if (Menu.Item("steal").IsActive() && !me.HasModifier("modifier_legion_commander_duel") && Q != null && Q.CanBeCasted())
			{
				if (!me.IsAlive) return;
				var v =
				   ObjectManager.GetEntities<Hero>()
					   .Where(x => x.Team != me.Team && x.IsAlive && x.Distance2D(me) <= Q.GetCastRange() && !x.IsIllusion)
					   .ToList();
				var dmg = GetLowestToQ(v, me);
				
				if (dmg != null && Utils.SleepCheck("Q") && (Menu.Item("-dmg").IsActive() && !R.CanBeCasted() || !Menu.Item("-dmg").IsActive()))
				{
					Q.UseAbility(dmg.Position);
					Utils.Sleep(150, "Q");
				}
			}
			if (Active && !Game.IsChatOpen)
            {
                R = me.Spellbook.SpellR;
                W = me.Spellbook.SpellW;
                blink = me.FindItem("item_blink");
                armlet = me.FindItem("item_armlet");
                blademail = me.FindItem("item_blade_mail");
                bkb = me.FindItem("item_black_king_bar");
                abyssal = me.FindItem("item_abyssal_blade");
                mjollnir = me.FindItem("item_mjollnir");
                halberd = me.FindItem("item_heavens_halberd");
                medallion = me.FindItem("item_medallion_of_courage");
                madness = me.FindItem("item_mask_of_madness");
                urn = me.FindItem("item_urn_of_shadows");
                satanic = me.FindItem("item_satanic");
                solar = me.FindItem("item_solar_crest");
				orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
				dust = me.FindItem("item_dust");
                sentry = me.FindItem("item_ward_sentry");
                mango = me.FindItem("item_enchanted_mango");
                arcane = me.FindItem("item_arcane_boots");
                buckler = me.FindItem("item_buckler");
                crimson = me.FindItem("item_crimson_guard");
                lotusorb = me.FindItem("item_lotus_orb");
                cheese = me.FindItem("item_cheese");
                stick = me.FindItem("item_magic_stick") ?? me.FindItem("item_magic_wand");
                soul = me.FindItem("item_soul_ring");
                force = me.FindItem("item_force_staff");
                cyclone = me.FindItem("item_cyclone");
                sheep = me.FindItem("item_sheepstick");
                e = me.ClosestToMouseTarget(1470);

                float angle = me.FindAngleBetween(e.Position, true);
                Vector3 pos = new Vector3((float)(e.Position.X - 55 * Math.Cos(angle)), (float)(e.Position.Y - 55 * Math.Sin(angle)), 0);
                if (e != null && e.IsAlive  && !e.IsInvul() &&
					(blink != null ? me.Distance2D(pos) <= 1180 : me.Distance2D(e) <= 600))
				{
					if (me.CanAttack() && me.CanCast())
					{
						if (CanInvisCrit(me))
							me.Attack(e);
						else
						{
						    uint manacost = 0;
						    if (me.IsAlive)
						    {
						        if (blademail != null && blademail.Cooldown <= 0 &&
						            Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(blademail.Name))
						            manacost += blademail.ManaCost;
						        if (abyssal != null && abyssal.Cooldown <= 0 &&
						            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name))
						            manacost += abyssal.ManaCost;
						        if (mjollnir != null && mjollnir.Cooldown <= 0 &&
						            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name))
						            manacost += mjollnir.ManaCost;
						        if (halberd != null && halberd.Cooldown <= 0 &&
						            Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(halberd.Name))
						            manacost += halberd.ManaCost;
						        if (madness != null && madness.Cooldown <= 0 &&
						            Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(madness.Name))
						            manacost += madness.ManaCost;
						        if (lotusorb != null && lotusorb.Cooldown <= 0 &&
						            Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(lotusorb.Name))
						            manacost += lotusorb.ManaCost;
						        if (buckler != null && buckler.Cooldown <= 0 &&
						            Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(buckler.Name))
						            manacost += buckler.ManaCost;
						        if (crimson != null && crimson.Cooldown <= 0 &&
						            Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(crimson.Name))
						            manacost += crimson.ManaCost;
						        if (force != null && force.Cooldown <= 0 &&
						            Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(force.Name))
						            manacost += force.ManaCost;
						        if (cyclone != null && cyclone.CanBeCasted() &&
						            Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(cyclone.Name))
						            manacost += cyclone.ManaCost;
						        if (sheep != null && sheep.Cooldown <= 0 &&
						            Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
						            manacost += sheep.ManaCost;
						        if (W.Cooldown <= 0 && W.Level > 0 &&
						            Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
						            manacost += W.ManaCost;
						        if (R.Cooldown <= 0 && R.Level > 0)
						            manacost += W.ManaCost;
						    }
						    if (manacost > me.Mana)
						    {
						        if (mango.CanBeCasted() && mango != null &&
						            Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(mango.Name) &&
						            Utils.SleepCheck("Mango"))
						        {
						            mango.UseAbility();
						            Utils.Sleep(Game.Ping, "Mango");
						        }
						        if (arcane.CanBeCasted() && arcane != null &&
						            Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(arcane.Name) &&
						            Utils.SleepCheck("Arcane"))
						        {
						            arcane.UseAbility();
						            Utils.Sleep(Game.Ping, "Arcane");
						        }
						        if (stick.CanBeCasted() && stick != null &&
						            Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(stick.Name) &&
						            Utils.SleepCheck("stick"))
						        {
						            stick.UseAbility();
						            Utils.Sleep(Game.Ping, "stick");
						        }
						        if ((cheese.CanBeCasted() && cheese != null &&
						             Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(cheese.Name) &&
						             me.Health <= me.MaximumHealth * 0.5) ||
						            me.Health <= me.MaximumHealth * 0.30 && Utils.SleepCheck("Cheese"))
						        {
						            cheese.UseAbility();
						            Utils.Sleep(Game.Ping, "Cheese");
						        }
						        if (soul.CanBeCasted() && soul != null &&
						            Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(soul.Name) &&
						            Utils.SleepCheck("soul"))
						        {
						            soul.UseAbility();
						            Utils.Sleep(Game.Ping, "soul");
						        }
						    }
						    if (e.IsLinkensProtected())
							{
								if ((cyclone.CanBeCasted() || force.CanBeCasted() || halberd.CanBeCasted() ||
									 sheep.CanBeCasted() || abyssal.CanBeCasted()) && Utils.SleepCheck("Combo2"))
								{
									if (blademail != null && blademail.Cooldown <= 0 &&
										Menu.Item("Item")
											.GetValue<AbilityToggler>()
											.IsEnabled(blademail.Name) && me.Mana - blademail.ManaCost >= 75)
										blademail.UseAbility();
									if (satanic != null && satanic.Cooldown <= 0 && me.Health <= me.MaximumHealth * 0.5 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(satanic.Name))
										satanic.UseAbility();
									if (crimson != null && crimson.Cooldown <= 0 &&
										Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(crimson.Name))
										crimson.UseAbility();
									if (buckler != null && buckler.Cooldown <= 0 &&
										Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(buckler.Name) &&
										me.Mana - buckler.ManaCost >= 75)
										buckler.UseAbility();
									if (lotusorb != null && lotusorb.Cooldown <= 0 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(lotusorb.Name) &&
										me.Mana - lotusorb.ManaCost >= 75)
										lotusorb.UseAbility(me);
									if (mjollnir != null && mjollnir.Cooldown <= 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name) &&
										me.Mana - mjollnir.ManaCost >= 75)
										mjollnir.UseAbility(me);
									if (armlet != null && !armlet.IsToggled &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(armlet.Name) &&
										Utils.SleepCheck("armlet"))
									{
										armlet.ToggleAbility();
										Utils.Sleep(300, "armlet");
									}
									if (madness != null && madness.Cooldown <= 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(madness.Name) &&
										me.Mana - madness.ManaCost >= 75)
										madness.UseAbility();
									if (W != null && W.Level > 0 && W.Cooldown <= 0 &&
										Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name) &&
										!me.IsMagicImmune() && me.Mana - W.ManaCost >= 75)
										W.UseAbility(me);
									if (bkb != null && bkb.Cooldown <= 0 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Menu.Item("BKB").GetValue<KeyBind>().Active 
										&& !Game.IsChatOpen && (!W.CanBeCasted() || W == null))
										bkb.UseAbility();
									if (blink != null && blink.Cooldown <= 0 && me.Distance2D(pos) <= 1180 &&
										me.Distance2D(e) >= 200 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
										blink.UseAbility(pos);
									if (urn != null && urn.CurrentCharges > 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name))
										urn.UseAbility(e);
									if (solar != null && solar.CanBeCasted() &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(solar.Name))
										solar.UseAbility(e);
									if (medallion != null && medallion.CanBeCasted() &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medallion.Name))
										medallion.UseAbility(e);
									if (cyclone != null && cyclone.CanBeCasted() &&
										Utils.SleepCheck("CycloneRemoveLinkens") &&
										Menu.Item("Link")
											.GetValue<AbilityToggler>()
											.IsEnabled(cyclone.Name) && me.Mana - cyclone.ManaCost >= 75)
									{
										cyclone.UseAbility(e);
										Utils.Sleep(100, "CycloneRemoveLinkens");
									}
									else if (force != null && force.CanBeCasted() &&
											 Utils.SleepCheck("ForceRemoveLinkens") &&
											 Menu.Item("Link")
												 .GetValue<AbilityToggler>()
												 .IsEnabled(force.Name) && me.Mana - force.ManaCost >= 75)
									{
										force.UseAbility(e);
										Utils.Sleep(100, "ForceRemoveLinkens");
									}
									else if (halberd != null && halberd.CanBeCasted() &&
											 Utils.SleepCheck("halberdLinkens") &&
											 Menu.Item("Link")
												 .GetValue<AbilityToggler>()
												 .IsEnabled(halberd.Name) && me.Mana - halberd.ManaCost >= 75)
									{
										halberd.UseAbility(e);
										Utils.Sleep(100, "halberdLinkens");
									}
									else if (sheep != null && sheep.CanBeCasted() &&
											 Utils.SleepCheck("sheepLinkens") &&
											 Menu.Item("Link")
												 .GetValue<AbilityToggler>()
												 .IsEnabled(sheep.Name) && me.Mana - sheep.ManaCost >= 75)
									{
										sheep.UseAbility(e);
										Utils.Sleep(100, "sheepLinkens");
									}
									else if (abyssal != null && abyssal.CanBeCasted() &&
											 Utils.SleepCheck("abyssal") &&
											 Menu.Item("Items")
												 .GetValue<AbilityToggler>()
												 .IsEnabled(abyssal.Name) && me.Mana - abyssal.ManaCost >= 75)
									{
										abyssal.UseAbility(e);
										Utils.Sleep(100, "abyssal");
									}
									Utils.Sleep(200, "Combo2");
								}
							}
							else
							{
								if (UsedInvis(e))
								{
									if (me.Distance2D(e) <= 500)
									{
										if (dust != null && dust.CanBeCasted() && Utils.SleepCheck("dust") &&
											dust != null &&
											Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(dust.Name))
										{
											dust.UseAbility();
											Utils.Sleep(100, "dust");
										}
										else if (sentry != null && sentry.CanBeCasted() && Utils.SleepCheck("sentry") &&
												 sentry != null &&
												 Menu.Item("Items3")
													 .GetValue<AbilityToggler>()
													 .IsEnabled(sentry.Name))
										{
											sentry.UseAbility(me.Position);
											Utils.Sleep(100, "sentry");
										}
									}
								}
								uint elsecount = 1;
								if (Utils.SleepCheck("combo"))
								{
									if (blademail != null && blademail.Cooldown <= 0 &&
										Menu.Item("Item")
											.GetValue<AbilityToggler>()
											.IsEnabled(blademail.Name) && me.Mana - blademail.ManaCost >= 75)
										blademail.UseAbility();
									else
										elsecount += 1;
									if (satanic != null && satanic.Cooldown <= 0 && me.Health <= me.MaximumHealth * 0.5 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(satanic.Name))
										satanic.UseAbility();
									else
										elsecount += 1;
									if (crimson != null && crimson.Cooldown <= 0 &&
										Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(crimson.Name))
										crimson.UseAbility();
									else
										elsecount += 1;
									if (buckler != null && buckler.Cooldown <= 0 &&
										Menu.Item("Items3").GetValue<AbilityToggler>().IsEnabled(buckler.Name) &&
										me.Mana - buckler.ManaCost >= 75)
										buckler.UseAbility();
									else
										elsecount += 1;
									if (lotusorb != null && lotusorb.Cooldown <= 0 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(lotusorb.Name) &&
										me.Mana - lotusorb.ManaCost >= 75)
										lotusorb.UseAbility(me);
									else
										elsecount += 1;
									if (mjollnir != null && mjollnir.Cooldown <= 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name) &&
										me.Mana - mjollnir.ManaCost >= 75)
										mjollnir.UseAbility(me);
									else
										elsecount += 1;
									if (armlet != null && !armlet.IsToggled &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(armlet.Name) &&
										Utils.SleepCheck("armlet"))
									{
										armlet.ToggleAbility();
										Utils.Sleep(300, "armlet");
									}
									else
										elsecount += 1;
									if (madness != null && madness.Cooldown <= 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(madness.Name) &&
										me.Mana - madness.ManaCost >= 75)
										madness.UseAbility();
									else
										elsecount += 1;
									if (W != null && W.Level > 0 && W.Cooldown <= 0 &&
										Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name) &&
										!me.IsMagicImmune() && me.Mana - W.ManaCost >= 75)
										W.UseAbility(me);
									else
										elsecount += 1;
									if (bkb != null && bkb.Cooldown <= 0 &&
										Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(bkb.Name)
										&& Menu.Item("BKB").GetValue<KeyBind>().Active 
										&& (!W.CanBeCasted() || W == null))
										bkb.UseAbility();
									else
										elsecount += 1;
									if (blink != null && blink.Cooldown <= 0 && me.Distance2D(pos) <= 1180 &&
										me.Distance2D(e) >= 200 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
										blink.UseAbility(pos);
									else
										elsecount += 1;
									if (abyssal != null && abyssal.Cooldown <= 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name) &&
										me.Mana - abyssal.ManaCost >= 75)
										abyssal.UseAbility(e);
									else
										elsecount += 1;
									if (urn != null && urn.CurrentCharges > 0 &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name))
										urn.UseAbility(e);
									else
										elsecount += 1;
									if (solar != null && solar.CanBeCasted() &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(solar.Name))
										solar.UseAbility(e);
									else
										elsecount += 1;
									if (medallion != null && medallion.CanBeCasted() &&
										Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medallion.Name))
										medallion.UseAbility(e);
									else
										elsecount += 1;
									if (orchid != null && orchid.CanBeCasted() &&
										 Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
										orchid.UseAbility(e);
									else
										elsecount += 1;
									if (R != null && R.Cooldown <= 0 && !e.IsLinkensProtected() &&
										!e.HasModifier("modifier_abaddon_borrowed_time") &&
										Utils.SleepCheck("R") && elsecount == 17)
									{
										R.UseAbility(e);
										Utils.Sleep(100, "R");
									}
									else if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
									{
										Orbwalking.Orbwalk(e, 0, 1600, true, true);
									}
									Utils.Sleep(150, "combo");
								}
							}
						}
					}
				}
				if (me.Distance2D(e) <= 1470 && me.Distance2D(e) >= 350 && Utils.SleepCheck("Move"))
				{
					me.Move(e.Position);
					Utils.Sleep(150, "Move");
				}
			}
        }
		private void DrawUltiDamage(EventArgs args)
		{
			if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
			{
				return;
			}
			if (Menu.Item("dmg").IsActive())
			{
				Q = me.Spellbook.SpellQ;
				int[] qDmg = {40, 80, 120, 160};
				int[] creepsDmg = {14, 16, 18, 20};
				int[] enemyDmg = {20, 40, 60, 80};
				int enemiesCount;
				int creepsECount;

				var units =
					ObjectManager.GetEntities<Hero>()
						.Where(x => x.Team != me.Team && x.IsAlive && x.Distance2D(me) <= Q.GetCastRange() && !x.IsIllusion)
						.ToList();
				foreach (var v in units.Where(x => !x.IsMagicImmune()))
				{
					creepsECount = ObjectManager.GetEntities<Unit>().Where(creep =>
						(creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
						 || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
						 || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
						 || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep) &&
						creep.IsAlive && creep.Team != me.Team && creep.IsVisible && v.Distance2D(creep) <= 330 &&
						creep.IsSpawned).ToList().Count();
					enemiesCount = ObjectManager.GetEntities<Hero>().Where(x =>
						x.Team != me.Team && x.IsAlive && x.IsVisible && v.Distance2D(x) <= 330 && !x.IsIllusion).ToList().Count();
					if (enemiesCount == 0)
					{
						enemiesCount = 0;
					}
					if (creepsECount == 0)
					{
						creepsECount = 0;
					}
					damage = ((creepsECount*creepsDmg[Q.Level - 1] + enemiesCount*enemyDmg[Q.Level - 1]) +
					          qDmg[Q.Level - 1])*(1 - v.MagicDamageResist);



					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damage =
							Math.Floor((((creepsECount*creepsDmg[Q.Level - 1] + enemiesCount*enemyDmg[Q.Level - 1]) +
							             qDmg[Q.Level - 1])*
							            (1 - (0.10 + v.Spellbook.Spell3.Level*0.04)))*(1 - v.MagicDamageResist));
					}
					if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
					    v.Spellbook.SpellR.CanBeCasted())
						damage = 0;
					var rum = v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb");
					if (rum) damage = damage*0.5;
					var mom = v.HasModifier("modifier_item_mask_of_madness_berserk");
					if (mom) damage = damage*1.3;
					var dmg = v.Health - damage;
					var canKill = dmg <= 0;
					var screenPos = HUDInfo.GetHPbarPosition(v);
					if (!OnScreen(v.Position)) continue;

					var text = canKill ? "Yes: " + Math.Floor(damage) : "No: " + Math.Floor(damage);
					var size = new Vector2(18, 18);
					var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
					var position = new Vector2(screenPos.X - textSize.X + 91, screenPos.Y + 62);
					Drawing.DrawText(
						text,
						position,
						size,
						(canKill ? Color.LawnGreen : Color.Red),
						FontFlags.AntiAlias);
					Drawing.DrawText(
						text,
						new Vector2(screenPos.X - textSize.X + 92, screenPos.Y + 63),
						size,
						(Color.Black),
						FontFlags.AntiAlias);
				}
			}

		}
		private bool OnScreen(Vector3 v)
		{
			return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
		}
		
       
		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");
			
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "ComboKey").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(
				new MenuItem("BKB", "Black King Bar").SetValue(new KeyBind('F', KeyBindType.Toggle)));
            Menu.AddSubMenu(items);
            Menu.AddSubMenu(skills);
            items.AddItem(
				new MenuItem("Items", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_blink", true},
				    {"item_armlet", true},
				    {"item_abyssal_blade", true},
				    {"item_mjollnir", true},
				    {"item_medallion_of_courage", true},
				    {"item_mask_of_madness", true},
				    {"item_urn_of_shadows", true},
				    {"item_solar_crest", true}
				})));
            items.AddItem(
				new MenuItem("Item", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{

					{"item_orchid", true},
					{"item_bloodthorn", true},
					{"item_black_king_bar", true},
				    {"item_blade_mail", true},
				    {"item_satanic", true},
				    {"item_lotus_orb", true},
				    {"item_magic_stick", true},
				    {"item_magic_wand", true}
				})));
			items.AddItem(
				new MenuItem("Items3", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_dust", true},
					{"item_ward_sentry", true},
					{"item_enchanted_mango", true},
					{"item_arcane_boots", true},
					{"item_buckler", true},
					{"item_crimson_guard", true},
					{"item_cheese", true},
					{"item_soul_ring", true}
				})));
			items.AddItem(
				new MenuItem("Link", "Auto triggre Linken").SetValue(
					new AbilityToggler(new Dictionary<string, bool>
					{
					    {"item_heavens_halberd", true},
					    {"item_force_staff", true},
					    {"item_cyclone", true},
					    {"item_sheepstick", true}
					})));
            skills.AddItem(new MenuItem("steal", "KillSteal Q").SetValue(true));
			skills.AddItem(new MenuItem("-dmg", "Dont Use KillSteal if i have Duel").SetValue(true));
			skills.AddItem(new MenuItem("dmg", "Show Damage Q Spell").SetValue(true));
			skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"legion_commander_press_the_attack", true},
			})));

			txt = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Segoe UI",
				   Height = 19,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });

			noti = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Segoe UI",
				   Height = 30,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });

			lines = new Line(Drawing.Direct3DDevice9);

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
			Drawing.OnDraw += DrawUltiDamage;
		}

		public void OnCloseEvent()
		{
			AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
			Drawing.OnPreReset -= Drawing_OnPreReset;
			Drawing.OnPostReset -= Drawing_OnPostReset;
			Drawing.OnEndScene -= Drawing_OnEndScene;
			Drawing.OnDraw-= DrawUltiDamage;
		}

		private bool UsedInvis(Hero v)
		{
			if (v.Modifiers.Any(
				x =>
					(x.Name == "modifier_bounty_hunter_wind_walk" ||
					 x.Name == "modifier_riki_permanent_invisibility" ||
					 x.Name == "modifier_mirana_moonlight_shadow" || x.Name == "modifier_treant_natures_guise" ||
					 x.Name == "modifier_weaver_shukuchi" ||
					 x.Name == "modifier_broodmother_spin_web_invisible_applier" ||
					 x.Name == "modifier_item_invisibility_edge_windwalk" || x.Name == "modifier_rune_invis" ||
					 x.Name == "modifier_clinkz_wind_walk" || x.Name == "modifier_item_shadow_amulet_fade" ||
					 x.Name == "modifier_item_silver_edge_windwalk" ||
					 x.Name == "modifier_item_edge_windwalk" ||
					 x.Name == "modifier_nyx_assassin_vendetta" ||
					 x.Name == "modifier_invisible" ||
					 x.Name == "modifier_invoker_ghost_walk_enemy")))
				return true;
			return false;
		}

		private bool CanInvisCrit(Hero x)
		{
			if (
				x.Modifiers.Any(
					m =>
						m.Name == "modifier_item_invisibility_edge_windwalk" ||
						m.Name == "modifier_item_silver_edge_windwalk"))
				return true;
			return false;
		}


		void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;
			bkb = me.FindItem("item_black_king_bar");
			if (bkb != null)
			{
				if (!Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(bkb.Name)
					|| !Menu.Item("BKB").GetValue<KeyBind>().Active)
				{
					DrawBox(2, 490, 90, 20, 1, new ColorBGRA(0, 0, 90, 70));
					DrawFilledBox(2, 490, 90, 20, new ColorBGRA(0, 0, 0, 90));
					DrawShadowText(" BKB Disable", 4, 490, Color.Gold, txt);
				}
			}
		}
		

		void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			noti.Dispose();
			lines.Dispose();
		}



		void Drawing_OnPostReset(EventArgs args)
		{
			txt.OnResetDevice();
			noti.OnResetDevice();
			lines.OnResetDevice();
		}

		void Drawing_OnPreReset(EventArgs args)
		{
			txt.OnLostDevice();
			noti.OnLostDevice();
			lines.OnLostDevice();
		}

		public void DrawFilledBox(float x, float y, float w, float h, Color color)
		{
			var vLine = new Vector2[2];

			lines.GLLines = true;
			lines.Antialias = false;
			lines.Width = w;

			vLine[0].X = x + w / 2;
			vLine[0].Y = y;
			vLine[1].X = x + w / 2;
			vLine[1].Y = y + h;

			lines.Begin();
			lines.Draw(vLine, color);
			lines.End();
		}

		public void DrawBox(float x, float y, float w, float h, float px, Color color)
		{
			DrawFilledBox(x, y + h, w, px, color);
			DrawFilledBox(x - px, y, px, h, color);
			DrawFilledBox(x, y - px, w, px, color);
			DrawFilledBox(x + w, y, px, h, color);
		}

		public void DrawShadowText(string stext, int x, int y, Color color, Font f)
		{
			f.DrawText(null, stext, x + 1, y + 1, Color.Black);
			f.DrawText(null, stext, x, y, color);
		}
	}
}