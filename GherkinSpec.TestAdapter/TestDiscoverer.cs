using GherkinSpec.Model;
using GherkinSpec.Model.Parsing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GherkinSpec.TestAdapter
{
    [DefaultExecutorUri(TestExecutor.ExecutorUri)]
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    public class TestDiscoverer : ITestDiscoverer
    {
        private readonly HashSet<string> discoveredFullyQualifiedTestNames = new HashSet<string>();
        private readonly Regex uniqueTestCaseSuffixRegex = new Regex(" \\((\\d+)\\)$");

        public void DiscoverTests(
            IEnumerable<string> sources,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            foreach (var testCase in DiscoverTests(sources, logger))
            {
                discoverySink.SendTestCase(testCase);
            }
        }

        public IEnumerable<TestCase> DiscoverTests(IEnumerable<string> sources, IMessageLogger logger)
        {
            foreach (var source in sources)
            {
                foreach (var testCase in DiscoverTests(source, logger))
                {
                    EnsureTestCaseNameIsUnique(testCase);
                    yield return testCase;
                }
            }
        }

        private static IEnumerable<TestCase> DiscoverTests(string source, IMessageLogger logger)
        {
            var sourceAssemblyPath = Path.IsPathRooted(source) ? source : Path.Combine(Directory.GetCurrentDirectory(), source);

            try
            {
                var assembly = Assembly.LoadFrom(sourceAssemblyPath);

                // ToArray forces the enumeration to be evaluated so any exceptions are caught and logged fully here.
                return DiscoverTests(source, assembly).ToArray();
            }
            catch (BadImageFormatException exception)
            {
                logger.SendMessage(TestMessageLevel.Warning, $"Skipping assembly because of an unsupported image format: {exception}");
            }
            catch (FileNotFoundException exception)
            {
                logger.SendMessage(TestMessageLevel.Warning, $"Skipping assembly because a dependent file ({exception.FileName}) could not be found.");
            }
            catch (FileLoadException exception)
            {
                logger.SendMessage(TestMessageLevel.Warning, $"Skipping assembly because a dependent assembly ({exception.FileName}) could not be loaded.");
            }
            catch (Exception exception)
            {
                logger.SendMessage(TestMessageLevel.Warning, $"Skipping assembly because an exception was thrown: {exception}");
            }
            return Enumerable.Empty<TestCase>();
        }

        private static IEnumerable<TestCase> DiscoverTests(string source, Assembly assembly)
        {
            var gherkinParser = new Parser();
            var featureFileLocator = new SourceFileLocator(source);

            var results = new List<TestCase>();

            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (!IsFeatureResourceName(resourceName))
                {
                    continue;
                }

                var featureText = ReadResourceText(assembly, resourceName);
                var feature = gherkinParser.Parse(featureText);
                var featureSourceFile = featureFileLocator.FindFeatureFileNameIfPossible(resourceName);

                var cleanedFeatureFolderAndName = CleanedDotSeparatedName(
                    featureSourceFile,
                    feature.Title);

                foreach (var scenario in feature.AllScenarios)
                {
                    var cleanedScenarioName = CleanDisallowedCharacters(scenario.Title);
                    var testCase = new TestCase(cleanedFeatureFolderAndName + "." + cleanedScenarioName, TestExecutor.ExecutorUriStronglyTyped, source)
                    {
                        DisplayName = scenario.Title,
                        LocalExtensionData = new DiscoveredTestData(assembly, feature, null, scenario),
                        CodeFilePath = featureSourceFile.SourceFileName,
                        LineNumber = scenario.StartingLineNumber
                    };

                    AddCategoryTraits(feature, rule: null, scenario, testCase);

                    results.Add(testCase);
                }

                foreach (var rule in feature.Rules)
                {
                    var cleanedRuleFolderAndName = cleanedFeatureFolderAndName + "." + CleanDisallowedCharacters(rule.Title);

                    foreach (var scenario in rule.AllScenarios)
                    {
                        var cleanedScenarioName = CleanDisallowedCharacters(scenario.Title);
                        var testCase = new TestCase(cleanedRuleFolderAndName + "." + cleanedScenarioName, TestExecutor.ExecutorUriStronglyTyped, source)
                        {
                            DisplayName = scenario.Title,
                            LocalExtensionData = new DiscoveredTestData(assembly, feature, rule, scenario),
                            CodeFilePath = featureSourceFile.SourceFileName,
                            LineNumber = scenario.StartingLineNumber
                        };

                        AddCategoryTraits(feature, rule, scenario, testCase);

                        results.Add(testCase);
                    }
                }
            }

            CommonPrefixStripper.StripNamePrefixesSharedByAllTestCases(results);

            return results;
        }

        private static void AddCategoryTraits(Feature feature, Rule rule, Scenario scenario, TestCase testCase)
        {
            var categories = feature.Tags.CategoryNames
                .Concat(scenario.Tags.CategoryNames);

            if (rule != null)
            {
                categories = categories.Concat(rule.Tags.CategoryNames);
            }

            testCase.Traits.AddRange(
                categories.Distinct().Select(
                    category => new Trait("Category", category)));
        }

        private static string CleanDisallowedCharacters(string text)
            => text?.Replace('.', '-');

        private static string CleanedDotSeparatedName(TestSourceFile featureSourceFile, string featureTitle)
            => string.Join(
                ".",
                featureSourceFile
                    .RelevantFolderNames
                    .Append(featureTitle)
                    .Select(CleanDisallowedCharacters)
                    .ToArray());

        private static string ReadResourceText(Assembly assembly, string resourceName)
        {
            using (var reader = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
            {
                return reader.ReadToEnd();
            }
        }

        private static bool IsFeatureResourceName(string resourceName)
            => resourceName.EndsWith(".feature")
                || resourceName.EndsWith(".gherkin");

        private void EnsureTestCaseNameIsUnique(TestCase testCase)
        {
            if (!discoveredFullyQualifiedTestNames.Contains(testCase.FullyQualifiedName))
            {
                discoveredFullyQualifiedTestNames.Add(testCase.FullyQualifiedName);
                return;
            }

            var distinctTestCaseSuffixMatch = uniqueTestCaseSuffixRegex.Match(testCase.FullyQualifiedName);
            if (!distinctTestCaseSuffixMatch.Success)
            {
                testCase.FullyQualifiedName += " (2)";
                testCase.DisplayName += " (2)";
            }
            else
            {
                var nextDistinctTestCaseCounterValue = int.Parse(distinctTestCaseSuffixMatch.Groups[1].Value) + 1;

                testCase.FullyQualifiedName = uniqueTestCaseSuffixRegex.Replace(
                    testCase.FullyQualifiedName,
                    $" ({nextDistinctTestCaseCounterValue})");

                testCase.DisplayName = uniqueTestCaseSuffixRegex.Replace(
                    testCase.DisplayName,
                    $" ({nextDistinctTestCaseCounterValue})");
            }

            EnsureTestCaseNameIsUnique(testCase);
            return;
        }
    }
}
