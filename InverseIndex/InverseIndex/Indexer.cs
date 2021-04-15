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

            var pathToCorpus = SpecifyValue("path to corpus");
            if (pathToCorpus == "")
            {
                pathToCorpus = Directory.GetCurrentDirectory() + "/InverseIndex/Corpus";
            }

            var pathToTokenizedCorpus = SpecifyValue("path to tokenized corpus");
            if (pathToTokenizedCorpus == "")
            {
                pathToTokenizedCorpus = Directory.GetCurrentDirectory() + "/InverseIndex/TokenizedCorpus";
            }
            Directory.CreateDirectory(pathToTokenizedCorpus);

            var tokenization = new Tokenizer(pathToCorpus, pathToTokenizedCorpus);
            tokenization.Tokenize();

            var pathToTermsAndDocIds = SpecifyValue("path to terms and documents id");
            if (pathToTermsAndDocIds == "")
            {
                pathToTermsAndDocIds = Directory.GetCurrentDirectory() + "/InverseIndex/TermsAndDocIds";
            }
            Directory.CreateDirectory(pathToTermsAndDocIds);

            var stemmer = new Stemmer(pathToTokenizedCorpus, pathToTermsAndDocIds);
            stemmer.GetLemmas();

            var pathToIndex = SpecifyValue("path to index");
            if (pathToIndex == "")
            {
                pathToIndex = Directory.GetCurrentDirectory() + "/InverseIndex/Index.txt";
            }

            var buildingIndex = new Spimi(pathToTermsAndDocIds, pathToIndex);
            buildingIndex.BuildIndex();

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
    }
}
