using System;
using System.IO;

namespace CssMergerApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Usage();
                return;
            }

            string inputFilename = args[0];
            string outputFilename = args[1];

            if (!File.Exists(inputFilename))
            {
                Usage("Input file not found");
                return;
            }

            new CssMerger.CssMerger().MergeCss(inputFilename, outputFilename);
        }

        private static void Usage()
        {
            Usage("");
        }

        private static void Usage(string errorMessage)
        {
            Console.WriteLine("Usage: CssMergerApp.exe <input file> <output file>");
            if (!String.IsNullOrEmpty(errorMessage))
                Console.Write("Error: " + errorMessage);
        }
    }
}