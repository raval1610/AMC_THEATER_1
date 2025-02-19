using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AMC_THEATER_1.Models;
using Antlr.Runtime.Misc;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;

namespace AMC_THEATER_1.Controllers
{
    public class TRN_REGISTRATIONController : Controller
    {
        private THEATER_MODULEEntities db = new THEATER_MODULEEntities();

        //public ActionResult Registration(int? id, string mode = "create")
        //{
        //    ViewBag.PageTitle = "Theater Registration";
        //    ViewBag.Mode = mode; // ✅ Mode is now always set

        //    ViewBag.Documents = GetActiveDocuments();

        //    if (id.HasValue)
        //    {
        //        var registrationData = GetRegistrationData(id.Value);
        //        if (registrationData == null)
        //        {
        //            TempData["ErrorMessage"] = "No registration data found for the provided ID.";
        //            return RedirectToAction("Index");
        //        }

        //        ViewBag.Screens = GetScreens(id.Value);
        //        ViewBag.UploadedDocs = GetUploadedDocs(id.Value);
        //    }

        //    return View();
        //}
        public ActionResult Registration(int? id, bool isViewPage = false, string mode = "create")
        {
            ViewBag.PageTitle = "Theater Registration";
            ViewBag.Mode = mode; // ✅ Mode is now always set
            ViewBag.IsViewPage = isViewPage;

            // Fetch active documents
            ViewBag.Documents = GetActiveDocuments();

            if (id.HasValue)
            {
                var registrationData = GetRegistrationData(id.Value);
                if (registrationData == null)
                {
                    TempData["ErrorMessage"] = "No registration data found for the provided ID.";
                    return RedirectToAction("Index");
                }

                ViewBag.Screens = GetScreens(id.Value);
                ViewBag.UploadedDocs = GetUploadedDocs(id.Value);
                ViewBag.IsEditPage = true;

                return View(registrationData);
            }

            return View();
        }

        private TRN_REGISTRATION GetRegistrationData(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;

            return db.TRN_REGISTRATION
                .Where(r => r.T_ID == id)
                .AsNoTracking()
                .FirstOrDefault();
        }

        private List<NO_OF_SCREENS> GetScreens(int id)
        {
            return db.NO_OF_SCREENS
                .Where(s => s.T_ID == id)
                .AsNoTracking()
                .ToList();
        }

        private List<TRN_THEATRE_DOCS> GetUploadedDocs(int id)
        {
            return db.TRN_THEATRE_DOCS
                .Where(d => d.T_ID == id)
                .AsNoTracking()
                .ToList();
        }

