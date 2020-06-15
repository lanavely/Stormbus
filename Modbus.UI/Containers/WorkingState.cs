using Stormbus.UI.Enums;

namespace Stormbus.UI.Containers
{
    public class WorkingState : NotifyPropertyChanged
    {
        private readonly StatusBar _statusBar;
        private WorkingStates _currentState;

        public WorkingState(StatusBar statusBar)
        {
            _statusBar = statusBar;
        }

        public WorkingStates CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                switch (value)
                {
                    case WorkingStates.Work:
                        _statusBar.SetWorkingState();
                        break;
                    case WorkingStates.Stopped:
                        _statusBar.SetStoppedState();
                        break;
                }
            }
        }
    }
}