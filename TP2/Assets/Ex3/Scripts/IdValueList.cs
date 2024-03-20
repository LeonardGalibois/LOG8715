using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdValueList<T> : IEnumerable<T>
{
    List<uint> m_ids;

#nullable enable
    T?[] m_elements;

    public IdValueList()
    {
        m_ids       = new List<uint>();
        m_elements  = new T[0];
    }

    public bool ContainsKey(uint key)
    {
        return key < m_elements.Length && m_elements[key] != null;
    }

    public T? this[uint key]
    {
        get => m_elements[key];
        set
        {
            if (ContainsKey(key))
            {
                if (value is null) Remove(key);
                else m_elements[key] = value;
            }
            else Add(key, value);
        }
    }

    public T[] Values
    {
        get
        {
            T[] values = new T[m_ids.Count];
            int index = 0;

            foreach (uint id in m_ids)
            {
                values[index++] = m_elements[id];
            }

            return values;
        }
    }

    public void Remove(uint key)
    {
        if (m_ids.Remove(key))
        {
            m_elements[key] = default(T?);
        }
    }

    public void Add(uint key, T element)
    {
        if (key >= m_elements.Length)
        {
            ulong newSize = key >= 4 ? key * 2 : 4;

            System.Array.Resize(ref m_elements, newSize > int.MaxValue ? int.MaxValue : (int)newSize);
        }

        m_elements[key] = element;
        m_ids.Add(key);
    }

    public void Clear()
    {
        m_ids.Clear();
        m_elements = new T[0];
    }

    public IEnumerator<T> GetEnumerator()
    {
        int currentIndex = 0;
        
        while (currentIndex < m_ids.Count)
        {
            uint id = m_ids[currentIndex++];
            yield return m_elements[id];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        int currentIndex = 0;

        while (currentIndex < m_ids.Count)
        {
            uint id = m_ids[currentIndex++];
            yield return m_elements[id];
        }
    }
#nullable disable
}
