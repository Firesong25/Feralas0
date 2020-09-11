using System;

namespace Feralas
{
    public class JournalNotFoundException : Exception
    {
        public JournalNotFoundException()
        {
        }

        public JournalNotFoundException(string message)
            : base(message)
        {
        }

        public JournalNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
