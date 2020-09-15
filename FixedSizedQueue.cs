using System.Collections.Generic;
using System.Linq;

namespace GarouToremo
{
    class FixedSizedQueue<T> : Queue<T>
    {
        public int Limit { get; set; }
        public T Last { get; private set; }

        public FixedSizedQueue(int limit)
        {
            this.Limit = limit;
        }

        public new void Enqueue(T item)
        {
            Last = item;
            if(Count >= Limit)
            {
                Dequeue();
            }
            base.Enqueue(Last);
        }

    }
}
