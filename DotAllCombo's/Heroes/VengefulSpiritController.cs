namespace DotaAllCombo.Heroes
{
    using Service;
    using Service.Debug;
    using Ensage;
    internal class VengefulSpiritController : Variables, IHeroController
    {
        public void Combo()
        {
           
        } // Combo

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("TODO", "0");
            
            Print.LogMessage.Error("This hero not Supported!");
        }
       
        public void OnCloseEvent()
        {
        }
    }
}