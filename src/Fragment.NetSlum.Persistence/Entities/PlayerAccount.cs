using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fragment.NetSlum.Persistence.Entities;

public class PlayerAccount
{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[MaxLength(32)]
		public required string SaveId { get; set; }
		
		[MaxLength(60)]
		public string? PasswordHash { get; set; }
		
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? LastLogin { get; set; } = null;
		
		[MaxLength(16)]
		public string? Unk1_16 { get; set;}
		
		public byte? Unk2_1 { get; set; }
		public byte? Unk3_1 { get; set; }
}
