﻿using GherkinSpec.Model;
using System.Linq;

namespace GherkinSpec.TestAdapter.UnitTests.Mocks
{
    internal class MockScenario : Scenario
    {
        public MockScenario() : base(
            "Test scenario",
            Enumerable.Empty<IStep>(),
            0,
            Enumerable.Empty<Tag>())
        {
        }
    }
}
