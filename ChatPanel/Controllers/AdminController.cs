using ChatPanel.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatPanel.Controllers
{
    public class AdminController : Controller
    {
        private readonly FirebaseService _firebase;
        private readonly IConfiguration _config;

        public AdminController(FirebaseService firebase, IConfiguration config)
        {
            _firebase = firebase;
            _config = config;
        }

        private bool IsAuthenticated =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUser"));

        public IActionResult Index()
        {
            if (!IsAuthenticated) return RedirectToAction("Login", "Account");

            // View'e Firebase JS SDK için gereken config'i geçir
            ViewBag.FirebaseApiKey = _config["Firebase:WebApiKey"];
            ViewBag.FirebaseAuthDomain = _config["Firebase:AuthDomain"];
            ViewBag.FirebaseDatabaseUrl = _config["Firebase:DatabaseUrl"];
            ViewBag.FirebaseProjectId = _config["Firebase:ProjectId"];
            ViewBag.AdminDisplayName = HttpContext.Session.GetString("AdminUser");

            return View();
        }

        [HttpGet]
        //public async Task<IActionResult> GetMessages(string roomId)
        //{
        //    if (!IsAuthenticated) return Unauthorized();

        //    var messages = await _firebase.GetMessagesAsync(roomId);
        //    return Json(messages);
        //}

        [HttpPost]
        public async Task<IActionResult> Approve(string roomId, string messageId)
        {
            if (!IsAuthenticated) return Unauthorized();

            var approvedBy = HttpContext.Session.GetString("AdminUser");
            try
            {
                await _firebase.ApproveMessageAsync(roomId, messageId, approvedBy);
                return Json(new { success = true, approvedBy });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string roomId, string messageId)
        {
            if (!IsAuthenticated) return Unauthorized();

            try
            {
                await _firebase.DeleteMessageAsync(roomId, messageId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
