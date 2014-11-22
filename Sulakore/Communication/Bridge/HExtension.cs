namespace Sulakore.Communication.Bridge
{
    public abstract class HExtension : HTriggerBase, IHExtension
    {
        public IHContractor Contractor { get; set; }

        void IHExtension.DisposeExtension()
        {
            base.Dispose();
            Disposed();
        }
        protected abstract void Disposed();

        void IHExtension.InitializeExtension()
        {
            Initialized();
        }
        protected abstract void Initialized();

        public abstract string Authors { get; }
        public abstract string Version { get; }
        public abstract string Identifier { get; }

        public virtual void DataToClient(byte[] data)
        {
            ProcessIncoming(data);
        }
        public virtual void DataToServer(byte[] data)
        {
            ProcessOutgoing(data);
        }
    }
}