using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using HubSpot.Main;
using Moq;
using Xunit;

namespace Hubspot.Tests
{
    // We mock the HTTP class so we can return the input data. 
    // We can the check what was passed to our mock function for POSTing data 
    // Expected values are those from the hubsput input example 
    public class HubspotTestCase
    {
        private readonly Mock<IHubspotHttpHandler> httpMock = new Mock<IHubspotHttpHandler>(MockBehavior.Strict);

        public bool AreEqual(Conversation actual, Conversation expected)
        {
            return
                actual.userId == expected.userId && 
                actual.avatar == expected.avatar &&
                actual.firstName == expected.firstName &&
                actual.lastName == expected.lastName &&
                actual.totalMessages == expected.totalMessages &&
                actual.mostRecentMessage.content == expected.mostRecentMessage.content &&
                actual.mostRecentMessage.timestamp == expected.mostRecentMessage.timestamp &&
                actual.mostRecentMessage.userId == expected.mostRecentMessage.userId; 
        }
        
        [Fact]
        public void Test1()
        {
            // SET UP 
            var inbox = new Inbox(httpMock.Object); 
            
            var inputJson =
                "{\"messages\": [{\"content\": \"The quick brown fox jumps over the lazy dog\",\"fromUserId\": 50210,\"timestamp\": 1529338342000,\"toUserId\": 67452},{\"content\": \"Pangrams originate in the discotheque\",\"fromUserId\": 67452,\"timestamp\": 1529075415000,\"toUserId\": 50210},{\"content\": \"Have you planned your holidays this year yet?\",\"fromUserId\": 67452,\"timestamp\": 1529542953000,\"toUserId\": 50210},{\"content\": \"Strange noises have been heard on the moors\",\"fromUserId\": 78596,\"timestamp\": 1533112961000,\"toUserId\": 50210},{\"content\": \"You go straight ahead for two hundred yards and then take the first right turn\",\"fromUserId\": 50210,\"timestamp\": 1533197225000,\"toUserId\": 78596},{\"content\": \"It's a privilege and an honour to have known you\",\"fromUserId\": 78596,\"timestamp\": 1533118270000,\"toUserId\": 50210}],\"userId\": 50210,\"users\": [{\"avatar\": \"octocat.jpg\",\"firstName\": \"John\",\"lastName\": \"Doe\",\"id\": 67452},{\"avatar\": \"genie.png\",\"firstName\": \"Michael\",\"lastName\": \"Crowley\",\"id\": 78596}]}"; 
            
            var input = Serialization.Deserialise<Dataset>(inputJson);

            Assert.True(input.users.Length == 2);

            var expectedUser1 = input.users[1];
            var latestMessage1 = input.messages[4]; 
            var expectedConversation1 = new Conversation(expectedUser1, new MostRecentMessage(latestMessage1), 3); 
            
            var expectedUser2 = input.users[0];
            var latestMessage2 = input.messages[2]; 
            var expectedConversation2 = new Conversation(expectedUser2, new MostRecentMessage(latestMessage2), 3);
            
            httpMock
                .Setup(x => x.GetData(It.Is<string>(getEndpoint => getEndpoint == "/get")))
                .ReturnsAsync(input);

            httpMock
                .Setup(x => x.PostData(It.Is<string>(getEndpoint => getEndpoint == "/post"), It.Is<Conversations>(
                    inbox =>
                    inbox.conversations.Length == 2 && 
                    AreEqual(inbox.conversations[0], expectedConversation1) && 
                    AreEqual(inbox.conversations[1], expectedConversation2))))
                .ReturnsAsync(new HttpResponseMessage()); 
            
            var _httpMockResponse = inbox.GetConversations("/get", "/post").Result;

        }
    }
}