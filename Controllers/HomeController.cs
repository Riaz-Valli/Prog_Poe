using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private static CustomLinkedList _customLinkedList = new CustomLinkedList();

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Report()
    {
        return View();
    }

    public IActionResult Feedback()
    {
        return View();
    }

    public IActionResult AllReports()
    {
        // Fetch reports from the custom linked list
        var reports = _customLinkedList.GetAllReports();
        return View(reports);
    }

    [HttpPost]
    public IActionResult SubmitReport(IFormFile attachment, string location, string category, string description)
    {
        // Create a new report
        var report = new Report
        {
            Location = location,
            Category = category,
            Description = description
        };

        // If an attachment is provided, save the file and set the attachment URL
        if (attachment != null && attachment.Length > 0)
        {
            // Get the path for the uploads directory
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            // Check if the uploads directory exists, if not create it
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Save the file
            var fileName = Path.GetFileName(attachment.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                attachment.CopyTo(stream);
            }

            // Set the attachment URL
            report.Attachment = $"/uploads/{fileName}";
        }
        else
        {
            // If no attachment is provided, set the Attachment property to null or an empty string
            report.Attachment = null;
        }

        // Add the report to the custom linked list
        _customLinkedList.Add(report);

        // Redirect to the Report view
        return RedirectToAction("Report");
    }
}