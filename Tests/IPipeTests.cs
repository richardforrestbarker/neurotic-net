using System;
using System.Collections.Generic;
using System.Linq;
using Neurotic;
using Neurotic.Factory;
using NUnit.Framework;

namespace Tests
{
    public class IPipeTests
    {
        public void Setup()
        {
            
        }
        [Test]
        public void IPipes_InMemoryMustEqalWhenReferencingSamePointer()
        {
            IPipe p = new IPipe();
            IPipe q = p;
            IPipe z = new IPipe();

            Assert.True(q == p);
            Assert.False(q == z);
            Assert.True(ReferenceEquals(q, p));
            Assert.False(ReferenceEquals(q, z));
        }
        [Test]
        public void IPipes_InMemoryMustHaveSameHashCodes()
        {
            IPipe p = new IPipe();
            IPipe q = p;
            IPipe z = new IPipe();

            Assert.True(q.GetHashCode().Equals(q.GetHashCode()));
            Assert.False(q.GetHashCode().Equals(z.GetHashCode()));
        }
    }
}