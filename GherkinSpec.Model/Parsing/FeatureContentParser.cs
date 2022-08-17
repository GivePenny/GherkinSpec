using System.Collections.Generic;

namespace GherkinSpec.Model.Parsing
{
    internal class FeatureContentParser
    {
        private bool isInRule;
        private IEnumerable<Tag> ruleTags;
        private string ruleTitle;
        private Background ruleBackground;
        private List<Scenario> activeScenariosList;
        private List<ScenarioOutline> activeScenarioOutlinesList;
        private readonly List<Rule> featureRules;
        private readonly LineReader reader;

        public FeatureContentParser(List<Scenario> featureScenariosList, List<ScenarioOutline> featureScenarioOutlinesList, List<Rule> featureRules, LineReader reader)
        {
            activeScenariosList = featureScenariosList;
            activeScenarioOutlinesList = featureScenarioOutlinesList;
            this.featureRules = featureRules;
            this.reader = reader;
        }

        public void ParseFeatureContent()
        {
            do
            {
                var tags = TagParser.ParseTagsIfPresent(reader);

                if (reader.IsScenarioStartLine)
                {
                    Add(ScenarioParser.ParseScenario(reader, tags));
                    continue;
                }

                if (reader.IsScenarioOutlineStartLine)
                {
                    Add(ScenarioParser.ParseScenarioOutline(reader, tags));
                    continue;
                }

                if (reader.IsRuleStartLine)
                {
                    ParseNewRule(tags);
                    continue;
                }

                throw new InvalidGherkinSyntaxException(
                    $"Expected a Scenario, a Scenario Outline or a Rule, found \"{reader.CurrentLineTrimmed}\"",
                    reader.CurrentLineNumber);

            } while (!reader.IsEndOfFile);

            AddRuleIfNeeded();
        }

        private void Add(Scenario scenario)
            => activeScenariosList.Add(scenario);

        private void Add(ScenarioOutline scenarioOutline)
            => activeScenarioOutlinesList.Add(scenarioOutline);

        private void ParseNewRule(IEnumerable<Tag> tags)
        {
            AddRuleIfNeeded();

            isInRule = true;
            activeScenariosList = new List<Scenario>();
            activeScenarioOutlinesList = new List<ScenarioOutline>();
            ruleTags = tags;

            ruleTitle = reader.CurrentLineRuleTitle;

            reader.ReadNextLine();
            ruleBackground = BackgroundParser.ParseBackgroundIfPresent(reader);
        }

        private void AddRuleIfNeeded()
        {
            if (!isInRule)
            {
                return;
            }

            featureRules.Add(
                new Rule(
                    ruleTitle,
                    ruleBackground,
                    activeScenariosList,
                    activeScenarioOutlinesList,
                    ruleTags));
        }
    }
}
