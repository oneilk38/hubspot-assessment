using System.Collections.Generic;

namespace HubSpot.Main
{
    // INPUT 
    public class Message
    {
        public string content { get; set; }
        public int fromUserId { get; set; }
        public long timestamp { get; set; }
        public int toUserId { get; set; }
    }

    public class User
    {
        public string avatar { get; set; }
        public string firstName { get; set; } 
        public string lastName { get; set; }
        public int id { get; set; }
    }
    
    // The response from the GET request will be deserialised to this type 
    public class Dataset
    {
        public Message[] messages { get; set; }
        public int userId { get; set; }
        public User[] users { get; set; }
    }

    // OUTPUT 
    public class MostRecentMessage
    {
        public string content { get; set; }
        public long timestamp { get; set; }
        public int userId { get; set; }

        public MostRecentMessage(Message latestMessage)
        {
            content = latestMessage.content;
            timestamp = latestMessage.timestamp;
            userId = latestMessage.fromUserId; 
        }
    }
    
    public class Conversation
    {
        public string avatar { get; set; }
        public string firstName { get; set; } 
        public string lastName { get; set; }
        public MostRecentMessage mostRecentMessage { get; set; }
        public int totalMessages { get; set; }
        public int userId { get; set; }
        
        public Conversation(User otherUser, MostRecentMessage message, int conversationSize)
        {
            avatar = otherUser.avatar;
            firstName = otherUser.firstName;
            lastName = otherUser.lastName;
            mostRecentMessage = message;
            totalMessages = conversationSize;
            userId = otherUser.id; 
        }
    }
    
    
    // The output type that we will POST to hubspot API 
    public class Conversations
    {
        public Conversation[] conversations { get; set; }

        public Conversations(Conversation[] conversations) => this.conversations = conversations;
    } 
}