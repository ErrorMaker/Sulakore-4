using System;
using System.Threading.Tasks;

using Sulakore.Protocol;
using Sulakore.Communication;

namespace Sulakore.Extensions
{
    public abstract class ExtensionBase : HTriggers, IExtension
    {
        public abstract string Name { get; }
        public abstract string Author { get; }
        public string Location { get; set; }
        public IContractor Contractor { get; set; }

        void IExtension.OnDisposed()
        {
            base.Dispose();
            OnDisposed();
        }
        protected abstract void OnDisposed();

        void IExtension.OnInitialized()
        {
            OnInitialized();
        }
        protected abstract void OnInitialized();

        void IExtension.DataToClient(byte[] data)
        {
            Task.Factory.StartNew(() => base.ProcessIncoming(data));

            var packet = new HMessage(data, HDestination.Client);
            DataToClient(packet);
        }
        protected abstract void DataToClient(HMessage packet);

        void IExtension.DataToServer(byte[] data)
        {
            Task.Factory.StartNew(() => base.ProcessOutgoing(data));

            var packet = new HMessage(data, HDestination.Server);
            DataToServer(packet);
        }
        protected abstract void DataToServer(HMessage packet);
    }
}