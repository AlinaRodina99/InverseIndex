using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace InverseIndex
{
    public class Spimi
    {
        private readonly string pathToTermsAndDocIds;

        public Spimi(string pathToTermsAndDocIds)
        {
            this.pathToTermsAndDocIds = pathToTermsAndDocIds;
        }

        public void BuildIndex()
        {
            BuildBlocks();
            MergeBlocks();
        }

        private void BuildBlocks()
        {
            var files = Directory.GetFiles(pathToTermsAndDocIds);
            var dictionary = new Dictionary<string, List<int>>();

            Parallel.ForEach(files, file =>
            {
                using (var streamReader = new StreamReader($"{pathToTermsAndDocIds}/{file}"))
                {
                    while (streamReader.ReadLine() != null)
                    {
                        var term = streamReader.ReadLine().Split(' ')[0];
                        var docId = Convert.ToInt32(streamReader.ReadLine().Split(' ')[1]);
                        if (!dictionary.ContainsKey(term))
                        {
                            dictionary.Add(term, new List<int>());
                        }

                        if (dictionary.TryGetValue(term, out List<int> value))
                        {
                            value.Add(docId);
                        }
                    }

                    foreach (var postingList in dictionary.Values)
                    {
                        postingList.Sort();
                    }

                    var sortedDictionary = new SortedDictionary<string, List<int>>(dictionary);
                    using (var streamWriter = new StreamWriter($"{pathToTermsAndDocIds}/{file}", false, Encoding.Default))
                    {
                        foreach (var element in sortedDictionary)
                        {
                            streamWriter.WriteLine(element.Key + " " + element.Value);
                        }
                    }
                }
            });
        }

        private void MergeBlocks()
        {

        }
    }
}
