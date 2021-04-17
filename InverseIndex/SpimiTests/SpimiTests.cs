using NUnit.Framework;
using InverseIndex;
using System.IO;

namespace SpimiTests
{
    public class SpimiTests
    {

        [Test]
        public void Test1ToBuildIndex()
        {
            var directory = Directory.GetCurrentDirectory();
            var spimi = new Spimi(directory + "/../../../TermsAndDocIds/test1", directory + "/../../../index.txt");
            spimi.BuildIndex();
            var lines = File.ReadAllLines(Directory.GetCurrentDirectory() + "/../../../index.txt");
            Assert.AreEqual("bag 1 4", lines[0]);
            Assert.AreEqual("ball 2 6 7", lines[1]);
            Assert.AreEqual("bird 1 9", lines[2]);
            Assert.AreEqual("cat 3 0 4 9", lines[3]);
            Assert.AreEqual("dog 2 1 8", lines[4]);
            Assert.AreEqual("elephant 2 3 5", lines[5]);
            Assert.AreEqual("skating 1 5", lines[6]);
        }

        [Test]
        public void Test2ToBuildIndex()
        {
            var directory = Directory.GetCurrentDirectory();
            var spimi = new Spimi(directory + "/../../../TermsAndDocIds/test2", directory + "/../../../index.txt");
            spimi.BuildIndex();
            var lines = File.ReadAllLines(Directory.GetCurrentDirectory() + "/../../../index.txt");
            Assert.AreEqual("bicycle 2 0 4", lines[0]);
            Assert.AreEqual("everything 1 0", lines[1]);
            Assert.AreEqual("fly 3 2 6 9", lines[2]);
            Assert.AreEqual("ride 3 3 5 8", lines[3]);
            Assert.AreEqual("shop 1 7", lines[4]);
            Assert.AreEqual("stop 1 6", lines[5]);
            Assert.AreEqual("trip 1 9", lines[6]);
        }
    }
}