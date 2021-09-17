// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using QnaLuisBot.Models;
using System.Threading.Tasks;

namespace Microsoft.BotBuilderSamples
{
    public interface IBotServices
    {
        LuisRecognizer Dispatch { get; }
        QnAMaker SampleQnA { get; }
        Task<WeatherResponseClass> GetWeatherUpdate(string cityName);
        Task<EmployeeAPIResponse> GetEmployee(int employee_id);
    }
}
