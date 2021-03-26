using NUnit.Framework;
using InverseIndex;
using System;

namespace ParserTests
{
    public class ParserTests
    {
        private Parser parser;

        [Test]
        public void SimpleANDParseTest()
        {
            parser = new Parser("word1 & word2");
            Assert.AreEqual("word1 word2 & ", parser.Parse());
        }

        [Test]
        public void SimpleORParseTest()
        {
            parser = new Parser("word1 | word2");
            Assert.AreEqual("word1 word2 | ", parser.Parse());
        }

        [Test]
        public void SimpleNOTParseTest()
        {
            parser = new Parser("- word1");
            Assert.AreEqual("word1 - ", parser.Parse());
        }

        [Test]
        public void DoubleNOTParseTest()
        {
            parser = new Parser("- - word1");
            Assert.AreEqual("word1 ", parser.Parse());
        }

        [Test]
        public void ManyNOTParseTest()
        {
            parser = new Parser("- - - - - - word1");
            Assert.AreEqual("word1 ", parser.Parse());
        }

        [Test]
        public void AnotherManyNOTParseTest()
        {
            parser = new Parser("- - - - - - - word1");
            Assert.AreEqual("word1 - ", parser.Parse());
        }

        [Test]
        public void SimpleParenthesisParseTest()
        {
            parser = new Parser("(word1 | word2) & word3");
            Assert.AreEqual("word1 word2 | word3 & ", parser.Parse());
        }

        [Test]
        public void MultipleParenthesisParseTest()
        {
            parser = new Parser("- ((- word1 | word2) & word3) | word4");
            Assert.AreEqual("word1 - word2 | word3 & - word4 | ", parser.Parse());
        }

        [Test]
        public void ManyParenthesisParseTest()
        {
            parser = new Parser("(((((((word1 | word2) & word3) | word4) & word5) | word6) & word7) | word8) & word9");
            Assert.AreEqual("word1 word2 | word3 & word4 | word5 & word6 | word7 & word8 | word9 & ", parser.Parse());
        }

        [Test]
        public void SomeParseTest()
        {
            parser = new Parser("- (word1 | word2) & word3");
            Assert.AreEqual("word1 word2 | - word3 & ", parser.Parse());
        }

        [Test]
        public void AnotherParseTest()
        {
            parser = new Parser("word1 | - - (word2 & word3 & - word4) & word5");
            Assert.AreEqual("word1 word2 word3 & word4 - & word5 & | ", parser.Parse());
        }

        [Test]
        public void HugeParseTest()
        {
            parser = new Parser("- (- word1 | - (word2 | word3 & - word4) | - word5 & word6) & (word7 | - word8 | - (word9 & word10))");
            Assert.AreEqual("word1 - word2 word3 word4 - & | - | word5 - word6 & | - word7 word8 - | word9 word10 & - | & ", parser.Parse());
        }

        [Test]
        public void InvalidInputParseTest()
        {
            parser = new Parser("- (word1 | word2 & word3");
            Assert.Throws<ArgumentException>(() => parser.Parse());
        }

        [Test]
        public void AnotherInvalidInputParseTest()
        {
            parser = new Parser("- word1 | word2 ) & word3");
            Assert.Throws<InvalidOperationException>(() => parser.Parse());
        }
    }
}