using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System;

namespace Bin2Reg
{
    internal sealed class ArgumentsChecker
    {
        // The pattern: allow any word character including underscore and the back slash (i.e. '\')
        //              except don't let it start with the back slash.
        private static readonly Regex KEY_PATTERN = new Regex(@"^\w[\w\\]*$", RegexOptions.Compiled);
        private static readonly char[] INVALID_CHARS = Path.GetInvalidPathChars();
        private string[] args;

        public string Error { get; private set; }
        public Action Action { get; private set; }
        public string FilePath { get; private set; }
        public string Key { get; private set; }


        public ArgumentsChecker(string[] arguments)
        {
            args = arguments;
            Error = null;
        }
        
        public bool Run()
        {
            if (args.Length < 3)
            {
                PrintUsage();

                return false;
            }

            Action = SetAction(args[0]);
            Key = SetKey(args[1]);
            FilePath = SetFilePath(args[2]);
            
            return !string.IsNullOrEmpty(Error);
        }

        private void PrintUsage()
        {
            string exe_name = new FileInfo(typeof(ArgumentsChecker).Assembly.Location).Name;

            Console.WriteLine($@"
Bin2Reg v1.0 - an application to store binary files in the registry

Usage: {exe_name} [-s, -r] [registry_key] [file_path]
    -s / -r      - the action to take, ""store/restore""
    registry_key - the registry key in HKCU
    file_path    - the path of the file

Ex: {exe_name} -s Software\HiddenData ""C:\my files\test.jpg""
");
        }

        private Action SetAction(string s)
        {
            switch (s)
            {
                case "-s":
                    return Action.Store;
                case "-r":
                    return Action.Restore;
                default:
                    Error += "Error: The first argument is wrong.\n";

                    return new Action();
            }
        }

        private string SetKey(string s)
        {
            if (!KEY_PATTERN.IsMatch(s))
                Error += "Error: The second argument is wrong.\n";

            return s;
        }

        private string SetFilePath(string s)
        {
            if (INVALID_CHARS.Any(s.Contains))
                Error += "Error: The third argument is wrong.\n";

            return s;
        }
    }
}