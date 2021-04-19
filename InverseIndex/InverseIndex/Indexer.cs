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
            Console.WriteLine("Enter your boolean query.");
            Console.WriteLine("NOTE: boolean operators AND (&&), OR (||), NOT (-) are written with capital letters.");
            var queryInput = Console.ReadLine();
            do
            {
                var parsedInput = "";
                try
                {
                    var parser = new Parser(queryInput);
                    parsedInput = parser.Parse();

                    var processor = new Processor(pathToIndex, Enumerable.Range(0, 11248).ToArray());
                    var docsId = processor.Process(parsedInput);
                    if (docsId == "")
                    {
                        Console.WriteLine($"Cannot find documents with \"{queryInput}\".");
                    }
                    else
                    {
                        var docsIdArray = docsId.Split(' ');
                        Console.WriteLine($"Documents found: {docsIdArray.Length}.");
                        Console.WriteLine();
                        Console.WriteLine("Enter the amount of documents' ids to show.");
                        var inputAmount = 0;
                        var flag = Int32.TryParse(Console.ReadLine(), out inputAmount) && inputAmount > 0 && inputAmount <= docsIdArray.Length;
                        while (!flag)
                        {
                            Console.WriteLine("Please, enter the correct amount of documents' ids.");
                            flag = Int32.TryParse(Console.ReadLine(), out inputAmount) && inputAmount > 0 && inputAmount <= docsIdArray.Length;
                        }
                        var firstDocsId = docsIdArray.Take(inputAmount);
                        foreach (var id in firstDocsId)
                        {
                            Console.Write(id + " ");
                        }
                        Console.WriteLine();
                    }
                }
                catch
                {
                    Console.WriteLine("Incorrect query.");
                }

                Console.WriteLine();
                Console.WriteLine($"Would you like to enter another query? Y/N");

                queryInput = ProvideInput("your boolean query");
            }
            while (queryInput != "");
        }

        /// <summary>
        /// Provides input if desired
        /// </summary>
        /// <param name="inputValue">Value to enter</param>
        /// <returns>Entered value if desired, empty string otherwise</returns>
        private string ProvideInput(string inputValue)
        {
            var input = Console.ReadLine();
            while (input != "Y" && input != "y" && input != "N" && input != "n")
            {
                Console.WriteLine("Please, enter Y or N.");
                input = Console.ReadLine();
            }

            if (input == "Y" || input == "y")
            {
                Console.WriteLine();
                Console.WriteLine($"Enter {inputValue}.");
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
            Console.WriteLine();
            Console.WriteLine($"Would you like to specify the path to corpus? Y/N");
            Console.WriteLine($"NOTE: if N, default value will be used.");

            var pathToCorpus = ProvideInput("the path to corpus");
            if (pathToCorpus == "")
            {
                pathToCorpus = Directory.GetCurrentDirectory() + "/InverseIndex/Corpus";
            }

            var pathToTokenizedCorpus = Directory.GetCurrentDirectory() + "/InverseIndex/TokenizedCorpus";
            Directory.CreateDirectory(pathToTokenizedCorpus);

            Console.WriteLine();
            Console.WriteLine("Please, wait.");

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
