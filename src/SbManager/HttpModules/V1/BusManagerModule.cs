using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using SbManager.BusHelpers;
using SbManager.CQRS.Commands;
using SbManager.CQRS.ModelBuilders;
using SbManager.Extensions;
using SbManager.InternalCommandHandlers;
using SbManager.Models.ViewModelBuilders;
using SbManager.Models.ViewModels;

namespace SbManager.HttpModules.V1
{
    public class BusManagerModule : NancyModule
    {
        private readonly IRequeueAndRemove _requeueAndRemove;
        private readonly IModelCreator _modelCreator;
        private readonly ICommandSender _commandSender;

        public BusManagerModule(IRequeueAndRemove requeueAndRemove, IModelCreator modelCreator, ICommandSender commandSender) : base("/api/v1/busmanager")
        {
            _requeueAndRemove = requeueAndRemove;
            _modelCreator = modelCreator;
            _commandSender = commandSender;

            Get["/", true] = 
                async (_, ct) => await _modelCreator.Build<Overview>();
            Post["/deleteall", true] =
                async (_, ct) => {
                    await _commandSender.Send(new DeleteAllBusEntititesCommand());
                    return new { success = true };
                };
            Post["/deletealldeadletters", true] =
               async (_, ct) => {
                   await _commandSender.Send(new DeleteAllDeadLettersCommand());
                   return new { success = true };
               };
            Get["/deadletters", true] =
                async (_, ct) => await _modelCreator.Build<DeadletterView>(); 
            Get["/topic/{tid}", true] =
                async (_, ct) => await _modelCreator.Build<Topic, TopicCriteria>(new TopicCriteria(To.String((object)_.tid)));
            Get["/queue/{qid}", true] =
                async (_, ct) => await _modelCreator.Build<Queue, QueueCriteria>(new QueueCriteria(To.String((object)_.qid)));
            Get["/topic/{tid}/{sid}", true] =
                async (_, ct) => await _modelCreator.Build<Subscription, SubscriptionCriteria>(new SubscriptionCriteria(To.String((object)_.tid), To.String((object)_.sid)));
            Get["queue/{qid}/messages/{count}", true] =
                async (_, ct) => await _modelCreator.Build<MessageView, FindQueuedMessages>(new FindQueuedMessages(To.String((object)_.qid).UnescapePathName(), To.Int(_.count)));
            Get["topic/{tid}/{sid}/messages/{count}", true] =
                async (_, ct) => await _modelCreator.Build<MessageView, FindSubscriptionMessages>(new FindSubscriptionMessages(To.String((object)_.tid), To.String((object)_.sid).UnescapePathName(), To.Int(_.count)));
            Post["/queue/{qid}/requeue/all", true] = async (_, ct) =>
            {
                var qid = To.String((object)_.qid).UnescapePathName();
                await _requeueAndRemove.RequeueAll(qid);
                return _modelCreator.Build<Queue, QueueCriteria>(new QueueCriteria(qid.RemoveDeadLetterPath()));
            };
            Post["/topic/{tid}/{sid}/requeue/all", true] = async (_, ct) =>
            {
                var tid = To.String((object)_.tid);
                var sid = To.String((object)_.sid).UnescapePathName();
                await _requeueAndRemove.RequeueAll(tid, sid);
                return _modelCreator.Build<Subscription, SubscriptionCriteria>(new SubscriptionCriteria(tid, sid.RemoveDeadLetterPath()));
            };
            Post["/queue/{qid}/remove/all", true] = async (_, ct) =>
            {
                var qid = To.String((object)_.qid).UnescapePathName();
                await _requeueAndRemove.RemoveAll(qid);
                return _modelCreator.Build<Queue, QueueCriteria>(new QueueCriteria(qid.RemoveDeadLetterPath()));
            };
            Post["/topic/{tid}/{sid}/remove/all", true] = async (_, ct) =>
            {
                var tid = To.String((object)_.tid);
                var sid = To.String((object)_.sid).UnescapePathName();
                await _requeueAndRemove.RemoveAll(tid, sid);
                return _modelCreator.Build<Subscription, SubscriptionCriteria>(new SubscriptionCriteria(tid, sid.RemoveDeadLetterPath()));
            };
            Post["queue/{qid}/delete", true] =
                async (_, ct) => {
                    await _commandSender.Send(new DeleteQueueCommand(To.String((object)_.qid).UnescapePathName().RemoveDeadLetterPath()));
                    await _commandSender.Send(new RefreshCachedOverviewCommand());
                    return new { success = true }; 
                };
            Post["topic/{tid}/delete", true] =
                async (_, ct) => {
                    await _commandSender.Send(new DeleteTopicCommand(To.String((object)_.tid).UnescapePathName()));
                    await _commandSender.Send(new RefreshCachedOverviewCommand());
                    return new { success = true }; 
                };
            Post["topic/{tid}/{sid}/delete", true] =
                async (_, ct) => { 
                    await _commandSender.Send(new DeleteSubscriptionCommand(To.String((object)_.tid).UnescapePathName(), To.String((object)_.sid).UnescapePathName().RemoveDeadLetterPath()));
                    await _commandSender.Send(new RefreshCachedOverviewCommand());
                    return new { success = true }; 
                };
            Post["queue/{qid}/remove", true] =
                async (_, ct) => { await _commandSender.Send(new RemoveMessageCommand(Request.Form.messageId, To.String((object)_.qid).UnescapePathName())); return new { success = true }; };
            Post["queue/{qid}/dead/{mid}", true] =
                async (_, ct) => { await _commandSender.Send(new DeadLetterMessageCommand(To.String((object)_.mid), To.String((object)_.qid).UnescapePathName())); return new { success = true }; };
            Post["queue/{qid}/dead", true] =
                async (_, ct) =>
                {
                    await _commandSender.Send(new DeadLetterAllMessagesCommand(To.String((object)_.qid).UnescapePathName()));
                    return _modelCreator.Build<Queue, QueueCriteria>(new QueueCriteria(To.String((object)_.qid).RemoveDeadLetterPath()));
                };
            Post["queue/{qid}/requeue", true] =
                async (_, ct) => { await _commandSender.Send(new RequeueMessageCommand(Request.Form.messageId, To.String((object)_.qid).UnescapePathName())); return new { success = true }; };
            Post["queue/{qid}/requeueModified", true] =
                async (_, ct) => { await _commandSender.Send(new RequeueMessageCommand(Request.Form.messageId, To.String((object)_.qid).UnescapePathName()) { NewBody = Request.Form.body}); return new { success = true }; };
            Post["topic/{tid}/{sid}/remove", true] =
                async (_, ct) => { await _commandSender.Send(new RemoveMessageCommand(Request.Form.messageId, To.String((object)_.tid), To.String((object)_.sid).UnescapePathName())); return new { success = true }; };
            Post["topic/{tid}/{sid}/dead/{mid}", true] =
                async (_, ct) => { await _commandSender.Send(new DeadLetterMessageCommand(To.String((object)_.mid), To.String((object)_.tid), To.String((object)_.sid).UnescapePathName())); return new { success = true }; };
            Post["topic/{tid}/{sid}/dead", true] =
                async (_, ct) =>
                {
                    await _commandSender.Send(new DeadLetterAllMessagesCommand(To.String((object)_.tid), To.String((object)_.sid).UnescapePathName()));
                    return _modelCreator.Build<Subscription, SubscriptionCriteria>(new SubscriptionCriteria(To.String((object)_.tid), To.String((object)_.sid).UnescapePathName()));
                };
            Post["topic/{tid}/{sid}/requeue", true] =
                async (_, ct) => { await _commandSender.Send(new RequeueMessageCommand(Request.Form.messageId,To.String( _.tid), To.String((object)_.sid).UnescapePathName())); return new { success = true }; };
            Post["topic/{tid}/{sid}/requeueModified", true] =
                async (_, ct) => { await _commandSender.Send(new RequeueMessageCommand(Request.Form.messageId, To.String(_.tid), To.String((object)_.sid).UnescapePathName()) { NewBody = Request.Form.body }); return new { success = true }; };

            Post["queue/{qid}", true] =
                async (_, ct) => { await _commandSender.Send(new SendMessageCommand(To.String((object)_.qid).UnescapePathName(), this.Bind<Message>())); return new { success = true }; };
            Post["topic/{tid}", true] =
                async (_, ct) => { await _commandSender.Send(new PublishMessageCommand(To.String((object)_.tid).UnescapePathName(), this.Bind<Message>())); return new { success = true }; };

        }
    }
}
