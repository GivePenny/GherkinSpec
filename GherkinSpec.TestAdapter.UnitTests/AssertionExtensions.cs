﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace GherkinSpec.TestAdapter.UnitTests
{
    internal static class AssertionExtensions
    {
        private const string TimestampRegexPattern = @"\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d.?\d*Z?";

        public static void MessagesAreEqualIgnoringTimestamp(
            this Assert _,
            string expectedWithoutTimestamp,
            string actualWithTimestamp)
        {
            var actualWithoutTimestamp = Regex.Replace(actualWithTimestamp, TimestampRegexPattern, string.Empty);

            Assert.AreEqual(expectedWithoutTimestamp, actualWithoutTimestamp);
        }
    }
}
