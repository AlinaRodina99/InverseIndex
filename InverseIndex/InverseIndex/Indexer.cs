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

            Console.WriteLine("Would you like to specify the path to corpus? Y/N");
            var corpusInput = Console.ReadLine();
            while (corpusInput != "Y" && corpusInput != "y" && corpusInput != "N" && corpusInput != "n")
            {
                Console.WriteLine("Please, enter Y or N.");
                corpusInput = Console.ReadLine();
            }
            var pathToCorpus = Directory.GetCurrentDirectory() + "/../../../../Corpus";
            if (corpusInput == "Y" || corpusInput == "y")
            {
                pathToCorpus = Console.ReadLine();
            }

            Console.WriteLine("Would you like to specify the path to tokenized corpus? Y/N");
            var tokenizedCorpusInput = Console.ReadLine();
            while (tokenizedCorpusInput != "Y" && tokenizedCorpusInput != "y" && tokenizedCorpusInput != "N" && tokenizedCorpusInput != "n")
            {
                Console.WriteLine("Please, enter Y or N.");
                tokenizedCorpusInput = Console.ReadLine();
            }
            var pathToTokenizedCorpus = Directory.GetCurrentDirectory() + "/../../../../TokenizedCorpus";
            if (tokenizedCorpusInput == "Y" || tokenizedCorpusInput == "y")
            {
                pathToTokenizedCorpus = Console.ReadLine();
            }
            Directory.CreateDirectory(pathToTokenizedCorpus);

            var tokenization = new Tokenization(pathToCorpus, pathToTokenizedCorpus);
            tokenization.Tokenize();
        }
    }
}
