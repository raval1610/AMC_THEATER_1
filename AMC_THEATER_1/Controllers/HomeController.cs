using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AMC_THEATER_1.Models;
using static System.Web.Razor.Parser.SyntaxConstants;

namespace Amc_theater.Controllers
{
    public class HomeController : Controller
    {

        private THEATER_MODULEEntities db = new THEATER_MODULEEntities();

        private ApplicationDbContext db1 = new ApplicationDbContext(); // Database context
        private readonly ApplicationDbContext _context = new ApplicationDbContext(); // Ass
    


        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=THEATER_MODULE;Integrated Security=True;";


        public JsonResult GetTheaterSuggestions(string theaterId)
        {
            if (string.IsNullOrEmpty(theaterId))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var theater = db.TRN_REGISTRATION
                 .Where(t => t.T_ID.ToString().StartsWith(theaterId))
                 .Select(t => new { T_Name = t.T_NAME ?? "", T_Owner = t.T_OWNER_NAME ?? "" }) // Ensure values are not null
                 .ToList();


            return Json(theater, JsonRequestBehavior.AllowGet);
        }





        public ActionResult ActionRequests()
        {
            ViewBag.CurrentAction = "ActionRequests"; // This is important!

            var query = from tr in db.TRN_REGISTRATION
                        where tr.T_ACTIVE == true
                        select new

                        {
                            tr.T_ID,
                            tr.T_NAME,
                            tr.T_CITY,
                            tr.T_ADDRESS,
                            tr.T_TENAMENT_NO,
                            tr.T_ZONE,
                            tr.T_WARD,
                            tr.STATUS,
                            tr.REJECT_REASON,
                            tr.UPDATE_DATE,
                            tr.T_OWNER_NAME,

                            TheaterScreenCount = db.NO_OF_SCREENS.Count(s => s.T_ID == tr.T_ID && s.SCREEN_TYPE == "Theater"),
                            VideoTheaterScreenCount = db.NO_OF_SCREENS.Count(s => s.T_ID == tr.T_ID && s.SCREEN_TYPE == "Video")
                        };

            var result = query.ToList();

            var theaterList = result.Select(tr => new TheaterViewModel
            {
                T_ID = tr.T_ID,
                T_NAME = tr.T_NAME,
                T_CITY = tr.T_CITY,
                T_ADDRESS = tr.T_ADDRESS,
                T_TENAMENT_NO = tr.T_TENAMENT_NO,
                T_ZONE = tr.T_ZONE,
                T_WARD = tr.T_WARD,
                STATUS = tr.STATUS,
                T_OWNER_NAME = tr.T_OWNER_NAME,
                REJECT_REASON = tr.REJECT_REASON,
                UPDATE_DATE = tr.UPDATE_DATE ?? DateTime.MinValue,
                SCREEN_COUNT = tr.TheaterScreenCount + tr.VideoTheaterScreenCount,
                THEATER_SCREEN_COUNT = tr.TheaterScreenCount,
                VIDEO_THEATER_SCREEN_COUNT = tr.VideoTheaterScreenCount
            }).ToList();

            return View(theaterList);
        }

        public ActionResult List_of_Application()
        {
            ViewBag.CurrentAction = "List of Application"; // This is important!

            var query = from tr in db.TRN_REGISTRATION
                        where (tr.STATUS.Trim().ToLower() == "pending" || tr.STATUS.Trim().ToLower() == "rejected")
                              && tr.T_ACTIVE == true
                        select new
                        {
                            tr.T_ID,
                            tr.T_NAME,
                            tr.T_CITY,
                            tr.T_ADDRESS,
                            tr.T_TENAMENT_NO,
                            tr.T_ZONE,
                            tr.T_WARD,
                            tr.STATUS,
                            tr.REJECT_REASON,
                            tr.UPDATE_DATE,
                            tr.T_OWNER_NAME,
                            tr.T_COMMENCEMENT_DATE,

                            TheaterScreenCount = db.NO_OF_SCREENS.Count(s => s.T_ID == tr.T_ID && s.SCREEN_TYPE == "Theater"),
                            VideoTheaterScreenCount = db.NO_OF_SCREENS.Count(s => s.T_ID == tr.T_ID && s.SCREEN_TYPE == "Video")
                        };

            var result = query.ToList();

            var theaterList = result.Select(tr => new TheaterViewModel
            {
                T_ID = tr.T_ID,
                T_NAME = tr.T_NAME,
                T_CITY = tr.T_CITY,
                T_ADDRESS = tr.T_ADDRESS,
                T_TENAMENT_NO = tr.T_TENAMENT_NO,
                T_ZONE = tr.T_ZONE,
                T_WARD = tr.T_WARD,
                STATUS = tr.STATUS,
                T_OWNER_NAME = tr.T_OWNER_NAME,
                T_COMMENCEMENT_DATE = (DateTime)tr.T_COMMENCEMENT_DATE,
                REJECT_REASON = tr.REJECT_REASON,
                UPDATE_DATE = tr.UPDATE_DATE ?? DateTime.MinValue,
                SCREEN_COUNT = tr.TheaterScreenCount + tr.VideoTheaterScreenCount,
                THEATER_SCREEN_COUNT = tr.TheaterScreenCount,
                VIDEO_THEATER_SCREEN_COUNT = tr.VideoTheaterScreenCount
            }).ToList();

            return View(theaterList);
        }



