using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class DeptViewModel
    {
        [Required(ErrorMessage = "Please enter your Department Username.")]
        public string DepartmentUsername { get; set; }  // Changed from DEPARTMENT_ID

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}