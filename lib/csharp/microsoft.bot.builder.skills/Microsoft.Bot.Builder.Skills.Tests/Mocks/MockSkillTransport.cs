﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Skills.Auth;
using Microsoft.Bot.Builder.Skills.Models.Manifest;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Skills.Tests.Mocks
{
    public class MockSkillTransport : SkillTransport
    {
        private Activity _activityForwarded;

        public override Task CancelRemoteDialogsAsync(ITurnContext turnContext, SkillManifest skillManifest, IServiceClientCredentials serviceClientCredentials, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public override void Disconnect()
        {
        }

        public override Task<Activity> ForwardToSkillAsync(ITurnContext turnContext, SkillManifest skillManifest, IServiceClientCredentials serviceClientCredentials, Activity activity, ISkillResponseHandler skillResponseHandler, CancellationToken cancellationToken = default)
        {
            _activityForwarded = activity;
            return Task.FromResult<Activity>(null);
        }

        public bool CheckIfSkillInvoked()
            => _activityForwarded != null;

        public void VerifyActivityForwardedCorrectly(Action<Activity> assertion)
            => assertion(_activityForwarded);
    }
}
