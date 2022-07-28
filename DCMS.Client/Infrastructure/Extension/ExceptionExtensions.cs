using System.Collections.Generic;
using System.Linq;

namespace Wesley.Client
{

    public static class ExceptionExtensions
    {
        public static IEnumerable<System.Exception> GetAllExceptions(this System.Exception exception)
        {
            yield return exception;

            if (exception is System.AggregateException)
            {
                var aggrEx = exception as System.AggregateException;
                foreach (System.Exception innerEx in aggrEx.InnerExceptions.SelectMany(e => e.GetAllExceptions()))
                {
                    yield return innerEx;
                }
            }
            else if (exception.InnerException != null)
            {
                foreach (System.Exception innerEx in exception.InnerException.GetAllExceptions())
                {
                    yield return innerEx;
                }
            }
        }

        public static string ToFormattedString(this System.Exception exception)
        {
            IEnumerable<string> messages = exception
                .GetAllExceptions()
                .Where(e => !System.String.IsNullOrWhiteSpace(e.Message))
                .Select(exceptionPart => exceptionPart.Message.Trim() + "\r\n" + (exceptionPart.StackTrace != null ? exceptionPart.StackTrace.Trim() : ""));
            string flattened = System.String.Join("\r\n\r\n", messages); // <-- the separator here
            return flattened;
        }
    }
}
