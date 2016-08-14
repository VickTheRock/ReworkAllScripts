using Ensage;

namespace DotaAllCombo.Heroes
{
    using Service;
    using Service.Debug;

    internal class LoneDruidController : Variables, IHeroController
    {
        public void Combo()
        {
           
        } // Combo

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("TODO", "0");
            Print.LogMessage.Success("This hero not Supported!");
        }
       
        public void OnCloseEvent()
        {
        }
    }
}