using System;
using System.IO;
using Edo.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edo
{
    [TestClass]
    public class SerizTest
    {
        public SerizTest()
        {
            Stream = new MemoryStream(100);
            Reader = new BinaryReader(Stream);
            Writer = new BinaryWriter(Stream);
        }
        [TestInitialize]
        public void Init()
        {
            Buffer = new byte[100];
            Stream.SetLength(0);
        }

        [TestMethod]
        public void TestSerialize()
        {
            Int32 number = 12314;
            Stream.SetLength(4);
            Seriz.Serialize(Stream.GetBuffer(), number);

            Assert.AreEqual(number, Reader.ReadInt32());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSerializeThrowsOnNullValue()
        {
            object value = null;
            Seriz.Serialize(Buffer, value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSerializeThrowsOnNullBuffer()
        {
            Seriz.Serialize(null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSerializeThrowsIfBufferTooSmall()
        {
            Seriz.Serialize(new byte[3], 0);
        }

        [TestMethod]
        public void TestSerializeArray()
        {
            Int32[] numbers = new Int32[3];
            numbers[0] = 1231230;
            numbers[1] = 12367777;
            numbers[2] = 9873748;

            Stream.SetLength(12);
            Seriz.Serialize(Stream.GetBuffer(), numbers);

            Assert.AreEqual(numbers[0], Reader.ReadInt32());
            Assert.AreEqual(numbers[1], Reader.ReadInt32());
            Assert.AreEqual(numbers[2], Reader.ReadInt32());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSerializeArrayThrowsOnNullValues()
        {
            byte[] values = null;
            Seriz.Serialize(values, Buffer);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSerializeArrayThrowsOnNullBuffer()
        {
            Seriz.Serialize(null, new byte[4]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSerializeArrayThrowsOnZeroLengthBuffer()
        {
            Seriz.Serialize(Buffer, new Int32[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSerializeArrayThrowsIfBufferTooSmall()
        {
            Seriz.Serialize(new byte[17], new byte[18]);
        }

        [TestMethod]
        public void TestParse()
        {
            Int32 number = 1321234;
            Writer.Write(number);

            Assert.AreEqual(number, Seriz.Parse<Int32>(Stream.GetBuffer()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestParseThrowsOnNullBuffer()
        {
            Seriz.Parse<Int32>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParseThrowsIfBufferIsTooSmall()
        {
            Seriz.Parse<Int32>(new byte[3]);
        }

        [TestMethod]
        public void TestParseArray()
        {
            Int32 number1 = 12399493;
            Int32 number2 = 12385848;
            Int32 number3 = 98632423;
            Writer.Write(number1);
            Writer.Write(number2);
            Writer.Write(number3);

            Int32[] results = Seriz.Parse<Int32>(Stream.GetBuffer(), 3);
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual(number1, results[0]);
            Assert.AreEqual(number2, results[1]);
            Assert.AreEqual(number3, results[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestParseArrayThrowsOnNullBuffer()
        {
            Seriz.Parse<Int32>(null, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParseArrayThrowsOnZeroCount()
        {
            Seriz.Parse<Int32>(Buffer, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParseArrayThrowsIfBufferIsTooSmall()
        {
            Seriz.Parse<Int32>(new byte[10], 3);
        }

        [TestMethod]
        public void TestSerializeParseWithStructure()
        {
            MockVector vector = new MockVector(12304.123f, 12598.443f, 58387.183f);
            Seriz.Serialize(Buffer, vector);
            MockVector result = Seriz.Parse<MockVector>(Buffer);

            Assert.AreEqual(vector.X, result.X);
            Assert.AreEqual(vector.Y, result.Y);
            Assert.AreEqual(vector.Z, result.Z);
        }

        [TestMethod]
        public void TestSerializeParseWithFormattedClass()
        {
            MockClass test = new MockClass(192399994, 1231234.1230230f, 86783.2774f, 488868484, 858828348844);
            Seriz.Serialize(Buffer, test);
            MockClass result = Seriz.Parse<MockClass>(Buffer);

            Assert.AreEqual(test.value1, result.value1);
            Assert.AreEqual(test.value2, result.value2);
            Assert.AreEqual(test.value3, result.value3);
            Assert.AreEqual(test.value4, result.value4);
            Assert.AreEqual(test.value5, result.value5);
        }

        [TestMethod]
        public void TestSerializeParseArrayWithStructure()
        {
            MockVector[] values = new MockVector[3];
            values[0] = new MockVector(12304.123f, 12598.443f, 58387.183f);
            values[1] = new MockVector(1230.123f, 1298.443f, 587.183f);
            values[2] = new MockVector(12323204.123f, 1259548.4143f, 583287.1f);

            Seriz.Serialize(Buffer, values);
            MockVector[] results = Seriz.Parse<MockVector>(Buffer, values.Length);

            Assert.AreEqual(values.Length, results.Length);
            for (int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(values[i].X, results[i].X);
                Assert.AreEqual(values[i].Y, results[i].Y);
                Assert.AreEqual(values[i].Z, results[i].Z);
            }
        }

        [TestMethod]
        public void TestSerializeParseArrayWithFormattedClass()
        {
            MockClass[] values = new MockClass[3];
            values[0] = new MockClass(192399994, 1231234.1230230f, 86783.2774f, 488868484, 858828348844);
            values[1] = new MockClass(19232299994, 14231234.123022230f, 867823.27374f, 4888484, 858828844);
            values[2] = new MockClass(1923269994, 1434531234.123042230f, 83467823.27374f, 466888484, 8588283453844);

            Seriz.Serialize(Buffer, values);
            MockClass[] results = Seriz.Parse<MockClass>(Buffer, values.Length);

            Assert.AreEqual(values.Length, results.Length);
            for (int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(values[i].value1, results[i].value1);
                Assert.AreEqual(values[i].value2, results[i].value2);
                Assert.AreEqual(values[i].value3, results[i].value3);
                Assert.AreEqual(values[i].value4, results[i].value4);
                Assert.AreEqual(values[i].value5, results[i].value5);
            }
        }

        public byte[] Buffer { get; set; }
        public MemoryStream Stream { get; set; }
        public BinaryReader Reader { get; set; }
        public BinaryWriter Writer { get; set; }
    }
}