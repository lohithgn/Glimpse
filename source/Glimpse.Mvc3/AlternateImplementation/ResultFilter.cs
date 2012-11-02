using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Mvc.Message;

namespace Glimpse.Mvc.AlternateImplementation
{
    public class ResultFilter : Alternate<IResultFilter>
    {
        public ResultFilter(IProxyFactory proxyFactory) : base(proxyFactory)
        {
        }

        public override IEnumerable<IAlternateImplementation<IResultFilter>> AllMethods()
        {
            yield return new OnResultExecuting();
            yield return new OnResultExecuted();
        }

        public class OnResultExecuting : IAlternateImplementation<IResultFilter>
        {
            public OnResultExecuting()
            {
                MethodToImplement = typeof(IResultFilter).GetMethod("OnResultExecuting");
            }

            public MethodInfo MethodToImplement { get; private set; }

            public void NewImplementation(IAlternateImplementationContext context)
            {
                var timer = context.ProceedWithTimerIfAllowed();

                if (timer == null)
                {
                    return;
                }

                context.MessageBroker.PublishMany(
                    new Message((ResultExecutingContext)context.Arguments[0]), 
                    new TimerResultMessage(timer, "OnResultExecuting", "ResultFilter"));
            }

            public class Message
            {
                public Message(ResultExecutingContext argument)
                {
                    // TODO
                }
            }
        }

        public class OnResultExecuted : IAlternateImplementation<IResultFilter>
        {
            public OnResultExecuted()
            {
                MethodToImplement = typeof(IResultFilter).GetMethod("OnResultExecuted");
            }

            public MethodInfo MethodToImplement { get; private set; }

            public void NewImplementation(IAlternateImplementationContext context)
            {
                var timer = context.ProceedWithTimerIfAllowed();

                if (timer == null)
                {
                    return;
                }

                context.MessageBroker.PublishMany(
                    new Message((ResultExecutedContext)context.Arguments[0]), 
                    new TimerResultMessage(timer, "OnResultExecuted", "ResultFilter"));
            }

            public class Message
            {
                public Message(ResultExecutedContext arguments)
                {
                    Canceled = arguments.Canceled;
                    ExceptionType = arguments.Exception == null ? null : arguments.Exception.GetType();
                    ExceptionHandled = arguments.ExceptionHandled;
                    ResultType = arguments.Result == null ? null : arguments.Result.GetType();
                }

                public Type ResultType { get; set; }

                public bool ExceptionHandled { get; set; }

                public Type ExceptionType { get; set; }

                public bool Canceled { get; set; }
            }
        }
    }
}