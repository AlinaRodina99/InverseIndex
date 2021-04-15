using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
        public Dictionary<string, int> ArticleNameAndNumber { get; private set; }

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

            var docsFirstIndex = new int[30];
            var index = 0;
            var docsAmount = 0;
            foreach (var corpusFile in corpusFiles)
            {
                docsFirstIndex[index] = docsAmount;
                docsAmount += Regex.Matches(File.ReadAllText(corpusFile), @"\[\[").Count;
                ++index;
            }

            Parallel.ForEach(corpusFiles, corpusFile =>
            {
                var docNumber = docsFirstIndex[Convert.ToInt32(Path.GetFileNameWithoutExtension(corpusFile).Substring(4))] - 1;
                var tokenizer = new TextTokenizer(File.ReadAllText(corpusFile));
                try
                {
                    using (var streamWriter = File.CreateText(pathToTokenizedCorpus + Path.AltDirectorySeparatorChar + @"tokenized_" + Path.GetFileName(corpusFile)))
                    {
                        foreach (var token in tokenizer.Tokenize())
                        {
                            var tokenWithoutSpecialCharacters = "";
                            if (token.Contains("ArticleBeggining"))
                            {
                                ++docNumber;
                                tokenWithoutSpecialCharacters = RemoveSpecialCharacters(token.Replace("ArticleBeggining", ""));
                            }
                            else
                            {
                                tokenWithoutSpecialCharacters = RemoveSpecialCharacters(token);
                            }

                            if (token != "")
                            {
                                streamWriter.WriteLine(tokenWithoutSpecialCharacters + " " + docNumber);
                            }
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
