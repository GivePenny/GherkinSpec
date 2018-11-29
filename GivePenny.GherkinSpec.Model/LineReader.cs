using System.IO;

namespace GivePenny.GherkinSpec.Model
{
    class LineReader
    {
        private const string FeatureLineStart = "Feature:";
        private const string ScenarioLineStart = "Scenario:";
        public const string GivenLineStart = "Given ";
        public const string WhenLineStart = "When ";
        public const string ThenLineStart = "Then ";
        private const string AndLineStart = "And ";

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

        public bool IsScenarioStartLine
            => CurrentLineStartsWith(ScenarioLineStart);

        public string CurrentLineScenarioTitle
            => CurrentLine.Substring(ScenarioLineStart.Length).Trim();

        public bool IsGivenLine
            => CurrentLineStartsWith(GivenLineStart);

        public bool IsWhenLine
            => CurrentLineStartsWith(WhenLineStart);

        public bool IsThenLine
            => CurrentLineStartsWith(ThenLineStart);

        public bool IsAndLine
            => CurrentLineStartsWith(AndLineStart);

        public bool IsStepLine
            => IsGivenLine || IsWhenLine || IsThenLine || IsAndLine;

        public bool IsEndOfFile
            => CurrentLine == null;

        private bool CurrentLineStartsWith(string prefix)
            => (CurrentLine?.StartsWith(prefix)).GetValueOrDefault(false);
    }
}
