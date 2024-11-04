using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class ServiceRequestController : Controller
{
    private static readonly ServiceRequestTree _serviceRequestTree = new ServiceRequestTree();
    public IActionResult Request()
    {
        var serviceRequests = _serviceRequestTree.GetAll();
        return View(serviceRequests);
    }

    public IActionResult Details(int id)
    {
        var serviceRequest = _serviceRequestTree.Search(id);
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
            request.Id = GenerateUniqueId(); 
            request.Status = "Pending"; 
            request.SubmissionDate = DateTime.Now; 
            _serviceRequestTree.Add(request); 
            return RedirectToAction("SubmitRequest"); 
        }
        return View(request); 
    }

    private int GenerateUniqueId()
    {
        var allRequests = _serviceRequestTree.GetAll();
        return allRequests.Any() ? allRequests.Max(r => r.Id) + 1 : 1;
    }
}
