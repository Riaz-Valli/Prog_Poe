using Microsoft.AspNetCore.Mvc;
using Prop_Poe.Models;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

public class HomeController : Controller
{
    private static CustomLinkedList _customLinkedList = new CustomLinkedList();
    private static Dictionary<string, EventModel> _eventDictionary = new Dictionary<string, EventModel>();

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

    public IActionResult Events(string searchCategory, DateTime? searchDate)
    {
        // Create a list to hold the event details
        var eventDetailsList = new List<EventModel>();

        // Check if the dictionary is not null and has entries
        if (_eventDictionary != null && _eventDictionary.Count > 0)
        {
            // Iterate over the key-value pairs in the dictionary
            foreach (var entry in _eventDictionary)
            {
                // Add the entry to the event details list
                eventDetailsList.Add(entry.Value);
            }

            // Filter
            if (!string.IsNullOrEmpty(searchCategory))
            {
                eventDetailsList = eventDetailsList
                    .Where(e => e.Category.Equals(searchCategory, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (searchDate.HasValue)
            {
                eventDetailsList = eventDetailsList
                    .Where(e => e.EventDate.Date == searchDate.Value.Date)
                    .ToList();
            }

            // Create sets for unique categories and dates
            var uniqueCategories = new HashSet<string>(eventDetailsList.Select(e => e.Category));
            var uniqueDates = new HashSet<DateTime>(eventDetailsList.Select(e => e.EventDate.Date));

            ViewBag.UniqueCategories = uniqueCategories;
            ViewBag.UniqueDates = uniqueDates;
            return View(eventDetailsList);
        }

        TempData["ErrorMessage"] = "No events found.";
        return View(eventDetailsList);
    }


    [HttpGet]
    public IActionResult AddEvents()
    {
        return View();
    }

    public IActionResult AddEvents(EventModel model)
    {
        if (ModelState.IsValid)
        {
            // Retrieve values from the model
            string eventName = model.EventName;
            string description = model.Description;
            DateTime eventDate = model.EventDate;
            string category = model.Category;

            // Create the EventModel object
            var eventModel = new EventModel
            {
                EventName = eventName,
                Description = description,
                EventDate = eventDate,
                Category = category
            };

            // Add the new event entry to the dictionary
            _eventDictionary[eventName] = eventModel; 

            var customHashtable = new CustomHashtable();
            foreach (var entry in _eventDictionary)
            {
                // Create composite keys
                string compositeKey = $"{entry.Value.Description}|{entry.Value.Category}@{entry.Value.EventDate}";
                customHashtable.Add(entry.Key, compositeKey);
            }

            // Store the updated hashtable back in session
            HttpContext.Session.SetObject("CustomHashtable", customHashtable);

            TempData["SuccessMessage"] = "Event added successfully!";
            return RedirectToAction("Index");
        }

        TempData["ErrorMessage"] = "Failed to add the event.";
        return View(model);
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
        // Ensure all required fields are provided
        if (string.IsNullOrEmpty(location) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(description))
        {
            TempData["ErrorMessage"] = "All fields are required!";
            return RedirectToAction("Report");
        }

        // Create a new report
        var report = new Report
        {
            Location = location,
            Category = category,
            Description = description
        };

        if (attachment != null && attachment.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var fileExtension = Path.GetExtension(attachment.FileName).ToLower();

            if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
            {
                TempData["ErrorMessage"] = "Invalid file type. Only .jpg, .jpeg, .png, and .pdf are allowed.";
                return RedirectToAction("Report");
            }

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
            // No attachment, set the Attachment property to null
            report.Attachment = null;
        }

        // Add the report to the custom linked list
        _customLinkedList.Add(report);

        // Redirect to the Report view with a success message
        TempData["SuccessMessage"] = "Report submitted successfully!";
        return RedirectToAction("Report");
    }
}
