using System;
using System.IO;
using System.Linq;

namespace InverseIndex
{
    /// <summary>
    /// Indexer
    /// </summary>
    public class Indexer
    {
        /// <summary>
        /// Starts indexer
        /// </summary>
        public void Start()
        {
            Console.WriteLine();
            Console.WriteLine("Hello!");
            Console.WriteLine("This program processes given corpus and provides boolean search within it using created inverse index.");

            var pathToIndex = Directory.GetCurrentDirectory() + "/InverseIndex/Index.txt";

            if (File.Exists(pathToIndex))
            {
                Console.WriteLine();
                Console.WriteLine($"Would you like to re-create index? Y/N");
                Console.WriteLine($"NOTE: if N, existed index will be used.");
                var indexInput = Console.ReadLine();
                while (indexInput != "Y" && indexInput != "y" && indexInput != "N" && indexInput != "n")
                {
                    Console.WriteLine("Please, enter Y or N.");
                    indexInput = Console.ReadLine();
                }

                if (indexInput == "Y" || indexInput == "y")
                {
                    pathToIndex = CreateIndex();
                }
            }
            else
            {
                pathToIndex = CreateIndex();
            }

            Console.WriteLine();
            Console.WriteLine("Enter your query.");
            var input = Console.ReadLine();
            var parsedInput = "";
            try
            {
                var parser = new Parser(input);
                parsedInput = parser.Parse();

                var processor = new Processor(pathToIndex, Enumerable.Range(0, 11248).ToArray());
                Console.WriteLine($"Documents' ids: {processor.Process(parsedInput)}");
            }
            catch
            {
                Console.WriteLine("Incorrect query.");
            }
        }

        /// <summary>
        /// Specifies given value from user if desired
        /// </summary>
        /// <param name="specifyingValue">Value to specify</param>
        /// <returns>Specified value if desired, empty string otherwise</returns>
        private string SpecifyValue(string specifyingValue)
        {
            Console.WriteLine();
            Console.WriteLine($"Would you like to specify the {specifyingValue}? Y/N");
            Console.WriteLine($"NOTE: if N, default value will be used.");
            var input = Console.ReadLine();
            while (input != "Y" && input != "y" && input != "N" && input != "n")
            {
                Console.WriteLine("Please, enter Y or N.");
                input = Console.ReadLine();
            }

            if (input == "Y" || input == "y")
            {
                Console.WriteLine($"Enter the {specifyingValue}");
                return Console.ReadLine();
            }

            return "";
        }

        /// <summary>
        /// Creates index
        /// </summary>
        /// <returns>Path to index</returns>
        private string CreateIndex()
        {
            var pathToCorpus = SpecifyValue("path to corpus");
            if (pathToCorpus == "")
            {
                pathToCorpus = Directory.GetCurrentDirectory() + "/InverseIndex/Corpus";
            }

            var pathToTokenizedCorpus = Directory.GetCurrentDirectory() + "/InverseIndex/TokenizedCorpus";
            Directory.CreateDirectory(pathToTokenizedCorpus);

            var tokenization = new Tokenizer(pathToCorpus, pathToTokenizedCorpus);
            tokenization.Tokenize();

            var pathToTermsAndDocIds = Directory.GetCurrentDirectory() + "/InverseIndex/TermsAndDocIds";
            Directory.CreateDirectory(pathToTermsAndDocIds);

            var stemmer = new Stemmer(pathToTokenizedCorpus, pathToTermsAndDocIds);
            stemmer.GetLemmas();

            var pathToIndex = Directory.GetCurrentDirectory() + "/InverseIndex/Index.txt";

            var buildingIndex = new Spimi(pathToTermsAndDocIds, pathToIndex);
            buildingIndex.BuildIndex();

            return pathToIndex;
        }
    }
}
