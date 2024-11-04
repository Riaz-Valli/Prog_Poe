public class ServiceRequestTree
{
    private ServiceRequestNode root;

    public void Add(ServiceRequestModel request)
    {
        root = AddRecursive(root, request);
    }

    private ServiceRequestNode AddRecursive(ServiceRequestNode node, ServiceRequestModel request)
    {
        if (node == null) return new ServiceRequestNode(request);

        if (request.Id < node.Data.Id)
            node.Left = AddRecursive(node.Left, request);
        else if (request.Id > node.Data.Id)
            node.Right = AddRecursive(node.Right, request);

        return node;
    }

    public ServiceRequestModel Search(int id)
    {
        return SearchRecursive(root, id);
    }

    private ServiceRequestModel SearchRecursive(ServiceRequestNode node, int id)
    {
        if (node == null || node.Data.Id == id)
            return node?.Data;

        return id < node.Data.Id
            ? SearchRecursive(node.Left, id)
            : SearchRecursive(node.Right, id);
    }

    public List<ServiceRequestModel> GetAll()
    {
        var result = new List<ServiceRequestModel>();
        InOrderTraversal(root, result);
        return result;
    }

    private void InOrderTraversal(ServiceRequestNode node, List<ServiceRequestModel> result)
    {
        if (node != null)
        {
            InOrderTraversal(node.Left, result);
            result.Add(node.Data);
            InOrderTraversal(node.Right, result);
        }
    }
}
