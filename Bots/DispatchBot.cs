// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            dynamic userFormValue = turnContext.Activity.Value;
            if (userFormValue != null)
            {
                goto validate;
                validate: ValidateUserInput(turnContext, userFormValue, cancellationToken);
                return;
            }


            // Call the card dispacher from user input
            await DispatchToRightCardFromUserInputAsync(turnContext, checkUserInput, cancellationToken);
        }
        //Check User input from submit form

        private async Task ValidateUserInput(ITurnContext<IMessageActivity> turnContext, dynamic input , CancellationToken cancellationToken)
        {
            try
            {
                // UserSubmitForm user = JsonConvert.DeserializeObject(input);
                UserSubmitForm user = JsonConvert.DeserializeObject<UserSubmitForm>(input.ToString());
                if (string.IsNullOrEmpty(user.fullName))
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Please fill out the admission form correctly!"));
                    goto DisplayAdmissionCardAgain;
                    //When user input is not in correct form send the addmission card again
                    DisplayAdmissionCardAgain: await DispatchToRightCardFromUserInputAsync(turnContext, "Admission", cancellationToken);
                }

                else
                {
                    var responseToUser = $"Hey, {user.fullName} thanks for submitting the form! " + "\r\n" + $" We will contract you on {user.mobileNumber} or on {user.email} soon";
                    await turnContext.SendActivityAsync(MessageFactory.Text(responseToUser));
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        //Reads Card From json File
        private static Attachment CreateAdaptiveCardAttachment(string filePath)
        {
            var adaptiveCardJson = File.ReadAllText(filePath);
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
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
                            //  await turnContext.SendActivityAsync(MessageFactory.Text($"Hello , good morning"), cancellationToken);
                            break;
                        case int time when (time >= 12 && time <= 17):
                            // await turnContext.SendActivityAsync(MessageFactory.Text($"Hello , good afternoon"), cancellationToken);
                            break;
                        case int time when time > 17:
                            // await turnContext.SendActivityAsync(MessageFactory.Text($"Hello , good evening"), cancellationToken);
                            break;
                        default:
                            // await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
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
                    case "HomePage":
                        await CardLearnMore(turnContext, cancellationToken);
                        // await ProcessWeatherAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
                        break;


                    case "Courses":
                        await CardCourses(turnContext, cancellationToken);
                        break;



                    case "Schedule Virtual Counselling":
                        await CardSchedule(turnContext, cancellationToken);
                        break;

                    case "Ask Query":
                        await CardAsk(turnContext, cancellationToken);
                        break;

                    case "B.E in Computer Science and Engineering":
                        await CardCS(turnContext, cancellationToken);
                        break;

                    case "B.E in Information Science and Engineering":
                        await CardIS(turnContext, cancellationToken);
                        break;

                    case "B.E in CS- AI and DS":
                        await CardDS(turnContext, cancellationToken);
                        break;

                    case "B.E in CS- AI and ML":
                        await CardML(turnContext, cancellationToken);
                        break;

                    case "B.E in Marine Engineering":
                        await CardMAE(turnContext, cancellationToken);
                        break;

                    case "B.E in Electronics And Communication":
                        await CardEC(turnContext, cancellationToken);
                        break;
                    case "B.E in Mechanical Engineering":
                        await CardME(turnContext, cancellationToken);
                        break;

                    case "B.E in Automobile Engineering":
                        await CardAT(turnContext, cancellationToken);
                        break;

                    case "B.E in Electricals and Electronic Engineering":
                        await CardEE(turnContext, cancellationToken);
                        break;

                    case "B.E in Aeronautical Engineering":
                        await CardAN(turnContext, cancellationToken);
                        break;

                    case "B.E in Architecture":
                        await CardAR(turnContext, cancellationToken);
                        break;

                    case "MBA":
                        await CardMBA(turnContext, cancellationToken);
                        break;

                    case "MCA":
                        await CardMCA(turnContext, cancellationToken);
                        break;

                    case "Close":
                        //Call Qna Maker Here 
                        await turnContext.SendActivityAsync(MessageFactory.Text("Thank you very much!"), cancellationToken);
                        break;

                    case "Admission":
                        //Assigning the admission card path info
                        var admissionInfoCardPath = Path.Combine(".", "Resources", "AdmissionInformationForm.json");
                        //Passing To Card Builder To build Admission info form
                        var getAdmissionFrom = CreateAdaptiveCardAttachment(admissionInfoCardPath);

                        await turnContext.SendActivityAsync(MessageFactory.Text("Please fill out the admission form"), cancellationToken);
                        //take some delay and reply card
                        await turnContext.SendActivitiesAsync(new Activity[] { new Activity { Type = ActivityTypes.Typing }, new Activity { Type = "delay", Value = 5000 } });
                        //Sending the admission card when user press admission
                        await turnContext.SendActivityAsync(MessageFactory.Attachment(getAdmissionFrom), cancellationToken);
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

        private async Task CardAdmission(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling Learn more Card");


            try
            {
                var cardLearnmore = CardAdmission();
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



        private async Task CardCourses(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardCourses();
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
        private async Task CardSchedule(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardSchedule();
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

        private async Task CardAsk(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardAsk();
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


        private async Task CardCS(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardCS();
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

        private async Task CardDS(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardDS();
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
        private async Task CardMAE(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardMAE();
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
        private async Task CardIS(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardIS();
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

        private async Task CardML(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardML();
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

        private async Task CardEC(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardEC();
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

        private async Task CardME(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardME();
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
        private async Task CardAT(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardAT();
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
        private async Task CardEE(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardEE();
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
        private async Task CardAN(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardAN();
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
        private async Task CardAR(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardAR();
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
        private async Task CardMBA(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardMBA();
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
        private async Task CardMCA(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calling AddMore Card");


            try
            {
                var cardLearnmore = CardMCA();
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
                    /* Title = "**Time Information Card**",
                     Text = string.Format("Time: **{0}** " + Environment.NewLine + "Day: **{1}**" + Environment.NewLine + "Date: **{2}**", time, day, date),
                     Images = new List<CardImage> { new CardImage("") },
                     Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Okay", value: "Okay") },*/
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
        //public static Attachment GetHeroCardModified()
        //{
        //    var heroCard = new HeroCard
        //    {
        //        // title of the card
        //        Title = "Md Farid Uddin Kiron",
        //        //subtitle of the card
        //        Subtitle = "Microsoft certified solution developer",
        //        // navigate to page , while tab on card
        //        // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
        //        Tap = new CardAction("", "Learn More", value: "Learn More"),
        //        //Detail Text
        //        Text = "Suthahar J is a Technical Lead and C# Corner MVP. He has extensive 10+ years of experience working on different technologies, mostly in Microsoft space. His focus areas are  Xamarin Cross Mobile Development ,UWP, SharePoint, Azure,Windows Mobile , Web , AI and Architecture. He writes about technology at his popular blog http://devenvexe.com",
        //        // list of  Large Image
        //        Images = new List<CardImage> { new CardImage("http://csharpcorner.mindcrackerinc.netdna-cdn.com/UploadFile/AuthorImage/jssuthahar20170821011237.jpg") },
        //        // list of buttons 
        //        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Learn More", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.OpenUrl, "C# Corner", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.OpenUrl, "MSDN", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction("", "Close", value: "Close") }
        //    };

        //    return heroCard.ToAttachment();
        //}
        // Example as per your requirement
        public IMessageActivity CardCourseInfo()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "Courses", value: "Courses"),

                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/assets/img/logo.png") },

                    //Detail Text
                    Text = "Hi! Welcome to Srinivas Institute of Technology. I am a virtual agent. I can help with your queries related to Admission, Programs, Courses Offered and more.",
                    // list of  Large Image

                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Courses", value: "Courses"), new CardAction(ActionTypes.ImBack, "Admission", value: "Admission"), new CardAction(ActionTypes.ImBack, "Schedule Virtual Counselling", value: "Schedule Virtual Counselling"), new CardAction(ActionTypes.ImBack, "Ask Query", value: "Ask Query") }
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
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "Courses", value: "Courses"),

                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/assets/img/logo.png") },

                    //Detail Text
                    Text = "Hi! Welcome to Srinivas Institute of Technology. I am a virtual agent. I can help with your queries related to Admission, Programs, Courses Offered and more.",
                    // list of  Large Image

                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Courses", value: "Courses"), new CardAction(ActionTypes.ImBack, "Admission", value: "Admission"), new CardAction(ActionTypes.ImBack, "Schedule Virtual Counselling", value: "Schedule Virtual Counselling"), new CardAction(ActionTypes.ImBack, "Ask Query", value: "Ask Query") }
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


        public IMessageActivity CardCourses()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "Here are the options. Please choose any",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "AddMore", value: "AddMore"),
                    //Detail Text
                    Text = "",
                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://stackoverflow.com/users/9663070/md-farid-uddin-kiron") },
                    // list of buttons 
                    Buttons = new List<CardAction> {

  new CardAction(ActionTypes.ImBack, " B.E in Computer Science and Engineering", value: "B.E in Computer Science and Engineering"),

  new CardAction(ActionTypes.ImBack," B.E in Information Science and Engineering", value: "B.E in Information Science and Engineering"),

  new CardAction(ActionTypes.ImBack, " B.E in CS- AI and DS", value: "B.E in CS- AI and DS"),
  new CardAction(ActionTypes.ImBack, " B.E in CS- AI and ML", value: "B.E in CS- AI and ML"),


  new CardAction(ActionTypes.ImBack, " B.E in Marine Engineering ", value: "B.E in Marine Engineering"),
  new CardAction(ActionTypes.ImBack, " B.E in Electronics And Communication", value: "B.E in Electronics And Communication"),

 new CardAction(ActionTypes.ImBack, " B.E in Mechanical Engineering", value: "B.E in Mechanical Engineering"),
 new CardAction(ActionTypes.ImBack, " B.E in Automobile Engineering", value: "B.E in Automobile Engineering"),
 new CardAction(ActionTypes.ImBack, " B.E in Electricals and Electronic Engineering", value: "B.E in Electricals and Electronic Engineering"),
 new CardAction(ActionTypes.ImBack, " B.E in Aeronautical Engineering", value: "B.E in Aeronautical Engineering"),
 new CardAction(ActionTypes.ImBack, " B.E in Architecture", value: "B.E in Architecture"),
 new CardAction(ActionTypes.ImBack, " MBA", value: "MBA"),
 new CardAction(ActionTypes.ImBack, " MCA", value: "MCA"),
 new CardAction(ActionTypes.ImBack, "Go To Home Page", value: "HomePage")

}

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
        public IMessageActivity CardAdmission()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {

                    //// title of the card
                    //Title = "",
                    ////subtitle of the card
                    //Text = "\r\n" + "\r\n" + "\r\n" + "\r\n" + "",
                    //Subtitle = "" + "\r\n",
                    //// navigate to page , while tab on card
                    //// Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    //Tap = new CardAction("", "Admission", value: "Admission"),
                    ////Detail Text
                    //TextArea="",
                    //// list of  Large Image

                    //Images = new List<CardImage> { new CardImage("https://stackoverflow.com/users/9663070/md-farid-uddin-kiron") },
                    //// list of buttons
                    //Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Learn More", value: "LearnMore"), new CardAction(ActionTypes.OpenUrl, "C# Corner", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.OpenUrl, "MSDN", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.ImBack, "Admission", value: "Close") }
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

        public IMessageActivity CardSchedule()
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
                    Tap = new CardAction("", "Schedule Virtual Counselling", value: "Schedule Virtual Counselling"),
                    //Detail Text
                    Text = "Hi! Welcome to Srinivas Institute of Technology. I am a virtual agent. I can help with your queries related to Admission, Programs, Courses Offered and more.",
                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://stackoverflow.com/users/9663070/md-farid-uddin-kiron") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Learn More", value: "LearnMore"), new CardAction(ActionTypes.OpenUrl, "C# Corner", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.OpenUrl, "MSDN", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.ImBack, "Schedule", value: "Close") }
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

        public IMessageActivity CardAsk()
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
                    Tap = new CardAction("", "Ask Query", value: "Ask Query"),
                    //Detail Text
                    Text = "Hi! Welcome to Srinivas Institute of Technology. I am a virtual agent. I can help with your queries related to Admission, Programs, Courses Offered and more.",
                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://stackoverflow.com/users/9663070/md-farid-uddin-kiron") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Learn More", value: "LearnMore"), new CardAction(ActionTypes.OpenUrl, "C# Corner", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.OpenUrl, "MSDN", value: "https://stackoverflow.com/users/9663070/md-farid-uddin-kiron"), new CardAction(ActionTypes.ImBack, "Ask", value: "Close") }
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

        public IMessageActivity CardCS()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in Computer Science and Engineering",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in Computer Science and Engineering", value: "B.E in Computer Science and Engineering"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,78,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   1,50,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   1,50,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;1,50,500/- " + "\r\n" +
                    "**For Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,40,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   1,25,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   1,25,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;1,25,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/Computer%20Labs/2.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://sitmng.ac.in/Department-Of-CSE/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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

        public IMessageActivity CardIS()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in Information Science and Engineering",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in Information Science and Engineering", value: "B.E in Information Science and Engineering"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,53,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   1,25,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   1,25,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;1,25,500/- " + "\r\n" +
                    "**For Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,25,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   1,00,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   1,00,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;1,00,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/Computer%20Labs/3.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-ISE/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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


        public IMessageActivity CardDS()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in CS- AI and DS",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in CS- AI and DS", value: "B.E in CS- AI and DS"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,43,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   95,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   95,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;95,500/- " + "\r\n" +
                    "**For Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 90,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/Computer%20Labs/2.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-AIDS/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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

        public IMessageActivity CardML()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in CS- AI and ML",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in CS- AI and ML", value: "B.E in CS- AI and ML"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,43,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   95,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   95,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;95,500/- " + "\r\n" +
                    "**For Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 90,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/Computer%20Labs/2.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-AIML/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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

        public IMessageActivity CardMAE()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in Marine Engineering",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in Marine Engineering", value: "B.E in Marine Engineering"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,43,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   95,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   95,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;95,500/- " + "\r\n" +
                    "**For Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 90,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/marine/gallery/1.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-Marine/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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

        public IMessageActivity CardEC()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in Electronics And Communication",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in Electronics And Communication", value: "B.E in Electronics And Communication"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,28,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- " + "\r\n" +
                    "**For Karnataka (ABOVE 50% IN PMB/PMC/PME/PMCS)**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 80,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/Electronics%20Lab/1.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-EC/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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

        public IMessageActivity CardME()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in Mechanical Engineering",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in Mechanical Engineering", value: "B.E in Mechanical Engineering"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,28,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- " + "\r\n" +
                    "**For Karnataka (ABOVE 50% IN PMB/PMC/PME/PMCS)**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 80,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/Mechanical%20Labs/1.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-ME/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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

        public IMessageActivity CardAT()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in Automobile Engineering",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in Automobile Engineering", value: "B.E in Automobile Engineering"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,28,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- " + "\r\n" +
                    "**For Karnataka  (ABOVE 50% IN PMB/PMC/PME/PMCS)**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 80,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/Automobile%20Lab/1.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-Automobile/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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

        public IMessageActivity CardEE()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in Electricals and Electronic Engineering",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in Electricals and Electronic Engineering", value: "B.E in Electricals and Electronic Engineering"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,28,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- " + "\r\n" +
                    "**For Karnataka(ABOVE 50% IN PMB/PMC/PME/PMCS)**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 80,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/Electrical%20Lab/1.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-EE/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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


        public IMessageActivity CardAN()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in Aeronautical Engineering",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in Aeronautical Engineering", value: "B.E in Aeronautical Engineering"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,28,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- " + "\r\n" +
                    "**For Karnataka(ABOVE 50% IN PMB/PMC/PME/PMCS)**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 80,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;75,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/Aeronautical%20Lab/gallery_1.png") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-Aeronautical/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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


        public IMessageActivity CardAR()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "B.E in Architecture",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "B.E in Architecture", value: "B.E in Architecture"),
                    //Detail Text

                    Text = "**For Non-Karnataka**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,78,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   1,50,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   1,50,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;1,50,500/- " + "\r\n" +
                    "**For Karnataka(ABOVE 50% Must)**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,30,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   1,00,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   1,00,500/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;1,00,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://www.ssamangalore.in/theme/images/events/805615978.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.ssamangalore.in/home"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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


        public IMessageActivity CardMBA()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "MBA",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "MBA", value: "MBA"),
                    //Detail Text

                    Text = "**For Non-Karnataka(SHOULD QUALIFY MAT, KMAT, PGCET)**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1,78,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   1,35,500/-" + "\r\n" +
                    "**For Karnataka (KMAT or Others)**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 95,500/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" +
                    "**SRINIVAS/PGCET WROTE /G.Q STUDENTS**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 85,500/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/MBA/1.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-MBA/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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

        public IMessageActivity CardMCA()
        {
            try
            {
                var courseInfoCard = Activity.CreateMessageActivity();
                var heroCard = new HeroCard
                {
                    // title of the card
                    Title = "MCA",
                    //subtitle of the card
                    Subtitle = "",
                    // navigate to page , while tab on card
                    // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "Learn More"),
                    Tap = new CardAction("", "MCA", value: "MCA"),
                    //Detail Text

                    Text = "**For Non-Karnataka(SHOULD QUALIFY MAT, KMAT, PGCET)**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 93,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" +
                    "**For Karnataka (PGCET QUAL.)/GOVT.QUOTA**" + "\r\n" + "I and II semester &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 80,000/-" + "\r\n" + "III and IV semester &nbsp;&nbsp;&nbsp;&nbsp;   75,500/-" + "\r\n" + "V and VI semester  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   1,25,000/-" + "\r\n" + "VII and VIII semester &nbsp;&nbsp;1,20,500/- ",






                    // list of  Large Image
                    Images = new List<CardImage> { new CardImage("https://sitmng.ac.in/img/gallery/Computer%20Labs/2.jpg") },
                    // list of buttons 
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Apply", value: "https://www.sitmng.ac.in/SIT/Academics/Admission"), new CardAction(ActionTypes.OpenUrl, "More Details", value: "https://www.sitmng.ac.in/Department-Of-MCA/Overview"), new CardAction(ActionTypes.ImBack, "Click Here to Select another Course", value: "Courses") }
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
