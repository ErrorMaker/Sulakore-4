using System;
using System.IO;
using System.Reflection;

namespace Sulakore.Communication.Bridge
{
    public class HExtensionLoader : MarshalByRefObject
    {
        private readonly IHContractor _contractor;
        private readonly Type _hExtensionType = typeof(HExtension);

        public HExtension Extension { get; private set; }

        public HExtensionLoader(string path, IHContractor contractor)
        {
            _contractor = contractor;
            _hExtensionType = typeof(HExtension);

            byte[] extensionData = File.ReadAllBytes(path);
            var extensionAssembly = Assembly.Load(extensionData);
            Type[] extensionTypes = extensionAssembly.GetTypes();
            foreach (Type extensionType in extensionTypes)
            {
                if (extensionType.BaseType != _hExtensionType) continue;
                Extension = (HExtension)Activator.CreateInstance(extensionType, path, _contractor);
            }
        }
    }
}