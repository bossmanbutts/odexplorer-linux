using System;
using System.Threading.Tasks;

namespace ODExplorer.Models
{
    public static class MessageBoxRequester
    {
        public static event EventHandler<MessageBoxEventArgsAsync>? Requested;

        public static void Request(MessageBoxEventArgsAsync args)
        {
            Requested?.Invoke(null, args);
        }
    }
}
