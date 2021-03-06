using System;
using System.IO;
using Porter2StemmerStandard;
using System.Threading.Tasks;

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
            Parallel.ForEach(tokenizedFiles, tokenizedFile =>
            {
                try
                {
                    using (var streamWriter = File.CreateText(pathToTermsAndDocIds + Path.AltDirectorySeparatorChar + @"lemmas_" + Path.GetFileName(tokenizedFile).Substring(10)))
                    { 
                        var lines = File.ReadAllLines(tokenizedFile);

                        foreach (var line in lines)
                        {
                            var tokenAndDocId = line.Split(' ');
                            var token = tokenAndDocId[0];
                            streamWriter.WriteLine(stemmer.Stem(token).Value + $" {tokenAndDocId[1]}");
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
