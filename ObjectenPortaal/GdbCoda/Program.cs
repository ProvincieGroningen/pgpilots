using System;

namespace GdbCoda
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            RenameCodaFields.Rename(args, Console.WriteLine);
        }
    }
}