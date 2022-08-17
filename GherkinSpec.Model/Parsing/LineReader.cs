using System;
using System.IO;

namespace GherkinSpec.Model.Parsing
{
    internal class LineReader
    {
        private string ExamplesLine => Resources.ExamplesKeyword + ":";

        private const string TagLineStart = "@";

        private const string MultiLineStringLineStart = "\"\"\"";

        private readonly TextReader textReader;

        public LineReader(TextReader textReader)
        {
            this.textReader = textReader;
        }

        public string CurrentLineTrimmed { get; private set; }

        public string CurrentLineUntrimmed { get; private set; }

        public int CurrentLineNumber { get; private set; }

        public bool ReadNextLine()
        {
            var textUntrimmed = textReader.ReadLine();
            var textTrimmed = textUntrimmed?.Trim();
            CurrentLineNumber++;

            while (textTrimmed != null
                && (textTrimmed.StartsWith("#") || textTrimmed == string.Empty))
            {
                textUntrimmed = textReader.ReadLine();
                textTrimmed = textUntrimmed?.Trim();
                CurrentLineNumber++;
            }

            CurrentLineTrimmed = textTrimmed;
            CurrentLineUntrimmed = textUntrimmed;

            return textTrimmed != null;
        }

        public bool ReadNextLineRaw()
        {
            var textUntrimmed = textReader.ReadLine();
            CurrentLineTrimmed = textUntrimmed?.Trim();
            CurrentLineUntrimmed = textUntrimmed;
            CurrentLineNumber++;

            return textUntrimmed != null;
        }

        public string CurrentLineIndentation
            => CurrentLineUntrimmed.Substring(
                0,
                CurrentLineUntrimmed.Length - CurrentLineUntrimmed.TrimStart().Length);

        public bool IsFeatureStartLine
            => CurrentLineStartsWithOneOf(Resources.FeaturePrefixes);

        public string CurrentLineFeatureTitle
            => CurrentLineTitle(Resources.FeaturePrefixes);

        public bool IsRuleStartLine
            => CurrentLineStartsWithOneOf(Resources.RulePrefixes);

        public string CurrentLineRuleTitle
            => CurrentLineTitle(Resources.RulePrefixes);

        public bool IsBackgroundStartLine
            => CurrentLineStartsWithOneOf(Resources.BackgroundPrefixes);

        public bool IsTagLine
            => CurrentLineStartsWith(TagLineStart);

        public bool IsScenarioStartLine
            => CurrentLineStartsWithOneOf(Resources.ScenarioPrefixes);

        public string CurrentLineScenarioTitle
            => CurrentLineTitle(Resources.ScenarioPrefixes);

        public bool IsScenarioOutlineStartLine
            => CurrentLineStartsWithOneOf(Resources.ScenarioOutlinePrefixes);

        public string CurrentLineScenarioOutlineTitle
            => CurrentLineTitle(Resources.ScenarioOutlinePrefixes);

        public bool IsExamplesStartLine
            => CurrentLineTrimmed == ExamplesLine;

        public bool IsTableLine
            => CurrentLineStartsWith("|");

        public bool IsMultiLineStringStartOrEndLine
            => CurrentLineStartsWith(MultiLineStringLineStart);

        public bool IsGivenLine
            => CurrentLineStartsWithOneOf(Resources.GivenPrefixes);

        public bool IsWhenLine
            => CurrentLineStartsWithOneOf(Resources.WhenPrefixes);

        public bool IsThenLine
            => CurrentLineStartsWithOneOf(Resources.ThenPrefixes);

        public bool IsAndLine
            => CurrentLineStartsWithOneOf(Resources.AndPrefixes);

        public bool IsButLine
            => CurrentLineStartsWithOneOf(Resources.ButPrefixes);

        public bool IsStepLine
            => IsGivenLine || IsWhenLine || IsThenLine || IsAndLine || IsButLine;

        public bool IsEndOfFile
            => CurrentLineTrimmed == null;

        private bool CurrentLineStartsWith(string prefix)
            => (CurrentLineTrimmed?.StartsWith(prefix)).GetValueOrDefault(false);

        private bool CurrentLineStartsWithOneOf(string[] possiblePrefixes)
            => Localisation.StartsWithLocalisedValue(CurrentLineTrimmed, possiblePrefixes, out _);

        private bool CurrentLineStartsWithOneOf(string[] possiblePrefixes, out string matchedPrefix)
            => Localisation.StartsWithLocalisedValue(CurrentLineTrimmed, possiblePrefixes, out matchedPrefix);

        private string CurrentLineTitle(string[] expectedPossiblePrefixes)
        {
            if (!CurrentLineStartsWithOneOf(expectedPossiblePrefixes, out var matchedPrefix))
            {
                throw new InvalidOperationException();
            }

            return CurrentLineTrimmed.Substring(matchedPrefix.Length).Trim();
        }
    }
}
