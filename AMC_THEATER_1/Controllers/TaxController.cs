using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using AMC_THEATER_1.Models;

namespace AMC_THEATER_1.Controllers
{
    public class TaxController : Controller
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=THEATER_MODULE;Integrated Security=True;";

        public ActionResult Index(int? tId)
        {
            if (!tId.HasValue)
            {
                return View();
            }

            TaxViewModel model = new TaxViewModel();
            model.TaxPayments = new List<TaxPayment>();
            model.Screens = new List<ScreenTax>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Fetch theater details
                    string theaterQuery = "SELECT * FROM TRN_REGISTRATION WHERE T_ID = @T_ID";
                    SqlCommand theaterCmd = new SqlCommand(theaterQuery, conn);
                    theaterCmd.Parameters.AddWithValue("@T_ID", tId.Value);
                    SqlDataReader theaterReader = theaterCmd.ExecuteReader();

                    if (theaterReader.Read())
                    {
                        model.TheaterName = theaterReader["THEATER_NAME"].ToString();
                        model.Location = theaterReader["LOCATION"].ToString();
                    }
                    theaterReader.Close();

                    // Fetch screens for the theater
                    string screenQuery = "SELECT * FROM NO_OF_SCREENS WHERE T_ID = @T_ID";
                    SqlCommand screenCmd = new SqlCommand(screenQuery, conn);
                    screenCmd.Parameters.AddWithValue("@T_ID", tId.Value);
                    SqlDataReader screenReader = screenCmd.ExecuteReader();

                    while (screenReader.Read())
                    {
                        model.Screens.Add(new ScreenTax
                        {
                            ScreenId = Convert.ToInt32(screenReader["SCREEN_ID"]),
                            ScreenName = screenReader["SCREEN_NAME"].ToString(),
                        });
                    }
                    screenReader.Close();

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error fetching data: " + ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(int T_ID, string Month, string Year, decimal TotalAmount, HttpPostedFileBase ShowStatement, List<ScreenTax> Screens)
        {
            string filePath = "";
            try
            {
                if (ShowStatement != null && ShowStatement.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ShowStatement.FileName);
                    filePath = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    ShowStatement.SaveAs(filePath);
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Insert into THEATER_TAX_PAYMENT
                    string insertTaxQuery = "INSERT INTO THEATER_TAX_PAYMENT (T_ID, MONTH, YEAR, TOTAL_AMOUNT, SHOW_STATEMENT) VALUES (@T_ID, @Month, @Year, @TotalAmount, @ShowStatement)";
                    SqlCommand taxCmd = new SqlCommand(insertTaxQuery, conn);
                    taxCmd.Parameters.AddWithValue("@T_ID", T_ID);
                    taxCmd.Parameters.AddWithValue("@Month", Month);
                    taxCmd.Parameters.AddWithValue("@Year", Year);
                    taxCmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                    taxCmd.Parameters.AddWithValue("@ShowStatement", filePath);
                    taxCmd.ExecuteNonQuery();

                    // Insert screen-wise data into NO_OF_SCREENS_TAX
                    foreach (var screen in Screens)
                    {
                        string insertScreenQuery = "INSERT INTO NO_OF_SCREENS_TAX (SCREEN_ID, T_ID, MONTH, YEAR, TOTAL_SHOW, CANCEL_SHOW, AMT_PER_SCREEN) VALUES (@ScreenId, @T_ID, @Month, @Year, @TotalShow, @CancelShow, @Amount)";
                        SqlCommand screenCmd = new SqlCommand(insertScreenQuery, conn);
                        screenCmd.Parameters.AddWithValue("@ScreenId", screen.ScreenId);
                        screenCmd.Parameters.AddWithValue("@T_ID", T_ID);
                        screenCmd.Parameters.AddWithValue("@Month", Month);
                        screenCmd.Parameters.AddWithValue("@Year", Year);
                        screenCmd.Parameters.AddWithValue("@TotalShow", screen.TotalShow);
                        screenCmd.Parameters.AddWithValue("@CancelShow", screen.CancelShow);
                        screenCmd.Parameters.AddWithValue("@Amount", screen.Amount);
                        screenCmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }

                TempData["SuccessMessage"] = "Tax payment added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error inserting data: " + ex.Message;
            }

            return RedirectToAction("Index", new { tId = T_ID });
        }
    }

    public class TaxViewModel
    {
        public string TheaterName { get; set; }
        public string Location { get; set; }
        public List<TaxPayment> TaxPayments { get; set; }
        public List<ScreenTax> Screens { get; set; }
    }

    public class ScreenTax
    {
        public int ScreenId { get; set; }
        public string ScreenName { get; set; }
        public int TotalShow { get; set; }
        public int CancelShow { get; set; }
        public decimal Amount { get; set; }
    }
}
