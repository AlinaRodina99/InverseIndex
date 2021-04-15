using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using PriorityQueues;
using System.Collections.Concurrent;

namespace InverseIndex
{
    public class Spimi
    {
        private readonly string pathToTermsAndDocIds;
        private readonly string pathToIndex;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pathToTermsAndDocIds">Path to files with terms and document ids.</param>
        public Spimi(string pathToTermsAndDocIds, string pathToIndex)
        {
            this.pathToTermsAndDocIds = pathToTermsAndDocIds;
            this.pathToIndex = pathToIndex;
        }

        public void BuildIndex()
        {
            BuildBlocks();
            MergeBlocks();
        }

        /// <summary>
        /// First part of Spimi algorithm: splitting into blocks and sorting and aggregating them.
        /// </summary>
        private void BuildBlocks()
        {
            var stemmedFiles = Directory.GetFiles(pathToTermsAndDocIds);

            foreach (var stemmedFile in stemmedFiles)
            { 
                var lines = File.ReadAllLines(stemmedFile);
                var dictionary = new Dictionary<string, List<int>>();
                foreach (var line in lines)
                {
                    var term = line.Split(' ')[0];
                    var docId = Convert.ToInt32(line.Split(' ')[1]);
                    if (!dictionary.ContainsKey(term))
                    {
                        dictionary.Add(term, new List<int>());
                    }

                    if (dictionary.TryGetValue(term, out List<int> value))
                    {
                        if (!value.Contains(docId))
                        {
                            value.Add(docId);
                        }
                    }
                }

                foreach (var postingList in dictionary.Values)
                {
                    postingList.Sort();
                }

                try
                {
                    var sortedDictionary = new SortedDictionary<string, List<int>>(dictionary);
                    using (var streamWriter = File.CreateText(stemmedFile))
                    {
                        foreach (var element in sortedDictionary)
                        {
                            streamWriter.Write(element.Key);
                            foreach (var docId in element.Value)
                            {
                                streamWriter.Write($" {docId}");
                            }
                            streamWriter.WriteLine();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Merging blocks into one.
        /// </summary>
        private void MergeBlocks()
        {
            var blocks = Directory.GetFiles(pathToTermsAndDocIds);
            var locker = new object();
            var queue = new BinaryHeap<List<int>, string>(PriorityQueueType.Minimum);
            var listWithTermsAndPostingLists = new ConcurrentBag<string>();

            Parallel.ForEach(blocks, block =>
            {
                var streamReader = new StreamReader(block);
                using (streamReader)
                {
                    var lines = File.ReadAllLines(block);
                    foreach (var line in lines)
                    {
                        listWithTermsAndPostingLists.Add(line);
                    }
                }
            });

            foreach (var termAndPostingListString in listWithTermsAndPostingLists)
            {
                var term = termAndPostingListString.Split(' ')[0];
                var termAndPostingList = termAndPostingListString.Split(' ');
                var postingListInt = new List<int>();

                for (var i = 1; i < termAndPostingList.Length; ++i)
                {
                    postingListInt.Add(Convert.ToInt32(termAndPostingList[i]));
                }
                
                queue.Enqueue(postingListInt, term);
            }

            while (queue.Count != 0)
            {
                var priorityOfHeadElement = queue.PeekPriority;
                var headElement = queue.Dequeue();

                while (queue.Count != 0 && priorityOfHeadElement == queue.PeekPriority)
                {
                    var nextPostingList = queue.Dequeue();

                    foreach (var el in nextPostingList)
                    {
                        headElement.Add(el);
                    }
                }

                using (var streamWriter = File.CreateText(pathToIndex))
                {
                    headElement.Sort();
                    streamWriter.Write($"{priorityOfHeadElement} {headElement.Count}");
                    foreach (var element in headElement)
                    {
                        streamWriter.Write($" {element}");
                    }

                    streamWriter.WriteLine();
                }
            }
        }
    }
}
