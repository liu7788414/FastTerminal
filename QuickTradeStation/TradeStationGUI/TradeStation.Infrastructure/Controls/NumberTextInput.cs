using System;
using System.Windows.Input;
using System.Windows.Controls;

namespace TradeStation.Infrastructure.Controls
{
    public class NumberTextInput : TextBox
    {
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0)
            {
                char lastChar = e.Text[e.Text.Length - 1];

                try
                {
                    var d = int.Parse(lastChar.ToString());
                }
                catch (Exception)
                {
                    e.Handled = true;
                }
            }

        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            //if (e.Key == Key.Back)
            //{
            //    if (this.Text.IndexOf('-') < 0)
            //    {
            //        minusExists = false;
            //    }

            //    if (this.Text.IndexOf('.') < 0)
            //    {
            //        dotExists = false;
            //    }
            //}

        }
    }
}
