using static System.Console;

namespace Bin2Reg
{
    using ResourceHandlers;
    using Encoders;

    public static class Program
    {
        public static int Main(string[] args)
        {
            ArgumentsChecker checker = new ArgumentsChecker(args);

            if (!checker.Run())
            {
                WriteLine(checker.Error);

                return -1;
            }

            using (ConversionManager manager = new ConversionManager(new FileHandler(), new RegistryHandler(), new Dpapi()))
                WriteLine(manager.Run(checker.Action, checker.Key, checker.FilePath));

            return 0;
        }
    }
}
