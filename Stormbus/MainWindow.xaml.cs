using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Stormbus.UI.Command.UI;
using Stormbus.UI.Helper;
using Stormbus.UI.Logger;
using Stormbus.UI.ViewModels;

namespace Stormbus.UI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public ViewModel ViewModel; 

        public MainWindow()
        {
            ThreadController.SetDispatcher(Dispatcher);
            StartStopReadCommand = new CommandDelegate {ExecuteHandler = StartStopRead};
            InitializeComponent();
            DataContext = ViewModel ??= new ViewModel();
            Console.SetOut(new StormbusTextWriter(LogTextBox));
        }

        public ICommand StartStopReadCommand { get; set; }

        private void StartStopRead(object arg)
        {
            switch (StartStopButton.Tag)
            {
                case @"Start":
                    ViewModel?.StartReadHandler();
                    break;
                case @"Stop":
                    ViewModel.StopReadHandler();
                    break;
            }
        }

        /// <summary>
        ///     Opens a window for executing the command
        /// </summary>
        private void DataGrid_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenCommandWindow();
        }

        private void OpenCommandWindow()
        {
            var selectedItems = new List<ResultItemModel>();
            foreach (var item in ResultItemsDataGrid.SelectedItems) selectedItems.Add((ResultItemModel) item);
            var commandWindow = new CommandWindow(selectedItems, ViewModel)
                {Owner = this};
            commandWindow.ShowDialog();
        }

        /// <summary>
        ///     Saves the configuration
        /// </summary>
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            ViewModel.ConfigurationSettings.Serialize(StormbusDirectory.ConfigurationFilePath);
        }

        private void DataGridMenu_SendCommandClick(object sender, RoutedEventArgs e)
        {
            OpenCommandWindow();
        }

        private void LoggingPanelVisibilityToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            Width += LogTextBox.Width;
            MinWidth += LogTextBox.Width;
        }

        private void LoggingPanelVisibilityToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            MinWidth -= LogTextBox.Width;
            Width -= LogTextBox.Width;
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel = new ViewModel();
            DataContext = ViewModel;
            Width = 300;
            MinWidth = 300;
            UpdateLayout();
        }
    }
}