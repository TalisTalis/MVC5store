using MVC_magic_store.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_magic_store.Models.ViewModels.Account
{
    public class UserVM
    {
        public UserVM() { } // конструктор по умолчанию

        public UserVM(UserDTO row)
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
        [DataType(DataType.EmailAddress)] // типа данных email
        [DisplayName("Email")]
        public string EmailAdress { get; set; }
        [Required]
        [DisplayName("Login")]
        public string Username { get; set; }
        [Required]
        //[DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        //[DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}