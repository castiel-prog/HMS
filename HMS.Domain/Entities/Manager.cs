using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMS.Domain.Entities
{
    public class Manager
    {
        [Key]
        public int ManagerId { get; set; }


        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string PersonalNumber { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string EmployeeId { get; set; } = null!;

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Manager";


        // Foreign Key
        [ForeignKey(nameof(Hotel))]
        public int HotelId { get; set; }

        // Navigation Property
        public Hotel Hotel { get; set; } = null!;
    }
}
