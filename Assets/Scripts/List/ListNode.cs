using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ListNode <T>
{
    private T value;
    private ListNode<T> next;

    public T Value { get => value; set => this.value = value; }
    internal ListNode<T> Next { get => next; set => next = value; }

    public ListNode(T item=default, ListNode<T> lastNode = null, ListNode<T> nextNode = null)
    {
        this.value = item;
        if (lastNode != null)
            lastNode.next = this;
        if (nextNode != null)
            this.next = nextNode;
    }
    public override string ToString()
    {
        return value.ToString();
    }
}
