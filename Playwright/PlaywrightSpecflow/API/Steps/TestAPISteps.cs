using NUnit.Framework;
using PlaywrightUtils.API.Actions;
using TechTalk.SpecFlow;

namespace PlaywrightSpecflow.API.Steps
{
    [Binding, Scope(Tag = "API")]
    internal class TestAPISteps
    {
        private readonly RestClientManager _restActions;

        public TestAPISteps(RestClientManager restActions)
        {
            _restActions = restActions;
        }

        [When(@"I send GET request to endpoint ""([^""]*)""")]
        public async Task ISendGETRequestToEndpoint(string endpoint)
        {
            await _restActions.ExecuteGETRequestAsync(endpoint);
        }

        [Then(@"I receive responce with status code ""([^""]*)""")]
        public void ThenIReceiveResponceWithStatusCode(int stausCode)
        {
            var response = _restActions.Response;
            Assert.That(_restActions.Response.Status, Is.EqualTo(stausCode));
        }
    }
}
