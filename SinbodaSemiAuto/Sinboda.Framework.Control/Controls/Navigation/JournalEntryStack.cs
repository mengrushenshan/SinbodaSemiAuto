using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Control.Controls.Navigation
{
    internal class JournalEntryStackCountChangedEventArgs : EventArgs
    {
        private readonly int oldValue;
        public int OldValue { get { return oldValue; } }

        public JournalEntryStackCountChangedEventArgs(int oldValue)
        {
            this.oldValue = oldValue;
        }
    }

    internal class JournalEntryStack<T> : IEnumerable<T>
    {
        private const int _defaultCapacity = 5; // 默认容量
        private List<T> _array;
        private int _capcaity;

        public event EventHandler<JournalEntryStackCountChangedEventArgs> CountChanged;

        public int Count
        {
            get { return _array.Count; }
        }

        public JournalEntryStack() : this(_defaultCapacity)
        { }
        public JournalEntryStack(int capcaity)
        {
            _array = new List<T>(capcaity);
            _capcaity = capcaity;
        }

        public T Peek()
        {
            T result = _array[_array.Count - 1];
            return result;
        }

        public T Pop()
        {
            T result = _array[_array.Count - 1];
            _array.Remove(result);

            if (CountChanged != null)
                CountChanged(this, new JournalEntryStackCountChangedEventArgs(Count + 1));

            return result;
        }

        public void Push(T item)
        {
            if (_array.Count >= _capcaity)
                _array.RemoveAt(0);

            if (CountChanged != null)
                CountChanged(this, new JournalEntryStackCountChangedEventArgs(Count - 1));

            _array.Add(item);
        }

        public void Clear()
        {
            _array.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }
    }
}
