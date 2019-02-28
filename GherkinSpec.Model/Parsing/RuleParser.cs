using System.Collections.Generic;

namespace GherkinSpec.Model.Parsing
{
    static class RuleParser
    {
        public static Rule ParseRule(LineReader reader, IEnumerable<Tag> ruleTags)
        {
            var ruleTitle = reader.CurrentLineRuleTitle;

            reader.ReadNextLine();

            var ruleBackground = BackgroundParser.ParseBackgroundIfPresent(reader);

            var scenarios = new List<Scenario>();
            var scenarioOutlines = new List<ScenarioOutline>();

            do
            {
                var tags = TagParser.ParseTagsIfPresent(reader);

                if (reader.IsScenarioStartLine)
                {
                    scenarios.Add(
                        ScenarioParser.ParseScenario(reader, tags));
                    continue;
                }

                if (reader.IsScenarioOutlineStartLine)
                {
                    scenarioOutlines.Add(
                        ScenarioParser.ParseScenarioOutline(reader, tags));
                    continue;
                }

                break;
            } while (!reader.IsEndOfFile);

            return new Rule(
                ruleTitle,
                ruleBackground,
                scenarios,
                scenarioOutlines,
                ruleTags);
        }
    }
}
