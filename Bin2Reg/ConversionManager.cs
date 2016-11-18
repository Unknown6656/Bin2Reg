using System.IO;
using System;

namespace Bin2Reg
{
    using ResourceHandlers;
    using Encoders;

    internal class ConversionManager
        : IDisposable
    {
        private IResourceHandler _fileHandler;
        private IResourceHandler _registryHandler;
        private IEncoder _encoder;
        

        public ConversionManager(IResourceHandler fileHandler, IResourceHandler registryHandler, IEncoder encoder)
        {
            _fileHandler = fileHandler;
            _registryHandler = registryHandler;
            _encoder = encoder;
        }

        internal string Run(Action action, string registryKey, string filePath)
        {
            switch (action)
            {
                case Action.Store:
                    return StoreFileInRegistry(filePath, registryKey);
                case Action.Restore:
                    return RestoreFileFromRegistry(filePath, registryKey);
                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }
        }

        private string StoreFileInRegistry(string path, string regkey)
        {
            byte[] data = _fileHandler.GetFile(path);

            if (data == null)
                return "Error: Could not find the specified file.";

            data = _encoder.Encode(data);

            return _registryHandler.SaveFile(data, regkey)
                 ? $"The file {Path.GetFileName(path)} has been stored successfully."
                 : "Error: Could not save the specified file.";
        }

        private string RestoreFileFromRegistry(string path, string regkey)
        {
            if (File.Exists(path))
                return "Error: The specified file already exists.";

            byte[] data = _registryHandler.GetFile(regkey);

            if (data == null)
                return "Error: Could not find the specified value in the registry.";

            data = _encoder.Decode(data);

            return _fileHandler.SaveFile(data, path)
                 ? $"The file {Path.GetFileName(path)} has been restored successfully."
                 : "Error: Could not restore the file.";
        }

        public void Dispose()
        {
            _encoder = null;
            _fileHandler = null;
            _registryHandler = null;

            // the following lines are optional
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public enum Action
    {
        Store,
        Restore
    }
}
