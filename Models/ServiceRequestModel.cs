public class ServiceRequestNode
{
    public ServiceRequestModel Data { get; set; }
    public ServiceRequestNode Left { get; set; }
    public ServiceRequestNode Right { get; set; }

    public ServiceRequestNode(ServiceRequestModel data)
    {
        Data = data;
        Left = null;
        Right = null;
    }
}

public class ServiceRequestModel
{
    public int Id { get; set; }
    public string RequesterName { get; set; }
    public string Description { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime SubmissionDate { get; set; }
    public string Category { get; set; }
    public string AssignedTo { get; set; } = "John Doe";
    public IFormFile Document { get; set; }
    /*  public string DocumentPath { get; set; }  */
}


