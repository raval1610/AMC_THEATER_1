using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMC_THEATER_1.Models;



namespace AMC_THEATER_1.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: Department
        private readonly ApplicationDbContext _context; // Declare _context at the class level
        private ApplicationDbContext db = new ApplicationDbContext();
        public DepartmentController()
        {
            _context = new ApplicationDbContext(); // Ensure _context is initialized
        }
        public ActionResult Index()
        {
            return View();
        }
       


        // GET: Department



        public ActionResult DeptHomePage()
        {


            return View();
        }

        //public ActionResult ViewApplication()
        //{


        //    return View();
        //}
        public ActionResult ViewApplication(int? id)
        {
            string pageTitle = "Theater Registration";
            bool isEditPage = false;
            TRN_REGISTRATION registrationData = null;
            List<NO_OF_SCREENS> screens = new List<NO_OF_SCREENS>();
            List<TRN_THEATRE_DOCS> uploadedDocs = new List<TRN_THEATRE_DOCS>();

            if (id.HasValue)
            {
                registrationData = db.TRN_REGISTRATION.FirstOrDefault(r => r.T_ID == id.Value);
                if (registrationData == null)
                {
                    TempData["ErrorMessage"] = "No registration data found for the provided ID.";
                    return RedirectToAction("Index");
                }

                screens = db.NO_OF_SCREENS.Where(s => s.T_ID == id.Value).ToList();
                ViewBag.Upload = db.TRN_THEATRE_DOCS.Where(d => d.T_ID == id.Value && d.ACTIVE).ToList();
                pageTitle = "Edit Registration";
                isEditPage = true;
            }
            else
            {
                registrationData = new TRN_REGISTRATION();
            }

            var documents = db.MST_DOCS.Where(doc => doc.DOC_ACTIVE == true).ToList();
            if (!documents.Any())
            {
                ViewBag.Documents = new List<MST_DOCS>();
                TempData["ErrorMessage"] = "No active documents found. Please check the MST_DOCS table.";
                return View();
            }

            ViewBag.PageTitle = pageTitle;
            ViewBag.IsEditPage = isEditPage;
            ViewBag.Documents = documents;
            ViewBag.Screens = screens;
            ViewBag.UploadedDocs = uploadedDocs;  // Pass uploaded documents to View

            return View(registrationData);
        }




        // POST: TRN_REGISTRATION/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TRN_REGISTRATION model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CREATE_USER = "System";
                    model.CREATE_DATE = DateTime.Now;
                    model.T_ACTIVE = true;

                    db.TRN_REGISTRATION.Add(model);
                    db.SaveChanges();

                    var tId = model.T_ID;
                    if (tId == 0)
                    {
                        throw new Exception("The T_ID was not generated correctly.");
                    }

                    var screenData = Request.Form["screenData"];
                    if (!string.IsNullOrEmpty(screenData))
                    {
                        var screens = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScreenViewModel>>(screenData);
                        foreach (var screen in screens)
                        {
                            NO_OF_SCREENS screenEntity = new NO_OF_SCREENS
                            {
                                T_ID = tId,
                                AUDIENCE_CAPACITY = string.IsNullOrEmpty(screen.SeatCapacity) ? (int?)null : int.Parse(screen.SeatCapacity),

                                SCREEN_TYPE = screen.ScreenType
                            };
                            db.NO_OF_SCREENS.Add(screenEntity);
                        }
                    }

                    // Process document uploads
                    foreach (var doc in db.MST_DOCS.Where(d => d.DOC_ACTIVE == true))
                    {
                        var uploadedFile = Request.Files[doc.DOC_ID.ToString()];
                        if (uploadedFile != null && uploadedFile.ContentLength > 0)
                        {
                            string fileExtension = Path.GetExtension(uploadedFile.FileName);
                            string formattedFileName = $"TT_{doc.DOC_NAME}_{tId}{fileExtension}";
                            string path = Path.Combine(Server.MapPath("~/UploadedFiles"), formattedFileName);
                            uploadedFile.SaveAs(path);

                            TRN_THEATRE_DOCS theatreDoc = new TRN_THEATRE_DOCS
                            {
                                T_ID = tId,
                                DOC_ID = doc.DOC_ID,
                                DOC_FILEPATH = path,
                                CREATE_USER = "System",
                                CREATE_DATE = DateTime.Now,
                                ACTIVE = true
                            };
                            db.TRN_THEATRE_DOCS.Add(theatreDoc);
                        }
                        else
                        {
                            ModelState.AddModelError(doc.DOC_NAME, $"Please upload the {doc.DOC_NAME} document.");
                        }
                    }

                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Theatre registered successfully!";
                    return RedirectToAction("UserDashboard");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                }
            }

            ViewBag.Documents = db.MST_DOCS.Where(d => d.DOC_ACTIVE == true).ToList();
            return View(model);
        }
        //public ActionResult ActionRequest()
        //{
        //    return View();
        //}

        //public ActionResult ActionRequests()
        //{
        //    // Query to fetch all data from TRN_REGISTRATION
        //    var query = from tr in db.TRN_REGISTRATION
        //                where tr.T_ACTIVE == true
        //                select new
        //                {
        //                    tr.T_ID,
        //                    tr.T_NAME,
        //                    tr.T_OWNER_NAME
        //                };

        //    // Execute the query and convert to a list
        //    var result = query.ToList();
        //    Console.WriteLine("Total Theaters Found: " + result.Count);

        //    // Convert the result to a list of ViewModel objects
        //    var theaterList = result.Select(tr => new TRN_REGISTRATION
        //    {
        //        T_ID = tr.T_ID,
        //        T_NAME = tr.T_NAME,
        //        T_OWNER_NAME = tr.T_OWNER_NAME,
        //    }).ToList();

        //    return View(theaterList);  // Return the data to the view
        //}

        //public ActionResult PendingDuesDept()
        //{

        //    var theaters = (from tr in db.TRN_REGISTRATION
        //                    join pd in db.PENDINGDUEADMIN
        //                    on tr.T_ID equals pd.T_ID
        //                    select new TheaterDueViewModel
        //                    {
        //                        T_ID = tr.T_ID,
        //                        T_NAME = tr.T_NAME,
        //                        T_CITY = tr.T_CITY,
        //                        T_WARD = tr.T_WARD,
        //                        T_ZONE = tr.T_ZONE,
        //                        T_ADDRESS = tr.T_ADDRESS,
        //                        T_TENAMENT_NO = tr.T_TENAMENT_NO,
        //                        P_STATUS = pd.P_STATUS
        //                    }).ToList();

        //    return View(theaters);

        //}        
        
      
    }
}
