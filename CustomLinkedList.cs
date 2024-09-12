// Topic : Custom Linked List
//Available at: https://www.geeksforgeeks.org/linked-list-implementation-in-c-sharp/ 

using System;
using System.Collections.Generic;

public class CustomLinkedList
{
    // The first node (head) of the linked list
    private Node head;

    // Method to add a report to the linked list
    public void Add(Report report)
    {
        Node newNode = new Node(report);

        if (head == null) // If the list is empty, the new node becomes the head
        {
            head = newNode;
        }
        else
        {
            // Traverse to the end of the list and add the new node
            Node current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }
    }

    // Method to retrieve all reports in the list
    public IEnumerable<Report> GetAllReports()
    {
        List<Report> reports = new List<Report>();
        Node current = head;

        while (current != null)
        {
            reports.Add(current.Data); // Add the current node's data (Report) to the list
            current = current.Next;    // Move to the next node
        }

        return reports;
    }

    //Node class for storing data and pointing to the next node
    private class Node
    {
        public Report Data { get; set; } // Data stored in the node
        public Node Next { get; set; }   // Reference to the next node

        public Node(Report data)
        {
            Data = data;
            Next = null;
        }
    }
}

public class Report
{
    public string Location { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Attachment { get; set; }
}
