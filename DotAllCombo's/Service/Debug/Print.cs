namespace DotaAllCombo.Service.Debug
{
	using System;
	using System.Reflection;
	using Ensage;

	class Print
	{
		public class LogMessage
		{
			public static void Error(string text, params object[] arguments)
			{
				Game.PrintMessage("<font color='#ff0000'>" + text + "</font>", MessageType.LogMessage);
			}

			public static void Success(string text, params object[] arguments)
			{
				Game.PrintMessage("<font color='#00ff00'>" + text + "</font>", MessageType.LogMessage);
			}

			public static void Info(string text, params object[] arguments)
			{
				Game.PrintMessage("<font color='#ffffff'>" + text + "</font>", MessageType.LogMessage);
			}
		} // Console class

		public class ConsoleMessage
		{
			public static void Encolored(string text, ConsoleColor color, params object[] arguments)
			{
				var clr = System.Console.ForegroundColor;
				System.Console.ForegroundColor = color;
				System.Console.WriteLine(text, arguments);
				System.Console.ForegroundColor = clr;
			}

			public static void Error(string text, params object[] arguments)
			{
				Encolored(text, ConsoleColor.Red, arguments);
			}

			public static void Success(string text, params object[] arguments)
			{
				Encolored(text, ConsoleColor.Green, arguments);
			}

			public static void Info(string text, params object[] arguments)
			{
				Encolored(text, ConsoleColor.Yellow, arguments);
			}
		} // LogMessage class
	} // Print class
}
