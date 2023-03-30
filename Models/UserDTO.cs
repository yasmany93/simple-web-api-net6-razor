﻿using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class UserDTO
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Gender { get; set; }
        [Required]
        public string Status { get; set; }
    }

     
}
