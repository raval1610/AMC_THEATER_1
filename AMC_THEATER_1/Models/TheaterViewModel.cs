using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class TheaterViewModel
    {
        public int? T_ID { get; set; }
        public string Display_T_ID => STATUS?.Equals("Approved", StringComparison.OrdinalIgnoreCase) == true ?
                                      (T_ID?.ToString() ?? "NOT GENERATED") : "NOT GENERATED";
        public string T_NAME { get; set; }
        public string T_CITY { get; set; }
        public string T_TENAMENT_NO { get; set; }
        public string T_WARD { get; set; }
        public string T_ZONE { get; set; }
        public string STATUS { get; set; }  // Remove the duplicate "Status" property
        public string REJECTREASON { get; set; }
        public string T_OWNER_NAME { get; set; }
        public string T_OWNER_NUMBER { get; set; }
        public string T_OWNER_EMAIL { get; set; }
        public string REG_ID { get; set; }

        public DateTime T_COMMENCEMENT_DATE { get; set; }

        public string T_ADDRESS { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public int SCREEN_COUNT { get; set; } // New field for screen count
        public int THEATER_SCREEN_COUNT { get; set; } // Separate count for Theater
        public int VIDEO_THEATER_SCREEN_COUNT { get; set; } // Separate count for Video Theater
        public List<ScreenViewModel> Screens { get; set; }
        // ✅ New Property for SCREEN TYPE
        public string SCREEN_TYPE { get; set; }

        public bool IsEditMode { get; set; }

        public List<TRN_SCREEN_TAX_PRICE> ScreenTypes { get; set; }

        // ✅ Method to Get SCREEN_TYPE from `ScreenTypes` List
        public string GetScreenType()
        {
            return ScreenTypes?.FirstOrDefault()?.SCREEN_TYPE ?? "N/A";
        }


    }




}