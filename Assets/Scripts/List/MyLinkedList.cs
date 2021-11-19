using System;
using System.Collections.Generic;
using UnityEngine;

public class MyLinkedList<T>
{
    private int count;
    private ListNode<T> first;
    private ListNode<T> last;

    public int Count { get => count; }
    internal ListNode<T> First { get => first; }
    internal ListNode<T> Last { get => last; }

    public MyLinkedList()
    {
        count = 0;
        first = null;
        last = null;
    }
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < count; ++i)
            yield return this[i];
    }
    public void AddLast(T value)
    {
        if (count==0)
        {
            ListNode<T> newNode = new ListNode<T>(value);
            first = newNode;
            last = newNode;
        }
        else
        {
            last = new ListNode<T>(value, last);
        }

        count++;
    }
    public void AddFirst(T value)
    {
        if (count == 0)
        {
            ListNode<T> newNode = new ListNode<T>(value);
            first = newNode;
            last = newNode;
        }
        else
        {
            first = new ListNode<T>(value, null, first);
        }
        count++;
    }
    public void AddAfter(T afterThisValue, T newValue)
    {
        if (count == 0 || afterThisValue == null)
            return;
        if (afterThisValue.Equals(first.Value))
        {
            new ListNode<T>(newValue, first, first.Next);
            count++;
            return;
        }
        ListNode<T> currentNode = first;
        for (int i = 0; i < count -2; i++, currentNode = currentNode.Next)
        {
            if (currentNode.Next.Value.Equals(afterThisValue))
            {
                new ListNode<T>(newValue, currentNode.Next, currentNode.Next.Next);
                count++;
                return;
            }
        }
        if (afterThisValue.Equals(last.Value))
        {
            AddLast(newValue);
        }
    }
    public void AddBefore(T beforeThisValue, T newValue)
    {
        if (count == 0 || beforeThisValue == null)
            return;
        if (beforeThisValue.Equals(first.Value))
        {
            first = new ListNode<T>(newValue, null, first);
            count++;
            return;
        }
        else
        {
            ListNode<T> currentNode = first;
            for (int i = 0; i < count - 1; i++, currentNode = currentNode.Next)
            {
                if (currentNode.Next.Value.Equals(beforeThisValue))
                {
                    new ListNode<T>(newValue, currentNode, currentNode.Next);
                    count++;
                    return;
                }
            }
        }
    }
    public void Remove(T value)
    {
        if (count == 0 || value == null)
            return;
        if (first.Value.Equals(value))
        {
            first = first.Next;
            count--;
        }
        else
        {
            ListNode<T> currentNode = first;
            for (int i = 0; i < count; i++, currentNode = currentNode.Next)
            {
                if (currentNode.Next.Value.Equals(value))
                {
                    currentNode.Next = currentNode.Next.Next;
                    if (value.Equals(last.Value))
                        last = currentNode;
                    count--;
                    break;
                }
            }
        }
    }
    public void RemoveFirst()
    {
        if (count > 1)
        {
            first = first.Next;
            count--;
        }
        else if (count ==1)
        {
            first = null;
            last = null;
            count = 0;
        }
    }
    public void RemoveLast()
    {
        if (count > 1)
        {
            ListNode<T> currentNode = first;
            for (int i = 0; i < count - 1; i++, currentNode = currentNode.Next)
            {
                if (currentNode.Next.Equals(last))
                {
                    currentNode.Next = null;
                    last = currentNode;
                    count--;
                    return;
                }
            }
        }
        else if (count == 1)
        {
            first = null;
            last = null;
            count = 0;
        }
    }
    public void PrintAll()
    {
        if (count == 0)
            return;

        ListNode<T> currentNode = first;
        for (int i = 0; i < count; i++, currentNode = currentNode.Next)
        {
            Debug.Log($"{i}: {currentNode}");
        }
    }
    public ListNode<T> Find(T value)
    {
        if (count == 0 || value == null)
            return null;

        ListNode<T> currentNode = first;
        for (int i = 0; i < count; i++, currentNode = currentNode.Next)
        {
            if (currentNode.Value.Equals(value))
            {
                return currentNode;
            }
        }

        return null;
    }
    public int IndexOf(T value)
    {
        if (count == 0 || value == null)
            return -1;

        ListNode<T> currentNode = first;
        for (int i = 0; i < count; i++, currentNode = currentNode.Next)
        {
            if (currentNode.Value.Equals(value))
            {
                return i;
            }
        }

        return -1;
    }
    public bool Contains(T value)
    {
        return Find(value) != null;
    }
    public void Clear()
    {
        first = null;
        last = null;
        count = 0;
    }
    public void CopyTo(T[] array, int startIndex = 0, int length = -1)
    {
        if (length == -1) length = count;

        ListNode<T> currentNode = first;
        int copyCount = 0;
        for (int i = 0; i < count && currentNode != null && copyCount < length; i++, currentNode = currentNode.Next)
        {
            if (i >= startIndex)
            {
                array[i - startIndex] = currentNode.Value;
                copyCount++;
            }
           
        }
    }
    public void CopyTo(MyLinkedList<T> array, int startIndex = 0, int length = -1)
    {
        if (length == -1) length = count;

        ListNode<T> currentNode = first;
        int copyCount = 0;
        for (int i = 0; i < count && currentNode != null && copyCount < length; i++, currentNode = currentNode.Next)
        {
            if (i >= startIndex)
            {
                array[i - startIndex] = currentNode.Value;
                copyCount++;
            }

        }
    }
    public T this[int index]
    {
        get
        {
            if (count == 0 || index < 0 || index >= count)
            {
                return default;
            }
            T value = default;
            if (index == 0)
                return first.Value;
            else
            {
                ListNode<T> current = first;
                for (int i = 0; i < count; i++, current = current.Next)
                {
                    if (i == index)
                    {
                        value = current.Value;
                        break;
                    }
                }
            }
            return value;
        }
        set
        {
            if (index >=0 && index < count)
            {
                ListNode<T> current = first;
                for (int i = 0; i < count; i++, current = current.Next)
                {
                    if (i == index)
                    {
                        current.Value = value;
                        break;
                    }
                }
            }
        }
    }
}
