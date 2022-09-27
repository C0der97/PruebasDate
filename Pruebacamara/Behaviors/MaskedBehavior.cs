using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xamarin.Forms;


namespace Pruebacamara.Behaviors
{
    public class MaskedBehavior : Behavior<Entry>
    {
        private string _mask = "";
        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;
                SetPositions();
            }
        }

        Stack<int> pos = new Stack<int>();

        private int CounSeparators { get; set; }
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        IDictionary<int, char> _positions;

        private void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positions = null;
                return;
            }

            Dictionary<int, char> list = new Dictionary<int, char>();
            for (int i = 0; i < Mask.Length; i++)
            {
                if (Mask[i] != '_')
                {
                    list.Add(i, Mask[i]);
                }
            }

            _positions = list;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            Entry entry = sender as Entry;
            string text = entry.Text;
            int cursor = entry.CursorPosition;
            bool erasingData = CheckErasingEntry(args);

            if (string.IsNullOrWhiteSpace(text) || _positions == null)
            {
                return;
            }

            if (text.Length > _mask?.Length)
            {
                entry.Text = args.OldTextValue;
            }

            const string separator = "/";

            int separatorCount = Regex.Matches(text, separator).Count;

            if (!erasingData)
            {
                text = AddSeparatorWriting(text);
            }
            else
            {
                pos.Push(cursor);
                text = text.Replace(separator, string.Empty);
                foreach (KeyValuePair<int, char> item in _positions)
                {
                    if (text.Length >= item.Key + 1)
                    {
                        text = text.Insert(item.Key, item.Value.ToString());
                    }
                }
            }


            if (entry.Text != text)
            {
                entry.Text = text;
            }

        }

        private bool CheckErasingEntry(TextChangedEventArgs eventData)
        {
            return eventData.NewTextValue?.Length
                < eventData.OldTextValue?.Length;
        }

        private string AddSeparatorWriting(string text)
        {

            const string separator = "/";

            text = text.Replace(separator, string.Empty);

            // 12
            const string patternTwoNumbers = @"^\d{2}";
            // 12/12/
            const string patternFourNumbers = @"^\d{2}/\d{2}";

            int separatorCount = Regex.Matches(text, separator).Count;


            if (Regex.IsMatch(text, patternTwoNumbers, RegexOptions.IgnoreCase) && separatorCount == 0)
            {
                text = text.Insert(2, separator);
            }

            separatorCount = Regex.Matches(text, separator).Count;

            if (Regex.IsMatch(text, patternFourNumbers, RegexOptions.IgnoreCase) && separatorCount == 1)
            {
                text = text.Insert(5, separator);
            }

            return text;
        }
    }
}
