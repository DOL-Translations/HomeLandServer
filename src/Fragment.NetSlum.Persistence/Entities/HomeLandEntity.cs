using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fragment.NetSlum.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fragment.NetSlum.Persistence.Entities;

public class HomeLandEntity : IConfigurableEntity<HomeLandEntity>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint HomeLandId { get; set; }
    
    public byte Status { get; set; }
    
    public int PlayerAccountId { get; set; }

    public uint LocalIp { get; set; }
    
    [MySqlCharSet("cp932")]
    [MySqlCollation("cp932_japanese_ci")]
    public required string HomeLandName { get; set; }

    public ushort Location { get; set; } //Country code

    public byte Countdown { get; set; }

    [MySqlCharSet("cp932")]
    [MySqlCollation("cp932_japanese_ci")]
    public required string Password { get; set; }

    [MySqlCharSet("cp932")]
    [MySqlCollation("cp932_japanese_ci")]
    public required string Comment { get; set; }

    public byte RegisteredPlayerCnt { get; set; }

    public byte MaxPlayerCnt { get; set; }

    public uint ClearCnt { get; set; }

    public ushort Latency { get; set; } // latency to matchmaking server
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime LastUpdate { get; set; }
    
    public byte Repeat { get; set; }
    
    public byte HeartbeatMode { get; set; }

    void IConfigurableEntity<HomeLandEntity>.Configure(EntityTypeBuilder<HomeLandEntity> entityBuilder)
    {
    }
}
