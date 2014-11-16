namespace Sulakore.Communication.Bridge
{
    public abstract class HExtension : IHExtension
    {
        IHContractor IHExtension.Contractor { get; set; }

        void IHExtension.InitializeExtension()
        {
        }
    }
}