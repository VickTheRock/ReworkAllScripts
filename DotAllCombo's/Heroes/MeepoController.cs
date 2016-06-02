namespace DotaAllCombo.Heroes
{
	using System;
	using System.Linq;
	using System.Windows.Input;
	using System.Collections.Generic;
	using Ensage;
	using Ensage.Common.Extensions;
	using Ensage.Common;
	using SharpDX;
	using SharpDX.Direct3D9;
	using System.Timers;
	using System.Threading.Tasks;
	using Ensage.Common.Menu;
	using Service;
	using Service.Debug;

    internal class MeepoController : Variables, IHeroController
    {
		public Hero initMeepo;

		public bool activated, PoofKey, SafePoof, PoofAutoMode, SliderCountUnit, dodge = true;

		public Item blink, travel, Travel, shiva, sheep, medall, dagon, cheese, ethereal, vail, atos, orchid, abyssal;
		public Font txt;
		public Font not;
		public List<Hero> meepos;
		public readonly Menu skills = new Menu("All Poof", "PoofMeepo");
		//public readonly Menu farm = new Menu("FarmMode", "FarmMode");
		

		public void Combo()
		{
			if (me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_Meepo || !Game.IsInGame) return;
            if(!me.IsAlive) return;

            activated = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);
            PoofKey = Game.IsKeyDown(menu.Item("poofKey").GetValue<KeyBind>().Key);
            PoofAutoMode = menu.Item("poofAutoMod").GetValue<KeyBind>().Active;
            SafePoof = menu.Item("poofSafe").IsActive();
            dodge = menu.Item("Dodge").GetValue<KeyBind>().Active;
            var checkObj = ObjectManager.GetEntities<Unit>().Where(x => x.IsAlive && x.Team != me.Team && x.IsValid).ToList();
            var meepos = ObjectManager.GetEntities<Hero>().Where(x => x.IsControllable && x.IsAlive && x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo).ToList();





            List<Unit> fount = ObjectManager.GetEntities<Unit>().Where(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain).ToList();
            //blink = me.FindItem("item_blink");



            e = ObjectManager.GetEntities<Hero>()
                        .Where(x => x.IsAlive && x.Team != me.Team && !x.IsIllusion)
                        .OrderBy(x => GetDistance2D(x.Position, meepos.OrderBy(y => GetDistance2D(x.Position, y.Position)).FirstOrDefault().Position))
                        .FirstOrDefault();


            /**************************************************DODGE*************************************************************/

            var f = ObjectManager.GetEntities<Hero>()
                        .Where(x => x.IsAlive && x.Team == me.Team && !x.IsIllusion && x.IsControllable && x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo)
                        .OrderBy(x => GetDistance2D(x.Position, fount.OrderBy(y => GetDistance2D(x.Position, y.Position)).FirstOrDefault().Position))
                        .FirstOrDefault();

            Ability[] q = new Ability[meepos.Count()];
            for (int i = 0; i < meepos.Count(); i++) q[i] = meepos[i].Spellbook.SpellQ;
            Ability[] w = new Ability[meepos.Count()];
            for (int i = 0; i < meepos.Count(); i++) w[i] = meepos[i].Spellbook.SpellW;
            if (dodge && me.IsAlive)
            {
                var baseDota =
                  ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_base" && unit.Team != me.Team).ToList();
                if (baseDota != null)
                {
                    for (int t = 0; t < baseDota.Count(); t++)
                    {
                        for (int i = 0; i < meepos.Count(); i++)
                        {
                            float angle = meepos[i].FindAngleBetween(baseDota[t].Position, true);
                            Vector3 pos = new Vector3((float)(baseDota[t].Position.X - 710 * Math.Cos(angle)), (float)(baseDota[t].Position.Y - 710 * Math.Sin(angle)), 0);
                            if (meepos[i].Distance2D(baseDota[t]) <= 700 && meepos[i].Modifiers.Any(y => y.Name != "modifier_bloodseeker_rupture") && Utils.SleepCheck(meepos[i].Handle + "MoveDodge"))
                            {
                                meepos[i].Move(pos);
                                Utils.Sleep(120, meepos[i].Handle + "MoveDodge");
                                //	Console.WriteLine("Name: " + baseDota[t].Name);
                                //	Console.WriteLine("Speed: " + baseDota[t].Speed);
                                //	Console.WriteLine("ClassID: " + baseDota[t].ClassID);
                                //	Console.WriteLine("Handle: " + baseDota[t].Handle);
                                //	Console.WriteLine("UnitState: " + baseDota[t].UnitState);
                            }
                        }
                    }
                }

                var thinker =
                   ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_thinker" && unit.Team != me.Team).ToList();
                if (thinker != null)
                {
                    for (int i = 0; i < thinker.Count(); i++)
                    {
                        for (int j = 0; j < meepos.Count(); j++)
                        {
                            float angle = meepos[j].FindAngleBetween(thinker[i].Position, true);
                            Vector3 pos = new Vector3((float)(thinker[i].Position.X - 360 * Math.Cos(angle)), (float)(thinker[i].Position.Y - 360 * Math.Sin(angle)), 0);
                            if (meepos[j].Distance2D(thinker[i]) <= 350 && meepos[j].Modifiers.Any(y => y.Name != "modifier_bloodseeker_rupture"))
                            {

                                if (Utils.SleepCheck(meepos[j].Handle + "MoveDodge"))
                                {
                                    meepos[j].Move(pos);
                                    Utils.Sleep(350, meepos[j].Handle + "MoveDodge");
                                }

                            }
                        }
                    }
                }
                foreach (var v in meepos)
                {
                    if (Utils.SleepCheck(v.Handle + "_move") && v.Health <= v.MaximumHealth / 100 * menu.Item("healh").GetValue<Slider>().Value
                        && v.Modifiers.Any(y => y.Name != "modifier_bloodseeker_rupture")
                        && v.Distance2D(fount.First().Position) >= 1000
                        )
                    {
                        v.Move(fount.First().Position);
                        Utils.Sleep(300, v.Handle + "_move");
                    }
                    if (activated)
                    {
                        float angle = v.FindAngleBetween(fount.First().Position, true);
                        Vector3 pos = new Vector3((float)(fount.First().Position.X - 500 * Math.Cos(angle)), (float)(fount.First().Position.Y - 500 * Math.Sin(angle)), 0);
                        if (
                            v.Health >= v.MaximumHealth * 0.58
                            && v.Distance2D(fount.First()) <= 400
                            && me.Team == Team.Radiant
                            && Utils.SleepCheck(v.Handle + "RadMove")
                            )
                        {
                            v.Move(pos);
                            Utils.Sleep(400, v.Handle + "RadMove");
                        }
                        if (
                            v.Health >= v.MaximumHealth * 0.58
                            && v.Distance2D(fount.First()) <= 400
                            && me.Team == Team.Dire
                            && Utils.SleepCheck(v.Handle + "DireMove")
                            )
                        {
                            v.Move(pos);
                            Utils.Sleep(400, v.Handle + "DireMove");
                        }
                    }
                }

                for (int i = 0; i < meepos.Count(); i++)
                {
                    travel = meepos[i].FindItem("item_travel_boots") ?? meepos[i].FindItem("item_travel_boots_2");
                    if (w[i] != null
                        && w[i].CanBeCasted()
                        && meepos[i].Health <= meepos[i].MaximumHealth
                        / 100 * menu.Item("healh").GetValue<Slider>().Value
                        && meepos[i].Handle != f.Handle
                        && meepos[i].Distance2D(f) >= 700
                        && e == null
                        && meepos[i].Distance2D(fount.First().Position) >= 1500
                        && Utils.SleepCheck(meepos[i].Handle + "W"))
                    {
                        w[i].UseAbility(f);
                        Utils.Sleep(1000, meepos[i].Handle + "W");
                    }
                    else if (
                        travel != null
                        && travel.CanBeCasted()
                        && meepos[i].Health <= meepos[i].MaximumHealth
                        / 100 * menu.Item("healh").GetValue<Slider>().Value
                       && (!w[i].CanBeCasted()
                       || meepos[i].Position.Distance2D(f) >= 1000
                       || (w[i].CanBeCasted()
                       && f.Distance2D(fount.First()) >= 1500))
                       || (meepos[i].IsSilenced()
                       || meepos[i].MovementSpeed <= 280)
                       && meepos[i].Distance2D(fount.First().Position) >= 1500
                       && e == null
                       && Utils.SleepCheck(meepos[i].Handle + "travel"))
                    {
                        travel.UseAbility(fount.First().Position);
                        Utils.Sleep(1000, meepos[i].Handle + "travel");
                    }
                    if (e != null
                        && q[i] != null
                        && meepos[i].Health <= meepos[i].MaximumHealth
                        / 100 * menu.Item("healh").GetValue<Slider>().Value
                        && q[i].CanBeCasted()
                        && e.Modifiers.Any(y => y.Name != "modifier_meepo_earthbind")
                        && !e.IsMagicImmune()
                        && meepos[i].Distance2D(e) <= q[i].CastRange - 50
                        && Utils.SleepCheck(meepos[i].Handle + "_net_casting"))
                    {
                        q[i].CastSkillShot(e);
                        Utils.Sleep(q[i].GetCastDelay(meepos[i], e, true) + 500, meepos[i].Handle + "_net_casting");
                    }
                    else if (!q[i].CanBeCasted() && meepos[i].Health <= meepos[i].MaximumHealth / 100 * menu.Item("healh").GetValue<Slider>().Value)
                    {
                        for (var j = 0; j < meepos.Count(); j++)
                        {
                            if (e != null
                                && q[j] != null
                                && meepos[i].Handle != meepos[j].Handle
                                && meepos[j].Position.Distance2D(e) < q[i].CastRange
                                && e.Modifiers.Any(y => y.Name != "modifier_meepo_earthbind")
                                && meepos[j].Position.Distance2D(meepos[i]) < q[j].CastRange
                                && !e.IsMagicImmune()
                                && Utils.SleepCheck(meepos[i].Handle + "_net_casting"))
                            {
                                q[j].CastSkillShot(e);
                                Utils.Sleep(q[j].GetCastDelay(meepos[j], e, true) + 1500, meepos[i].Handle + "_net_casting");
                                break;
                            }
                        }
                    }
                    if (e != null
                        && w[i] != null
                        && w[i].CanBeCasted()
                        && meepos[i].Health <= meepos[i].MaximumHealth
                        / 100 * menu.Item("healh").GetValue<Slider>().Value
                        && meepos[i].Handle != f.Handle && meepos[i].Distance2D(f) >= 700
                        && (meepos[i].Distance2D(e) >= (e.AttackRange + 60)
                        || meepos[i].MovementSpeed <= 290)
                        && (q == null || (!q[i].CanBeCasted()
                        || e.HasModifier("modifier_meepo_earthbind")
                        || !e.IsMagicImmune()) || meepos[i].Distance2D(e) >= 1000)
                        && meepos[i].Distance2D(fount.First().Position) >= 1100
                        && Utils.SleepCheck(meepos[i].Handle + "W"))
                    {
                        w[i].UseAbility(f);
                        Utils.Sleep(1000, meepos[i].Handle + "W");
                    }
                    else if (
                            e != null
                            && travel != null
                            && travel.CanBeCasted()
                            && meepos[i].Health <= meepos[i].MaximumHealth
                            / 100 * menu.Item("healh").GetValue<Slider>().Value
                            && (!w[i].CanBeCasted()
                            || meepos[i].Position.Distance2D(f) >= 1000
                            || (w[i].CanBeCasted()
                            && f.Distance2D(fount.First()) >= 2000))
                            && (meepos[i].Distance2D(e) >= (e.AttackRange + 60)
                            || (meepos[i].IsSilenced()
                            || meepos[i].MovementSpeed <= 290))
                            && meepos[i].Distance2D(fount.First().Position) >= 1100
                            && Utils.SleepCheck(meepos[i].Handle + "travel"))
                    {
                        travel.UseAbility(fount.First().Position);
                        Utils.Sleep(1000, meepos[i].Handle + "travel");
                    }
                }
            }
            /**************************************************DODGE*************************************************************/
            /***************************************************POOF*************************************************************/
            if (PoofKey)
            {
                for (int i = 0; i < meepos.Count(); i++)
                {
                    for (int j = 0; j < checkObj.Count(); j++)
                    {
                        if (w[i] != null
                            && ((meepos[i].Distance2D(checkObj[j]) <= 365
                            && SafePoof)
                            || (!SafePoof))
                            && w[i].CanBeCasted()
                            && (meepos[i].Health >= meepos[i].MaximumHealth
                            / 100 * menu.Item("healh").GetValue<Slider>().Value
                            || !dodge)
                            && Utils.SleepCheck(meepos[i].Handle + "Wpos"))
                        {
                            w[i].UseAbility(meepos[i]);
                            Utils.Sleep(250, meepos[i].Handle + "Wpos");
                        }
                    }
                }
            }



            if (PoofAutoMode)
            {
                for (int i = 0; i < meepos.Count(); i++)
                {
                    var nCreeps = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                        || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                        || x.ClassID == ClassID.CDOTA_BaseNPC_Creep
                        || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral) && x.Team != me.Team && x.IsSpawned && x.IsAlive).Where(x => x.Distance2D(meepos[i]) <= 345).Count();

                    SliderCountUnit = nCreeps >= (skills.Item("poofCount").GetValue<Slider>().Value);

                    if (SliderCountUnit
                        && w[i] != null
                        && w[i].CanBeCasted()
                        && meepos[i].CanCast()
                        && meepos[i].Health >= meepos[i].MaximumHealth
                        / 100 * menu.Item("healh").GetValue<Slider>().Value - 0.05
                        && meepos[i].Mana >= (meepos[i].MaximumMana
                        / 100 * menu.Item("mana").GetValue<Slider>().Value)
                        && Utils.SleepCheck(meepos[i].Handle + "Wpos"))
                    {
                        w[i].UseAbility(meepos[i]);
                        Utils.Sleep(250, meepos[i].Handle + "Wpos");
                    }
                }
            }
            /***************************************************POOF*************************************************************/
            /**************************************************COMBO*************************************************************/
            if (activated)
            {
                for (int i = 0; i < meepos.Count(); i++)
                {
                    target = ClosestToMouse(meepos[i]);

                    if (target == null) return;
                    initMeepo = GetClosestToTarget(meepos, target);


                    if (
                    w[i] != null
                    && meepos[i].CanCast()
                    && (
                        meepos[i].Handle != f.Handle && f.HasModifier("modifier_fountain_aura_buff")
                        || meepos[i].Handle == f.Handle && !f.HasModifier("modifier_fountain_aura_buff")
                        )
                    && meepos.Count(x => x.Distance2D(meepos[i]) <= 1000) > 1
                    && meepos[i].Health >= meepos[i].MaximumHealth * 0.8
                    && w[i].CanBeCasted()
                    && initMeepo.Distance2D(target) <= 350
                    && Utils.SleepCheck(meepos[i].Handle + "poof")
                    )
                    {
                        w[i].UseAbility(target.Position);
                        Utils.Sleep(250, meepos[i].Handle + "poof");
                    }

                    if (me.HasModifier("modifier_fountain_aura_buff"))
                    {
                        if (
                            me.Spellbook.SpellW != null
                            && me.Spellbook.SpellW.CanBeCasted()
                            && me.Health >= me.MaximumHealth * 0.8
                            && meepos.Count(x => x.Distance2D(me) <= 1000) > 1
                            && initMeepo.Distance2D(target) <= 350
                            && Utils.SleepCheck(me.Handle + "pooff")
                            )
                        {
                            me.Spellbook.SpellW.UseAbility(target.Position);
                            Utils.Sleep(250, me.Handle + "pooff");
                        }
                    }
                    //	
                    /*int[] cool;
					var core = me.FindItem("item_octarine_core");
					if (core !=null)
						cool = new int[4] { 20, 16, 12, 8 };
					else
						cool = new int[4] { 15, 12, 9, 6 };*/

                    orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
                    blink = meepos[i].FindItem("item_blink");
                    sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");


                    if ( // sheep
                    sheep != null
                    && sheep.CanBeCasted()
                    && me.CanCast()
                    && !target.IsLinkensProtected()
                    && !target.IsMagicImmune()
                    && me.Distance2D(target) <= 900
                    && meepos[i].Distance2D(target) <= 350
                    && Utils.SleepCheck("sheep")
                    )
                    {
                        sheep.UseAbility(target);
                        Utils.Sleep(250, "sheep");
                    } // sheep Item end

                    if ( // Medall
                    medall != null
                    && medall.CanBeCasted()
                    && Utils.SleepCheck("Medall")
                    && meepos[i].Distance2D(target) <= 300
                    && me.Distance2D(target) <= 700
                    )
                    {
                        medall.UseAbility(target);
                        Utils.Sleep(250, "Medall");
                    } // Medall Item end
                    if ( // orchid
                        orchid != null
                        && orchid.CanBeCasted()
                        && me.CanCast()
                        && !target.IsLinkensProtected()
                        && !target.IsMagicImmune()
                        && meepos[i].Distance2D(target) <= 300
                        && me.Distance2D(target) <= 900
                        && Utils.SleepCheck("orchid")
                        )
                    {
                        orchid.UseAbility(target);
                        Utils.Sleep(250, "orchid");
                    } // orchid Item end
                    if (Utils.SleepCheck("Q")
                        && !target.HasModifier("modifier_meepo_earthbind")
                        && (((!blink.CanBeCasted()
                        || blink == null)
                        && meepos[i].Distance2D(target) <= q[i].GetCastRange())
                        || (blink.CanBeCasted()
                        && meepos[i].Distance2D(target) <= 350))
                        )
                    {
                        if (q[i] != null
                            && (meepos[i].Health >= meepos[i].MaximumHealth
                            / 100 * menu.Item("healh").GetValue<Slider>().Value
                            || !dodge)
                            && q[i].CanBeCasted()
                            && !e.IsMagicImmune()
                             && !meepos[i].IsChanneling()
                             && meepos[i].Distance2D(target) <= q[i].GetCastRange()
                             && Utils.SleepCheck(meepos[i].Handle + "_net_casting"))
                        {
                            q[i].CastSkillShot(e);
                            Utils.Sleep(q[i].GetCastDelay(meepos[i], e, true) + 1500, meepos[i].Handle + "_net_casting");
                            Utils.Sleep(1500, "Q");
                        }
                    }

                    if (
                        blink != null
                        && me.CanCast()
                        && blink.CanBeCasted()
                        && me.Distance2D(target) >= 350
                        && me.Distance2D(target) <= 1150
                        )
                    {
                        if (blink.CanBeCasted()
                               && !menu.Item("blinkDelay").IsActive()
                               && meepos[i].Health >= meepos[i].MaximumHealth / 100 * menu.Item("healh").GetValue<Slider>().Value
                               && Utils.SleepCheck("13"))
                        {
                            blink.UseAbility(target.Position);
                            Utils.Sleep(200, "13");
                        }

                        Task.Delay(1350 - (int)Game.Ping).ContinueWith(_ =>
                        {
                            if (blink.CanBeCasted()
                            && menu.Item("blinkDelay").IsActive()
                            && Utils.SleepCheck("12"))
                            {
                                blink.UseAbility(target.Position);
                                Utils.Sleep(200, "12");
                            }
                        });
                        for (int j = 0; j < meepos.Count(); j++)
                        {
                            if (
                            w[j] != null
                            && meepos[j].Handle != me.Handle
                            && meepos[j].CanCast()
                            && ((f.Handle != meepos[j].Handle && f.HasModifier("modifier_fountain_aura_buff")
                            || !f.HasModifier("modifier_fountain_aura_buff"))
                            )
                            && meepos[j].Health >= meepos[j].MaximumHealth / 100 * menu.Item("healh").GetValue<Slider>().Value
                            && !target.IsMagicImmune()
                            && w[j].CanBeCasted()
                            && Utils.SleepCheck(meepos[j].Handle + "poof")
                            )
                            {
                                w[j].UseAbility(target.Position);
                                Utils.Sleep(250, meepos[j].Handle + "poof");
                            }
                        }
                    }

                    if (
                       meepos[i].Distance2D(target) <= 200 && (!meepos[i].IsAttackImmune() || !target.IsAttackImmune())
                       && meepos[i].NetworkActivity != NetworkActivity.Attack && meepos[i].CanAttack()
                       && meepos[i].Health >= meepos[i].MaximumHealth / 100 * menu.Item("healh").GetValue<Slider>().Value
                       && Utils.SleepCheck(meepos[i].Handle + "Attack")
                       )
                    {
                        meepos[i].Attack(target);
                        Utils.Sleep(200, meepos[i].Handle + "Attack");
                    }
                    else if (((
                       (!meepos[i].CanAttack()
                       || meepos[i].Distance2D(target) >= 0)
                       && meepos[i].NetworkActivity != NetworkActivity.Attack
                       && meepos[i].Distance2D(target) <= 1000))
                       && ((meepos[i].Handle != me.Handle
                       && (blink != null && blink.CanBeCasted()
                       && me.Distance2D(target) <= 350)
                       || (meepos[i].Handle == me.Handle
                       && !blink.CanBeCasted()))
                       || blink == null)
                       && meepos[i].Health >= meepos[i].MaximumHealth / 100 * menu.Item("healh").GetValue<Slider>().Value
                       && Utils.SleepCheck(meepos[i].Handle + "Move"))
                    {
                        meepos[i].Move(target.Predict(450));
                        Utils.Sleep(250, meepos[i].Handle + "Move");
                    }


                }

                vail = me.FindItem("item_veil_of_discord");
                shiva = me.FindItem("item_shivas_guard");
                medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
                atos = me.FindItem("item_rod_of_atos");
                cheese = me.FindItem("item_cheese");
                abyssal = me.FindItem("item_abyssal_blade");
                dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
                ethereal = me.FindItem("item_ethereal_blade");

                target = me.ClosestToMouseTarget(2000);
                if (target == null) return;
                if ( // ethereal
                    ethereal != null
                    && ethereal.CanBeCasted()
                    && me.CanCast()
                    && !target.IsLinkensProtected()
                    && !target.IsMagicImmune()
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
                    || (target.HasModifier("modifier_item_ethereal_blade_slow")
                    || ethereal.Cooldown < 17))
                    && !target.IsLinkensProtected()
                    && dagon.CanBeCasted()
                    && !target.IsMagicImmune()
                    && Utils.SleepCheck("dagon")
                    )
                {
                    dagon.UseAbility(target);
                    Utils.Sleep(200, "dagon");
                } // Dagon Item end
                if ( // vail
                    vail != null
                    && vail.CanBeCasted()
                    && me.CanCast()
                    && !target.IsMagicImmune()
                    && me.Distance2D(target) <= 1100
                    && Utils.SleepCheck("vail")
                    )
                {
                    vail.UseAbility(target.Position);
                    Utils.Sleep(250, "vail");
                } // orchid Item end
                if (// Shiva Item
                    shiva != null
                    && shiva.CanBeCasted()
                    && me.CanCast()
                    && !target.IsMagicImmune()
                    && Utils.SleepCheck("shiva")
                    && me.Distance2D(target) <= 600
                    )
                {
                    shiva.UseAbility();
                    Utils.Sleep(250, "shiva");
                } // Shiva Item end
                if (
                    // cheese
                    cheese != null
                    && cheese.CanBeCasted()
                    && me.Health <= (me.MaximumHealth * 0.3)
                    && me.Distance2D(target) <= 700
                    && Utils.SleepCheck("cheese")
                    )
                {
                    cheese.UseAbility();
                    Utils.Sleep(200, "cheese");
                } // cheese Item end

                if ( // atos Blade
                    atos != null
                    && atos.CanBeCasted()
                    && me.CanCast()
                    && !target.IsLinkensProtected()
                    && !target.IsMagicImmune()
                    && me.Distance2D(target) <= 2000
                    && Utils.SleepCheck("atos")
                    )
                {
                    atos.UseAbility(target);
                    Utils.Sleep(250, "atos");
                } // atos Item end
                if ( // Abyssal Blade
                    abyssal != null
                    && abyssal.CanBeCasted()
                    && me.CanCast()
                    && !target.IsStunned()
                    && !target.IsHexed()
                    && Utils.SleepCheck("abyssal")
                    && me.Distance2D(target) <= 300
                    )
                {
                    abyssal.UseAbility(target);
                    Utils.Sleep(250, "abyssal");
                } // Abyssal Item end
            }
        }

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("<font face='verdana' color='#f80000'>Probably gonna dig a grave or two before this is done.</font>", MessageType.LogMessage);

			Console.WriteLine("Meepo combo loaded!");
			menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			menu.AddItem(new MenuItem("Dodge", "Dodge meepo's").SetValue(new KeyBind('T', KeyBindType.Toggle)));
			menu.AddItem(new MenuItem("blinkDelay", "Use Blink delay before all poof meepo the enemy").SetValue(true));
			menu.AddItem(new MenuItem("healh", "Min Healh to Move Fount").SetValue(new Slider(58, 10, 100)));
			skills.AddItem(new MenuItem("poofSafe", "Use poof if ability radius suitable targets.").SetValue(true));
			skills.AddItem(new MenuItem("poofKey", "All Poof Key").SetValue(new KeyBind('F', KeyBindType.Press)));
			skills.AddItem(new MenuItem("poofAutoMod", "AutoPoofFarm").SetValue(new KeyBind('J', KeyBindType.Toggle)));
			skills.AddItem(new MenuItem("poofCount", "Min units to Poof").SetValue(new Slider(3, 1, 10)));
			skills.AddItem(new MenuItem("mana", "Min Mana % to Poof").SetValue(new Slider(35, 10, 100)));
			menu.AddSubMenu(skills);
			
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
					FaceName = "Monospace",
					Height = 35,
					OutputPrecision = FontPrecision.Default,
					Quality = FontQuality.ClearType
				});

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}

		public void OnCloseEvent()
		{
			AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
			Drawing.OnPreReset -= Drawing_OnPreReset;
			Drawing.OnPostReset -= Drawing_OnPostReset;
			Drawing.OnEndScene -= Drawing_OnEndScene;
		}
		
		public Hero ClosestToMouse(Hero source, float range = 90000)
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


		public Hero GetClosestToTarget(List<Hero> units, Hero target)
		{
			Hero closestHero = null;
			foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(target) > v.Distance2D(target)))
			{
				closestHero = v;
			}
			return closestHero;
		}
		
		double GetDistance2D(Vector3 A, Vector3 B)
		{
			return Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
		}


		void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			not.Dispose();
		}


		public void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectManager.LocalPlayer;
			if (player == null || player.Team == Team.Observer || me == null)
				return;
			if (activated)
			{
				txt.DrawText(null, "Combo meepo's active", 1200, 12, Color.Green);
			}

			if (!dodge)
			{
				txt.DrawText(null, "Warning! Dodge unActive", 1200, 22, Color.DarkRed);
			}

			if (PoofAutoMode)
			{
				txt.DrawText(null, "Auto Poof On", 1200, 30, Color.Green);
			}

			if (!PoofAutoMode)
			{
				txt.DrawText(null, "Auto Poof Off", 1200, 30, Color.DarkRed);
			}

			/*
			if (farm)
			{
				txt.DrawText(null, "Farm Meepo On", 1200, 32, Color.Green);
			}

			if (!farm)
			{
				txt.DrawText(null, "Farm Meepo Off", 1200, 32, Color.DarkRed);
			}
			if (push)
			{
				txt.DrawText(null, "Push Meepo On", 1200, 42, Color.Green);
			}

			if (!push)
			{
				txt.DrawText(null, "Push Meepo Off", 1200, 42, Color.DarkRed);
			}
			модификатор зевса
			аутопуф - комбо
			*/
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
	}
}