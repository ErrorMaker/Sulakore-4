using System;
using System.IO;
using Sulakore.Protocol;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sulakore.Communication.Bridge
{
    public abstract class HContractor : IHContractor
    {
        protected readonly List<IHExtension> Extensions = new List<IHExtension>();

        public HProtocols Protocol
        {
            get { return Connection.Protocol; }
        }
        public abstract IHConnection Connection { get; }

        public virtual IHExtension LoadExtension(string path, AppDomain domain = null)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path) ||
                !path.EndsWith(".dll")) return null;

            domain = domain ?? AppDomain.CurrentDomain;
            const string ExtensionsDirName = "Extensions";

            if (!Directory.Exists(ExtensionsDirName))
                Directory.CreateDirectory(ExtensionsDirName);

            var extensionInfo = new FileInfo(path);
            string suffixName = Guid.NewGuid().ToString().Remove(0, 24);
            string extName = extensionInfo.Name.Split('.')[0];
            string newPath = Path.Combine(Environment.CurrentDirectory, ExtensionsDirName, string.Format("{0}({1}).dll", extName, suffixName));
            extensionInfo.CopyTo(newPath, true);

            Type ihExtensionType = typeof(IHExtension);
            IHExtension extension = null;

            var extensionAssembly = Assembly.LoadFile(newPath);
            Type[] extensionTypes = extensionAssembly.GetTypes();

            foreach (Type extensionType in extensionTypes)
            {
                if (extensionType.IsInterface || extensionType.IsAbstract) continue;
                if (extensionType.GetInterface(ihExtensionType.FullName, true) == null) continue;

                extension = (IHExtension)domain.CreateInstanceFromAndUnwrap(newPath, extensionType.FullName);
                extension.Contractor = this;
                Extensions.Add(extension);
            }
            return extension;
        }

        public virtual void DistributeToClient(byte[] data)
        {
            Task.Factory.StartNew(() =>
                Parallel.ForEach(Extensions, extension =>
                    extension.OnDataToClient(data)), TaskCreationOptions.PreferFairness)
                    .ContinueWith(DistributionException, TaskContinuationOptions.OnlyOnFaulted);
        }
        public virtual void DistributeToServer(byte[] data)
        {
            Task.Factory.StartNew(() =>
                Parallel.ForEach(Extensions, extension =>
                    extension.OnDataToServer(data)), TaskCreationOptions.PreferFairness)
                    .ContinueWith(DistributionException, TaskContinuationOptions.OnlyOnFaulted);
        }
        private void DistributionException(Task<ParallelLoopResult> task)
        {
            SKore.Debugger(task.Exception.ToString());
        }

        public void SendToClient(byte[] data)
        {
            Connection.SendToClient(data);
        }
        public void SendToServer(byte[] data)
        {
            Connection.SendToServer(data);
        }

        public void DockExtension(IHExtension extension)
        {
            extension.DisposeExtension();
        }
    }
}