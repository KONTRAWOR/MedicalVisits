﻿using MedicalVisits.Models.Entities.ChatEntities;
using Microsoft.AspNetCore.Identity;

namespace MedicalVisits.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public ICollection<ChatParticipants> Chats { get; set; }
    public Address Address { get; set; }

    // Метод для оновлення адреси
    public void UpdateAddress(string city, string street, string building, string region, string apartment, string country)
    {
        Address = new Address(city, street, building, region, apartment, country);
    }
}
    