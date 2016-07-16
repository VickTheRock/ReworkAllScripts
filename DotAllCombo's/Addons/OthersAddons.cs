
using System.Collections.Generic;
using Ensage.Common.Menu;

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
		private ParticleEffect rangeDisplay;
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
                
                if (me.HasModifier("modifier_troll_warlord_berserkers_rage"))
			        AttackRange = 150 + me.HullRadius+24;
                   else if (!me.HasModifier("modifier_troll_warlord_berserkers_rage"))
                    AttackRange = me.GetAttackRange() + me.HullRadius + 24;
                else
                if (me.Name == "npc_dota_hero_templar_assassin")
                    AttackRange = me.GetAttackRange() + me.HullRadius;
                else 
                if (me.Name == "npc_dota_hero_dragon_knight" && me.HasModifier("modifier_dragon_knight_dragon_form"))
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
			if (e == null) return;
			if (!e.IsIllusion && e.IsAlive)
			{

				if (!OnScreen(e.Position)) return;
				var screenPos = HUDInfo.GetHPbarPosition(e);
				var text = "Target to Death";
				var size = new Vector2(18, 18);
				var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
				var position = new Vector2(screenPos.X - textSize.X + 85, screenPos.Y -35);
				Drawing.DrawText(text,position,size,(me.Distance2D(e) <= 1200 ? Color.Coral : Color.White),
					FontFlags.AntiAlias);
				Drawing.DrawText(text, new Vector2(screenPos.X - textSize.X + 84, screenPos.Y - 35),
					 size, Color.Black,FontFlags.AntiAlias);
				// Drawing.DrawText("Target to Death", HeroPositionOnScreen(e), new Vector2(16, 20), me.Distance2D(e) < 1200 ? Color.Coral : Color.WhiteSmoke, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
			}
		}

		private bool OnScreen(Vector3 v)
		{
			return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
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