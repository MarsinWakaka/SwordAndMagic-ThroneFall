namespace MyFramework.DataStructure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 泛型优先队列（最小堆实现）
    /// 要求：类型T必须实现IComparable<T>接口
    /// 时间复杂度：Enqueue O(log n), Dequeue O(log n), Peek O(1)
    /// </summary>
    /// <typeparam name="T">队列元素类型</typeparam>
    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        // 默认容量
        private const int DefaultCapacity = 4;

        // 内部堆存储结构
        private T[] _heap;
        private int _count;

        /// <summary>当前队列元素数量</summary>
        public int Count => _count;

        /// <summary>队列是否为空</summary>
        public bool IsEmpty => _count == 0;

        public PriorityQueue() : this(DefaultCapacity)
        {
        }

        public PriorityQueue(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            _heap = new T[capacity];
        }

        public PriorityQueue(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection is ICollection<T> c)
            {
                _heap = new T[c.Count];
                c.CopyTo(_heap, 0);
                _count = c.Count;
            }
            else
            {
                _heap = new T[DefaultCapacity];
                foreach (var item in collection)
                    Enqueue(item);
            }

            Heapify();
        }

        /// <summary>添加元素到队列</summary>
        public void Enqueue(T item)
        {
            if (_count == _heap.Length)
                Grow(_count + 1);

            _heap[_count] = item;
            SiftUp(_count);
            _count++;
        }

        /// <summary>移除并返回优先级最高的元素</summary>
        public T Dequeue()
        {
            if (_count == 0)
                throw new InvalidOperationException("Queue is empty");

            T result = _heap[0];
            _count--;
            if (_count > 0)
            {
                _heap[0] = _heap[_count];
                SiftDown(0);
            }

            _heap[_count] = default; // 清除引用
            return result;
        }

        /// <summary>查看优先级最高的元素（不移除）</summary>
        public T Peek()
        {
            if (_count == 0)
                throw new InvalidOperationException("Queue is empty");
            return _heap[0];
        }

        /// <summary>清空队列</summary>
        public void Clear()
        {
            Array.Clear(_heap, 0, _count);
            _count = 0;
        }

        /// <summary>调整内部堆结构</summary>
        private void Heapify()
        {
            for (int i = (_count - 1) / 2; i >= 0; i--)
                SiftDown(i);
        }

        private void SiftUp(int index)
        {
            T item = _heap[index];
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (item.CompareTo(_heap[parent]) >= 0)
                    break;

                _heap[index] = _heap[parent];
                index = parent;
            }

            _heap[index] = item;
        }

        private void SiftDown(int index)
        {
            T item = _heap[index];
            int child;
            while ((child = 2 * index + 1) < _count)
            {
                // 选择较小的子节点
                if (child + 1 < _count && _heap[child + 1].CompareTo(_heap[child]) < 0)
                    child++;

                if (item.CompareTo(_heap[child]) <= 0)
                    break;

                _heap[index] = _heap[child];
                index = child;
            }

            _heap[index] = item;
        }

        private void Grow(int minCapacity)
        {
            int newCapacity = _heap.Length == 0 ? DefaultCapacity : 2 * _heap.Length;
            // if ((uint)newCapacity > Array.MaxLength) newCapacity = Array.MaxLength;
            if (newCapacity < minCapacity) newCapacity = minCapacity;

            Array.Resize(ref _heap, newCapacity);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return _heap[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>调试用：验证堆结构是否有效</summary>
        internal bool Validate()
        {
            for (int i = 1; i < _count; i++)
            {
                int parent = (i - 1) / 2;
                if (_heap[parent].CompareTo(_heap[i]) > 0)
                    return false;
            }

            return true;
        }
    }
}