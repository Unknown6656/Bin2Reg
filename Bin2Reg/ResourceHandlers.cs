using System.Collections.Generic;
using System.Collections;
using Microsoft.Win32;
using System.Text;
using System.Linq;
using System.IO;
using System;

namespace Bin2Reg.ResourceHandlers
{
    internal interface IResourceHandler
    {
        bool SaveFile(byte[] data, string path);
        byte[] GetFile(string path);
    }

    internal class RegistryHandler
        : IResourceHandler
    {
        private const string RegistryValueName = "Info";


        public bool SaveFile(byte[] file, string registryKey) => file == null ? false : FileToRegistry(file, registryKey);

        public byte[] GetFile(string registryKey)
        {
            byte[] file = null;

            using (RegistryKey hkcu = Registry.CurrentUser)
            {
                RegistryKey key = hkcu.OpenSubKey(registryKey, false);

                if (key != null)
                    file = (byte[])key.GetValue(RegistryValueName);
            }

            return file;
        }

        private static bool FileToRegistry(IEnumerable file, string registryKey)
        {
            using (RegistryKey registry = Registry.CurrentUser)
            {
                RegistryKey key = OpenRegistryKey(registry, registryKey);

                if (key != null)
                {
                    key.SetValue(RegistryValueName, file, RegistryValueKind.Binary);

                    return true;
                }
            }

            return false;
        }

        private static RegistryKey OpenRegistryKey(RegistryKey registry, string registryKey) =>
            registry.OpenSubKey(registryKey, true) ?? registry.CreateSubKey(registryKey);
    }

    internal class FileHandler
        : IResourceHandler
    {
        public bool SaveFile(byte[] data, string path)
        {
            if (data == null)
                return false;

            File.WriteAllBytes(path, data);

            return true;
        }

        public byte[] GetFile(string path) => !File.Exists(path) ? null : File.ReadAllBytes(path);
    }
}
