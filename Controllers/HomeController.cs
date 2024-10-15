using Microsoft.AspNetCore.Mvc;
using Prop_Poe.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Linq;

public class HomeController : Controller
{
    private static CustomLinkedList _customLinkedList = new CustomLinkedList();
    private static Dictionary<string, EventModel> _eventDictionary = new Dictionary<string, EventModel>();
    private static Dictionary<string, AnnouncementModel> _announcementDictionary = new Dictionary<string, AnnouncementModel>();
    private static Dictionary<string, FeedbackModel> _feedbackDictionary = new Dictionary<string, FeedbackModel>();
    private static Dictionary<string, int> _categoryCount = new Dictionary<string, int>();

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

    public IActionResult DisplayFeedback()
    {
        var feedbackList = _feedbackDictionary.Values.ToList();
        return View(feedbackList);
    }

    [HttpPost]
    public IActionResult SubmitFeedback(FeedbackModel model)
    {
        if (ModelState.IsValid)
        {
            if (!_feedbackDictionary.ContainsKey(model.Email)) 
            {
                _feedbackDictionary[model.Email] = new FeedbackModel
                {
                    Name = model.Name,
                    Email = model.Email,
                    Feedback = model.Feedback 
                };

                TempData["SuccessMessage"] = "Feedback submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Feedback from this email already exists.";
            }
        }

        TempData["ErrorMessage"] = "Failed to submit feedback. Ensure all fields are filled.";
        return View(model);
    }

    public IActionResult Events(string searchCategory, DateTime? searchDate)
    {
        var eventDetailsList = _eventDictionary.Values.ToList();

        // Count selected categories
        if (!string.IsNullOrEmpty(searchCategory))
        {
            if (_categoryCount.ContainsKey(searchCategory))
            {
                _categoryCount[searchCategory]++;
            }
            else
            {
                _categoryCount[searchCategory] = 1;
            }

            // Filter by selected category
            eventDetailsList = eventDetailsList
                .Where(e => e.Category.Equals(searchCategory, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Filter by date
        if (searchDate.HasValue)
        {
            eventDetailsList = eventDetailsList
                .Where(e => e.EventDate.Date == searchDate.Value.Date)
                .ToList();
        }

        // Find the most frequently selected category
        var recommendedCategory = _categoryCount.OrderByDescending(x => x.Value).FirstOrDefault().Key;

        // Get recommended events from the most selected category
        var recommendedEvents = string.IsNullOrEmpty(recommendedCategory)
            ? new List<EventModel>()
            : _eventDictionary.Values
                .Where(e => e.Category.Equals(recommendedCategory, StringComparison.OrdinalIgnoreCase))
                .ToList();

        var uniqueCategories = new HashSet<string>(eventDetailsList.Select(e => e.Category));
        var uniqueDates = new HashSet<DateTime>(eventDetailsList.Select(e => e.EventDate.Date));

        ViewBag.UniqueCategories = uniqueCategories;
        ViewBag.UniqueDates = uniqueDates;

        // Pass announcements and recommended events to the view
        var announcements = _announcementDictionary.Values.ToList();
        var model = new Tuple<List<EventModel>, List<AnnouncementModel>>(eventDetailsList, announcements);
        ViewBag.RecommendedEvents = recommendedEvents; 
        return View(model);
    }

    [HttpGet]
    public IActionResult AddEvents()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddEvents(EventModel model)
    {
        if (ModelState.IsValid)
        {
            _eventDictionary[model.EventName] = model;
            TempData["SuccessMessage"] = "Event added successfully!";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Failed to add the event.";
        return View(model);
    }

    [HttpGet]
    public IActionResult AddAnnouncement()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddAnnouncement(AnnouncementModel model)
    {
        if (ModelState.IsValid)
        {
            if (!_announcementDictionary.ContainsKey(model.Name)) // Prevent duplicates
            {
                _announcementDictionary[model.Name] = new AnnouncementModel
                {
                    Name = model.Name,
                    Description = model.Description,
                    DatePosted = DateTime.Now
                };

                TempData["SuccessMessage"] = "Announcement added successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "An announcement with this name already exists.";
            }
        }

        TempData["ErrorMessage"] = "Failed to add the announcement. Ensure all fields are filled.";
        return View(model);
    }

    public IActionResult AllReports()
    {
        var reports = _customLinkedList.GetAllReports();
        return View(reports);
    }

    [HttpPost]
    public IActionResult SubmitReport(IFormFile attachment, string location, string category, string description)
    {
        if (string.IsNullOrEmpty(location) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(description))
        {
            TempData["ErrorMessage"] = "All fields are required!";
            return RedirectToAction(nameof(Report));
        }

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

            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["ErrorMessage"] = "Invalid file type. Only .jpg, .jpeg, .png, and .pdf are allowed.";
                return RedirectToAction(nameof(Report));
            }

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var fileName = Path.GetFileName(attachment.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                attachment.CopyTo(stream);
            }

            report.Attachment = $"/uploads/{fileName}";
        }

        _customLinkedList.Add(report);
        TempData["SuccessMessage"] = "Report submitted successfully!";
        return RedirectToAction(nameof(Report));
    }
}
