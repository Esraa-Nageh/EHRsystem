using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHRsystem.Controllers
{
    public class MedicalRecordController : Controller
    {
        [HttpGet]
        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // TODO: Save file to wwwroot/uploads or database
                ViewBag.Message = "File uploaded successfully.";
            }
            else
            {
                ViewBag.Message = "No file selected.";
            }

            return View();
        }

        public IActionResult ViewFiles()
        {
            // TODO: Fetch list of files from storage or DB
            return View(); // Pass model if needed
        }
    }
}
