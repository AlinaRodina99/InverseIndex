using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Porter2Stemmer;
using System.Threading.Tasks;

namespace InverseIndex
{
    public class Stemmer
    {
        private readonly string pathToTokenizedCorpus;
        private readonly string pathToTermsAndDocIds;

        public Stemmer(string pathToTokenizedCorpus, string pathToTermsAndDocIds)
        {
            this.pathToTokenizedCorpus = pathToTokenizedCorpus;
            this.pathToTermsAndDocIds = pathToTermsAndDocIds;
        }

        public void GetLemmas()
        {
            var stemmer = new EnglishPorter2Stemmer();
            var tokenizedFiles = Directory.GetFiles(pathToTokenizedCorpus);
            Parallel.ForEach(tokenizedFiles, tokenizedFile =>
            {
                try
                {
                    using (var streamWriter = new StreamWriter($"{pathToTermsAndDocIds}/lemmas{tokenizedFile.Substring(tokenizedFile.IndexOf('_'))}", false, Encoding.Default))
                    {
                        var tokensAndDocIds = new List<string>();
                        using (var streamReader = new StreamReader($"{pathToTokenizedCorpus}/{tokenizedFile}"))
                        {
                            while (streamReader.ReadLine() != null)
                            {
                                tokensAndDocIds.Add(streamReader.ReadLine());
                            }
                        }

                        foreach (var tokenAndDocId in tokensAndDocIds)
                        {
                            var token = tokenAndDocId.Split(' ')[0];
                            streamWriter.Write(stemmer.Stem(token) + $" {tokenAndDocId.Split(' ')[1]}");
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
