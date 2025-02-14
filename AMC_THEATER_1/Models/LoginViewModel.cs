using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string UN { get; set; } // Username or phone number

        [Required(ErrorMessage = "Password is required")]
        public string PASS{ get; set; }

    }
}