namespace DotaAllCombo.Addons
{
	using System.Security.Permissions;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using SharpDX;
	using System;
	using Heroes;

	using System.Linq;
	using SharpDX.Direct3D9;
	using Service;
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
	internal class OthersAddons : Variables, IAddon
	{
#pragma warning disable CS0414 // The field 'OthersAddons._load' is assigned but its value is never used
		private static bool _load;
		private static Font _text;
		public static Line _line = new Line(Drawing.Direct3DDevice9);

		public void Load()
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			_text = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Monospace",
				   Height = 21,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });
			_load = false;
			Drawing.OnDraw += Drawing_OnDraw;
			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			
			OnLoadMessage();
			me = ObjectManager.LocalHero;
		}

		public void Unload()
		{
			
			Drawing.OnPreReset -= Drawing_OnPreReset;
			Drawing.OnPostReset -= Drawing_OnPostReset;
			Drawing.OnDraw -= Drawing_OnDraw;
			_load = false;
		}
		private float _lastRange, AttackRange;
		private ParticleEffect rangeDisplay, particleEffect;
		/*
		public static readonly List<TrackingProjectile> Projectiles = ObjectManager.TrackingProjectiles.Where(x=>
						x.Source.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Terrorblade
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_TemplarAssassin
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_DrowRanger
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Weaver
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Windrunner
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Enchantress
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Nevermore
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Obsidian_Destroyer
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Clinkz
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Silencer
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Huskar
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Viper
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Sniper
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Razor
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_Morphling
						|| x.Source.ClassID == ClassID.CDOTA_Unit_Hero_DragonKnight).ToList(); */
		public void RunScript()
		{
			if (!MainMenu.OthersMenu.Item("others").IsActive() || !Game.IsInGame || me == null || Game.IsPaused || Game.IsWatchingGame) return;


			e = me.ClosestToMouseTarget(10000);
			//TODO:UNAGRRO
			if (MainMenu.OthersMenu.Item("Auto Un Aggro").GetValue<bool>())
			{
				Toolset.UnAggro(me);
			}
			//TODO:ESCAPE
			/*
	        if (MainMenu.OthersMenu.Item("EscapeAttack").GetValue<bool>() && me.Level>= Menu.Item("minLVL").GetValue<Slider>().Value)
			{
				
				var meed = Toolset.IfITarget(me, Projectiles);
				var v =
					 ObjectManager.GetEntities<Hero>()
						 .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && x.ClassID == meed.Source.ClassID)
						 .ToList();
				foreach (var victim in v)
				{
					if (victim.Distance2D(me) <= 1000 && me.IsVisibleToEnemies && (victim.Handle!=e.Handle || me.Health <= (me.MaximumHealth * 0.4)))
					{
						AutoDodge.qqNyx();
						AutoDodge.qqTemplarRefraction();
						AutoDodge.qqallHex(victim);
						AutoDodge.qquseShiva();
						AutoDodge.qquseManta();
						AutoDodge.qquseHelbard(victim);
						AutoDodge.qquseGhost();
						AutoDodge.qquseEulEnem(victim);
						AutoDodge.qquseSDisription(victim);
						AutoDodge.qquseSheep(victim);
						AutoDodge.qquseColba(victim);
						AutoDodge.qqsilencerLastWord(victim);
						AutoDodge.qquseSDisription(victim);
						AutoDodge.qquseSheep(victim);
						AutoDodge.qquseColba(victim);
						AutoDodge.qqsilencerLastWord(victim);
						AutoDodge.qqabadonWme();
						AutoDodge.qqodImprisomentMe(victim);
						AutoDodge.qqallStun(victim);
					}
				}
	        }*/
		}
		void Drawing_OnDraw(EventArgs args)
		{

			if (me == null)
				return;
			if (!MainMenu.OthersMenu.Item("others").IsActive() || !Game.IsInGame || me == null || Game.IsPaused || Game.IsWatchingGame) return;

			//TODO:ATTACKRANGE
			if (MainMenu.OthersMenu.Item("ShowAttakRange").GetValue<bool>())
			{
                Item item = me.Inventory.Items.FirstOrDefault(x => x != null && x.IsValid && (x.Name.Contains("item_dragon_lance") || x.Name.Contains("item_hurricane_pike")));
                
                if (me.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord && me.HasModifier("modifier_troll_warlord_berserkers_rage"))
			        AttackRange = 150 + me.HullRadius+24;
                   else if (me.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord && !me.HasModifier("modifier_troll_warlord_berserkers_rage"))
                    AttackRange = me.GetAttackRange() + me.HullRadius + 24;
                else
                if (me.ClassID == ClassID.CDOTA_Unit_Hero_TemplarAssassin)
                    AttackRange = me.GetAttackRange() + me.HullRadius;
                else 
                if (me.ClassID == ClassID.CDOTA_Unit_Hero_DragonKnight && me.HasModifier("modifier_dragon_knight_dragon_form"))
                    AttackRange = me.GetAttackRange() + me.HullRadius+24;
                else
                if(item == null && me.IsRanged)
                    AttackRange = me.GetAttackRange() + me.HullRadius + 24;
                else
               if (item !=null && me.IsRanged)
                    AttackRange = me.GetAttackRange() + me.HullRadius + 24;
                else
                    AttackRange = me.GetAttackRange() + me.HullRadius;
                if (rangeDisplay == null)
					{
						if (me.IsAlive)
						{
							rangeDisplay = me.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
							rangeDisplay.SetControlPoint(1, new Vector3(255, 0, 222));
							rangeDisplay.SetControlPoint(3, new Vector3(5, 0, 0));
							rangeDisplay.SetControlPoint(2, new Vector3(_lastRange, 255, 0));
						}
					}
					else
					{
						if (!me.IsAlive)
						{
							rangeDisplay.Dispose();
							rangeDisplay = null;
						}
						else if (_lastRange != AttackRange)
						{
							rangeDisplay.Dispose();
							_lastRange = AttackRange;
							rangeDisplay = me.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
							rangeDisplay.SetControlPoint(1, new Vector3(255, 0, 222));
							rangeDisplay.SetControlPoint(3, new Vector3(5, 0, 0));
							rangeDisplay.SetControlPoint(2, new Vector3(_lastRange, 255, 0));
						}
					}
				}
				else
				{
					if (rangeDisplay != null) rangeDisplay.Dispose();
					rangeDisplay = null;
				}
			

			//TODO:TARGETMARKER
			e = me.ClosestToMouseTarget(10000);
			if (e != null && e.IsValid && !e.IsIllusion && e.IsAlive && e.IsVisible &&
			    MainMenu.OthersMenu.Item("ShowTargetMarker").GetValue<bool>())
			{
				DrawTarget();
			}
			else if (particleEffect != null)
			{
				particleEffect.Dispose();
				particleEffect = null;
			}
			// TY  splinterjke.:)

		}

		private void DrawTarget()
	    {
			if (particleEffect == null)
			{
				particleEffect = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", e);    
				particleEffect.SetControlPoint(2, new Vector3(me.Position.X, me.Position.Y, me.Position.Z));
				particleEffect.SetControlPoint(6, new Vector3(1, 0, 0)); 
				particleEffect.SetControlPoint(7, new Vector3(e.Position.X, e.Position.Y, e.Position.Z));
			}
			else 
			{
				particleEffect.SetControlPoint(2, new Vector3(me.Position.X, me.Position.Y, me.Position.Z));
				particleEffect.SetControlPoint(6, new Vector3(1, 0, 0));
				particleEffect.SetControlPoint(7, new Vector3(e.Position.X, e.Position.Y, e.Position.Z));
			}
		}
		private static void Drawing_OnPostReset(EventArgs args)
		{
			_text.OnResetDevice();
		}

		private static void Drawing_OnPreReset(EventArgs args)
		{
			_text.OnLostDevice();
		}
		
		private void OnLoadMessage()
		{
			Game.PrintMessage("<font face='verdana' color='#ffa420'>@addon OtherAddons is Loaded!</font>", MessageType.LogMessage);
			Service.Debug.Print.ConsoleMessage.Encolored("@addon OtherAddons is Loaded!", ConsoleColor.Yellow);
		}
	}
}
