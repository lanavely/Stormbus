using Stormbus.UI.Enums;

namespace Stormbus.UI.Containers
{
    public class WorkingState : NotifyPropertyChanged
    {
        private readonly StatusBar _statusBar;

        public WorkingState(StatusBar statusBar)
        {
            _statusBar = statusBar;
        }

        public WorkingStates CurrentState { get; set; }
    }
}