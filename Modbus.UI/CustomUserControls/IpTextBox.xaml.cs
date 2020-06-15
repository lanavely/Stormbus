using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Stormbus.UI.CustomUserControls
{
    /// <summary>
    ///     Interaction logic for IpTextBox.xaml
    /// </summary>
    public partial class IpTextBox
    {
        private static readonly List<Key> DigitKeys = new List<Key>
            {Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9};

        private static readonly List<Key> MoveForwardKeys = new List<Key> {Key.Right};
        private static readonly List<Key> MoveBackwardKeys = new List<Key> {Key.Left};
        private static readonly List<Key> OtherAllowedKeys = new List<Key> {Key.Tab, Key.Delete};

        public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
            nameof(Address), typeof(string), typeof(IpTextBox),
            new FrameworkPropertyMetadata(default(string), AddressChanged)
            {
                BindsTwoWayByDefault = true
            });

        private readonly List<TextBox> _segments;

        private bool _suppressAddressUpdate;

        public IpTextBox()
        {
            InitializeComponent();
            _segments = new List<TextBox> {FirstSegment, SecondSegment, ThirdSegment, LastSegment};
        }

        public string Address
        {
            get => (string) GetValue(AddressProperty);
            set => SetValue(AddressProperty, value);
        }

        private static void AddressChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var ipTextBox = dependencyObject as IpTextBox;

            if (e.NewValue is string text && ipTextBox != null)
            {
                ipTextBox._suppressAddressUpdate = true;
                var i = 0;
                foreach (var segment in text.Split('.'))
                {
                    ipTextBox._segments[i].Text = segment;
                    i++;
                }

                ipTextBox._suppressAddressUpdate = false;
            }
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DigitKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelDigitKeyPress();
                HandleDigitPress();
            }
            else if (MoveBackwardKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelBackwardKeyPress();
                HandleBackwardKeyPress();
            }
            else if (MoveForwardKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelForwardKeyPress();
                HandleForwardKeyPress();
            }
            else if (e.Key == Key.Back)
            {
                HandleBackspaceKeyPress();
            }
            else if (e.Key == Key.OemPeriod)
            {
                e.Handled = true;
                HandlePeriodKeyPress();
            }
            else if (e.Key == Key.Space)
            {
                e.Handled = true;
                HandleSpaceKeyPress();
            }
            else
            {
                e.Handled = !AreOtherAllowedKeysPressed(e);
            }
        }

        private bool AreOtherAllowedKeysPressed(KeyEventArgs e)
        {
            return (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0 &&
                   (e.Key == Key.C ||
                    e.Key == Key.V ||
                    e.Key == Key.A ||
                    e.Key == Key.X) ||
                   OtherAllowedKeys.Contains(e.Key);
        }

        private void HandleDigitPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.Text.Length == 3 &&
                currentTextBox.CaretIndex == 3 && currentTextBox.SelectedText.Length == 0)
                MoveFocusToNextSegment(currentTextBox);
        }

        private bool ShouldCancelDigitKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;
            return currentTextBox != null &&
                   currentTextBox.Text.Length == 3 &&
                   currentTextBox.CaretIndex == 3 &&
                   currentTextBox.SelectedText.Length == 0;
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_suppressAddressUpdate)
                Address = $"{FirstSegment.Text}.{SecondSegment.Text}.{ThirdSegment.Text}.{LastSegment.Text}";

            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.Text.Length == 3 && currentTextBox.CaretIndex == 3)
                MoveFocusToNextSegment(currentTextBox);
        }

        private bool ShouldCancelBackwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;
            return currentTextBox != null && currentTextBox.CaretIndex == 0;
        }

        private void HandleBackspaceKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.CaretIndex == 0 && currentTextBox.SelectedText.Length == 0)
                MoveFocusToPreviousSegment(currentTextBox);
        }

        private void HandleBackwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.CaretIndex == 0) MoveFocusToPreviousSegment(currentTextBox);
        }

        private bool ShouldCancelForwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;
            return currentTextBox != null && currentTextBox.CaretIndex == 3;
        }

        private void HandleForwardKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.CaretIndex == currentTextBox.Text.Length)
                MoveFocusToNextSegment(currentTextBox);
        }

        private void HandlePeriodKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.Text.Length > 0 &&
                currentTextBox.CaretIndex == currentTextBox.Text.Length) MoveFocusToNextSegment(currentTextBox);
        }

        private void HandleSpaceKeyPress()
        {
            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null) MoveFocusToNextSegment(currentTextBox);
        }

        private void MoveFocusToPreviousSegment(TextBox currentTextBox)
        {
            if (!ReferenceEquals(currentTextBox, FirstSegment))
            {
                var previousSegmentIndex = _segments.FindIndex(box => ReferenceEquals(box, currentTextBox)) - 1;
                currentTextBox.SelectionLength = 0;
                currentTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                _segments[previousSegmentIndex].CaretIndex = _segments[previousSegmentIndex].Text.Length;
            }
        }

        private void MoveFocusToNextSegment(TextBox currentTextBox)
        {
            if (!ReferenceEquals(currentTextBox, LastSegment))
            {
                currentTextBox.SelectionLength = 0;
                currentTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                var textBox = FocusManager.GetFocusedElement(this) as TextBox;
                textBox?.SelectAll();
            }
        }

        private void DataObject_OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText)
            {
                e.CancelCommand();
                return;
            }

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

            int num;

            if (!int.TryParse(text, out num)) e.CancelCommand();
        }
    }
}