﻿using System.IO;

namespace GherkinSpec.Model.Parsing
{
    class LineReader
    {
        private const string FeatureLineStart = "Feature:";
        private const string BackgroundLineStart = "Background:";
        private const string ScenarioLineStart = "Scenario:";
        private const string ScenarioOutlineLineStart = "Scenario Outline:";
        private const string ExampleLineStart = "Example:";
        private const string ExamplesLine = "Examples:";
        public const string GivenLineStart = "Given ";
        public const string WhenLineStart = "When ";
        public const string ThenLineStart = "Then ";
        private const string AndLineStart = "And ";
        private const string ButLineStart = "But ";
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
            => CurrentLineStartsWith(FeatureLineStart);

        public string CurrentLineFeatureTitle
            => CurrentLineTrimmed.Substring(FeatureLineStart.Length).Trim();

        public bool IsBackgroundStartLine
            => CurrentLineStartsWith(BackgroundLineStart);

        public bool IsTagLine
            => CurrentLineStartsWith(TagLineStart);

        public bool IsScenarioStartLine
            => CurrentLineStartsWith(ScenarioLineStart)
                || CurrentLineStartsWith(ExampleLineStart);

        public string CurrentLineScenarioTitle
            => CurrentLineStartsWith(ScenarioLineStart)
                ? CurrentLineTrimmed.Substring(ScenarioLineStart.Length).Trim()
                : CurrentLineTrimmed.Substring(ExampleLineStart.Length).Trim();

        public bool IsScenarioOutlineStartLine
            => CurrentLineStartsWith(ScenarioOutlineLineStart);

        public string CurrentLineScenarioOutlineTitle
            => CurrentLineTrimmed.Substring(ScenarioOutlineLineStart.Length).Trim();

        public bool IsExamplesStartLine
            => CurrentLineTrimmed == ExamplesLine;

        public bool IsTableLine
            => CurrentLineStartsWith("|");

        public bool IsMultiLineStringStartOrEndLine
            => CurrentLineStartsWith(MultiLineStringLineStart);

        public bool IsGivenLine
            => CurrentLineStartsWith(GivenLineStart);

        public bool IsWhenLine
            => CurrentLineStartsWith(WhenLineStart);

        public bool IsThenLine
            => CurrentLineStartsWith(ThenLineStart);

        public bool IsAndLine
            => CurrentLineStartsWith(AndLineStart);

        public bool IsButLine
            => CurrentLineStartsWith(ButLineStart);

        public bool IsStepLine
            => IsGivenLine || IsWhenLine || IsThenLine || IsAndLine || IsButLine;

        public bool IsEndOfFile
            => CurrentLineTrimmed == null;

        private bool CurrentLineStartsWith(string prefix)
            => (CurrentLineTrimmed?.StartsWith(prefix)).GetValueOrDefault(false);
    }
}