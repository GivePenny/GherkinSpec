using GherkinSpec.Model;
using GherkinSpec.TestAdapter.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.TestAdapter.UnitTests.Execution
{
    [TestClass]
    public class StepsIteratorShould
    {
        [TestMethod]
        public void YieldTheStepsItIsGivenInOrder()
        {
            var steps = new List<IStep>
            {
                new GivenStep("Given something", null, null),
                new GivenStep("Given something else", null, null),
                new WhenStep("When something", null, null),
                new ThenStep("Then something", null, null),
                new ThenStep("Then something else", null, null)
            };

            var stepsIterator = new StepsIterator(steps);

            var index = 0;
            foreach (var step in stepsIterator.Iterate())
            {
                Assert
                    .AreEqual(
                        steps.ElementAt(index),
                        step);

                index++;
            }
        }

        [TestMethod]
        public void ThrowWhenGoToMostRecentWhenStepIsCalledDuringIterationAndThereIsNoPriorWhenStep()
        {
            var steps = new List<IStep>
            {
                new GivenStep("Given something", null, null),
                new WhenStep("When something", null, null),
                new ThenStep("Then something", null, null)
            };

            var stepsIterator = new StepsIterator(steps);

            foreach (var step in stepsIterator.Iterate())
            {
                if (step is GivenStep)
                {
                    Assert.ThrowsException<Exception>(() => stepsIterator.GoToMostRecentWhenStep());
                    break;
                }
            }
        }

        [TestMethod]
        public void YieldTheWhenStepWhenGoToMostRecentWhenStepIsCalledDuringIteration()
        {
            var steps = new List<IStep>
            {
                new GivenStep("Given something", null, null),
                new WhenStep("When something", null, null),
                new ThenStep("Then something", null, null)
            };

            var stepsIterator = new StepsIterator(steps);

            var readyToAssert = false;
            foreach (var step in stepsIterator.Iterate())
            {
                if (readyToAssert)
                {
                    Assert.IsInstanceOfType(step, typeof(WhenStep));
                    break;
                }

                if (step is ThenStep)
                {
                    stepsIterator.GoToMostRecentWhenStep();
                    readyToAssert = true;
                }
            }
        }

        [TestMethod]
        public void YieldTheMostRecentWhenStepWhenGoToMostRecentWhenStepIsCalledDuringIteration()
        {
            var mostRecentWhenStep = new WhenStep("When something else", null, null);
            var steps = new List<IStep>
            {
                new GivenStep("Given something", null, null),
                new WhenStep("When something", null, null),
                mostRecentWhenStep,
                new ThenStep("Then something", null, null)
            };

            var stepsIterator = new StepsIterator(steps);

            var readyToAssert = false;
            foreach (var step in stepsIterator.Iterate())
            {
                if (readyToAssert)
                {
                    Assert.AreEqual(mostRecentWhenStep, step);
                    break;
                }

                if (step is ThenStep)
                {
                    stepsIterator.GoToMostRecentWhenStep();
                    readyToAssert = true;
                }
            }
        }

        [TestMethod]
        public void YieldTheMostRecentWhenStepBeforeTheCurrentStepWhenGoToMostRecentWhenStepIsCalledDuringIteration()
        {
            var mostRecentWhenStep = new WhenStep("When something else", null, null);
            var steps = new List<IStep>
            {
                new GivenStep("Given something", null, null),
                mostRecentWhenStep,
                new ThenStep("Then something before doing something else", null, null),
                new WhenStep("When something", null, null),
                new ThenStep("Then something", null, null)
            };

            var stepsIterator = new StepsIterator(steps);

            var readyToAssert = false;
            foreach (var step in stepsIterator.Iterate())
            {
                if (readyToAssert)
                {
                    Assert.AreEqual(mostRecentWhenStep, step);
                    break;
                }

                if (step is ThenStep)
                {
                    stepsIterator.GoToMostRecentWhenStep();
                    readyToAssert = true;
                }
            }
        }
    }
}
