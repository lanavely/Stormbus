using System;
using System.Threading;
using Stormbus.UI.Helper;

namespace Stormbus.UI.Logger
{
    public static class UserLogger
    {
        public static void WriteLine(string message)
        {
            ThreadController.InvokeToMain(() => { Console.WriteLine(message); });
        }

        public static void WriteLine(string message, CancellationTokenSource cancellationToken)
        {
            cancellationToken.Token.ThrowIfCancellationRequested();
            ThreadController.InvokeToMain(() => { Console.WriteLine(message); });
        }
    }
}