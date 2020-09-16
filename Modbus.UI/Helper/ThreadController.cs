using System;
using System.Windows.Threading;

namespace Stormbus.UI.Helper
{
    public static class ThreadController
    {
        public static Dispatcher Dispatcher;

        public static void SetDispatcher(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public static void InvokeToMain(Action callback)
        {
            Dispatcher.Invoke(callback);
        }
    }
}