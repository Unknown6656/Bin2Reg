using System;

namespace Bin2Reg
{
    using ResourceHandlers;
    using Encoders;

    public static class Program
    {
        public static int Main(string[] args)
        {
            ArgumentsChecker argumentsChecker = new ArgumentsChecker(args);

            if (!argumentsChecker.Run())
            {
                Console.WriteLine(argumentsChecker.Error);

                return -1;
            }

            Action action = argumentsChecker.Action;
            string registryKey = argumentsChecker.Key;
            string filePath = argumentsChecker.FilePath;

            //var encoder = new XorEncoder();
            IEncoder encoder = new Dpapi();
            ConversionManager conversion = new ConversionManager(new FileHandler(), new RegistryHandler(), encoder);

            conversion.Run(action, registryKey, filePath);

            Console.WriteLine(conversion.Result);

            return 0;
        }
    }

    public enum Action
    {
        Store,
        Restore
    }
}
