using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace MessageHandleApi.Test
{
    [Binding]
    public class CalSteps
    {
        private int result { get; set; }
        private Calculator calculator = new Calculator();

        [Given(@"I have enter (.*) into the calculator")]
        public void GivenIHaveEnterIntoTheCalculator(int p0)
        {
            calculator.FirstNumber = p0;
        }
        
        [Given(@"I also have enter (.*) into the calculator")]
        public void GivenIAlsoHaveEnterIntoTheCalculator(int p0)
        {
            calculator.SecondNumber = p0;
        }
        
        [When(@"I press minus")]
        public void WhenIPressMinus()
        {
            result = calculator.Minus();
        }
        
        [Then(@"the result should be (.*)")]
        public void ThenTheResultShouldBe(int expectedResult)
        {
            Assert.AreEqual(expectedResult, result);
        }
    }
}
