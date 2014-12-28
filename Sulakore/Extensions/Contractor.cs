using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Sulakore.Communication;

namespace Sulakore.Extensions
{
    public sealed class Contractor : IContractor
    {
        #region Private/Public Fields
        private readonly IHConnection _connection;
        private readonly IList<IExtension> _extensions;

        private static readonly Type _iExtensionT;
        private static readonly string _currentAsmName;

        private const string ExtDirName = "Extensions";
        #endregion

        #region Private/Public Properties
        public IList<IExtension> Extensions
        {
            get { return _extensions; }
        }
        #endregion

        #region Constructor(s)
        static Contractor()
        {
            _iExtensionT = typeof(IExtension);
            _currentAsmName = Assembly.GetExecutingAssembly().FullName;
        }
        public Contractor(IHConnection connection)
        {
            _connection = connection;
            _extensions = new List<IExtension>();
        }
        #endregion

        public int SendToClient(byte[] data)
        {
            return _connection.SendToClient(data);
        }
        public int SendToServer(byte[] data)
        {
            return _connection.SendToServer(data);
        }

        public void ProcessIncoming(byte[] data)
        {
            if (_extensions.Count < 1) return;

            IList<IExtension> extensions = _extensions;
            foreach (IExtension extension in extensions)
                Task.Factory.StartNew(() => extension.DataToClient(data), TaskCreationOptions.LongRunning)
                    .ContinueWith(ProcessException, TaskContinuationOptions.OnlyOnFaulted);
        }
        public void ProcessOutgoing(byte[] data)
        {
            if (_extensions.Count < 1) return;

            IList<IExtension> extensions = _extensions;
            foreach (IExtension extension in extensions)
                Task.Factory.StartNew(() => extension.DataToServer(data), TaskCreationOptions.LongRunning)
                    .ContinueWith(ProcessException, TaskContinuationOptions.OnlyOnFaulted);
        }

        public IExtension Install(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path) || !path.EndsWith(".dll"))
                return null;

            IExtension extension = null;
            if (!Directory.Exists(ExtDirName))
                Directory.CreateDirectory(ExtDirName);

            string extensionId = Guid.NewGuid().ToString();
            string extensionName = Path.GetFileNameWithoutExtension(path);
            string extensionPath = Path.Combine(Environment.CurrentDirectory, ExtDirName, string.Format("{0}({1}).dll", extensionName, extensionId));
            File.Copy(path, extensionPath);

            byte[] extensionData = File.ReadAllBytes(extensionPath);
            Assembly extensionAssembly = Assembly.Load(extensionData);
            Type[] extensionTypes = extensionAssembly.GetTypes();
            foreach (Type extensionType in extensionTypes)
            {
                if (extensionType.IsInterface || extensionType.IsAbstract) continue;
                if (extensionType.GetInterface(_iExtensionT.FullName) == null) continue;

                extension = (IExtension)Activator.CreateInstance(extensionType);
                extension.Location = extensionPath;
                extension.Contractor = this;
                extension.Version = FileVersionInfo.GetVersionInfo(extensionPath).FileVersion;

                _extensions.Add(extension);
                break;
            }
            return extension;
        }
        public void Uninstall(IExtension extension)
        {
            if (File.Exists(extension.Location))
                File.Delete(extension.Location);

            extension.OnDisposed();
            _extensions.Remove(extension);
        }

        private void ProcessException(Task task)
        {
            Debug.Assert(false, task.Exception.Message);
        }
    }
}