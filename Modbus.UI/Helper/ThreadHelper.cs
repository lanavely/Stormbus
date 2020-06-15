using System;
using System.Windows.Threading;

namespace Stormbus.UI.Helper
{
    public static class ThreadHelper
    {
        private static Dispatcher _dispatcher;

        public static void SetDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public static void InvokeToMain(Action callback)
        {
            _dispatcher.Invoke(callback);
        }
    }
}