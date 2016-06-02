
using System.Security.Permissions;

namespace DotaAllCombo
{
	using System;
	using System.Threading;
	internal class Program
    {
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        static void Main(string[] args)
		{

            /*new Thread(delegate () {
				
			}).Start();*/
        Service.Bootstrap.Initialize();
			Service.Debug.Print.ConsoleMessage.Success("[DotaAllCombo's] Pre-initialization complete!");


		}
	}
}

