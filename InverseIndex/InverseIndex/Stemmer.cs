using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Porter2Stemmer;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace InverseIndex
{
    /// <summary>
    /// Class that implements stemming.
    /// </summary>
    public class Stemmer
    {
        private readonly string pathToTokenizedCorpus;
        private readonly string pathToTermsAndDocIds;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pathToTokenizedCorpus">Path to files with tokens.</param>
        /// <param name="pathToTermsAndDocIds">Path to files with pairs(terms and document ids).</param>
        public Stemmer(string pathToTokenizedCorpus, string pathToTermsAndDocIds)
        {
            this.pathToTokenizedCorpus = pathToTokenizedCorpus;
            this.pathToTermsAndDocIds = pathToTermsAndDocIds;
        }

        /// <summary>
        /// Method to get lemmas from tokens.
        /// </summary>
        public void GetLemmas()
        {
            var stemmer = new EnglishPorter2Stemmer();
            var tokenizedFiles = Directory.GetFiles(pathToTokenizedCorpus);
            var locker = new object();
            Parallel.ForEach(tokenizedFiles, tokenizedFile =>
            {
                try
                {
                    using (var streamWriter = new StreamWriter($"{pathToTermsAndDocIds}\\lemmas{Path.GetFileNameWithoutExtension(Path.GetFileName(tokenizedFile)).Substring(4)}.txt", false, Encoding.Default))
                    {
                        var tokensAndDocIds = new BlockingCollection<string>();
                        var lines = File.ReadAllLines(tokenizedFile);
                        foreach (var line in lines)
                        {
                            tokensAndDocIds.Add(line);
                        }

                        foreach (var tokenAndDocId in tokensAndDocIds)
                        {
                            var token = tokenAndDocId.Split(' ')[0];
                            streamWriter.Write(stemmer.Stem(token).Value + $" {tokenAndDocId.Split(' ')[1]}");
                            streamWriter.WriteLine();
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
