using GherkinSpec.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.TestAdapter.Execution
{
    public class StepsIterator
    {
        private readonly List<IStep> steps;
        private int currentStepIndex;

        public StepsIterator(
            IEnumerable<IStep> steps)
        {
            this.steps = steps.ToList();
        }

        public IEnumerable<IStep> Iterate()
        {
            for (currentStepIndex = 0; currentStepIndex < steps.Count; currentStepIndex++)
            {
                yield return steps.ElementAt(currentStepIndex);
            }
        }

        public void GoToMostRecentWhenStep()
        {
            var previousSteps = steps.Take(currentStepIndex + 1);
            var mostRecentWhenStep = previousSteps.LastOrDefault(s => s is WhenStep);

            if (mostRecentWhenStep == null)
            {
                throw new Exception();
            }

            currentStepIndex = steps.IndexOf(mostRecentWhenStep) - 1;
        }
    }
}
