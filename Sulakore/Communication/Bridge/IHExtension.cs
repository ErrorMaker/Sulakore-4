namespace Sulakore.Communication.Bridge
{
    public interface IHExtension
    {
        IHContractor Contractor { get; set; }

        void InitializeExtension();
    }
}