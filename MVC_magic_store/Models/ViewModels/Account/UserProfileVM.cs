using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MVC_magic_store.Models.Data;
using System.ComponentModel;

namespace MVC_magic_store.Models.ViewModels.Account
{
    public class UserProfileVM
    {
        public UserProfileVM() { } // конструктор по умолчанию

        public UserProfileVM (UserDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAdress = row.EmailAdress;
            Username = row.Username;
            Password = row.Password;
        }

        public int Id { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [DisplayName("Email")]
        public string EmailAdress { get; set; }
        [Required]
        [DisplayName("Login")]
        public string Username { get; set; }
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}