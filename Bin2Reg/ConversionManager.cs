using System.IO;
using System;

namespace Bin2Reg
{
    using ResourceHandlers;
    using Encoders;

    internal class ConversionManager
    {
        private readonly IResourceHandler _fileHandler;
        private readonly IResourceHandler _registryHandler;
        private readonly IEncoder _encoder;

        public string Result { get; set; }


        public ConversionManager(IResourceHandler fileHandler, IResourceHandler registryHandler, IEncoder encoder)
        {
            _fileHandler = fileHandler;
            _registryHandler = registryHandler;
            _encoder = encoder;
        }

        internal void Run(Action action, string registryKey, string filePath)
        {
            switch (action)
            {
                case Action.Store:
                    StoreFileInRegistry(filePath, registryKey);

                    break;
                case Action.Restore:
                    RestoreFileFromRegistry(filePath, registryKey);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }
        }

        private void StoreFileInRegistry(string path, string regkey)
        {
            byte[] data = _fileHandler.GetFile(path);

            if (data == null)
            {
                Result = "Error: Could not find the specified file.";

                return;
            }

            data = _encoder.Encode(data);
            Result = (_registryHandler.SaveFile(data, regkey))
                   ? $"The file {Path.GetFileName(path)} has been stored successfully."
                   : "Error: Could not save the specified file.";
        }

        private void RestoreFileFromRegistry(string path, string regkey)
        {
            if (File.Exists(path))
            {
                Result = "Error: The file already exists.";

                return;
            }

            byte[] data = _registryHandler.GetFile(regkey);

            if (data == null)
            {
                Result = "Error: Could not find the value in the registry.";

                return;
            }

            data = _encoder.Decode(data);
            Result = (_fileHandler.SaveFile(data, path))
                   ? $"The file {Path.GetFileName(path)} has been restored successfully."
                   : "Error: Could not restore the file.";
        }
    }
}
