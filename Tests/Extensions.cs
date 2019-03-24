using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Neurotic;
using System.Linq;

namespace Tests
{
    class Extensions
    {
        int fillCount = 49;
        List<int> list;
        [SetUp]
        public void setitup()
        {
            var i = 0;
            list = new List<int>();
            list.Fill(() => i++, 50);
        }
        [Test]
        public void Fill()
        {
            Assert.That(list[0] == 0);
            Assert.That(list[49] == 49);
            Assert.That(list[25] == 25);
        }

        [Test]
        public void OffsetCenteredWrappedSubsetPutsOffsetInTheMiddle()
        {
            var count = 5;
            var wrapped = list.OffsetCenteredWrappedSubset(list.Count / 2, count);
            Assert.That(wrapped.Length == count);
            Assert.That(wrapped[0] == list[(list.Count / 2) - count / 2]); 
            Assert.That(wrapped[count / 2] == list[(list.Count / 2)]); 
            Assert.That(wrapped[count - 1] == list[(list.Count / 2) + count / 2]);
        }

        [Test]
        public void OffsetCenteredWrappedSubsetWrapsToTheEnd()
        {
            var count = 3;
            var wrapped = list.OffsetCenteredWrappedSubset(0, count);
            Assert.That(wrapped.Length == count);
            Assert.That(wrapped[0] == 49);
            Assert.That(wrapped[1] == 0);
            Assert.That(wrapped[2] == 1);
        }

        [Test]
        public void OffsetCenteredWrappedSubsetWrapsToTheStart()
        {
            var count = 3;
            var wrapped = list.OffsetCenteredWrappedSubset(list.IndexOf(list.Last()), count);
            Assert.That(wrapped.Length == count);
            Assert.That(wrapped[0] == 48);
            Assert.That(wrapped[1] == 49);
            Assert.That(wrapped[2] == 0);
        }

        [Test]
        public void OffsetCenteredWrappedSubsetWrapsWhenCountExceedsLength()
        {
            var count = 5;
            var wrappedStart = list.OffsetCenteredWrappedSubset(1, count);
            Assert.That(wrappedStart.Length == count);
            Assert.That(wrappedStart[0] == 49);
            Assert.That(wrappedStart[1] == 0);
            Assert.That(wrappedStart[2] == 1);
            Assert.That(wrappedStart[3] == 2);
            Assert.That(wrappedStart[4] == 3);


            var wrappedEnd = list.OffsetCenteredWrappedSubset(fillCount - 1, count);
            Assert.That(wrappedEnd.Length == count);
            Assert.That(wrappedEnd[0] == 47);
            Assert.That(wrappedEnd[1] == 48);
            Assert.That(wrappedEnd[2] == 49);
            Assert.That(wrappedEnd[3] == 0);
            Assert.That(wrappedEnd[4] == 1);
        }
        
            
        
    }
}
