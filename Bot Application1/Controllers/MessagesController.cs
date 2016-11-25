using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace YesBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        // Only instantiate a random object once
        private Random _rand { get; set; }
        public Random rand
        {
            get
            {
                if (_rand == null)
                {
                    _rand = new Random();
                }
                return _rand;
            }
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message && activity.Text.Length > 0)
            {
                // Request intent and entities from LUIS
                LUISResponse luis = await LUISFactory.GetIntentFromLUIS(activity.Text);

                // Manual calculation is LUIS caps out
                if (luis.intents.Count() == 0 && (
                    activity.Text.ToLower().Contains("yesbot") ||
                    activity.Text.ToLower().Contains("agreed?") ||
                    activity.Text.ToLower().Contains("agree?") ||
                    activity.Text.ToLower().Contains("yes?") ||
                    activity.Text.ToLower().Contains("think?") ||
                    activity.Text.ToLower().Contains("do you agree") ||
                    activity.Text.ToLower().Contains("what do you think")) ||
                    // Or if LUIS decides to trigger agreement
                    luis.intents[0].intent == "RequestAgreement"
                    )
                {
                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    // random statements of agreement to return
                    string[] statements = new[] { "I agree with you 100 percent.", "I couldn't agree with you more.", "I'm with you on this one!", "That's so true!", "For sure!", "Tell me about it!", "You're absolutely right.", "That's exactly how I feel.", "No doubt about it.", "You have a point there.", "I was just going to say that!", "You are so right.", "I couldn't have said it better myself." };

                    // return a random statement to the user
                    Activity reply = activity.CreateReply($"{statements[rand.Next(statements.Length)]}");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}