        private List<MST_DOCS> GetActiveDocuments()
        {
            return db.MST_DOCS
                .Where(doc => doc.DOC_ACTIVE == true)
                .AsNoTracking()
                .ToList();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(TRN_REGISTRATION model, string[] seatCapacity, string[] screenType, string actionType, string rejectReason = null)
        {
            try
            {
                int theaterId;

                // Check if theater already exists
                TRN_REGISTRATION existingRegistration = db.TRN_REGISTRATION.Find(model.T_ID);

                if (existingRegistration != null)
                {
                    LogOldValues(existingRegistration);
                    UpdateRegistration(existingRegistration, model);
                    theaterId = model.T_ID; // Keep the same T_ID
                }
                else
                {
                    // Create new theater and get T_ID
                    theaterId = CreateNewRegistration(model);
                }

                // Update status (Approve, Reject, Edit)
                UpdateTheaterStatus(theaterId, actionType, rejectReason);

                // Process screens and documents after updating the status
                HandleScreens(theaterId, seatCapacity, screenType);
                HandleDocuments(theaterId);

                db.SaveChanges(); // Save all updates

                TempData["SuccessMessage"] = "Theater saved successfully!";

                // **Redirect based on action type**
                if (actionType == "Approve" || actionType == "Reject")
                {
                    return RedirectToAction("ActionRequests", "Home");
                }
                else
                {
                    return RedirectToAction("List_of_Application", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                Console.WriteLine(ex);
            }

            ViewBag.Documents = db.MST_DOCS.Where(d => d.DOC_ACTIVE == true).ToList();
            return View(model);
        }

        private void HandleScreens(int tId, string[] seatCapacity, string[] screenType)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 🔹 Fetch existing screens
                    var existingScreens = db.NO_OF_SCREENS.Where(s => s.T_ID == tId).ToList();

                    // 🔹 Log old records before deletion
                    foreach (var screen in existingScreens)
                    {
                        db.NO_OF_SCREENS_LOG.Add(new NO_OF_SCREENS_LOG
                        {
                            T_ID = screen.T_ID,
                            SCREEN_ID = screen.SCREEN_ID,
                            AUDIENCE_CAPACITY = screen.AUDIENCE_CAPACITY,
                            SCREEN_TYPE = screen.SCREEN_TYPE,
                            ACTION_TYPE = "DELETE",
                            CHANGED_BY = "System",
                            CHANGED_ON = DateTime.Now
                        });
                    }

                    db.SaveChanges(); // ✅ Save logs before deleting old records

                    // 🔹 Remove existing records
                    db.NO_OF_SCREENS.RemoveRange(existingScreens);
                    db.SaveChanges(); // ✅ Ensure deletion before inserting new screens

                    // 🔹 Insert new screens
                    List<NO_OF_SCREENS> newScreens = new List<NO_OF_SCREENS>();

                    for (int i = 0; i < seatCapacity.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(seatCapacity[i]) && !string.IsNullOrEmpty(screenType[i]))
                        {
                            var screen = new NO_OF_SCREENS
                            {
                                T_ID = tId,
                                AUDIENCE_CAPACITY = int.Parse(seatCapacity[i]),
                                SCREEN_TYPE = screenType[i]
                            };
                            newScreens.Add(screen);
                        }
                    }

                    db.NO_OF_SCREENS.AddRange(newScreens);
                    db.SaveChanges(); // ✅ Save NO_OF_SCREENS first to get SCREEN_IDs

                    // 🔹 Insert logs for newly inserted screens
                    foreach (var screen in newScreens)
                    {
                        db.NO_OF_SCREENS_LOG.Add(new NO_OF_SCREENS_LOG
                        {
                            T_ID = tId,
                            SCREEN_ID = screen.SCREEN_ID, // ✅ Now valid because SaveChanges() was called
                            AUDIENCE_CAPACITY = screen.AUDIENCE_CAPACITY,
                            SCREEN_TYPE = screen.SCREEN_TYPE,
                            ACTION_TYPE = "INSERT",
                            CHANGED_BY = "System",
                            CHANGED_ON = DateTime.Now
                        });
                    }

                    db.SaveChanges(); // ✅ Save logs after inserting new screens

                    transaction.Commit(); // ✅ Commit transaction if all steps succeed
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // 🔥 Rollback on error to maintain consistency
                    throw; // 🔍 Debug to find root cause
                }
            }
        }



        private void UpdateTheaterStatus(int tId, string actionType, string rejectReason)
        {
            var existingRegistration = db.TRN_REGISTRATION.Find(tId);
            if (existingRegistration != null)
            {
                if (actionType == "Edit")
                {
                    existingRegistration.STATUS = "Pending"; // Ensure "Edit" sets status to Pending
                }
                else if (actionType == "Approve")
                {
                    existingRegistration.STATUS = "Approved";
                    existingRegistration.REJECT_REASON = null; // Clear any previous rejection reason
                                                               // Assign REG_ID if not already assigned
                    if (string.IsNullOrEmpty(existingRegistration.REG_ID))
                    {
                        existingRegistration.REG_ID = GenerateNextRegId();
                    }

                }
                else if (actionType == "Reject")
                {
                    existingRegistration.STATUS = "Rejected";
                    existingRegistration.REJECT_REASON = rejectReason; // Store the rejection reason
                    if (!string.IsNullOrEmpty(existingRegistration.REG_ID))
                    {
                        existingRegistration.REG_ID =null;
                    }
                }

                existingRegistration.UPDATE_USER = "System";
                existingRegistration.UPDATE_DATE = DateTime.Now;

                db.Entry(existingRegistration).State = EntityState.Modified;
                db.SaveChanges();
            }

        }



        // Method to generate the next REG_ID
        private string GenerateNextRegId()
        {
            var lastReg = db.TRN_REGISTRATION
                .Where(r => r.REG_ID.StartsWith("T"))
                .OrderByDescending(r => r.REG_ID)
                .Select(r => r.REG_ID)
                .FirstOrDefault();

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(lastReg) && lastReg.Length > 1)
            {
                string numberPart = lastReg.Substring(1); // Extract number part (e.g., "0001")
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"T{nextNumber:D4}"; // Format as T0001, T0002, etc.
        }






