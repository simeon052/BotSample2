using BotSample2.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BotSample2
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            switch (activity?.GetActivityType())
            {
                case ActivityTypes.Message:
                    await Conversation.SendAsync(activity, MakeRootDialog);

                    break;
                default:
                    break;
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        internal static IDialog<SandwichOrder> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(SandwichOrder.BuildForm))
                .Do(async (context, order) =>
                {
                    try
                    {
                        var completed = await order;
                        await context.PostAsync("Thank you for visitting.");
                    }
                    catch (FormCanceledException<SandwichOrder> e)
                    {
                        var item = e.Last;
                        var form = e.LastForm;

                        var reply = e.InnerException == null ?
                                  $" Please come again." : " System was troubled. Please try again";
                        await context.PostAsync(reply);
                    }
                });
        }
    }
}