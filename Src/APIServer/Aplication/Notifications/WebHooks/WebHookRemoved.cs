using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using System.Net.Http;
using System.Diagnostics;
using SharedCore.Aplication.Interfaces;
using APIServer.Domain.Core.Models.Events;
using APIServer.Aplication.Commands.Internall.Hooks;
using APIServer.Domain.Core.Models.WebHooks;

namespace APIServer.Aplication.Notifications.WebHooks
{

    /// <summary>
    /// Notifi webhook created
    /// </summary>
    public class WebHookRemovedNotifi : WebHookBaseNotifi
    {

    }

    /// <summary>
    /// Command handler for user <c>WebHookRemovedNotifi</c>
    /// </summary>
    public class WebHookRemovedEventLogHandler : INotificationHandler<WebHookRemovedNotifi>
    {

        /// <summary>
        /// Injected <c>IScheduler</c>
        /// </summary>
        private readonly IScheduler _scheduler;

        /// <summary>
        /// Injected <c>ICurrentUser</c>
        /// </summary>
        private readonly ICurrentUser _currentuser;

        /// <summary>
        /// Injected <c>ILogger</c>
        /// </summary>
        private readonly ILogger _logger;

        public WebHookRemovedEventLogHandler(
            IScheduler scheduler,
            ICurrentUser currentuser,
            ILogger logger
            )
        {

            _scheduler = scheduler;

            _currentuser = currentuser;

            _logger = logger;
        }

        /// <summary>
        /// Command handler for <c>WebHookRemovedNotifi</c>
        /// </summary>
        public async Task Handle(WebHookRemovedNotifi request, CancellationToken cancellationToken)
        {

            if (request == null)
                return;

            await Task.CompletedTask;

            try
            {
                _scheduler.Enqueue(new EnqueSaveEvent<WebHookRemoved>()
                {
                    Event = new WebHookRemoved()
                    {
                        ActorID = _currentuser.UserId,
                        WebHookId = request.WebHookId,
                        TimeStamp = request.TimeStamp != default ? request.TimeStamp : DateTime.Now,
                    },
                    ActivityId = Activity.Current != null ? Activity.Current.Id : null
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Enqueue IWebHookRemovedEventLog");
            }

            return;
        }
    }


    /// <summary>
    /// Command handler for user <c>WebHookRemovedNotifi</c>
    /// </summary>
    public class WebHookRemovedHookQueueHandler : INotificationHandler<WebHookRemovedNotifi>
    {

        /// <summary>
        /// Injected <c>IScheduler</c>
        /// </summary>
        private readonly IScheduler _scheduler;

        /// <summary>
        /// Injected <c>ICurrentUser</c>
        /// </summary>
        private readonly ICurrentUser _currentuser;

        /// <summary>
        /// Injected <c>ILogger</c>
        /// </summary>
        private readonly ILogger _logger;

        public WebHookRemovedHookQueueHandler(
            IScheduler scheduler,
            ICurrentUser currentuser,
            ILogger logger,
            IHttpClientFactory clientFactory
            )
        {

            _scheduler = scheduler;

            _currentuser = currentuser;

            _logger = logger;
        }

        /// <summary>
        /// Command handler for <c>WebHookRemovedNotifi</c>
        /// </summary>
        public async Task Handle(WebHookRemovedNotifi request, CancellationToken cancellationToken)
        {

            if (request == null)
                return;

            await Task.CompletedTask;

            WebHookRemoved ev = new WebHookRemoved()
            {
                ActorID = _currentuser.UserId,
                WebHookId = request.WebHookId,
                TimeStamp = request.TimeStamp != default ? request.TimeStamp : DateTime.Now,
            };

            try
            {
                _scheduler.Enqueue(new EnqueueRelatedWebHooks()
                {
                    Event = new Hook_HookRemoved(
                                HookResourceAction.hook_removed,
                                new Hook_User_DTO()
                                {
                                    id = ev?.ActorID?.ToString(),
                                    name = _currentuser.Name,
                                },
                                new Hook_HookRemovedPayload() { }
                            ),
                    EventType = HookEventType.hook,
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to Enqueue Related webhooks for WebHookRemoved");
            }

            return;
        }
    }
}