using System;
using System.Reflection;

namespace Sulakore.Communication.Bridge
{
    public sealed class HExtensionProxy : MarshalByRefObject, IHExtension
    {
        #region Private Fields
        private IHExtension _linkedExtension;

        private static readonly string _iHExtensionFullName;
        #endregion

        #region Public Properties
        public string Name
        {
            get { return _linkedExtension.Name; }
        }
        public string Author
        {
            get { return _linkedExtension.Author; }
        }
        public string Version
        {
            get { return _linkedExtension.Version; }
        }
        public string Location
        {
            get { return _linkedExtension.Location; }
            set { _linkedExtension.Location = value; }
        }
        public string Identifier
        {
            get { return _linkedExtension.Identifier; }
            set { _linkedExtension.Identifier = value; }
        }
        public IHContractor Contractor
        {
            get { return _linkedExtension.Contractor; }
            set { _linkedExtension.Contractor = value; }
        }
        #endregion

        #region Constructor(s)
        static HExtensionProxy()
        {
            _iHExtensionFullName = typeof(IHExtension).FullName;
        }
        public HExtensionProxy(string path)
        {
            Assembly extensionAssembly = Assembly.LoadFile(path);
            Type[] extensionTypes = extensionAssembly.GetTypes();
            foreach (Type extensionType in extensionTypes)
            {
                if (extensionType.IsInterface || extensionType.IsAbstract) continue;
                if (extensionType.GetInterface(_iHExtensionFullName) == null) continue;

                _linkedExtension = (IHExtension)Activator.CreateInstance(extensionType, null, null);
            }
        }
        #endregion

        #region Public Methods
        public void DisposeExtension()
        {
            _linkedExtension.DisposeExtension();
        }
        public void InitializeExtension()
        {
            _linkedExtension.InitializeExtension();
        }
        public void SetExtensionState(HExtensionState state)
        {
            _linkedExtension.SetExtensionState(state);
        }

        public void DataToClient(byte[] data)
        {
            _linkedExtension.DataToClient(data);
        }
        public void DataToServer(byte[] data)
        {
            _linkedExtension.DataToServer(data);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
        #endregion
    }
}