        // GET: TRN_REGISTRATION
        public ActionResult Index()
        {
            return View(db.TRN_REGISTRATION.ToList());
        }

        // GET: TRN_REGISTRATION/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRN_REGISTRATION tRN_REGISTRATION = db.TRN_REGISTRATION.Find(id);
            if (tRN_REGISTRATION == null)
            {
                return HttpNotFound();
            }
            return View(tRN_REGISTRATION);
        }

        // POST: TRN_REGISTRATION/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TRN_REGISTRATION tRN_REGISTRATION = db.TRN_REGISTRATION.Find(id);
            db.TRN_REGISTRATION.Remove(tRN_REGISTRATION);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
  


        public ActionResult PendingDuesDept()
        {

            var theaters = (from tr in db.TRN_REGISTRATION
                            join pd in db.PENDINGDUEADMINs
                            on tr.T_ID equals pd.T_ID
                            select new TheaterDueViewModel
                            {
                                T_ID = tr.T_ID,
                                T_NAME = tr.T_NAME,
                                T_CITY = tr.T_CITY,
                                T_WARD = tr.T_WARD,
                                T_ZONE = tr.T_ZONE,
                                T_ADDRESS = tr.T_ADDRESS,
                                T_TENAMENT_NO = tr.T_TENAMENT_NO,
                                P_STATUS = pd.P_STATUS
                            }).ToList();

            return View(theaters);

        }
        [HttpGet]
        public ActionResult PaymentList()
        {
            ViewBag.StatusFilterOptions = new SelectList(new List<string> { "Paid", "Pending", "Overdue" });
            return View(new List<PAYMENTLIST>()); // Return an empty list initially
        }

