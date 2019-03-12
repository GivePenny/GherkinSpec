using GherkinSpec.Model.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GherkinSpec.Model.UnitTests
{
    [TestClass]
    public class EventuallyConsistentTagParserShould
    {
        [TestMethod]
        public void ReturnFalseWhenTryParseIsCalledWithADifferentTagLabel()
        {
            Assert
                .IsFalse(
                    EventuallyConsistentTagParser.TryParse("somethingElse", out var tag));

            Assert.IsNull(tag);
        }

        [DataTestMethod]
        [DataRow("eventuallyConsistent")]
        [DataRow("eventuallyconsistent")]
        [DataRow("EVENTUALLYCONSISTENT")]
        [DataRow("evEntuallyCONSISTENT")]
        public void ReturnTrueWhenTryParseIsCalledWithAnEventuallyConsistentTagLabel(string label)
        {
            Assert
                .IsTrue(
                    EventuallyConsistentTagParser.TryParse(label, out var tag));

            Assert.IsNotNull(tag);
        }

        [TestMethod]
        public void SetTheOutVariableWithTheExpectedEventuallyConsistentTagWhenNoSettingsAreProvidedInTheLabel()
        {
            EventuallyConsistentTagParser.TryParse("eventuallyConsistent", out var tag);

            Assert.AreEqual(
                TimeSpan.FromSeconds(20),
                tag.Within);

            Assert.AreEqual(
                TimeSpan.FromSeconds(5),
                tag.RetryInterval);
        }

        [TestMethod]
        public void SetTheOutVariableWithTheExpectedEventuallyConsistentTagWhenAWithinValueIsProvidedInTheLabel()
        {
            EventuallyConsistentTagParser.TryParse("eventuallyConsistent(within=00:00:30)", out var tag);

            Assert.AreEqual(
                TimeSpan.FromSeconds(30),
                tag.Within);

            Assert.AreEqual(
                TimeSpan.FromSeconds(5),
                tag.RetryInterval);
        }

        [TestMethod]
        public void SetTheOutVariableWithTheExpectedEventuallyConsistentTagWhenARetryIntervalValueIsProvidedInTheLabel()
        {
            EventuallyConsistentTagParser.TryParse("eventuallyConsistent(retryInterval=00:00:10)", out var tag);

            Assert.AreEqual(
                TimeSpan.FromSeconds(20),
                tag.Within);

            Assert.AreEqual(
                TimeSpan.FromSeconds(10),
                tag.RetryInterval);
        }

        [DataTestMethod]
        [DataRow("eventuallyConsistent(within=00:00:30;retryInterval=00:00:10)")]
        [DataRow("eventuallyConsistent(retryInterval=00:00:10;within=00:00:30)")]
        public void SetTheOutVariableWithTheExpectedEventuallyConsistentTagWhenABothWithinAndRetryIntervalValuesAreProvidedInTheLabel(string label)
        {
            EventuallyConsistentTagParser.TryParse(label, out var tag);

            Assert.AreEqual(
                TimeSpan.FromSeconds(30),
                tag.Within);

            Assert.AreEqual(
                TimeSpan.FromSeconds(10),
                tag.RetryInterval);
        }
    }
}