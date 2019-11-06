﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Globalization;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Solutions.Middleware;
using Microsoft.Bot.Builder.Solutions.Responses;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using RestaurantBookingSkill.Responses.Shared;
using RestaurantBookingSkill.Services;

namespace RestaurantBookingSkill.Adapters
{
    public class DefaultAdapter : BotFrameworkHttpAdapter
    {
        public DefaultAdapter(
            BotSettings settings,
            ICredentialProvider credentialProvider,
            IBotTelemetryClient telemetryClient,
            ResponseManager responseManager)
            : base(credentialProvider)
        {
            OnTurnError = async (context, exception) =>
            {
                CultureInfo.CurrentUICulture = new CultureInfo(context.Activity.Locale);
                await context.SendActivityAsync(responseManager.GetResponse(RestaurantBookingSharedResponses.ErrorMessage));
                await context.SendActivityAsync(new Activity(type: ActivityTypes.Trace, text: $"Restaurant Booking Skill Error: {exception.Message} | {exception.StackTrace}"));
                telemetryClient.TrackException(exception);
            };

            Use(new TranscriptLoggerMiddleware(new AzureBlobTranscriptStore(settings.BlobStorage.ConnectionString, settings.BlobStorage.Container)));
            Use(new TelemetryLoggerMiddleware(telemetryClient, logPersonalInformation: true));
            Use(new ShowTypingMiddleware());
            Use(new SetLocaleMiddleware(settings.DefaultLocale ?? "en-us"));
            Use(new EventDebuggerMiddleware());
        }
    }
}