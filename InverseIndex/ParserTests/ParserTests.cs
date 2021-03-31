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
            parser = new Parser("word1 AND word2");
            Assert.AreEqual("word1 word2 &", parser.Parse());
        }

        [Test]
        public void SimpleORParseTest()
        {
            parser = new Parser("word1 OR word2");
            Assert.AreEqual("word1 word2 |", parser.Parse());
        }

        [Test]
        public void SimpleNOTParseTest()
        {
            parser = new Parser("NOT word1");
            Assert.AreEqual("word1 -", parser.Parse());
        }

        [Test]
        public void DoubleNOTParseTest()
        {
            parser = new Parser("NOT NOT word1");
            Assert.AreEqual("word1", parser.Parse());
        }

        [Test]
        public void ManyNOTParseTest()
        {
            parser = new Parser("NOT NOT NOT NOT NOT NOT word1");
            Assert.AreEqual("word1", parser.Parse());
        }

        [Test]
        public void AnotherManyNOTParseTest()
        {
            parser = new Parser("NOT NOT NOT NOT NOT word1");
            Assert.AreEqual("word1 -", parser.Parse());
        }

        [Test]
        public void SimpleParenthesisParseTest()
        {
            parser = new Parser("(word1 OR word2) AND word3");
            Assert.AreEqual("word1 word2 | word3 &", parser.Parse());
        }

        [Test]
        public void MultipleParenthesisParseTest()
        {
            parser = new Parser("NOT ((NOT word1 OR word2) AND word3) OR word4");
            Assert.AreEqual("word1 - word2 | word3 & - word4 |", parser.Parse());
        }

        [Test]
        public void ManyParenthesisParseTest()
        {
            parser = new Parser("(((((((word1 OR word2) AND word3) OR word4) AND word5) OR word6) AND word7) OR word8) AND word9");
            Assert.AreEqual("word1 word2 | word3 & word4 | word5 & word6 | word7 & word8 | word9 &", parser.Parse());
        }

        [Test]
        public void SomeParseTest()
        {
            parser = new Parser("NOT (word1 OR word2) AND word3");
            Assert.AreEqual("word1 word2 | - word3 &", parser.Parse());
        }

        [Test]
        public void AnotherParseTest()
        {
            parser = new Parser("word1 OR NOT NOT (word2 AND word3 AND NOT word4) AND word5");
            Assert.AreEqual("word1 word2 word3 & word4 - & word5 & |", parser.Parse());
        }

        [Test]
        public void HugeParseTest()
        {
            parser = new Parser("NOT (NOT word1 OR NOT (word2 OR word3 AND NOT word4) OR NOT word5 AND word6) AND (word7 OR NOT word8 OR NOT (word9 AND word10))");
            Assert.AreEqual("word1 - word2 word3 word4 - & | - | word5 - word6 & | - word7 word8 - | word9 word10 & - | &", parser.Parse());
        }

        [Test]
        public void InvalidInputParseTest()
        {
            parser = new Parser("NOT (word1 OR word2 AND word3");
            Assert.Throws<ArgumentException>(() => parser.Parse());
        }

        [Test]
        public void AnotherInvalidInputParseTest()
        {
            parser = new Parser("NOT word1 OR word2 ) AND word3");
            Assert.Throws<InvalidOperationException>(() => parser.Parse());
        }
    }
}