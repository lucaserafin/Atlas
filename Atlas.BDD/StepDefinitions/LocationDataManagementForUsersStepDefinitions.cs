using System;
using SpecFlow;

namespace Atlas.BDD.StepDefinitions
{
    [Binding]
    public class LocationDataManagementForUsersStepDefinitions
    {
        [Given("a user with user_id {string} exists")]
        public void GivenAUserWithUser_IdExists(string p0)
        {
            throw new PendingStepException();
        }

        [When("I send a POST request to {string} with:")]
        public void WhenISendAPOSTRequestToWith(string p0, Table table)
        {
            throw new PendingStepException();
        }

        [Then("associate it with the user_id {string}")]
        public void ThenAssociateItWithTheUser_Id(string p0)
        {
            throw new PendingStepException();
        }

        [Given("a user with user_id {string} doesn't exists")]
        public void GivenAUserWithUser_IdDoesntExists(string p0)
        {
            throw new PendingStepException();
        }

        [Then("the system should respond with a {int} status code")]
        public void ThenTheSystemShouldRespondWithAStatusCode(int p0)
        {
            throw new PendingStepException();
        }

        [Then("an error message {string}")]
        public void ThenAnErrorMessage(string p0)
        {
            throw new PendingStepException();
        }

        [When("I send a GET request to {string}")]
        public void WhenISendAGETRequestTo(string p0)
        {
            throw new PendingStepException();
        }
    }
}
