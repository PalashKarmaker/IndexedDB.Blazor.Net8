﻿using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace IndexedDB.Blazor
{
    public class IndexedSet<T> : ICollection<T>, IEnumerable<T> where T : new()
    {
        /// <summary>
        /// The internal stored items
        /// </summary>
        private readonly List<IndexedEntity<T>> internalItems;
        /// <summary>
        /// The type T primary key, only != null if at least once requested by remove
        /// </summary>
        private readonly PropertyInfo primaryKey;

        // ToDo: Remove PK dependency
        public IndexedSet(IEnumerable<T> records, PropertyInfo primaryKey)
        {
            this.primaryKey = primaryKey;
            internalItems = [];

            if (records == null)
                return;

            Debug.WriteLine($"{nameof(IndexedEntity)} - Construct - Add records");

            foreach (var item in records)
                internalItems.Add(new(item) { State = EntityState.Unchanged });

            Debug.WriteLine($"{nameof(IndexedEntity)} - Construct - Add records DONE");
        }

        public bool IsReadOnly => false;

        public int Count => internalItems.Count;

        public void Add(T item)
        {
            if (!internalItems.Select(x => x.Instance).Contains(item))
            {
                Debug.WriteLine($"{nameof(IndexedEntity)} - Added item of type {typeof(T).Name}");

                internalItems.Add(new(item) { State = EntityState.Added });
            }
        }

        public void Clear()
        {
            //modified
            foreach (var item in internalItems)
                item.State = EntityState.Deleted;
        }

        public bool Contains(T item) => Enumerable.Contains(this, item);

        public bool Remove(T item)
        {
            var internalItem = internalItems.FirstOrDefault(x => x.Instance.Equals(item));

            if (internalItem != null)
            {
                internalItem.State = EntityState.Deleted;
                return true;
            }
            // If reference was lost search for pk, increases the required time
            else
            {
                Debug.WriteLine("Searching for equality with PK");

                var value = primaryKey.GetValue(item);

                internalItem = internalItems.FirstOrDefault(x => primaryKey.GetValue(x.Instance).Equals(value));

                if (internalItem != null)
                {
                    Debug.WriteLine($"Found item with id {value}");

                    internalItem.State = EntityState.Deleted;

                    return true;
                }
            }

            Debug.WriteLine("Could not find internal stored item");
            return false;
        }

        public IEnumerator<T> GetEnumerator() => internalItems.Select(x => x.Instance).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        // ToDo: replace change tracker with better alternative 
        internal IEnumerable<IndexedEntity> GetChanged()
        {
            foreach (var item in this.internalItems)
            {
                item.DetectChanges();

                if (item.State == EntityState.Unchanged)
                    continue;

                Debug.WriteLine("Item yield");
                yield return item;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Not enough elements after arrayIndex in the destination array.");
            for (int i = 0; i < Count; ++i)
                array[i + arrayIndex] = this.internalItems[i].Instance;
        }
    }
}
