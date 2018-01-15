using System;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;

namespace TextRangeFromListItem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var doc = new FlowDocument();
            doc.Blocks.Add(new List(new ListItem(new Paragraph(new Run("first bullet")))));

            var range1 = new TextRange(doc.ContentStart, doc.ContentEnd);

            var textRangeBase = typeof(TextRange).Assembly.GetType("System.Windows.Documents.TextRangeBase");
            var getTextInternal = textRangeBase.GetMethod("GetTextInternal", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TextPointer), typeof(TextPointer) }, null);

            // as expected
            var text1 = getTextInternal.Invoke(null, new[] { range1.Start, range1.End });

            // as seen with "Text" property
            var text2 = getTextInternal.Invoke(null, new[] { range1.Start
                .GetNextContextPosition(LogicalDirection.Backward)
                .GetNextContextPosition(LogicalDirection.Backward)
                .GetNextContextPosition(LogicalDirection.Backward)
                , range1.End });

        }
    }
}
