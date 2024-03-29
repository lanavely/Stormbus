﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Stormbus.UI.Enums;

namespace Stormbus.UI.CustomUserControls
{
    /// <summary>
    ///     Interaction logic for DataTypeMenu.xaml
    /// </summary>
    public partial class DataTypeMenu : INotifyPropertyChanged
    {
        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register(
            nameof(DataType), typeof(DataType), typeof(DataTypeMenu),
            new FrameworkPropertyMetadata(default(DataType), DataTypePropertyChangedCallback));

        public static readonly DependencyProperty BytesEndianProperty = DependencyProperty.Register(
            nameof(BytesEndian), typeof(EndianType), typeof(DataTypeMenu), new PropertyMetadata(default(EndianType)));

        public static readonly DependencyProperty NumberSystemProperty = DependencyProperty.Register(
            nameof(NumberSystem), typeof(NumberSystem), typeof(DataTypeMenu),
            new PropertyMetadata(default(NumberSystem)));

        public static readonly DependencyProperty FunctionProperty = DependencyProperty.Register(
            nameof(Function), typeof(byte), typeof(DataTypeMenu), new PropertyMetadata(default(byte)));

        public DataTypeMenu()
        {
            InitializeComponent();
        }

        public DataType DataType
        {
            get => (DataType) GetValue(DataTypeProperty);
            set => SetValue(DataTypeProperty, value);
        }

        public byte Function
        {
            get => (byte) GetValue(FunctionProperty);
            set => SetValue(FunctionProperty, value);
        }

        public EndianType BytesEndian
        {
            get => (EndianType) GetValue(BytesEndianProperty);
            set => SetValue(BytesEndianProperty, value);
        }

        public NumberSystem NumberSystem
        {
            get => (NumberSystem) GetValue(NumberSystemProperty);
            set => SetValue(NumberSystemProperty, value);
        }

        public int DataTypeLength
        {
            get
            {
                return DataType switch
                {
                    DataType.Short => 16,
                    DataType.UShort => 16,
                    DataType.Int => 32,
                    DataType.UInt => 32,
                    DataType.Float => 32,
                    DataType.Long => 64,
                    DataType.Double => 64,
                    _ => -1
                };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void DataTypePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataTypeMenu dataTypeMenu)
            {
                //dataTypeMenu.PropertyChanged.Invoke(dataTypeMenu, new PropertyChangedEventArgs(nameof(DataTypeLength))); TODO: implement endian settings later
            }
        }

        private void RadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                if (radioButton.Tag is DataType dataType)
                {
                    DataType = dataType;
                    if (DataType != DataType.UShort) NumberSystem = NumberSystem.Decimal;
                    return;
                }

                if (radioButton.Tag is NumberSystem numberSystem)
                {
                    NumberSystem = numberSystem;
                    if (numberSystem != NumberSystem.Decimal) DataType = DataType.UShort;
                    return;
                }

                if (radioButton.Tag is EndianType endian) BytesEndian = endian;
            }
        }
    }
}