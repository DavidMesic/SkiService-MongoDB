﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Auftrag
{
    // Mit [BsonId] wird dieses Feld als Primärschlüssel in MongoDB genutzt.
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? AuftragID { get; set; }

    [BsonElement("account_id"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string? KundeID { get; set; }

    // Dieses Feld wird nicht in der Datenbank gespeichert, sondern dient nur der internen Nutzung.
    [BsonIgnore]
    [JsonIgnore]
    public Account? Kunde { get; set; }

    [Required]
    [MaxLength(50)]
    [BsonElement("service"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Dienstleistung { get; set; }

    [Required]
    [Range(1, 3)]
    [BsonElement("priority"), BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
    public int Priorität { get; set; }

    [MaxLength(20)]
    [JsonIgnore]
    [BsonElement("status"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Status { get; set; } = "Offen";

    [JsonIgnore]
    [BsonElement("created_at"), BsonRepresentation(MongoDB.Bson.BsonType.DateTime)]
    public DateTime ErstelltAm { get; set; } = DateTime.Now;
}