        [HttpPost]
        public ActionResult PaymentList(int? theaterId, string fromMonth, string fromYear, string toMonth, string toYear, string statusFilter)
        {
            DateTime? fromDate = null, toDate = null;
            if (!string.IsNullOrEmpty(fromMonth) && !string.IsNullOrEmpty(fromYear))
            {
                fromDate = new DateTime(int.Parse(fromYear), int.Parse(fromMonth), 1);
            }
            if (!string.IsNullOrEmpty(toMonth) && !string.IsNullOrEmpty(toYear))
            {
                toDate = new DateTime(int.Parse(toYear), int.Parse(toMonth), DateTime.DaysInMonth(int.Parse(toYear), int.Parse(toMonth)));
            }

            // Use the correct DbSet for filtering
            var payments = db.PAYMENTLISTs.AsQueryable();

            if (theaterId.HasValue)
            {
                payments = payments.Where(p => p.T_ID == theaterId.Value);
            }
            if (fromDate.HasValue)
            {
                payments = payments.Where(p => p.STARTDATE >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                payments = payments.Where(p => p.ENDDATE <= toDate.Value);
            }
            if (!string.IsNullOrEmpty(statusFilter))
            {
                payments = payments.Where(p => p.STATUS_PAYMENT == statusFilter);
            }

            ViewBag.StatusFilterOptions = new SelectList(new List<string> { "Paid", "Pending", "Overdue" });
            return View(payments.ToList());
        }
        [HttpPost]
        public ActionResult ApproveReject(int theaterId, string action)
        {
            var theater = _context.TRN_REGISTRATION.SingleOrDefault(t => t.T_ID == theaterId);
            if (theater == null)
            {
                return HttpNotFound();
            }

            // Only update if the status is Pending
            if (theater.STATUS == "Pending")
            {
                if (action == "Approve")
                {
                    theater.STATUS = "Approved";
                }
                else if (action == "Reject")
                {
                    theater.STATUS = "Rejected";
                }

                // Save the changes to the database
                _context.SaveChanges();
            }
            else
            {
                // If the status is not Pending, we can show a message or handle as needed
                ViewBag.Message = "This request has already been processed.";
            }

            // Redirect back to the ActionRequests page
            return RedirectToAction("ActionRequests");
        }
      

        [HttpPost]
        public ActionResult Theater_Tax(int theater_id)
        {
            if (theater_id <= 0)
            {
                TempData["Error"] = "Please enter a valid Theater ID.";
                return RedirectToAction("Index");
            }

            var model = new TaxPaymentViewModel
            {
                TheaterId = theater_id,
                FromMonth = DateTime.Now.ToString("MMMM"), // Default to current month
                ToMonth = DateTime.Now.Year // Default to current year
            };

            // ✅ Step 1: Fetch screen prices first and store in memory
          var screenPrices = db.TRN_SCREEN_TAX_PRICE
                .AsNoTracking() // Improves performance
                .ToDictionary(p => p.SCREEN_TYPE, p => p.SCREEN_PRICE);

            // ✅ Step 2: Fetch theater details along with associated screens
            var theaterDetails = db.TRN_REGISTRATION
                .Where(t => t.T_ID == theater_id)
                .Select(t => new
                {
                    TheaterID = t.T_ID,
                    TheaterName = t.T_NAME,
                    OwnerName = t.T_OWNER_NAME,
                    MobileNo = t.T_OWNER_NUMBER != null ? t.T_OWNER_NUMBER.ToString() : string.Empty,
                    Address = t.T_ADDRESS,
                    Email = t.T_OWNER_EMAIL,
                    Screens = db.NO_OF_SCREENS
                        .Where(s => s.T_ID == t.T_ID)
                        .ToList() // ✅ Move data to memory first
                })
                .FirstOrDefault();

            if (theaterDetails == null)
            {
                TempData["Error"] = "Theater ID not found.";
                return RedirectToAction("Index");
            }

            // ✅ Step 3: Map the screens manually after fetching from DB
            model.Screens = theaterDetails.Screens
                .Select(s => new ScreenViewModel
                {
                    ScreenId = s.SCREEN_ID,
                    AudienceCapacity = (int)s.AUDIENCE_CAPACITY,
                    ScreenType = s.SCREEN_TYPE,
                  
                    ScreenPrice = screenPrices.ContainsKey(s.SCREEN_TYPE) ? screenPrices[s.SCREEN_TYPE].GetValueOrDefault(0) : 0
                }).ToList();

            model.OwnerName = theaterDetails.OwnerName;
            model.MobileNo = theaterDetails.MobileNo;
            model.Address = theaterDetails.Address;
            model.Email = theaterDetails.Email;
            model.TheaterName = theaterDetails.TheaterName;

            return View(model);
        }
        public ActionResult ProcessTaxPayment(TaxPaymentViewModel model, HttpPostedFileBase DocumentPath)
        {
            if (model == null || model.Screens == null || model.Screens.Count == 0)
            {
                TempData["Error"] = "Invalid data. Please check your input.";
                return RedirectToAction("Index");
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                   
                        string filePath = "Generated Automatically"; // Default value

                        // ✅ Step 1: Handle file upload
                        if (DocumentPath != null && DocumentPath.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(DocumentPath.FileName);
                            string uploadPath = Path.Combine(Server.MapPath("~/UploadedDoc/"), fileName);

                            // Ensure directory exists
                            if (!Directory.Exists(Server.MapPath("~/UploadedDoc/")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/UploadedDoc/"));
                            }

                            // Save file
                            DocumentPath.SaveAs(uploadPath);

                            // Store relative file path in DB
                            filePath = "/UploadedDoc/" + fileName;
                        }

                        // Step 1: Insert into THEATER_TAX_PAYMENT (Main Table)
                        var taxPayment = new THEATER_TAX_PAYMENT
                    {
                        T_ID = model.TheaterId,
                        PAYMENT_MONTH = model.FromMonth,
                        PAYMENT_YEAR = model.ToMonth,
                        TAX_AMOUNT = model.Screens.Sum(s => s.AmtPerScreen),
                        SHOW_STATEMENT = filePath,
                        CREATE_USER = "System",
                        CREATE_DATE = DateTime.Now
                      
                    };
                    db.THEATER_TAX_PAYMENT.Add(taxPayment);
                    db.SaveChanges();

                    // ✅ Step 2: Fetch NO_OF_SCREENS details from DB for correct mapping
                    var screenDetails = db.NO_OF_SCREENS
                        .Where(s => s.T_ID == model.TheaterId)
                        .ToList(); // Fetching list for iteration

                    // ✅ Step 3: Insert into NO_OF_SCREENS_TAX (Child Table)
                    foreach (var screen in model.Screens)
                    {
                        var masterScreen = screenDetails.FirstOrDefault(s => s.SCREEN_ID == screen.ScreenId);

                        if (masterScreen != null) // Ensure the screen exists in master table
                        {
                            var screenTax = new NO_OF_SCREENS_TAX
                            {
                                T_ID = model.TheaterId,
                                TAX_ID = taxPayment.TAX_ID,
                                SCREEN_TYPE = masterScreen.SCREEN_TYPE, // Fetched from Master Table
                                TOTAL_SHOW = screen.TotalShow, // Fetched from View
                                CANCEL_SHOW = screen.CancelShow, // Fetched from View
                                ACTUAL_SHOW = screen.TotalShow - screen.CancelShow, // ✅ Store Actual Show
                               RATE_PER_SCREEN = (masterScreen.SCREEN_TYPE == "Theater") ? 75 : 25, // ✅ Store 75 or 25 based on Screen Type
                                AMT_PER_SCREEN = screen.AmtPerScreen // Fetched from View
                            };
                            db.NO_OF_SCREENS_TAX.Add(screenTax);
                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    TempData["Success"] = "Tax payment saved successfully!";
                    return RedirectToAction("Theater_Tax");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Error"] = "Error saving data: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
        }
      
        public ActionResult Login()
        {
            Session["HideMenu"] = null;  // Reset sidebar visibility after login
            ViewBag.ShowSideBar = true;  // Ensure sidebar is explicitly shown

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            

            // Ensure phone numbers are compared as strings
            var userExists = db.LOGIN_DETAILS
                .Any(u => u.PHONE_NUMBER.ToString() == model.PhoneNumber);

            if (!userExists)
            {
                ModelState.AddModelError("PhoneNumber", "Phone number not found.");
                return View(model);
            }

            return RedirectToAction("Theater_List", "Home");
        }
        public ActionResult Department_Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Department_Login(LoginViewModel model)
        {


            // Check if the user exists based on the UN (username/phone number)
            bool userExists = db.DEPT_LOGIN_DETAILS.Any(u => u.DEPT_USERNAME.Trim() == model.UN.Trim());

            if (!userExists)
            {
                ModelState.AddModelError("UN", "User not found.");
                return View(model);
            }
            return RedirectToAction("DeptHomePage", "Department");

        }
        // GET: Department
        [HttpGet]
        public ActionResult Theater_List()
        {
            ViewBag.CurrentAction = "TheaterList";
            // Query to fetch all data from TRN_REGISTRATION
            var query = from tr in db.TRN_REGISTRATION
                        where tr.T_ACTIVE == true && tr.STATUS == "Approved"
                        select new
                        {
                            tr.T_ID,
                            tr.REG_ID,
                            tr.T_NAME,
                            tr.T_CITY,
                            tr.T_ADDRESS,
                            tr.T_TENAMENT_NO,
                            tr.T_ZONE,
                            tr.T_WARD,
                            tr.STATUS,
                            tr.REJECT_REASON,
                            tr.UPDATE_DATE,
                            tr.T_OWNER_NAME,
                            tr.T_COMMENCEMENT_DATE,
                            TheaterScreenCount = db.NO_OF_SCREENS.Count(s => s.T_ID == tr.T_ID && s.SCREEN_TYPE == "Theater"),
                            VideoTheaterScreenCount = db.NO_OF_SCREENS.Count(s => s.T_ID == tr.T_ID && s.SCREEN_TYPE == "Video")
                        };


            // Execute the query and convert to a list
            var result = query.ToList();
            Console.WriteLine("Total Theaters Found: " + result.Count);

            // Convert the result to a list of ViewModel objects
            var theaterList = result.Select(tr => new TheaterViewModel
            {
                T_ID = tr.T_ID,
                REG_ID = tr.REG_ID,
                T_NAME = tr.T_NAME,
                T_CITY = tr.T_CITY,
                T_ADDRESS = tr.T_ADDRESS,
                T_TENAMENT_NO = tr.T_TENAMENT_NO,
                T_ZONE = tr.T_ZONE,
                T_WARD = tr.T_WARD,
                STATUS = tr.STATUS,
                T_OWNER_NAME = tr.T_OWNER_NAME,
                REJECT_REASON = tr.REJECT_REASON,
                T_COMMENCEMENT_DATE = (DateTime)tr.T_COMMENCEMENT_DATE,
                UPDATE_DATE = tr.UPDATE_DATE ?? DateTime.MinValue,
                SCREEN_COUNT = tr.TheaterScreenCount + tr.VideoTheaterScreenCount,
                THEATER_SCREEN_COUNT = tr.TheaterScreenCount,
                VIDEO_THEATER_SCREEN_COUNT = tr.VideoTheaterScreenCount

            }).ToList();

            return View(theaterList);
        }


        public ActionResult Theater_List(string theaterId, DateTime? fromDate, DateTime? toDate,
                                string statusFilter, string cityFilter,
                                string wardFilter, string zoneFilter, int? deleteId)
        {
            // ✅ Debugging - Log Incoming Data
            Debug.WriteLine($"Received Data -> FromDate: {fromDate}, ToDate: {toDate}");

            // ✅ Soft delete logic
            if (deleteId.HasValue)
            {
                var theaterToDelete = db.TRN_REGISTRATION.FirstOrDefault(t => t.T_ID == deleteId.Value);
                if (theaterToDelete != null)
                {
                    theaterToDelete.T_ACTIVE = false;  // Soft delete (mark inactive)
                    db.SaveChanges();
                }
            }

            // Static status values for the filter dropdown
            var statuses = new List<string> { "Pending", "Approved", "Reject" };

            // ✅ Base Query - Fetch Active Theaters
            var query = db.TRN_REGISTRATION.Where(tr => tr.T_ACTIVE == true);

            // ✅ Apply Filters and Persist Them in ViewBag
            if (!string.IsNullOrEmpty(theaterId) && int.TryParse(theaterId, out int theaterIdInt))
            {
                query = query.Where(tr => tr.T_ID == theaterIdInt);
                ViewBag.SelectedTheaterId = theaterId;
            }
            else
            {
                if (fromDate.HasValue)
                {
                    DateTime from = fromDate.Value.Date; // Start of the day (00:00:00)
                    query = query.Where(tr => tr.T_COMMENCEMENT_DATE.HasValue && tr.T_COMMENCEMENT_DATE.Value >= from);
                }

                if (toDate.HasValue)
                {
                    DateTime to = toDate.Value.Date.AddDays(1).AddTicks(-1); // End of the day (23:59:59)
                    query = query.Where(tr => tr.T_COMMENCEMENT_DATE.HasValue && tr.T_COMMENCEMENT_DATE.Value <= to);
                }
            }

            if (!string.IsNullOrEmpty(cityFilter))
            {
                query = query.Where(tr => tr.T_CITY == cityFilter);
                ViewBag.SelectedCity = cityFilter;
            }

            if (!string.IsNullOrEmpty(wardFilter))
            {
                query = query.Where(tr => tr.T_WARD == wardFilter);
                ViewBag.SelectedWard = wardFilter;
            }

            if (!string.IsNullOrEmpty(zoneFilter))
            {
                query = query.Where(tr => tr.T_ZONE == zoneFilter);
                ViewBag.SelectedZone = zoneFilter;
            }

            if (!string.IsNullOrEmpty(statusFilter) && statuses.Contains(statusFilter))
            {
                query = query.Where(tr => tr.STATUS == statusFilter);
                ViewBag.SelectedStatus = statusFilter;
            }

            var result = query.ToList();

            // ✅ Persist Dates in ViewBag for input fields
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            // ✅ Populate Filter Dropdowns
            ViewBag.Cities = result.Select(tr => tr.T_CITY).Distinct().ToList();
            ViewBag.Wards = result.Select(tr => tr.T_WARD).Distinct().ToList();
            ViewBag.Zones = result.Select(tr => tr.T_ZONE).Distinct().ToList();
            ViewBag.Statuses = statuses;

            // ✅ Convert Result to ViewModel
            var theaterList = result.Select(tr => new TheaterViewModel
            {
                T_ID = tr.T_ID,
                T_NAME = tr.T_NAME,
                T_CITY = tr.T_CITY,
                T_ADDRESS = tr.T_ADDRESS,
                T_TENAMENT_NO = tr.T_TENAMENT_NO,
                T_WARD = tr.T_WARD,
                T_ZONE = tr.T_ZONE,
                STATUS = tr.STATUS
            }).ToList();

            return View(theaterList);
        }


        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Dues()
        {
            var query = from tr in db.PENDING_DUES
                        where tr.D_ACTIVE == true
                        select new
                        {
                            tr.D_MON,
                            tr.D_YEAR,
                            tr.D_STATUS
                            

                        };


            // Execute the query and convert to a list
            var result = query.ToList();
            Console.WriteLine("Total Theaters Found: " + result.Count);

            // Convert the result to a list of ViewModel objects
            var theaterList = result.Select(tr => new DueViewModel
            {
                 D_MON = tr.D_MON,
                D_YEAR = tr.D_YEAR,
                D_STATUS = tr.D_STATUS

            }).ToList();

            return View(theaterList);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Theater_Registration()
        {

            return View();
        }


        public ActionResult Theater_Tax(bool? hideMenu)
        {
            if (hideMenu == true)
            {
                Session["HideMenu"] = true;  // Hide sidebar when clicking "Pay Now"
            }
            else if (Session["HideMenu"] != null)
            {
                hideMenu = (bool)Session["HideMenu"];
            }
            else
            {
                hideMenu = false; // Default to showing sidebar
            }

            ViewBag.ShowSideBar = !hideMenu; // Control sidebar visibility

            return View();
        }


        public ActionResult Make_Payment()
        {

            return View();
        }

       

        public ActionResult Payment_Receipt()
        {

            return View();
        }
        public ActionResult FormWithPagination()
        {
            return View();
        }

        // Handle form submission (if necessary)
        [HttpPost]
        public ActionResult SubmitForm(FormCollection form)
        {
            // Access the form data using the FormCollection
            var name = form["name"];
            var email = form["email"];
            var address = form["address"];
            var city = form["city"];
            var cardNumber = form["cardNumber"];
            var expiry = form["expiry"];

            // Do something with the form data (e.g., save to database)

            return RedirectToAction("Success");
        }


        [HttpGet]
        public ActionResult Receipt()
        {
            return View();
        }
        public ActionResult Edit_Registration()
        {
            return View();
        }
       

        public ActionResult Success()
        {
            return View();
        }
        public ActionResult New()
        {
            return View();
        }
        [HttpGet]
        public ActionResult SearchID()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchID(string searchTerm)
        {
            var results = db.TRN_REGISTRATION
                            .Where(t => t.T_TENAMENT_NO == searchTerm)
                            .Select(t => new SearchViewModel
                            {
                                T_NAME = t.T_NAME,
                                REG_ID = t.REG_ID
                                // Add other properties if needed
                            })
                            .ToList();

            return View(results);
        }

        //[HttpPost]
        //public ActionResult Theater_Tax(int theater_id)
        //{
        //    if (theater_id <= 0)
        //    {
        //        TempData["Error"] = "Please enter a valid Theater ID.";
        //        return RedirectToAction("Index");
        //    }

        //    // Fetch the theater details along with the associated screens
        //    var theaterDetails = db.TRN_REGISTRATION
        //        .Where(t => t.T_ID == theater_id)
        //        .Select(t => new
        //        {
        //            TheaterID = t.T_ID,
        //            OwnerName = t.T_OWNER_NAME,
        //            MobileNo = t.T_OWNER_NUMBER != null ? t.T_OWNER_NUMBER.ToString() : string.Empty,
        //            Address = t.T_ADDRESS,
        //            Email = t.T_OWNER_EMAIL,
        //            Screens = db.NO_OF_SCREENS
        //                .Where(s => s.T_ID == t.T_ID)
        //                .Select(s => new ScreenViewModel
        //                {
        //                    ScreenId = s.SCREEN_ID,
        //                    AudienceCapacity =(int) s.AUDIENCE_CAPACITY,
        //                    ScreenType = s.SCREEN_TYPE
        //                })
        //                .ToList()
        //        })
        //        .FirstOrDefault();

        //    if (theaterDetails == null)
        //    {
        //        TempData["Error"] = "Theater ID not found.";
        //        return RedirectToAction("Index");
        //    }

        //    // Create a new TaxPayment view model and populate it with theater details and screens
        //    var taxPayment = new TaxPayment
        //    {
        //        TheaterID = theaterDetails.TheaterID,
        //        OwnerName = theaterDetails.OwnerName,
        //        MobileNo = theaterDetails.MobileNo,
        //        Address = theaterDetails.Address,
        //        Email = theaterDetails.Email,
        //        Screens = theaterDetails.Screens // Assuming TaxPayment has a Screens property
        //    };

        //    return View(taxPayment); // Return the same view with the model populated
        //}

        public ActionResult PaymentHistoryFilter()
        {

            return View();
        }

        public ActionResult Home_Page()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PaymentHistoryFilter(
       int? theaterId,
       string fromDate,
       string toDate,
       string statusFilter,
       decimal? minAmount,
       decimal? maxAmount)
        {
            // Initialize date variables
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (DateTime.TryParse(fromDate, out DateTime parsedFromDate))
            {
                startDate = parsedFromDate;
            }

            if (DateTime.TryParse(toDate, out DateTime parsedToDate))
            {
                endDate = parsedToDate;
            }

            var query = db.PAYMENT_HISTORY.AsQueryable();

            if (theaterId.HasValue)
            {
                query = query.Where(p => p.T_ID == theaterId.Value);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(p => p.STATUS == statusFilter);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(p => p.PAYMENT_DATE >= startDate.Value && p.PAYMENT_DATE <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(p => p.PAYMENT_DATE >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                query = query.Where(p => p.PAYMENT_DATE <= endDate.Value);
            }

            if (minAmount.HasValue)
            {
                query = query.Where(p => p.AMOUNT >= minAmount.Value);
            }

            if (maxAmount.HasValue)
            {
                query = query.Where(p => p.AMOUNT <= maxAmount.Value);
            }

            // Convert to ViewModel and format date
            var filteredPayments = query.ToList().Select(p => new PaymentHistoryViewModel
            {
                PAYMENT_ID = p.PAYMENT_ID,
                T_ID = p.T_ID,
                AMOUNT = p.AMOUNT,
                STATUS = p.STATUS,
                PAYMENT_DATE = p.PAYMENT_DATE.ToString("dd-MM-yyyy") // ✅ Correctly formatted date
            }).ToList();

            return View(filteredPayments);
        }




        [HttpGet]
        public ActionResult AllReceipt(
            int? theaterId,
            string fromDate,
            string toDate,
            string statusFilter,
            string paymentModeFilter,
            int? monthFilter,
            int? yearFilter)
        {
            // Initialize date variables
            DateTime? startDate = null;
            DateTime? endDate = null;

            // Parse From Date
            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime parsedFromDate))
            {
                startDate = parsedFromDate;
            }

            // Parse To Date
            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime parsedToDate))
            {
                endDate = parsedToDate;
            }

            Console.WriteLine($"Theater ID: {theaterId}, Start Date: {startDate}, End Date: {endDate}, Status: {statusFilter}, Payment Mode: {paymentModeFilter}");

            // Query to fetch filtered data from RECEIPT_FILTER and TRN_REGISTRATION
            var query = from rf in db.RECEIPT_FILTER
                        join tr in db.TRN_REGISTRATION on rf.T_ID equals tr.T_ID
                        select new
                        {
                            rf.RCPT_NO,
                            rf.RCPT_GEN_DATE,
                            rf.T_ID,
                            tr.T_NAME,
                            rf.PAY_MODE,
                            rf.STATUS
                        };

            // Apply filters dynamically
            if (theaterId.HasValue)
            {
                query = query.Where(r => r.T_ID == theaterId.Value);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(r => r.STATUS == statusFilter);
            }

            if (!string.IsNullOrEmpty(paymentModeFilter))
            {
                query = query.Where(r => r.PAY_MODE == paymentModeFilter);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => r.RCPT_GEN_DATE >= startDate.Value && r.RCPT_GEN_DATE <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(r => r.RCPT_GEN_DATE >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                query = query.Where(r => r.RCPT_GEN_DATE <= endDate.Value);
            }

            var result = query.ToList();
            Console.WriteLine("Total Records Found: " + result.Count);

            var receiptList = result.Select(r => new ReceiptFilterViewModel
            {
                RCPT_NO = r.RCPT_NO,
                RCPT_GEN_DATE = r.RCPT_GEN_DATE,
                T_ID = r.T_ID ?? 0,
                T_NAME = r.T_NAME,
                PAY_MODE = r.PAY_MODE,
                STATUS = r.STATUS
            }).ToList();

            return View(receiptList);
        }
        [HttpPost]
        public ActionResult AllReceipt(ReceiptFilterViewModel model)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrEmpty(model.FromDate) && DateTime.TryParse(model.FromDate, out DateTime parsedFromDate))
            {
                startDate = parsedFromDate.Date; // Remove time
            }

            if (!string.IsNullOrEmpty(model.ToDate) && DateTime.TryParse(model.ToDate, out DateTime parsedToDate))
            {
                endDate = parsedToDate.Date; // Remove time
            }

            var query = from rf in db.RECEIPT_FILTER
                        join tr in db.TRN_REGISTRATION on rf.T_ID equals tr.T_ID
                        select new
                        {
                            rf.RCPT_NO,
                            RCPT_GEN_DATE = rf.RCPT_GEN_DATE,  // Stored as Date only
                            rf.T_ID,
                            tr.T_NAME,
                            rf.PAY_MODE,
                            rf.STATUS
                        };

            if (model.TheaterId.HasValue)
            {
                query = query.Where(r => r.T_ID == model.TheaterId.Value);
            }

            if (!string.IsNullOrEmpty(model.StatusFilter))
            {
                query = query.Where(r => r.STATUS == model.StatusFilter);
            }

            if (!string.IsNullOrEmpty(model.PaymentModeFilter))
            {
                query = query.Where(r => r.PAY_MODE == model.PaymentModeFilter);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => r.RCPT_GEN_DATE >= startDate.Value && r.RCPT_GEN_DATE <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(r => r.RCPT_GEN_DATE >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                query = query.Where(r => r.RCPT_GEN_DATE <= endDate.Value);
            }

            var result = query.ToList();

            var receiptList = result.Select(r => new ReceiptFilterViewModel
            {
                RCPT_NO = r.RCPT_NO,
                RCPT_GEN_DATE = r.RCPT_GEN_DATE, // Store as Date only

                T_ID = r.T_ID ?? 0,
                T_NAME = r.T_NAME,
                PAY_MODE = r.PAY_MODE,
                STATUS = r.STATUS
            }).ToList();

            return View(receiptList);
        }

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
        //    var theaterList = result.Select(tr => new ActionRequestViewModel
        //    {
        //        T_ID = tr.T_ID,
        //        T_NAME = tr.T_NAME,

        //        T_OWNER_NAME = tr.T_OWNER_NAME
        //    }).ToList();

        //    return View(theaterList);
        //}
        //[HttpPost]
        //public ActionResult ActionRequest()
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
        //    var theaterList = result.Select(tr => new TheaterViewModel
        //    {
        //        T_ID = tr.T_ID,
        //        T_NAME = tr.T_NAME,

        //        T_OWNER_NAME = tr.T_OWNER_NAME
        //    }).ToList();

        //    return View(theaterList);
        //}
        [HttpPost]
        public ActionResult UpdateStatus(int theaterId, string status)
        {
            try
            {
                var theater = db.TRN_REGISTRATION.FirstOrDefault(t => t.T_ID == theaterId);
                if (theater != null)
                {
                    // Update the status based on the string value ("Approved", "Rejected", or "Pending")
                    if (status == "Approved")
                    {
                        theater.STATUS = "Approved";
                    }
                    else if (status == "Rejected")
                    {
                        theater.STATUS = "Rejected";
                    }
                    else
                    {
                        theater.STATUS = "Pending";  // Default to "Pending" if the status is not "Approved" or "Rejected"
                    }

                    // Save changes to the database
                    db.SaveChanges();
                }

                // Redirect to the ActionRequests page after update
                return RedirectToAction("ActionRequests", "Department");
            }
            catch (Exception ex)
            {
                // Handle any errors that occur
                TempData["ErrorMessage"] = "Error updating status: " + ex.Message;
                return RedirectToAction("ActionRequests", "Department");
            }
        }
        public ActionResult GenerateReceipt(int id)
        {
            // Try fetching the receipt directly
            var debugReceipt = _context.Receipts.FirstOrDefault(r => r.Id == id);

            if (debugReceipt == null)
            {
                throw new Exception($"Receipt with ID {id} not found in the database.");
            }

            System.Diagnostics.Debug.WriteLine($"Receipt Found: {debugReceipt.ReceiptNo}, Amount: {debugReceipt.Amount}");

            var receipt = _context.Receipts
                .Where(r => r.Id == id)
                .Select(r => new
                {
                    r.ReceiptNo,
                    r.Amount,
                    r.PaymentMode,
                    r.Description,
                    T_ID = r.T_ID ?? 21,
                    Theater = _context.TRN_REGISTRATION
                        .Where(t => t.T_ID == (r.T_ID ?? 21))
                        .Select(t => new
                        {
                            t.T_NAME,
                            t.T_OWNER_EMAIL,
                            t.T_ADDRESS,
                            t.T_OWNER_NUMBER
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefault();

            if (receipt == null)
            {
                TempData["ErrorMessage"] = "Receipt not found.";
                return RedirectToAction("Index");
            }

            var viewModel = new ReceiptViewModel
            {
                ReceiptNo = receipt.ReceiptNo ?? "N/A",
                Amount = receipt.Amount ?? 0,
                PaymentMode = receipt.PaymentMode ?? "N/A",
                TheaterName = receipt.Theater?.T_NAME ?? "N/A",
                Email = receipt.Theater?.T_OWNER_EMAIL ?? "N/A",
                Address = receipt.Theater?.T_ADDRESS ?? "N/A",
                T_OWNER_NUMBER = receipt.Theater?.T_OWNER_NUMBER?.ToString() ?? "N/A",
                Description = receipt.Description ?? "N/A",
                T_ID = receipt.T_ID
            };

            return View(viewModel);
        }


        public ActionResult Approve(int id)
        {
            try
            {
                var theater = db.TRN_REGISTRATION.FirstOrDefault(t => t.T_ID == id);
                if (theater != null)
                {
                    theater.STATUS = "Approved";  // Update status to "Approved"
                    db.SaveChanges();  // Save the changes to the database
                }

                TempData["SuccessMessage"] = "Request Approved successfully!";
                return RedirectToAction("ActionRequests");  // Redirect back to the ActionRequests page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating status: " + ex.Message;
                return RedirectToAction("ActionRequests");  // Redirect back to the ActionRequests page on error
            }
        }

        public ActionResult Reject(int id)
        {
            try
            {
                var theater = db.TRN_REGISTRATION.FirstOrDefault(t => t.T_ID == id);
                if (theater != null)
                {
                    theater.STATUS = "Rejected";  // Update status to "Rejected"
                    db.SaveChanges();  // Save the changes to the database
                }

                TempData["SuccessMessage"] = "Request Rejected successfully!";
                return RedirectToAction("ActionRequests");  // Redirect back to the ActionRequests page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating status: " + ex.Message;
                return RedirectToAction("ActionRequests");  // Redirect back to the ActionRequests page on error
            }
        }

        public ActionResult Tax_Hold()
        {
            return View();
        }

        public ActionResult Seprate_Theater_Tax()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Seprate_Theater_Tax(int theater_id)
        {
            if (theater_id <= 0)
            {
                TempData["Error"] = "Please enter a valid Theater ID.";
                return RedirectToAction("Index");
            }

            var model = new TaxPaymentViewModel
            {
                TheaterId = theater_id,
                FromMonth = DateTime.Now.ToString("MMMM"), // Default to current month
                ToMonth = DateTime.Now.Year // Default to current year
            };

            // ✅ Step 1: Fetch screen prices first and store in memory
            var screenPrices = db.TRN_SCREEN_TAX_PRICE
                .AsNoTracking() // Improves performance
                .ToDictionary(p => p.SCREEN_TYPE, p => p.SCREEN_PRICE);

            // ✅ Step 2: Fetch theater details along with associated screens
            var theaterDetails = db.TRN_REGISTRATION
                .Where(t => t.T_ID == theater_id)
                .Select(t => new
                {
                    TheaterID = t.T_ID,
                    OwnerName = t.T_OWNER_NAME,
                    MobileNo = t.T_OWNER_NUMBER != null ? t.T_OWNER_NUMBER.ToString() : string.Empty,
                    Address = t.T_ADDRESS,
                    Email = t.T_OWNER_EMAIL,
                    Screens = db.NO_OF_SCREENS
                        .Where(s => s.T_ID == t.T_ID)
                        .ToList() // ✅ Move data to memory first
                })
                .FirstOrDefault();

            if (theaterDetails == null)
            {
                TempData["Error"] = "Theater ID not found.";
                return RedirectToAction("Index");
            }

            // ✅ Step 3: Map the screens manually after fetching from DB
            model.Screens = theaterDetails.Screens
                .Select(s => new ScreenViewModel
                {
                    ScreenId = s.SCREEN_ID,
                    AudienceCapacity = (int)s.AUDIENCE_CAPACITY,
                    ScreenType = s.SCREEN_TYPE,

                    ScreenPrice = screenPrices.ContainsKey(s.SCREEN_TYPE) ? screenPrices[s.SCREEN_TYPE].GetValueOrDefault(0) : 0
                }).ToList();

            model.OwnerName = theaterDetails.OwnerName;
            model.MobileNo = theaterDetails.MobileNo;
            model.Address = theaterDetails.Address;
            model.Email = theaterDetails.Email;

            return View(model);
        }
        public JsonResult GetTheaters(string term)
    {
        var theaters = db.TRN_REGISTRATION
                          .Where(t => t.T_NAME.Contains(term)) // Search by theater name
                          .Select(t => new
                          {
                              Id = t.T_ID,  // Theater ID
                              T_NAME = t.T_NAME, // Theater Name
                              T_OWNER_NAME = t.T_OWNER_NAME // Owner Name
                          })
                          .ToList();

        return Json(theaters, JsonRequestBehavior.AllowGet);
    }



    }
}