using System.Text;

namespace FG.Diagnostics.AutoLogger.Model
{
    public class ListBuilder
    {
        protected readonly StringBuilder Builder = new StringBuilder();
        protected readonly string Delimiter;
        protected string CurrentDelimiter;

        public ListBuilder(string initialContent = "", string delimiter = "", string initialDelimiter = "")
        {
            Delimiter = delimiter;
            CurrentDelimiter = initialDelimiter;

            Builder.Append(initialContent);
        }

        public void Append(string argument)
        {
            Builder.Append($"{CurrentDelimiter}{argument}");
            CurrentDelimiter = Delimiter;
        }

        public int Length => Builder.Length;

        public override string ToString()
        {
            return Builder.ToString();
        }
    }
}