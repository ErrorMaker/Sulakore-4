namespace Sulakore.Communication.Bridge
{
    public interface IHExtension
    {
        string Name { get; }
        string Author { get; }
        string Version { get; }
        string Location { get; }
        IHContractor Contractor { get; }

        void DisposeExtension();
        void InitializeExtension();
        void SetExtensionState(HExtensionState state);

        void DataToClient(byte[] data);
        void DataToServer(byte[] data);
    }
}