using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using Stormbus.UI.Helper;

namespace Stormbus.UI.Logger
{
    public class StormbusTextWriter : TextWriter
    {
        private readonly TextBox _textBox;

        public StormbusTextWriter(TextBox textBox)
        {
            _textBox = textBox;
        }


        public override Encoding Encoding => Encoding.UTF8;

        public override void WriteLine(string value)
        {
            ThreadController.InvokeToMain(() =>
            {
                _textBox.Select(_textBox.Text.Length + 1, 0);
                _textBox.AppendText("[" + DateTime.Now.TimeOfDay + "] > ");
                _textBox.AppendText(value);
                _textBox.AppendText(Environment.NewLine);
                _textBox.ScrollToEnd();
            });
        }
    }
}