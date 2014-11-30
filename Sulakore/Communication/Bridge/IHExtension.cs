namespace Sulakore.Communication.Bridge
{
    public interface IHExtension
    {
        string Authors { get; }
        string Version { get; }
        string Identifier { get; }
        IHContractor Contractor { get; set; }

        void InitializeExtension();
        void DisposeExtension();

        void OnDataToClient(byte[] data);
        void OnDataToServer(byte[] data);
    }
}