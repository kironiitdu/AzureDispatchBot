// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using QnaLuisBot.Models;

namespace Microsoft.BotBuilderSamples
{
    public class DispatchBot : ActivityHandler
    {
        private ILogger<DispatchBot> _logger;
        private IBotServices _botServices;

        public DispatchBot(IBotServices botServices, ILogger<DispatchBot> logger)
        {
            _logger = logger;
            _botServices = botServices;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //Checking User Input
            dynamic checkUserInput = turnContext.Activity.Text;

            // Call the card dispacher from user input
            await DispatchToRightCardFromUserInputAsync(turnContext, checkUserInput, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            // const string WelcomeText = "Type a greeting, or a question about the weather to get started.";

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    DateTime dateTime = DateTime.Now;
                    DateTime utcTime = dateTime.ToUniversalTime();
                    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

                    DateTime yourLocalTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, cstZone);
                    var hour = yourLocalTime.Hour;

                    switch (hour)
                    {
                        case int time when time <= 12:
                            await turnContext.SendActivityAsync(MessageFactory.Text($"Hello , good morning"), cancellationToken);
                            break;
                        case int time when (time >= 12 && time <= 17):
                            await turnContext.SendActivityAsync(MessageFactory.Text($"Hello , good afternoon"), cancellationToken);
                            break;
                        case int time when time > 17:
                            await turnContext.SendActivityAsync(MessageFactory.Text($"Hello , good evening"), cancellationToken);
                            break;
                        default:
                            await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                            break;
                    }
                    //Showing your course at the begining
                    var courseCard = CardCourseInfo();
                    if (courseCard != null)
                    {
                        await turnContext.SendActivityAsync(courseCard);
                    }

                }
            }
        }

        private async Task DispatchToRightCardFromUserInputAsync(ITurnContext<IMessageActivity> turnContext, string intent, CancellationToken cancellationToken)
        {
            //Check user input and process the card
            if (intent == null)
            {
                await ProcessNotFoundResult(turnContext, cancellationToken);
            }
            else
            {
                switch (intent)
                {
                    case "checktime":
                        await GetTimeInfo(turnContext, cancellationToken);
                        break;
                    case "LearnMore":
                        await CardLearnMore(turnContext, cancellationToken);
                        // await ProcessWeatherAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
                        break;
                    case "Close":
                        //Call Qna Maker Here 
                        await turnContext.SendActivityAsync(MessageFactory.Text("Thank you very much!"), cancellationToken);
                        break;
                    case "None":
                        //Call Qna Maker Here 
                        await ProcessNotFoundResult(turnContext, cancellationToken);
                        break;
                    default:// If no answer founded always call QNA maker again.
                        await CallCourseInfoCard(turnContext, cancellationToken);
                        break;

                }
            }

        }





        private async Task ProcessNotFoundResult(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessSampleQnAAsync");


            try
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer "), cancellationToken);
            }
            catch (System.Exception ex)
            {

                throw;
            }

        }

        private async Task CardLearnMore(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling Learn more Card");


            try
            {
                var cardLearnmore = CardLearnMore();
                if (cardLearnmore != null)
                {
                    await turnContext.SendActivityAsync(cardLearnmore);
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not found the course!"), cancellationToken);
                }
            }
            catch (System.Exception ex)
            {

                throw;
            }

        }
        private async Task CallCourseInfoCard(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling Course Info Card");


            try
            {
                var courseCard = CardCourseInfo();
                if (courseCard != null)
                {
                    await turnContext.SendActivityAsync(courseCard);
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not found the course!"), cancellationToken);
                }
            }
            catch (System.Exception ex)
            {

                throw;
            }

        }
        private async Task GetTimeInfo(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling Time Method");


            try
            {
                var timeCard = CardCurrentTimeInfo();
                await turnContext.SendActivityAsync(timeCard);
            }
            catch (System.Exception ex)
            {

                throw;
            }

        }
        //Example sample for time 
        public IMessageActivity CardCurrentTimeInfo()
        {
            try
            {
                //Break Time in Segment
                var time = DateTime.Now.ToString("h:mm:ss tt");
                var day = System.DateTime.Now.DayOfWeek.ToString();
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                var timeInfoCard = Activity.CreateMessageActivity();
                //Bind to Card
                var heroCard = new HeroCard
                {
                    Title = "**Time Information Card**",
                    Text = string.Format("Time: **{0}** " + Environment.NewLine + "Day: **{1}**" + Environment.NewLine + "Date: **{2}**", time, day, date),
                    Images = new List<CardImage> { new CardImage("") },
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Okay", value: "Okay") },
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

        //This is your sample 
        public static Attachment GetHeroCardModified()
        {
            var heroCard = new HeroCard
            {
                // title of the card
                Title = "Md Farid Uddin Kiron",
                //subtitle of the card
                Subtitle = "Microsoft certified solution developer",
                // navigate to page , while tab on card
                // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                Tap = new CardAction("", "Learn More", value: "Learn More"),
                //Detail Text
                Text = "Suthahar J is a Technical Lead and C# Corner MVP. He has extensive 10+ years of experience working on different technologies, mostly in Microsoft space. His focus areas are  Xamarin Cross Mobile Development ,UWP, SharePoint, Azure,Windows Mobile , Web , AI and Architecture. He writes about technology at his popular blog http://devenvexe.com",
                // list of  Large Image
                Images = new List<CardImage> { new CardImage("http://csharpcorner.mindcrackerinc.netdna-cdn.com/UploadFile/AuthorImage/jssuthahar20170821011237.jpg") },
                // list of buttons 
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Learn More", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.OpenUrl, "C# Corner", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.OpenUrl, "MSDN", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron") }
            };

            return heroCard.ToAttachment();
        }
        // Example as per your requirement
        public IMessageActivity CardCourseInfo()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "Md Farid Uddin Kiron",
                    //subtitle of the card
                    Subtitle = "Microsoft certified solution developer",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "Learn More", value: "LearnMore"),
                    //Detail Text
                    Text = "Suthahar J is a Technical Lead and C# Corner MVP. He has extensive 10+ years of experience working on different technologies, mostly in Microsoft space. His focus areas are  Xamarin Cross Mobile Development ,UWP, SharePoint, Azure,Windows Mobile , Web , AI and Architecture. He writes about technology at his popular blog https://stackoverflow.com/users/9663070/md-farid-uddin-kiron",
                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://stackoverflow.com/users/9663070/md-farid-uddin-kiron") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Learn More", value: "LearnMore"), new CardAction(ActionTypes.OpenUrl, "C# Corner", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.OpenUrl, "MSDN", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron") }
                };

                // Create the attachment.
                var attachment = heroCard.ToAttachment();

                courseInfoCard.Attachments.Add(attachment);
                courseInfoCard.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                return courseInfoCard;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex.InnerException);
            }

        }
        public IMessageActivity CardLearnMore()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                    {
                    // title of the card
                    Title = "Md Farid Uddin Kiron",
                    //subtitle of the card
                    Subtitle = "Microsoft certified solution developer",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "Learn More", value: "LearnMore"),
                    //Detail Text
                    Text = "Suthahar J is a Technical Lead and C# Corner MVP. He has extensive 10+ years of experience working on different technologies, mostly in Microsoft space. His focus areas are  Xamarin Cross Mobile Development ,UWP, SharePoint, Azure,Windows Mobile , Web , AI and Architecture. He writes about technology at his popular blog https://stackoverflow.com/users/9663070/md-farid-uddin-kiron",
                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://stackoverflow.com/users/9663070/md-farid-uddin-kiron") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Learn More", value: "LearnMore"), new CardAction(ActionTypes.OpenUrl, "C# Corner", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.OpenUrl, "MSDN", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron") }
                };

                // Create the attachment.
                var attachment = heroCard.ToAttachment();

                courseInfoCard.Attachments.Add(attachment);
                courseInfoCard.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                return courseInfoCard;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex.InnerException);
            }

        }

    }
}
