using System;
using System.IO;
using System.Threading.Tasks;
using McBits.Tokenization;

namespace InverseIndex
{
    /// <summary>
    /// Tokenization
    /// </summary>
    public class Tokenization
    {
        private string pathToCorpus;
        private string pathToTokenizedCorpus;

        /// <summary>
        /// Tokenization constuctor
        /// </summary>
        /// <param name="pathToCorpus">Path to corpus</param>
        /// <param name="pathToTokenizedCorpus">Path to tokenized corpus</param>
        public Tokenization(string pathToCorpus, string pathToTokenizedCorpus)
        {
            if (!Directory.Exists(pathToCorpus))
            {
                throw new IOException($"{pathToCorpus} is not a valid directory.");
            }
            else
            {
                this.pathToCorpus = pathToCorpus;
            }
            this.pathToTokenizedCorpus = pathToTokenizedCorpus;
        }

        /// <summary>
        /// Tokenizes corpus and adds it to the path of tokenized corpus
        /// </summary>
        public void Tokenize()
        {
            string[] corpusFiles = Directory.GetFiles(pathToCorpus);
            Parallel.ForEach(corpusFiles, corpusFile =>
            {
                var tokenizer = new TextTokenizer(corpusFile);
                try
                {
                    using (StreamWriter streamWriter = File.CreateText(pathToTokenizedCorpus + @"\tokenized_" + Path.GetFileName(corpusFile)))
                    {
                        foreach (var token in tokenizer.Tokenize())
                        {
                            streamWriter.WriteLine(token);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            });
        }
    }
}
