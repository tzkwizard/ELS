using System;
using Nest;
using TechTalk.SpecFlow;

namespace ElasticSearch
{
    [Binding]
    public class CreateIndexSteps
    {
        private string name;
        const string elUri = "http://localhost:9200/";
        [Given(@"I have enter an index name")]
public void GivenIHaveEnterAnIndexName()
        {
            name = "AzureEL";
        }

        [When(@"I press create")]
public void WhenIPressCreate()
{
            Program p=new Program();
           p.CreateIndex(name,elUri);
}

        [Then(@"new index is created")]
public void ThenNewIndexIsCreated()
{
    Console.WriteLine("success");
}
    }
}
