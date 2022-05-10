using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QnaLuisBot.Bots
{
    public class StellarFanBot: ActivityHandler
    {
        private ILogger<StellarFanBot> _logger;
        private IBotServices _botServices;


        public StellarFanBot(IBotServices botServices, ILogger<StellarFanBot> logger)
        {
            _logger = logger;
            _botServices = botServices;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //Checking User Input
            dynamic checkUserInput = turnContext.Activity.Text;

            dynamic userTypeValue = turnContext.Activity.Value;

            switch (checkUserInput)
            {
                case "one":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thank you very much for your \U0001F929 feedback"), cancellationToken);
                    break;
                case "two":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thank you very much for your \U0001F929 \U0001F929 feedback"), cancellationToken);
                    break;
                case "three":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thank you very much for your \U0001F929 \U0001F929 \U0001F929 \U0001F929 feedback"), cancellationToken);
                    break;
                case "four":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thank you very much for your \U0001F929 \U0001F929 \U0001F929 \U0001F929 \U0001F929 feedback"), cancellationToken);
                    break;
                case "five":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thank you very much for your \U0001F929 \U0001F929 \U0001F929 \U0001F929 \U0001F929 \U0001F929 feedback"), cancellationToken);
                    break;
                case "SearchByIssueType":
                    var replyFromUserCardSearchByIssueType = UserSelectionCard(checkUserInput);
                    await turnContext.SendActivityAsync(replyFromUserCardSearchByIssueType);
                    break;
                case "SearchByProductCategory":
                    var replyFromUserCardSearchByProductCategort = UserSelectionCard(checkUserInput);
                    await turnContext.SendActivityAsync(replyFromUserCardSearchByProductCategort);
                    break;
                case "Okay":
                    var likeDislikeCard = LikeDislikeCard();
                    await turnContext.SendActivityAsync(likeDislikeCard);
                    break;
                case "like":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"You have choosed Like"), cancellationToken);
                    var eof = EndOfConversationCard();
                    await turnContext.SendActivityAsync(eof);
                    break;
                case "dislike":
                    await turnContext.SendActivityAsync(MessageFactory.Text($"You have choosed Dislike"), cancellationToken);
                    break;

                default:// If no answer founded always call QNA maker again.
                    var endConversation = EndOfConversationCard();
                    await turnContext.SendActivityAsync(endConversation);
                    break;

            }

           
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            // const string WelcomeText = "Type a greeting, or a question about the weather to get started.";

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var productProactivePromptCardEvening = ProductAndCategoryPromptCard();
                    await turnContext.SendActivityAsync(productProactivePromptCardEvening);

                }
            }
        }

        public IMessageActivity EndOfConversationCard()
        {
            try
            {
                //Break in Segment
                var timeInfoCard = Activity.CreateMessageActivity();
                var thanks = "Thank you very much for your interaction";
                var rate = "You could rate our service...";
                //Bind to Card
                var heroCard = new HeroCard
                {
                    // Title = "Thank you very much for your interaction, you could rate me...",
                    Text = string.Format("**{0}** " + Environment.NewLine + "**{1}**", thanks, rate),
                    Images = new List<CardImage> { new CardImage("") },
                    Buttons = new List<CardAction> {
                        new CardAction(ActionTypes.ImBack, "\U0001F929", value: "one") ,
                        new CardAction(ActionTypes.ImBack, "\U0001F929 \U0001F929 ", value: "two"),
                        new CardAction(ActionTypes.ImBack, "\U0001F929 \U0001F929 \U0001F929", value: "three"),
                        new CardAction(ActionTypes.ImBack, "\U0001F929 \U0001F929 \U0001F929 \U0001F929 \U0001F929", value: "four"),
                        new CardAction(ActionTypes.ImBack, "\U0001F929 \U0001F929 \U0001F929 \U0001F929 \U0001F929 \U0001F929", value: "five"),
                    },
                };

                // Create the attachment.
                var attachment = heroCard.ToAttachment();

                timeInfoCard.Attachments.Add(attachment);
                timeInfoCard.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                return timeInfoCard;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex.InnerException);
            }

        }

        public IMessageActivity LikeDislikeCard()
        {
            try
            {
                //Break in Segment
                var timeInfoCard = Activity.CreateMessageActivity();
                //Bind to Card
                var heroCard = new HeroCard
                {
                    Title = "How do you think about the answer?",
                    // Text = string.Format("Time: **{0}** " + Environment.NewLine + "Day: **{1}**" + Environment.NewLine + "Date: **{2}**", time, day, date),
                    Images = new List<CardImage> { new CardImage("") },
                    Buttons = new List<CardAction> {
                        new CardAction(ActionTypes.ImBack, "👍👍👍👍👍", value: "like") ,
                        new CardAction(ActionTypes.ImBack, "👎👎👎👎👎", value: "dislike")
                    },
                };

                // Create the attachment.
                var attachment = heroCard.ToAttachment();

                timeInfoCard.Attachments.Add(attachment);
                timeInfoCard.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                return timeInfoCard;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex.InnerException);
            }

        }
        public IMessageActivity ProductAndCategoryPromptCard()
        {
            try
            {
                //Break in Segment
                var timeInfoCard = Activity.CreateMessageActivity();
                //Bind to Card
                var heroCard = new HeroCard
                {
                    Title = "Try to search using the type of issue, or type of device",
                    // Text = string.Format("Time: **{0}** " + Environment.NewLine + "Day: **{1}**" + Environment.NewLine + "Date: **{2}**", time, day, date),
                    Images = new List<CardImage> { new CardImage("") },
                    Buttons = new List<CardAction> {
                        new CardAction(ActionTypes.ImBack, "Search By Issue Type", value: "SearchByIssueType") ,
                        new CardAction(ActionTypes.ImBack, "Search By Product Category", value: "SearchByProductCategory")
                    },
                };

                // Create the attachment.
                var attachment = heroCard.ToAttachment();

                timeInfoCard.Attachments.Add(attachment);
                timeInfoCard.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                return timeInfoCard;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex.InnerException);
            }

        }

        public IMessageActivity UserSelectionCard(string customerSelecteddValue)
        {
            try
            {
                //Break Segment

                var timeInfoCard = Activity.CreateMessageActivity();
                //Bind to Card
                var heroCard = new HeroCard
                {
                    Title = "**" + customerSelecteddValue + " Card**",
                    Text = string.Format("You have selected: **{0}** ", customerSelecteddValue),
                    Images = new List<CardImage> { new CardImage("") },
                    Buttons = new List<CardAction> {
                        new CardAction(ActionTypes.ImBack, "Thank you very much", value: "Okay")
                    },
                };

                // Create the attachment.
                var attachment = heroCard.ToAttachment();

                timeInfoCard.Attachments.Add(attachment);
                timeInfoCard.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                return timeInfoCard;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex.InnerException);
            }

        }
    }
}
