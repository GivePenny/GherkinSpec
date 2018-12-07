﻿using System.IO;

namespace GivePenny.GherkinSpec.Model.Parsing
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

        private readonly TextReader textReader;

        public LineReader(TextReader textReader)
        {
            this.textReader = textReader;
        }

        public string CurrentLine { get; private set;  }
        public int CurrentLineNumber { get; private set; }

        public bool ReadNextLine()
        {
            var text = textReader.ReadLine()?.Trim();
            CurrentLineNumber++;

            while (text != null
                && (text.StartsWith("#") || text == string.Empty))
            {
                text = textReader.ReadLine()?.Trim();
                CurrentLineNumber++;
            }

            CurrentLine = text;
            return text != null;
        }

        public bool IsFeatureStartLine
            => CurrentLineStartsWith(FeatureLineStart);

        public string CurrentLineFeatureTitle
            => CurrentLine.Substring(FeatureLineStart.Length).Trim();

        public bool IsBackgroundStartLine
            => CurrentLineStartsWith(BackgroundLineStart);

        public bool IsScenarioStartLine
            => CurrentLineStartsWith(ScenarioLineStart)
                || CurrentLineStartsWith(ExampleLineStart);

        public string CurrentLineScenarioTitle
            => CurrentLineStartsWith(ScenarioLineStart)
                ? CurrentLine.Substring(ScenarioLineStart.Length).Trim()
                : CurrentLine.Substring(ExampleLineStart.Length).Trim();

        public bool IsScenarioOutlineStartLine
            => CurrentLineStartsWith(ScenarioOutlineLineStart);

        public string CurrentLineScenarioOutlineTitle
            => CurrentLine.Substring(ScenarioOutlineLineStart.Length).Trim();

        public bool IsExamplesStartLine
            => CurrentLine == ExamplesLine;

        public bool IsTableLine
            => CurrentLine.StartsWith("|");

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
            => CurrentLine == null;

        private bool CurrentLineStartsWith(string prefix)
            => (CurrentLine?.StartsWith(prefix)).GetValueOrDefault(false);
    }
}
