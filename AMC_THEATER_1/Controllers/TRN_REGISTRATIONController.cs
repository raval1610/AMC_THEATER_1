using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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
        private THEATER_MODULEEntities1 db = new THEATER_MODULEEntities1();

        public ActionResult Edit(int id, string mode = "edit")
        {
            var model = db.TRN_REGISTRATION
                          .Include(r => r.NO_OF_SCREENS)
                          .FirstOrDefault(r => r.T_ID == id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "No record found!";
                return RedirectToAction("List_of_Application", "Home");
            }

            // ✅ Ensure lists are initialized
            model.NO_OF_SCREENS = model.NO_OF_SCREENS ?? new List<NO_OF_SCREENS>();
            model.TRN_THEATRE_DOCS = model.TRN_THEATRE_DOCS ?? new List<TRN_THEATRE_DOCS>();

            // ✅ Fetch active documents
            ViewBag.Documents = GetActiveDocuments() ?? new List<MST_DOCS>();

            // ✅ Fetch uploaded documents
            ViewBag.UploadedDocs = GetUploadedDocs(id);

            // ✅ Ensure ViewBag.Mode is properly set
            ViewBag.Mode = mode;
            model.IsEditMode = (mode == "edit");

            return View("Registration", model);
        }

        //public ActionResult Registration(int? id, bool isViewPage = false, string mode = "create")
        //{
        //    ViewBag.PageTitle = "Theater Registration";
        //    ViewBag.Mode = mode; // ✅ Set mode initially

        //    ViewBag.Documents = GetActiveDocuments() ?? new List<MST_DOCS>();

        //    var model = new TRN_REGISTRATION
        //    {
        //        NO_OF_SCREENS = new List<NO_OF_SCREENS> { new NO_OF_SCREENS { SCREEN_ID = 1 } },
        //        TRN_THEATRE_DOCS = new List<TRN_THEATRE_DOCS>()
        //    };

        //    if (id.HasValue)
        //    {
        //        var registrationData = GetRegistrationData(id.Value);
        //        if (registrationData == null || registrationData.T_ID == 0)
        //        {
        //            TempData["ErrorMessage"] = "No registration data found for the provided ID.";
        //            return RedirectToAction("Index");
        //        }

        //        ViewBag.Screens = registrationData.NO_OF_SCREENS ?? new List<NO_OF_SCREENS>();

        //        // ✅ Fetch Uploaded Documents
        //        ViewBag.UploadedDocs = GetUploadedDocs(id.Value);

        //        // 🔥 Debugging: Print uploaded docs in Output Window

        //        model = registrationData;

        //        //if (mode == "create")
        //        //{
        //        //    mode = isViewPage ? "view" : "edit";
        //        //}
        //    }

        //    ViewBag.Mode = mode; // ✅ Correctly assign mode
        //    return View(model);
        //}
        public ActionResult Registration(int? id, bool isViewPage = false, string mode = "create")
        {
            ViewBag.PageTitle = "Theater Registration";
            ViewBag.Mode = mode;
            ViewBag.Documents = GetActiveDocuments() ?? new List<MST_DOCS>();

            // Fetch months from the database
            ViewBag.Months = GetMonthsList();

            var model = new TRN_REGISTRATION
            {
                NO_OF_SCREENS = new List<NO_OF_SCREENS> { new NO_OF_SCREENS { SCREEN_ID = 1 } },
                TRN_THEATRE_DOCS = new List<TRN_THEATRE_DOCS>()
            };

            if (id.HasValue)
            {
                var registrationData = GetRegistrationData(id.Value);
                if (registrationData == null || registrationData.T_ID == 0)
                {
                    TempData["ErrorMessage"] = "No registration data found for the provided ID.";
                    return RedirectToAction("Index");
                }

                ViewBag.Screens = registrationData.NO_OF_SCREENS ?? new List<NO_OF_SCREENS>();
                ViewBag.UploadedDocs = GetUploadedDocs(id.Value);
                model = registrationData;
            }

            return View(model);
        }

        private List<SelectListItem> GetMonthsList()
        {
            return Enumerable.Range(1, 12)
                .Select(i => new SelectListItem
                {
                    Value = i.ToString("D2"),  // 01, 02, ..., 12
                    Text = new DateTime(2000, i, 1).ToString("MMMM")  // January, February, etc.
                }).ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(TRN_REGISTRATION model, HttpPostedFileBase[] documents, string actionType, string rejectReason = null)
        {
            try
            {

                if (model.T_ID == 0)
                {
                    // ✅ Set default values before inserting
                    model.T_ACTIVE = true;
                    model.STATUS = "Pending";
                    // Insert new record into TRN_REGISTRATION
                    db.TRN_REGISTRATION.Add(model);


                    db.SaveChanges(); // ✅ Save first to generate T_ID
                }
                else
                {
                    // Fetch existing entity from the database
                    var existingEntity = db.TRN_REGISTRATION.Find(model.T_ID);
                    if (existingEntity == null)
                    {
                        TempData["ErrorMessage"] = "No record found for the provided ID.";
                        return View(model);
                    }

                    // ✅ Update existing entity with new values
                    db.Entry(existingEntity).CurrentValues.SetValues(model);

                    // 🧹 Clear existing child records before adding new ones
                    db.NO_OF_SCREENS.RemoveRange(db.NO_OF_SCREENS.Where(d => d.T_ID == model.T_ID));

                    // 🆕 Add new child records if available
                    if (model.NO_OF_SCREENS != null && model.NO_OF_SCREENS.Any())
                    {
                        db.NO_OF_SCREENS.AddRange(model.NO_OF_SCREENS);
                    }


                    UpdateTheaterStatus(existingEntity, actionType, rejectReason);

                    // ✅ Mark existing entity as modified
                    db.Entry(existingEntity).State = EntityState.Modified;
                    db.SaveChanges();
                }

                // ✅ Ensure documents are handled **after** T_ID is saved
                if (documents != null && documents.Length > 0)
                {
                    HandleDocuments(model.T_ID, documents);
                }
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
                return View(model);
            }
        }

        private TRN_REGISTRATION GetRegistrationData(int id)
        {
            return db.TRN_REGISTRATION.Include(r => r.NO_OF_SCREENS).FirstOrDefault(r => r.T_ID == id) ?? new TRN_REGISTRATION { NO_OF_SCREENS = new List<NO_OF_SCREENS>(), TRN_THEATRE_DOCS = new List<TRN_THEATRE_DOCS>() };
        }

        private List<NO_OF_SCREENS> GetScreens(int id)
        {
            return db.NO_OF_SCREENS.Where(s => s.T_ID == id).ToList() ?? new List<NO_OF_SCREENS>();
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
                .Where(doc => doc.DOC_ACTIVE == true)  // ✅ Fetch only active documents
                .AsNoTracking()
                .ToList();
        }

        private void HandleDocuments(int tId, HttpPostedFileBase[] documents)
        {
            if (documents == null || documents.Length == 0)
            {
                Console.WriteLine("No documents uploaded.");
                return; // Prevent NullReferenceException
            }

            // ✅ Step 1: Verify that tId exists in TRN_REGISTRATION
            bool tIdExists = db.TRN_REGISTRATION.Any(r => r.T_ID == tId);
            if (!tIdExists)
            {
                Console.WriteLine($"Error: T_ID {tId} does not exist in TRN_REGISTRATION.");
                return; // Prevent foreign key violation
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    string uploadPath = Server.MapPath("~/UploadedFiles");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var docList = db.MST_DOCS.Where(d => d.DOC_ACTIVE == true).ToList();
                    if (docList.Count == 0)
                    {
                        Console.WriteLine("Document list is empty in the database.");
                        return;
                    }

                    var existingDocs = db.TRN_THEATRE_DOCS.Where(d => d.T_ID == tId).ToList();

                    for (int i = 0; i < documents.Length; i++)
                    {
                        var uploadedFile = documents[i];
                        if (uploadedFile == null || uploadedFile.ContentLength <= 0)
                        {
                            Console.WriteLine($"Skipping file at index {i} - Null or empty.");
                            continue;
                        }

                        if (i >= docList.Count)
                        {
                            Console.WriteLine($"Skipping file at index {i} - No matching document in MST_DOCS.");
                            continue;
                        }

                        var doc = docList[i];

                        string fileExtension = Path.GetExtension(uploadedFile.FileName);
                        string formattedFileName = $"TT_{doc.DOC_NAME}_{tId}{fileExtension}";
                        formattedFileName = SanitizeFileName(formattedFileName);
                        string path = Path.Combine(uploadPath, formattedFileName);
                        uploadedFile.SaveAs(path);

                        var existingDoc = existingDocs.FirstOrDefault(d => d.DOC_ID == doc.DOC_ID);
                        if (existingDoc != null)
                        {
                            db.TRN_THEATRE_DOCS.Remove(existingDoc);
                            db.SaveChanges();
                        }

                        db.TRN_THEATRE_DOCS.Add(new TRN_THEATRE_DOCS
                        {
                            T_ID = tId,
                            DOC_ID = doc.DOC_ID,
                            DOC_FILEPATH = path,
                            CREATE_USER = "System",
                            CREATE_DATE = DateTime.Now,
                            ACTIVE = true
                        });

                        db.SaveChanges();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error in HandleDocuments: " + ex.Message);
                    throw;
                }
            }
        }

        private void HandleScreens(int tId, string[] seatCapacity = null, string[] screenType = null)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var existingScreens = db.NO_OF_SCREENS.Where(s => s.T_ID == tId).ToList();

                    if (existingScreens.Any())
                    {
                        db.NO_OF_SCREENS.RemoveRange(existingScreens);
                        db.SaveChanges();
                    }

                    if (seatCapacity != null && screenType != null && seatCapacity.Length == screenType.Length)
                    {
                        for (int i = 0; i < seatCapacity.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(seatCapacity[i]) && !string.IsNullOrEmpty(screenType[i]))
                            {
                                db.NO_OF_SCREENS.Add(new NO_OF_SCREENS
                                {
                                    T_ID = tId,
                                    AUDIENCE_CAPACITY = int.Parse(seatCapacity[i]),
                                    SCREEN_TYPE = screenType[i]
                                });
                            }
                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
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

        private void UpdateTheaterStatus(TRN_REGISTRATION existingRegistration, string actionType, string rejectReason)
        {
            var dbEntity = db.TRN_REGISTRATION.SingleOrDefault(r => r.T_ID == existingRegistration.T_ID);

            if (dbEntity == null)
            {
                throw new Exception($"❌ Error: Record with T_ID = {existingRegistration.T_ID} not found. It may have been deleted.");
            }

            db.Entry(dbEntity).Reload(); // ✅ Reload entity to avoid concurrency conflicts

            if (actionType == "Edit")
            {
                dbEntity.STATUS = "Pending";
            }
            else if (actionType == "Approve")
            {
                dbEntity.T_ACTIVE = true;
                dbEntity.STATUS = "Approved";
                dbEntity.REJECT_REASON = null;
              
                if (string.IsNullOrEmpty(dbEntity.REG_ID))
                {
                    dbEntity.REG_ID = GenerateNextRegId();
                }

            }
            else if (actionType == "Reject")
            {
                dbEntity.T_ACTIVE = true;
                dbEntity.STATUS = "Rejected";
                dbEntity.REJECT_REASON = rejectReason;
            }

            dbEntity.UPDATE_USER = "System";
            dbEntity.UPDATE_DATE = DateTime.Now;

            db.Entry(dbEntity).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Concurrency issue detected for T_ID: {existingRegistration.T_ID}. Retrying update...");

                // ✅ Reload entity and retry
                db.Entry(dbEntity).Reload();
                db.SaveChanges();
            }
        }

        private string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
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
