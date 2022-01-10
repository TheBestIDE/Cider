using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math.Tests
{
    [TestClass()]
    public class VandermondeEnumeratorTests
    {
        [TestMethod()]
        public void VandermondeEnumeratorTest()
        {
            VandermondeEnumerator ve = new(2, 3, 8);
            Assert.IsNotNull(ve);
            Assert.AreEqual(ve.RowNumber, 0);
            Assert.AreEqual(ve.ColumnNumber, -1);
            ve.MoveNext();
            Assert.AreEqual(ve.RowNumber, 0);
            Assert.AreEqual(ve.ColumnNumber, 0);
        }

        [TestMethod()]
        public void ResetTest()
        {
            VandermondeEnumerator ve = new (2, 3, 8);
            ve.MoveNext();
            ve.MoveNext();
            var ori_current = ve.Current;
            ve.MoveNext();
            ve.MoveNext();
            ve.MoveNext();
            ve.MoveNext();
            ve.Reset();
            Assert.IsTrue(ve.MoveNext());
            Assert.IsTrue(ve.MoveNext());
            Assert.AreEqual((ulong)ve.Current, (ulong)ori_current);
        }

        [TestMethod()]
        public void MoveNextTest()
        {
            VandermondeEnumerator ve = new (2, 3, 8);
            ve.MoveNext();
            ve.MoveNext();
            ve.MoveNext();
            ve.MoveNext();
            ve.MoveNext();
            ve.MoveNext();
            Assert.IsFalse(ve.MoveNext());
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            VandermondeEnumerator ve = new(2, 3, 8);
            var e = ve.GetEnumerator();
            ve.MoveNext();
            Assert.IsNotNull(e);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual((ulong)ve.Current, (ulong)ve.Current);
        }
    }
}