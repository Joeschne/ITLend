﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace API.Models;

[Index(nameof(Username), IsUnique = true)]
public class Student
{
    public int Id { get; set; }
    [Required]
    [MaxLength(30)]
    public string Username { get; set; }

    public ICollection<Booking> Bookings { get; set; }
}

