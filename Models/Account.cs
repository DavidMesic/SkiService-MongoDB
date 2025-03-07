﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Account
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? AccountID { get; set; }

    [Required]
    [MaxLength(50)]
    [BsonElement("username"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Username { get; set; }

    [Required]
    [MaxLength(100)]
    [BsonElement("email"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Email { get; set; }

    [Required]
    [BsonElement("password"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Password { get; set; }

    [MaxLength(20)]
    [BsonElement("phone"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Phone { get; set; } = null;

    [MaxLength(20)]
    [JsonIgnore]
    [BsonElement("role"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Role { get; set; } = "Kunde";
}
