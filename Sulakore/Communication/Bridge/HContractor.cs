using System;
using System.IO;
using Sulakore.Protocol;
using System.Reflection;
using System.Collections.Generic;

namespace Sulakore.Communication.Bridge
{
    public abstract class HContractor : IHContractor
    {
        protected readonly List<IHExtension> Extensions = new List<IHExtension>();
        public abstract IHConnection Connection { get; }

        public IHExtension LoadExtension(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path) ||
                !path.EndsWith(".dll")) return null;

            const string ExtensionsDirName = "Extensions";
            IHExtension extension = null;
            if (!Directory.Exists(ExtensionsDirName))
                Directory.CreateDirectory(ExtensionsDirName);

            var extensionInfo = new FileInfo(path);
            string suffixName = Guid.NewGuid().ToString().Remove(0, 24);
            string extName = extensionInfo.Name.Split('.')[0];
            string newPath = Path.Combine(Environment.CurrentDirectory, ExtensionsDirName, string.Format("{0}({1}).dll", extName, suffixName));
            extensionInfo.CopyTo(newPath, true);

            var extensionName = AssemblyName.GetAssemblyName(newPath);
            var extensionAssembly = Assembly.Load(extensionName);

            Type ihExtensionType = typeof(IHExtension);
            Type[] extensionTypes = extensionAssembly.GetTypes();
            foreach (Type extensionType in extensionTypes)
            {
                if (extensionType.IsInterface || extensionType.IsAbstract) continue;
                if (extensionType.GetInterface(ihExtensionType.FullName) == null) continue;

                extension = (IHExtension)Activator.CreateInstance(extensionType);
                extension.Contractor = this;
                Extensions.Add(extension);
            }
            return extension;
        }

        public void SendToClient(byte[] data)
        {
            Connection.SendToClient(data);
        }
        public void SendToServer(byte[] data)
        {
            Connection.SendToServer(data);
        }
    }
}