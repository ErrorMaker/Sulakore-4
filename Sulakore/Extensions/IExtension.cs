namespace Sulakore.Extensions
{
    public interface IExtension
    {
        string Name { get; }
        string Author { get; }
        string Location { get; set; }
        IContractor Contractor { get; set; }

        void OnDisposed();
        void OnInitialized();

        void DataToClient(byte[] data);
        void DataToServer(byte[] data);
    }
}