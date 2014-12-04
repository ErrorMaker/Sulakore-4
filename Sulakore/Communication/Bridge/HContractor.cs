using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Sulakore.Protocol;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sulakore.Communication.Bridge
{
    public sealed class HContractor : MarshalByRefObject, IHContractor
    {
        #region Private Fields
        private readonly IList<IHExtension> _extensionList;
        private readonly object _unloadAllLock, _unloadLock, _setAllExtStateLock;
        private readonly IDictionary<IHExtension, AppDomain> _extensionDictionary;

        private static readonly string _currentAssemblyName, _hExtensionProxyFullName;

        private const string InitialExtensionDirectory = "Extensions";
        private const BindingFlags ProxyBindingFlags = (BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance);
        #endregion

        #region Public Properties
        public HProtocol Protocol
        {
            get { return Connection.Protocol; }
        }
        public IHConnection Connection { get; set; }
        #endregion

        #region Constructor(s)
        static HContractor()
        {
            _hExtensionProxyFullName = typeof(HExtensionProxy).FullName;
            _currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        }
        public HContractor()
        {
            _unloadLock = new object();
            _unloadAllLock = new object();
            _setAllExtStateLock = new object();
            _extensionList = new List<IHExtension>();
            _extensionDictionary = new Dictionary<IHExtension, AppDomain>();
        }
        #endregion

        #region Private Methods
        private void DistributionException(Task task)
        {
            SKore.Debugger(task.Exception.ToString());
        }
        #endregion

        #region Public Methods
        public void SendToClient(byte[] data)
        {
            Connection.SendToClient(data);
        }
        public void SendToServer(byte[] data)
        {
            Connection.SendToServer(data);
        }

        public void DistributeIncoming(byte[] data)
        {
            if (_extensionDictionary.Count < 1) return;

            IHExtension[] extensions = _extensionDictionary.Keys.ToArray();
            foreach (IHExtension extension in extensions)
                Task.Factory.StartNew(() => extension.DataToClient(data), TaskCreationOptions.PreferFairness)
                    .ContinueWith(DistributionException, TaskContinuationOptions.OnlyOnFaulted);
        }
        public void DistributeOutgoing(byte[] data)
        {
            if (_extensionDictionary.Count < 1) return;

            IHExtension[] extensions = _extensionDictionary.Keys.ToArray();
            foreach (IHExtension extension in extensions)
                Task.Factory.StartNew(() => extension.DataToServer(data), TaskCreationOptions.PreferFairness)
                    .ContinueWith(DistributionException, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void SetAllExtensionsState(HExtensionState state)
        {
            lock (_setAllExtStateLock)
            {
                IHExtension[] extensions = _extensionList.ToArray();
                foreach (IHExtension extension in extensions) extension.SetExtensionState(state);
            }
        }
        public void SetExtensionStateAt(HExtensionState state, int index)
        {
            if (index < 0 || index >= _extensionDictionary.Count) return;
            _extensionList[index].SetExtensionState(state);
        }
        public void SetExtensionState(HExtensionState state, IHExtension extension)
        {
            extension.SetExtensionState(state);
        }

        public void UnloadAllExtensions()
        {
            lock (_unloadAllLock)
            {
                IHExtension[] extensions = _extensionList.ToArray(); 
                foreach (IHExtension extension in extensions) InitiateUnload(extension);
            }
        }
        public void UnloadExtensionAt(int index)
        {
            if (index < 0 || index >= _extensionList.Count) return;
            InitiateUnload(_extensionList[index]);
        }
        public void InitiateUnload(IHExtension extension)
        {
            lock (_unloadLock)
            {
                try
                {
                    if (extension == null || !_extensionDictionary.ContainsKey(extension)) return;

                    AppDomain extensionDomain = _extensionDictionary[extension];
                    _extensionDictionary.Remove(extension);
                    _extensionList.Remove(extension);
                    extension.DisposeExtension();

                    AppDomain.Unload(extensionDomain);
                    extensionDomain = null;

                    File.Delete(extension.Location);
                    extension = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (AppDomainUnloadedException) { }
            }
        }

        public IHExtension LoadExtension(string path)
        {
            HExtensionProxy loadedExtension = null;

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path) || !path.EndsWith(".dll"))
                return loadedExtension;

            if (!Directory.Exists(InitialExtensionDirectory))
                Directory.CreateDirectory(InitialExtensionDirectory);

            string randomId = Guid.NewGuid().ToString().Remove(0, 24);
            string extensionName = Path.GetFileNameWithoutExtension(path);
            string copiedTo = Path.Combine(Environment.CurrentDirectory, InitialExtensionDirectory, string.Format("{0}({1}).dll", extensionName, randomId));
            File.Copy(path, copiedTo);

            AppDomain extensionDomain = AppDomain.CreateDomain(randomId);
            loadedExtension = (HExtensionProxy)extensionDomain.CreateInstanceAndUnwrap(_currentAssemblyName,
                _hExtensionProxyFullName, false, ProxyBindingFlags, null, new[] { copiedTo }, null, null);

            loadedExtension.Contractor = this;
            loadedExtension.Location = copiedTo;
            loadedExtension.Identifier = randomId;

            _extensionList.Add(loadedExtension);
            _extensionDictionary.Add(loadedExtension, extensionDomain);

            return loadedExtension;
        }
        public override object InitializeLifetimeService()
        {
            return null;
        }
        #endregion
    }
}