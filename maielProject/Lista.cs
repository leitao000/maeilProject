using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maielProject
{
    class Lista<Row> : IList<Row>, IList
    {

        private Row[] _items;
        private const int _defaultCapacity = 4;
        [ContractPublicPropertyName("Count")]
        private int _size;
        static readonly Row[] _emptyArray = new Row[0];
        private const int MAXLENGTH = 0X7FEFFFFF;

        private int _version;

        public Lista()
        {
            _items = _emptyArray;
        }

        public Lista(int size)
        {
            _size = size;
        }

        public int Capacity
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return _items.Length;
            }
            set
            {
                if (value < _size)
                {
                    throw new ArgumentException("Index out of bound.");
                }


                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        Row[] newItems = new Row[value];
                        if (_size > 0)
                        {
                            Array.Copy(_items, 0, newItems, 0, _size);
                        }
                        _items = newItems;
                    }
                    else
                    {
                        _items = _emptyArray;
                    }
                }
            }
        }

        public Row this[int index]
        {
            get
            {
                // Following trick can reduce the range check by one
                if ((uint)index >= (uint)_size)
                {
                    throw new ArgumentException("Index out of bound.");
                }

                return _items[index];
            }

            set
            {
                if ((uint)index >= (uint)_size)
                {
                    throw new ArgumentException("Index out of bound.");
                }

                _items[index] = value;
                _version++;
            }
        }
        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return _size;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public void Add(Row item)
        {
            if (_size == _items.Length) EnsureCapacity(_size + 1);
            _items[_size++] = item;
            _version++;
        }

        int System.Collections.IList.Add(Object item)
        {
            try
            {
                Add((Row)item);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Index out of bound.");
            }

            return Count - 1;
        }

        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
                _size = 0;
            }
            _version++;
        }

        public bool Contains(Row item)
        {
            if ((Object)item == null)
            {
                for (int i = 0; i < _size; i++)
                    if ((Object)_items[i] == null)
                        return true;
                return false;
            }
            else
            {
                EqualityComparer<Row> c = EqualityComparer<Row>.Default;
                for (int i = 0; i < _size; i++)
                {
                    if (c.Equals(_items[i], item)) return true;
                }
                return false;
            }
        }

        bool System.Collections.IList.Contains(Object item)
        {
            if (IsCompatibleObject(item))
            {
                return Contains((Row)item);
            }
            return false;
        }

        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int newCapacity = _items.Length == 0 ? _defaultCapacity : _items.Length * 2;
                // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                if ((uint)newCapacity > MAXLENGTH) newCapacity = MAXLENGTH;
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }

        public Row Find(Predicate<Row> match)
        {
            if (match == null)
            {
                throw new ArgumentException("Index out of bound.");
            }


            for (int i = 0; i < _size; i++)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }
            return default(Row);
        }

        public Row FindByIndex(int index)
        {
            if (index < 0 || index > _size)
            {
                throw new ArgumentException("Index out of bound.");
            }

            return _items[index];
        }


        public int IndexOf(Row item)
        {
            Contract.Ensures(Contract.Result<int>() >= -1);
            Contract.Ensures(Contract.Result<int>() < Count);
            return Array.IndexOf(_items, item, 0, _size);
        }

        int System.Collections.IList.IndexOf(Object item)
        {
            if (IsCompatibleObject(item))
            {
                return IndexOf((Row)item);
            }
            return -1;
        }

        public void Insert(int index, Row item)
        {
            // Note that insertions at the end are legal.
            if ((uint)index > (uint)_size)
            {
                throw new ArgumentException("Index out of bound.");
            }

            if (_size == _items.Length) EnsureCapacity(_size + 1);
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }
            _items[index] = item;
            _size++;
            _version++;
        }

        void System.Collections.IList.Insert(int index, Object item)
        {
            try
            {
                Insert(index, (Row)item);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Index out of bound.");
            }
        }

        public bool Remove(Row item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        void System.Collections.IList.Remove(Object item)
        {
            if (IsCompatibleObject(item))
            {
                Remove((Row)item);
            }
        }

        private static bool IsCompatibleObject(object value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>. 
            return ((value is Row) || (value == null && default(Row) == null));
        }

        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_size)
            {
                throw new ArgumentException("Index out of bound.");
            }

            _size--;
            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
            _items[_size] = default(Row);
            _version++;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void CopyTo(Row[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Row> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        [Serializable]
        public struct Enumerator : IEnumerator<Row>, System.Collections.IEnumerator
        {
            private Lista<Row> list;
            private int index;
            private int version;
            private Row current;

            internal Enumerator(Lista<Row> list)
            {
                this.list = list;
                index = 0;
                version = list._version;
                current = default(Row);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {

                Lista<Row> localList = list;

                if (version == localList._version && ((uint)index < (uint)localList._size))
                {
                    current = localList._items[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (version != list._version)
                {
                    throw new ArgumentException("Index out of bound.");
                }

                index = list._size + 1;
                current = default(Row);
                return false;
            }

            public Row Current
            {
                get
                {
                    return current;
                }
            }

            Object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == list._size + 1)
                    {
                        throw new ArgumentException("Index out of bound.");
                    }
                    return Current;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                if (version != list._version)
                {
                    throw new ArgumentException("Index out of bound.");
                }

                index = 0;
                current = default(Row);
            }

        }
    }
}
