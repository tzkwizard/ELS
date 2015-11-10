using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace MessageHandleApi.Test
{
    [Binding]
    public class LmsSteps
    {
        private int result { get; set; }
        private Calculator calculator = new Calculator();

        [Given(@"I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredIntoTheCalculator(int number)
        {
            calculator.FirstNumber = number;
        }
        
        [Given(@"I also have entered (.*) into the calculator")]
        public void GivenIAlsoHaveEnteredIntoTheCalculator(int number)
        {
            calculator.SecondNumber = number;
        }
        
        [When(@"I press add")]
        public void WhenIPressAdd()
        {
            result = calculator.Add();
        }
        
        [Then(@"the result should be (.*) on the screen")]
        public void ThenTheResultShouldBeOnTheScreen(int expectedResult)
        {
            Assert.AreEqual(expectedResult, result);
        }
    }
}
