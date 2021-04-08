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
            var dictionary = new Dictionary<string, List<int>>();
            var locker = new object();

            Parallel.ForEach(stemmedFiles, stemmedFile =>
            {
                var lines = File.ReadAllLines(stemmedFile);
                foreach (var line in lines)
                {
                    var term = line.Split(' ')[0];
                    var docId = Convert.ToInt32(line.Split(' ')[1]);

                    lock (locker)
                    {
                        if (!dictionary.ContainsKey(term))
                        {
                            dictionary.TryAdd(term, new List<int>());
                        }

                        if (dictionary.TryGetValue(term, out List<int> value))
                        {
                            value.Add(docId);
                        }
                    }
                }
            });

            foreach (var postingList in dictionary.Values)
            {
                postingList.Sort();
            }

            var sortedDictionary = new SortedDictionary<string, List<int>>(dictionary);

            Parallel.ForEach(stemmedFiles, stemmedFile =>
            {
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
            });
        }

        /// <summary>
        /// Merging blocks into one.
        /// </summary>
        private void MergeBlocks()
        {
            var blocks = Directory.GetFiles(pathToTermsAndDocIds);
            var locker = new object();
            var queue = new BinaryHeap<List<int>, string>(PriorityQueueType.Minimum);

            Parallel.ForEach(blocks, block =>
            {
                var lines = File.ReadAllLines(block);
                foreach (var line in lines)
                {
                    var term = line.Split(' ')[0];
                    var postingListString = line.Split(' ');
                    var postingListInt = new List<int>();

                    lock (locker)
                    {
                        foreach (var docId in postingListString)
                        {
                            postingListInt.Add(Convert.ToInt32(docId));
                        }
                    }

                    lock (locker)
                    {
                        queue.Enqueue(postingListInt, term);
                    }
                }
            });

            while (queue.Count != 0)
            {
                var priorityOfHeadElement = queue.PeekPriority;
                var headElement = queue.Dequeue();

                while (priorityOfHeadElement == queue.PeekPriority)
                {
                    var nextPostingList = queue.Dequeue();

                    foreach (var el in nextPostingList)
                    {
                        headElement.Add(el);
                    }
                }

                using (var streamWriter = File.CreateText(pathToIndex))
                {
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