        private void UpdateRegistration(TRN_REGISTRATION existingRegistration, TRN_REGISTRATION model)
        {
            existingRegistration.T_NAME = model.T_NAME;
            existingRegistration.T_ADDRESS = model.T_ADDRESS;
            existingRegistration.T_OWNER_NAME = model.T_OWNER_NAME;
            existingRegistration.T_OWNER_NUMBER = model.T_OWNER_NUMBER;
            existingRegistration.T_OWNER_EMAIL = model.T_OWNER_EMAIL;
            existingRegistration.T_COMMENCEMENT_DATE = model.T_COMMENCEMENT_DATE;
            existingRegistration.T_CITY = model.T_CITY;
            existingRegistration.T_ZONE = model.T_ZONE;
            existingRegistration.T_WARD = model.T_WARD;
            existingRegistration.T_TENAMENT_NO = model.T_TENAMENT_NO;
            existingRegistration.T_PEC_NO = model.T_PEC_NO;
            existingRegistration.T_PRC_NO = model.T_PRC_NO;
            existingRegistration.T_TAX_PAYING_OFFLINE = model.T_TAX_PAYING_OFFLINE;
            existingRegistration.OFFLINE_TAX_PAYMENT = model.OFFLINE_TAX_PAYMENT;
            existingRegistration.OFFLINE_TAX_PAID_DATE = model.OFFLINE_TAX_PAID_DATE;
            existingRegistration.OFFLINE_DUE_DATE = model.OFFLINE_DUE_DATE;

            // Set STATUS to Pending when any update occurs
            existingRegistration.STATUS = "Pending";

            existingRegistration.UPDATE_USER = "System";
            existingRegistration.UPDATE_DATE = DateTime.Now;

            db.Entry(existingRegistration).State = EntityState.Modified;
        }


        private int CreateNewRegistration(TRN_REGISTRATION model)
        {
            var registration = new TRN_REGISTRATION
            {
                T_NAME = model.T_NAME,
                T_ADDRESS = model.T_ADDRESS,
                T_OWNER_NAME = model.T_OWNER_NAME,
                T_OWNER_NUMBER = model.T_OWNER_NUMBER,
                T_OWNER_EMAIL = model.T_OWNER_EMAIL,
                T_COMMENCEMENT_DATE = model.T_COMMENCEMENT_DATE,
                T_CITY = model.T_CITY,
                T_ZONE = model.T_ZONE,
                T_WARD = model.T_WARD,
                T_TENAMENT_NO = model.T_TENAMENT_NO,
                T_PEC_NO = model.T_PEC_NO,
                T_PRC_NO = model.T_PRC_NO,
                T_TAX_PAYING_OFFLINE = model.T_TAX_PAYING_OFFLINE,
                OFFLINE_TAX_PAYMENT = model.OFFLINE_TAX_PAYMENT,
                OFFLINE_TAX_PAID_DATE = model.OFFLINE_TAX_PAID_DATE,
                OFFLINE_DUE_DATE = model.OFFLINE_DUE_DATE,
                CREATE_USER = "System",
                CREATE_DATE = DateTime.Now,
                T_ACTIVE = true,
                STATUS = "Pending"
            };

            db.TRN_REGISTRATION.Add(registration);
            db.SaveChanges();

            return registration.T_ID;
        }


