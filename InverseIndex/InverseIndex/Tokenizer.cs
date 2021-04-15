using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using McBits.Tokenization;

namespace InverseIndex
{
    /// <summary>
    /// Tokenization
    /// </summary>
    public class Tokenizer
    {
        private string pathToCorpus;
        private string pathToTokenizedCorpus;

        /// <summary>
        /// Tokenizer constuctor
        /// </summary>
        /// <param name="pathToCorpus">Path to corpus</param>
        /// <param name="pathToTokenizedCorpus">Path to tokenized corpus</param>
        public Tokenizer(string pathToCorpus, string pathToTokenizedCorpus)
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
        /// Removes special characters from string
        /// </summary>
        /// <param name="str">Given string</param>
        /// <returns>String without special characters</returns>
        private string RemoveSpecialCharacters(string str)
        {
            var newString = new StringBuilder();
            foreach (char character in str)
            {
                if ((character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z'))
                {
                    newString.Append(character);
                }
            }
            return newString.ToString();
        }

        /// <summary>
        /// Tokenizes corpus and adds it to the path of tokenized corpus
        /// </summary>
        public void Tokenize()
        {
            var corpusFiles = Directory.GetFiles(pathToCorpus);
            Parallel.ForEach(corpusFiles, corpusFile =>
            {
                var tokenizer = new TextTokenizer(File.ReadAllText(corpusFile));
                try
                {
                    using (var streamWriter = File.CreateText(pathToTokenizedCorpus + @"\tokenized_" + Path.GetFileName(corpusFile)))
                    {
                        foreach (var token in tokenizer.Tokenize())
                        {
                            var tokenWithoutSpecialCharacters = RemoveSpecialCharacters(token);
                            streamWriter.WriteLine(tokenWithoutSpecialCharacters + " " + Path.GetFileNameWithoutExtension(corpusFile).Substring(4));
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
