using System;
using System.IO;

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
            Console.WriteLine("Hello!");
            Console.WriteLine("This program processes given corpus and provides boolean search within it using created inverse index.");

            var pathToCorpus = SpecifyValue("path to corpus");
            if (pathToCorpus == "")
            {
                pathToCorpus = Directory.GetCurrentDirectory() + "/../../../../Corpus";
            }

            var pathToTokenizedCorpus = SpecifyValue("path to tokenized corpus");
            if (pathToTokenizedCorpus == "")
            {
                pathToTokenizedCorpus = Directory.GetCurrentDirectory() + "/../../../../TokenizedCorpus";
            }
            Directory.CreateDirectory(pathToTokenizedCorpus);

            var tokenization = new Tokenizer(pathToCorpus, pathToTokenizedCorpus);
            tokenization.Tokenize();

            var pathToTermsAndDocIds = SpecifyValue("path to terms and documents id");
            if (pathToTermsAndDocIds == "")
            {
                pathToTermsAndDocIds = Directory.GetCurrentDirectory() + "/../../../../TermsAndDocIds";
            }
            Directory.CreateDirectory(pathToTermsAndDocIds);

            var stemmer = new Stemmer(pathToTokenizedCorpus, pathToTermsAndDocIds);
            stemmer.GetLemmas();

            var pathToIndex = SpecifyValue("path to index");
            if (pathToIndex == "")
            {
                pathToIndex = Directory.GetCurrentDirectory() + "/../../../../Index.txt";
            }
            File.Create(pathToIndex);

            var buildingIndex = new Spimi(pathToTermsAndDocIds, pathToIndex);
            buildingIndex.BuildIndex();

            Console.WriteLine("Enter your query.");
            var input = Console.ReadLine();
            var parsedInput = "";
            try
            {
                var parser = new Parser(input);
                parsedInput = parser.Parse();
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
            Console.WriteLine($"Would you like to specify the {specifyingValue}? Y/N");
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
