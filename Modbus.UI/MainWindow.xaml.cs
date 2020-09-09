using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
            ThreadHelper.SetDispatcher(Dispatcher);
            StartStopReadCommand = new WpfCommandDelegate {ExecuteHandler = StartStopRead};
            InitializeComponent();
            DataContext = ViewModel ??
                          (ViewModel = new ViewModel());
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
            if (sender is DataGrid dataGrid && dataGrid.SelectedItems is IList items)
            {
                var selectedItems = new List<ResultItemModel>();
                foreach (var item in items) selectedItems.Add((ResultItemModel) item);
                var commandWindow = new CommandWindow(selectedItems, ViewModel)
                    {Owner = this};
                commandWindow.ShowDialog();
            }
        }

        /// <summary>
        ///     Saves the configuration
        /// </summary>
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            ViewModel.ConfigurationSettings.Serialize(StormbusDirectory.ConfigurationFilePath);
        }
    }
}