        private void HandleDocuments(int tId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    string uploadPath = Server.MapPath("~/UploadedFiles");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    foreach (var doc in db.MST_DOCS.Where(d => d.DOC_ACTIVE == true))
                    {
                        var uploadedFile = Request.Files[doc.DOC_ID.ToString()];
                        if (uploadedFile != null && uploadedFile.ContentLength > 0)
                        {
                            string fileExtension = Path.GetExtension(uploadedFile.FileName);

                            // Generate unique filename
                            int fileCount = db.TRN_THEATRE_DOCS.Count(d => d.T_ID == tId && d.DOC_ID == doc.DOC_ID) + 1;
                            string formattedFileName = $"TT_{doc.DOC_NAME}_{tId}_{fileCount}{fileExtension}";

                            formattedFileName = SanitizeFileName(formattedFileName);
                            string path = Path.Combine(uploadPath, formattedFileName);
                            uploadedFile.SaveAs(path);

                            // Fetch existing document to log and delete it before inserting the new one
                            var existingDoc = db.TRN_THEATRE_DOCS
                                .FirstOrDefault(d => d.T_ID == tId && d.DOC_ID == doc.DOC_ID);

                            if (existingDoc != null)
                            {
                                // Log OLD record before deletion
                                db.TRN_THEATRE_DOCS_LOG.Add(new TRN_THEATRE_DOCS_LOG
                                {
                                    T_ID = tId,
                                    TH_DOC_ID = existingDoc.TH_DOC_ID,
                                    DOC_FILEPATH = existingDoc.DOC_FILEPATH,
                                    CREATE_DATE = DateTime.Now,
                                    CREATE_USER = "System",
                                    ACTION_TYPE = "OLD_VALUE",
                                    CHANGED_BY = "System",
                                    CHANGED_ON = DateTime.Now
                                });

                                db.SaveChanges(); // Save log before deleting old file

                                // Remove old record from MAIN table
                                db.TRN_THEATRE_DOCS.Remove(existingDoc);
                                db.SaveChanges();
                            }

                            // Insert new document into MAIN table
                            var theatreDoc = new TRN_THEATRE_DOCS
                            {
                                T_ID = tId,
                                DOC_ID = doc.DOC_ID,
                                DOC_FILEPATH = path,
                                CREATE_USER = "System",
                                CREATE_DATE = DateTime.Now,
                                ACTIVE = true
                            };
                            db.TRN_THEATRE_DOCS.Add(theatreDoc);
                            db.SaveChanges(); // Generate TH_DOC_ID
                        }
                        else
                        {
                            ModelState.AddModelError(doc.DOC_NAME, $"Please upload the {doc.DOC_NAME} document.");
                        }
                    }

                    db.SaveChanges(); // Save all logs
                    transaction.Commit(); // Commit transaction
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Rollback if any error occurs
                    throw;
                }
            }
        }



        // Helper function to sanitize filenames
        private string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        private void LogOldValues(TRN_REGISTRATION oldRegistration)
        {
            db.TRN_REGISTRATION_LOG.Add(new TRN_REGISTRATION_LOG
            {
                T_ID = oldRegistration.T_ID,
                ACTION_TYPE = "UPDATE",
                T_NAME = oldRegistration.T_NAME,
                T_ADDRESS = oldRegistration.T_ADDRESS,
                T_OWNER_NAME = oldRegistration.T_OWNER_NAME,
                T_OWNER_NUMBER = oldRegistration.T_OWNER_NUMBER,
                T_OWNER_EMAIL = oldRegistration.T_OWNER_EMAIL,
                T_COMMENCEMENT_DATE = oldRegistration.T_COMMENCEMENT_DATE,
                T_CITY = oldRegistration.T_CITY,
                T_ZONE = oldRegistration.T_ZONE,
                T_WARD = oldRegistration.T_WARD,
                T_TENAMENT_NO = oldRegistration.T_TENAMENT_NO,
                T_PEC_NO = oldRegistration.T_PEC_NO,
                T_PRC_NO = oldRegistration.T_PRC_NO,
                T_TAX_PAYING_OFFLINE = oldRegistration.T_TAX_PAYING_OFFLINE,
                OFFLINE_TAX_PAYMENT = oldRegistration.OFFLINE_TAX_PAYMENT,
                OFFLINE_TAX_PAID_DATE = oldRegistration.OFFLINE_TAX_PAID_DATE,
                OFFLINE_DUE_DATE = oldRegistration.OFFLINE_DUE_DATE,
                UPDATE_DATE = DateTime.Now,
                UPDATE_USER = "System",
                LOG_TIMESTAMP = DateTime.Now,
                T_ACTIVE = oldRegistration.T_ACTIVE,
                STATUS = "Pending"
            });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(TRN_REGISTRATION model, string[] seatCapacity, string[] screenType, string action, string rejectReason)
        //{
        //    try
        //    {
        //        // Declare the registration variable
        //        TRN_REGISTRATION registration = null;

        //        // Check if T_ID exists
        //        TRN_REGISTRATION existingRegistration = db.TRN_REGISTRATION.Find(model.T_ID);

        //        if (existingRegistration != null) // If T_ID exists, update the existing registration
        //        {
        //            // If the action is Approve, set the status to Approved
        //            if (action == "Approve")
        //            {
        //                existingRegistration.STATUS = "Approved";
        //                existingRegistration.REJECTREASON = null;
        //            }
        //            else if (action == "Reject")
        //            {
        //                existingRegistration.STATUS = "Rejected";
        //                existingRegistration.REJECTREASON = rejectReason;
        //            }

        //            db.SaveChanges(); // Save to DBng
        //            //LogOldValues(existingRegistration);

        //            // Update the existing registration
        //            existingRegistration.T_NAME = model.T_NAME;
        //            existingRegistration.T_ADDRESS = model.T_ADDRESS;
        //            existingRegistration.T_OWNER_NAME = model.T_OWNER_NAME;
        //            existingRegistration.T_OWNER_NUMBER = model.T_OWNER_NUMBER;
        //            existingRegistration.T_OWNER_EMAIL = model.T_OWNER_EMAIL;
        //            existingRegistration.T_COMMENCEMENT_DATE = model.T_COMMENCEMENT_DATE;
        //            existingRegistration.T_CITY = model.T_CITY;
        //            existingRegistration.T_ZONE = model.T_ZONE;
        //            existingRegistration.T_WARD = model.T_WARD;
        //            existingRegistration.T_TENAMENT_NO = model.T_TENAMENT_NO;
        //            existingRegistration.T_PEC_NO = model.T_PEC_NO;
        //            existingRegistration.T_PRC_NO = model.T_PRC_NO;
        //            existingRegistration.T_TAX_PAYING_OFFLINE = model.T_TAX_PAYING_OFFLINE;
        //            existingRegistration.OFFLINE_TAX_PAYMENT = model.OFFLINE_TAX_PAYMENT;
        //            existingRegistration.OFFLINE_TAX_PAID_DATE = model.OFFLINE_TAX_PAID_DATE;
        //            existingRegistration.OFFLINE_DUE_DATE = model.OFFLINE_DUE_DATE;
        //            existingRegistration.UPDATE_USER = "System";
        //            existingRegistration.UPDATE_DATE = DateTime.Now;

        //            // Update the registration in the database
        //            db.Entry(existingRegistration).State = EntityState.Modified;


        //        }
        //        else // If T_ID does not exist, create a new registration
        //        {
        //            registration = new TRN_REGISTRATION
        //            {
        //                T_NAME = model.T_NAME,
        //                T_ADDRESS = model.T_ADDRESS,
        //                T_OWNER_NAME = model.T_OWNER_NAME,
        //                T_OWNER_NUMBER = model.T_OWNER_NUMBER,
        //                T_OWNER_EMAIL = model.T_OWNER_EMAIL,
        //                T_COMMENCEMENT_DATE = model.T_COMMENCEMENT_DATE,
        //                T_CITY = model.T_CITY,
        //                T_ZONE = model.T_ZONE,
        //                T_WARD = model.T_WARD,
        //                T_TENAMENT_NO = model.T_TENAMENT_NO,
        //                T_PEC_NO = model.T_PEC_NO,
        //                T_PRC_NO = model.T_PRC_NO,
        //                T_TAX_PAYING_OFFLINE = model.T_TAX_PAYING_OFFLINE,
        //                OFFLINE_TAX_PAYMENT = model.OFFLINE_TAX_PAYMENT,
        //                OFFLINE_TAX_PAID_DATE = model.OFFLINE_TAX_PAID_DATE,
        //                OFFLINE_DUE_DATE = model.OFFLINE_DUE_DATE,
        //                CREATE_USER = "System",
        //                CREATE_DATE = DateTime.Now,
        //                T_ACTIVE = true
        //            };

        //            // Add new registration to the database
        //            db.TRN_REGISTRATION.Add(registration);
        //            db.SaveChanges(); // Save changes to get the T_ID


        //            int tId = existingRegistration?.T_ID ?? registration?.T_ID ?? 0; // Ensure registration is not null

        //            // Handle Screens and insert logs
        //            var existingScreens = db.NO_OF_SCREENS.Where(s => s.T_ID == tId).ToList();
        //            db.NO_OF_SCREENS.RemoveRange(existingScreens);

        //            for (int i = 0; i < seatCapacity.Length; i++)
        //            {
        //                if (!string.IsNullOrEmpty(seatCapacity[i]) && !string.IsNullOrEmpty(screenType[i]))
        //                {
        //                    var screen = new NO_OF_SCREENS
        //                    {
        //                        T_ID = tId,
        //                        AUDIENCE_CAPACITY = int.Parse(seatCapacity[i]),
        //                        SCREEN_TYPE = screenType[i]
        //                    };
        //                    db.NO_OF_SCREENS.Add(screen);

        //                    // Insert log for screen insertion

        //                }
        //            }

        //            // Handle Documents and insert logs
        //            foreach (var doc in db.MST_DOCS.Where(d => d.DOC_ACTIVE == true))
        //            {
        //                var uploadedFile = Request.Files[doc.DOC_ID.ToString()];
        //                if (uploadedFile != null && uploadedFile.ContentLength > 0)
        //                {
        //                    string fileExtension = Path.GetExtension(uploadedFile.FileName);
        //                    string formattedFileName = $"TT_{doc.DOC_NAME}_{tId}{fileExtension}";
        //                    string uploadPath = Server.MapPath("~/UploadedFiles");

        //                    if (!Directory.Exists(uploadPath))
        //                    {
        //                        Directory.CreateDirectory(uploadPath);
        //                    }

        //                    formattedFileName = SanitizeFileName(formattedFileName);
        //                    string path = Path.Combine(uploadPath, formattedFileName);
        //                    uploadedFile.SaveAs(path);

        //                    db.TRN_THEATRE_DOCS.Add(new TRN_THEATRE_DOCS
        //                    {
        //                        T_ID = tId,
        //                        DOC_ID = doc.DOC_ID,
        //                        DOC_FILEPATH = path,
        //                        CREATE_USER = "System",
        //                        CREATE_DATE = DateTime.Now,
        //                        ACTIVE = true
        //                    });

        //                    // Insert log for document insertion
        //                    db.TRN_THEATRE_DOCS_LOG.Add(new TRN_THEATRE_DOCS_LOG
        //                    {
        //                        T_ID = tId,
        //                        TH_DOC_ID = db.TRN_THEATRE_DOCS.Max(d => d.TH_DOC_ID),
        //                        DOC_FILEPATH = path,
        //                        CREATE_DATE = DateTime.Now,
        //                        CREATE_USER = "System",
        //                        ACTION_TYPE = "INSERT",
        //                        CHANGED_BY = "System",
        //                        CHANGED_ON = DateTime.Now
        //                    });
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError(doc.DOC_NAME, $"Please upload the {doc.DOC_NAME} document.");
        //                }
        //            }

        //            db.SaveChanges();
        //            TempData["SuccessMessage"] = "Theatre registered successfully!";
        //            return RedirectToAction("SecondPage", "Home");


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
        //        Console.WriteLine(ex);
        //    }

        //    ViewBag.Documents = db.MST_DOCS.Where(d => d.DOC_ACTIVE == true).ToList();
        //    return View(model);
        //}

        //private void LogOldValues(TRN_REGISTRATION oldRegistration)
        //{
        //    db.TRN_REGISTRATION_LOG.Add(new TRN_REGISTRATION_LOG
        //    {
        //        T_ID = oldRegistration.T_ID,
        //        ACTION_TYPE = "UPDATE",
        //        T_NAME = oldRegistration.T_NAME,
        //        T_ADDRESS = oldRegistration.T_ADDRESS,
        //        T_OWNER_NAME = oldRegistration.T_OWNER_NAME,
        //        T_OWNER_NUMBER = oldRegistration.T_OWNER_NUMBER,
        //        T_OWNER_EMAIL = oldRegistration.T_OWNER_EMAIL,
        //        T_COMMENCEMENT_DATE = oldRegistration.T_COMMENCEMENT_DATE,
        //        T_CITY = oldRegistration.T_CITY,
        //        T_ZONE = oldRegistration.T_ZONE,
        //        T_WARD = oldRegistration.T_WARD,
        //        T_TENAMENT_NO = oldRegistration.T_TENAMENT_NO,
        //        T_PEC_NO = oldRegistration.T_PEC_NO,
        //        T_PRC_NO = oldRegistration.T_PRC_NO,
        //        T_TAX_PAYING_OFFLINE = oldRegistration.T_TAX_PAYING_OFFLINE,
        //        OFFLINE_TAX_PAYMENT = oldRegistration.OFFLINE_TAX_PAYMENT,
        //        OFFLINE_TAX_PAID_DATE = oldRegistration.OFFLINE_TAX_PAID_DATE,
        //        OFFLINE_DUE_DATE = oldRegistration.OFFLINE_DUE_DATE,
        //        UPDATE_DATE = DateTime.Now,
        //        UPDATE_USER = "System",
        //        LOG_TIMESTAMP = DateTime.Now,
        //        T_ACTIVE = oldRegistration.T_ACTIVE,
        //        STATUS = "Pending"
        //    });
        //}


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
    





    }
}
