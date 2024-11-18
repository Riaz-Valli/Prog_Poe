using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

public class ServiceRequestController : Controller
{
    private static readonly ServiceRequestHeap _serviceRequestHeap = new ServiceRequestHeap(); // Heap for priority-based requests
    private static readonly ServiceRequestTree _serviceRequestTree = new ServiceRequestTree(); // Tree for sorting by Id

    public IActionResult Request(string searchQuery)
    {
        var serviceRequests = _serviceRequestHeap.GetAll(); 
        if (!string.IsNullOrEmpty(searchQuery))
        {
            serviceRequests = serviceRequests
                .Where(r => r.RequesterName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                            r.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                            r.Category.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return View(serviceRequests);
    }

    public IActionResult Details(int id)
    {
        var serviceRequest = _serviceRequestTree.GetAll().FirstOrDefault(r => r.Id == id);
        if (serviceRequest != null)
        {
            return View(serviceRequest);
        }
        return NotFound();
    }

    public IActionResult SubmitRequest()
    {
        return View(new ServiceRequestModel());
    }

    [HttpPost]
    public IActionResult SubmitRequest(ServiceRequestModel request)
    {
        if (ModelState.IsValid)
        {
            // Process the file upload if a document is provided
            if (request.Document != null && request.Document.Length > 0)
            {
                // Define the file path
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", request.Document.FileName);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    request.Document.CopyTo(fileStream);
                }
            }

            request.Id = GenerateUniqueId(); 
            request.Status = "Pending";
            request.SubmissionDate = DateTime.Now;
            request.AssignedTo = "John Doe";

            // Add the request to both the heap and the tree
            _serviceRequestHeap.Add(request); // Add to heap for priority 
            _serviceRequestTree.Add(request); // Add to tree for sorting by Id

            return RedirectToAction("SubmitRequest"); 
        }

        return View(request); 
    }

    // Generate a unique ID for each service request
    private int GenerateUniqueId()
    {
        var allRequests = _serviceRequestTree.GetAll(); // Get all requests from the tree
        return allRequests.Any() ? allRequests.Max(r => r.Id) + 1 : 1; // Generate ID based on the max ID
    }

    [HttpPost]
    public IActionResult UpdateRequestStatus(int id, string newStatus, string searchQuery)
    {
        // Fetch the request using the tree or heap
        var request = _serviceRequestTree.Search(id);
        if (request != null)
        {
            request.Status = newStatus; // Update the status
            TempData["Message"] = $"Status updated for request ID {id} to {newStatus}.";
        }
        else
        {
            TempData["Error"] = $"Request ID {id} not found.";
        }
        return RedirectToAction("UpdateStatus", new { searchQuery });
    }


    public IActionResult UpdateStatus(string searchQuery)
    {
        var serviceRequests = _serviceRequestHeap.GetAll();
        if (!string.IsNullOrEmpty(searchQuery))
        {
            serviceRequests = serviceRequests
                .Where(r => r.RequesterName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                            r.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                            r.Category.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        var statusDictionary = new Dictionary<int, string>();
        foreach (var request in serviceRequests)
        {
            statusDictionary[request.Id] = request.Status;
        }

        ViewData["StatusDictionary"] = statusDictionary;

        return View(serviceRequests);
    }


}
