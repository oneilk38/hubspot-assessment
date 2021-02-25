using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace HubSpot.Main
{
    public class Inbox
    {
        public IHubspotHttpHandler httpHandler { get; }

        public Inbox(IHubspotHttpHandler handler) => httpHandler = handler;

        public async Task<Dataset> GetData(string endpoint) => 
            await httpHandler.GetData(endpoint);
        

        // This function is used to always return the other user's id, so that we can group by this and associate all messages to/from that user to that user id
        public int GetOtherUserId(int userId, Message message) =>
            (message.fromUserId == userId) ? message.toUserId : message.fromUserId;

        // returns a dictionary where key=userId and value=all messages that user was involved with 
        public Dictionary<int, List<Message>> GetMessagesByOtherUserId(Message[] messages, int userId) => 
            messages
                .GroupBy(message => GetOtherUserId(userId, message))
                .ToDictionary(group => group.Key, group => group.ToList());
        
        public List<Message> OrderMessagesByTimestampDesc(IEnumerable<Message> messages) => 
            messages.OrderByDescending(x => x.timestamp)
                .ToList();

        public Conversation ToConversation(List<Message> messages, User otherUser)
        {
            var orderedConversationByTs = OrderMessagesByTimestampDesc(messages);

            var mostRecentMessage = new MostRecentMessage(orderedConversationByTs[0]);
            
            var conversation = new Conversation(otherUser, mostRecentMessage, orderedConversationByTs.Count);

            return conversation;
        }

        public Conversation[] ToInboxSortedByTimestampDesc(Dictionary<int, List<Message>> conversationByOtherUserId, Dictionary<int, User> userInfoByUserId)
        {
            // For each list of messages by user, find most recent and return a new conversation
            var conversations = 
                conversationByOtherUserId.Keys.Select(otherUserId =>
                    ToConversation(conversationByOtherUserId[otherUserId], userInfoByUserId[otherUserId])); 
            
            // Sort conversations by latest message
            var conversationsSortedByTs =
                conversations.OrderByDescending(conversation => conversation.mostRecentMessage.timestamp)
                    .ToArray();

            return conversationsSortedByTs; 
        }
        
        public Conversations ToConversations(Dataset input)
        {
            // group message by the user id of person current user is having conversation with 
            var conversationByOtherUserId = GetMessagesByOtherUserId(input.messages, input.userId); 
  
            // group user info by user id for look up later on 
            var userInfoByUserId = input.users.ToDictionary(user => user.id, user => user);

            // Order conversation by latest message, per user and sort by latest conversation
            var conversationsSortedByTs = ToInboxSortedByTimestampDesc(conversationByOtherUserId, userInfoByUserId); 
            
            return new Conversations(conversationsSortedByTs); 
        }

        public async Task<HttpResponseMessage> PostData(string endpoint, Conversations output) => 
            await httpHandler.PostData(endpoint, output);

        public async Task<HttpResponseMessage> GetConversations(string getEndpoint, string postEndpoint)
        {
            // GET input data from API 
            var messages = await GetData(getEndpoint);

            // Transform data to expected POST request format 
            var conversations = ToConversations(messages);
            
            // POST transformed data to API 
            var response = await PostData(postEndpoint, conversations);
            
            return response; 
        }
        
    }
}