using System.Collections.Generic;
using System.Linq;

public class ServiceRequestHeap
{
    private List<ServiceRequestModel> heap = new List<ServiceRequestModel>();

    public void Add(ServiceRequestModel request)
    {
        heap.Add(request);
        HeapifyUp(heap.Count - 1);
    }

    public ServiceRequestModel ExtractMin()
    {
        if (heap.Count == 0) return null;
        var root = heap[0];
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(0);
        return root;
    }

    public List<ServiceRequestModel> GetAll()
    {
        return heap.OrderBy(r => r.SubmissionDate).ToList(); 
    }

    private void HeapifyUp(int index)
    {
        while (index > 0 && heap[index].SubmissionDate < heap[Parent(index)].SubmissionDate)
        {
            Swap(index, Parent(index));
            index = Parent(index);
        }
    }

    private void HeapifyDown(int index)
    {
        int smallest = index;
        int left = Left(index);
        int right = Right(index);

        if (left < heap.Count && heap[left].SubmissionDate < heap[smallest].SubmissionDate)
            smallest = left;
        if (right < heap.Count && heap[right].SubmissionDate < heap[smallest].SubmissionDate)
            smallest = right;
        if (smallest != index)
        {
            Swap(index, smallest);
            HeapifyDown(smallest);
        }
    }

    private void Swap(int i, int j)
    {
        var temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }

    private int Parent(int index) => (index - 1) / 2;
    private int Left(int index) => 2 * index + 1;
    private int Right(int index) => 2 * index + 2;
}
