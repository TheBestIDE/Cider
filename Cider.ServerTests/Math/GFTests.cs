﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cider.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.Math.Tests
{
    [TestClass()]
    public class GFTests
    {
        [TestMethod()]
        public void GFTest()
        {
            LongBytes num = 0xFF1F;
            GF gf = new GF(num);
            Assert.AreEqual(64, gf.BitLength);
        }

        [TestMethod()]
        public void PrimitiveDictTest()
        {
            LongBytes one = 1;
            LongBytes exp = one << 64 | 0b11011;
            Assert.AreEqual(exp, GF.PrimitiveDict[64]);
        }

        [TestMethod()]
        public void BytesToGFTest()
        {
            byte[] data = new byte[4];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }
            GF gf = data;
            LongBytes num = data;
            Assert.AreEqual("0x0000000003020100", num.ToString());
            Assert.AreEqual(num.ToString(), gf.ToString());
        }

        [TestMethod()]
        public void InverseTest1()
        {
            GF gf = new GF(8, 0x8C), exp = new GF(8, 0xF7);
            Assert.AreEqual(exp.ToString(), gf.Inverse().ToString());

            gf = new GF(8, 0xBE); exp = new GF(8, 0x86);
            Assert.AreEqual(exp.ToString(), gf.Inverse().ToString());

            gf = new GF(8, 0x01); exp = new GF(8, 0x01);
            Assert.AreEqual(exp.ToString(), gf.Inverse().ToString());

            gf = new GF(8, 0x2D); exp = new GF(8, 0x44);
            Assert.AreEqual(exp.ToString(), gf.Inverse().ToString());
        }

        [TestMethod()]
        public void InverseTest2()
        {
            GF gf = new GF(8, 0x8C), one = GF.One(8);
            Assert.AreEqual(one.ToString(), (gf * gf.Inverse()).ToString());

            gf = new GF(8, 0xBE);
            Assert.AreEqual(one.ToString(), (gf * gf.Inverse()).ToString());

            gf = new GF(8, 0x01);
            Assert.AreEqual(one.ToString(), (gf * gf.Inverse()).ToString());

            gf = new GF(8, 0x2D);
            Assert.AreEqual(one.ToString(), (gf * gf.Inverse()).ToString());
        }

        [TestMethod()]
        public void InverseTest64()
        {
            GF gf = new GF(64, 0xA9E8CUL), one = GF.One(64);
            GF ivs = gf.Inverse();
            Assert.AreEqual("0x" + ("0000000000000000cdefbda74d3efa83".ToUpper()), ivs.ToString());
            Assert.AreEqual(one, gf * gf.Inverse());
        }

        [TestMethod()]
        public void InverseTest512()
        {
            GF gf = new GF(512, 0x90877A9E8CUL), one = GF.One(512);
            Assert.AreEqual(one, gf * gf.Inverse());
        }

        [TestMethod()]
        public void InverseTest1024()
        {
            GF gf = new GF(1024, 0xB90B5C4D5EUL), one = GF.One(1024);
            Assert.AreEqual(one, gf * gf.Inverse());
        }

        [TestMethod()]
        public void InverseTest2048()
        {
            GF gf = new GF(2048, 0xB90B567EC4D5EUL), one = GF.One(2048);
            Assert.AreEqual(one, gf * gf.Inverse());
        }

        [TestMethod()]
        public void InverseTest4096()
        {
            GF gf = new GF(4096, 0xB90B59BAC4D5EUL), one = GF.One(4096);
            Assert.AreEqual(one, gf * gf.Inverse());
        }

        [TestMethod()]
        public void InverseTest8192()
        {
            GF gf = new GF(8192, 0xB90B59BAC4D5EUL), one = GF.One(8192);
            Assert.AreEqual(one, gf * gf.Inverse());
        }

        [TestMethod()]
        public void AddTest()
        {
            GF gf1 = 0x89;
            GF gf2 = 0x4D;
            GF expected = 0xC4;
            Assert.IsTrue(expected == gf1 + gf2);

            gf1 = 0xAF; gf2 = 0x3B; expected = 0x94;
            Assert.IsTrue(expected == gf1 + gf2);

            gf1 = 0x35; gf2 = 0xC6; expected = 0xF3;
            Assert.IsTrue(expected == gf1 + gf2);
        }

        [TestMethod()]
        public void MultifyTest()
        {
            GF gf1 = new (8, 0xCE), gf2 = new (8, 0xF1), expected = new (8, 0xEF);
            Assert.IsTrue(expected == gf1 * gf2);

            gf1 = new GF(8, 0x70); gf2 = new GF(8, 0x99); expected = new GF(8, 0xA2);
            Assert.IsTrue(expected == gf1 * gf2);

            gf1 = new GF(8, 0x00); gf2 = new GF(8, 0xA4); expected = new GF(8, 0x00);
            Assert.IsTrue(expected == gf1 * gf2);
        }

        [TestMethod()]
        public void MultifyTest1()
        {
            byte[] bytes = new byte[] { 0x74, 0x33, 0x32, 0x31 };
            GF gf1 = bytes, gf2 = new(32, 0x1), expected = new(32, 0x31323374);
            Assert.AreEqual(expected, GF.Multiply(gf1, gf2));

            gf1 = new GF(64, 0x36c07f6ace0); gf2 = new GF(64, 0x77A9E8UL); expected = new GF(64, 0x8987ff0a2b1a5300);
            Assert.AreEqual(expected, gf1 * gf2);
        }

        [TestMethod()]
        public void DivideTest()
        {
            GF gf = new GF(8, 0x8C), one = GF.One(8);
            Assert.AreEqual(one.ToString(), (gf / gf).ToString());

            gf = new GF(8, 0xBE);
            Assert.AreEqual(one.ToString(), GF.Divide(gf, gf).ToString());

            gf = new GF(8, 0x01);
            Assert.AreEqual(one.ToString(), (gf / gf).ToString());

            gf = new GF(8, 0x2D);
            Assert.AreEqual(one.ToString(), GF.Divide(gf, gf).ToString());
        }

        [TestMethod()]
        public void DivModTest()
        {
            GF gf1 = new(8, 0xDE), gf2 = new(8, 0xC6), expQtnt = new GF(8, 0x01), expRmd = new GF(8, 0x18);
            GF qtnt, rmd;
            GF.DivMod(gf1, gf2, out qtnt, out rmd);
            Assert.AreEqual(expQtnt.ToString(), qtnt.ToString());
            Assert.AreEqual(expRmd.ToString(), rmd.ToString());

            gf1 = new(8, 0x8C); gf2 = new GF(8, 0x0A); expQtnt = new GF(8, 0x14); expRmd = new GF(8, 0x04);
            GF.DivMod(gf1, gf2, out qtnt, out rmd);
            Assert.AreEqual(expQtnt.ToString(), qtnt.ToString());
            Assert.AreEqual(expRmd.ToString(), rmd.ToString());

            gf1 = new(8, 0x3E); gf2 = new GF(8, 0xA4); expQtnt = new GF(8, 0x00); expRmd = new GF(8, 0x3E);
            GF.DivMod(gf1, gf2, out qtnt, out rmd);
            Assert.AreEqual(expQtnt.ToString(), qtnt.ToString());
            Assert.AreEqual(expRmd.ToString(), rmd.ToString());
        }
    }
}