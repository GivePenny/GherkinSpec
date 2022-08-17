using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GherkinSpec.TestAdapter.UnitTests
{
    [TestClass]
    public class CommonPrefixStripperShould
    {
        [TestMethod]
        public void StripPrefixesCommonToAllCases()
        {
            var cases = new List<TestCase>
            {
                new() { FullyQualifiedName = "A1.B1" },
                new() { FullyQualifiedName = "A1.B1.C1" },
                new() { FullyQualifiedName = "A1.B1.C2" },
                new() { FullyQualifiedName = "A1.B2" },
            };

            CommonPrefixStripper.StripNamePrefixesSharedByAllTestCases(cases);

            Assert.AreEqual("B1", cases[0].FullyQualifiedName);
            Assert.AreEqual("B1.C1", cases[1].FullyQualifiedName);
            Assert.AreEqual("B1.C2", cases[2].FullyQualifiedName);
            Assert.AreEqual("B2", cases[3].FullyQualifiedName);
        }

        [TestMethod]
        public void LeavePrefixComponentsIfTheyAreAlsoTestNamesThemselves()
        {
            var cases = new List<TestCase>
            {
                new() { FullyQualifiedName = "A1.B1" },
                new() { FullyQualifiedName = "A1.B1.C1" },
                new() { FullyQualifiedName = "A1.B1.C2" }
            };

            CommonPrefixStripper.StripNamePrefixesSharedByAllTestCases(cases);

            Assert.AreEqual("B1", cases[0].FullyQualifiedName);
            Assert.AreEqual("B1.C1", cases[1].FullyQualifiedName);
            Assert.AreEqual("B1.C2", cases[2].FullyQualifiedName);
        }

        [TestMethod]
        public void StripMultipleLevelsIfNeeded()
        {
            var cases = new List<TestCase>
            {
                new() { FullyQualifiedName = "A1.B1.C1" },
                new() { FullyQualifiedName = "A1.B1.C2" }
            };

            CommonPrefixStripper.StripNamePrefixesSharedByAllTestCases(cases);

            Assert.AreEqual("C1", cases[0].FullyQualifiedName);
            Assert.AreEqual("C2", cases[1].FullyQualifiedName);
        }
    }
}
