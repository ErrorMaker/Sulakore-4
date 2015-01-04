﻿using System.Threading.Tasks;

using Sulakore.Protocol;
using Sulakore.Communication;

namespace Sulakore.Extensions
{
    public abstract class ExtensionBase : HTriggers, IExtension
    {
        public abstract string Name { get; }
        public abstract string Author { get; }

        public string Version { get; set; }
        public string Location { get; set; }
        public IContractor Contractor { get; set; }

        void IExtension.OnDisposed()
        {
            Dispose();
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
            Task.Factory.StartNew(() => base.ProcessIncoming(data),
                TaskCreationOptions.LongRunning);

            var packet = new HMessage(data, HDestination.Client);
            DataToClient(packet);
        }
        protected abstract void DataToClient(HMessage packet);

        void IExtension.DataToServer(byte[] data)
        {
            Task.Factory.StartNew(() => base.ProcessOutgoing(data),
                TaskCreationOptions.LongRunning);

            var packet = new HMessage(data, HDestination.Server);
            DataToServer(packet);
        }
        protected abstract void DataToServer(HMessage packet);
    }
}