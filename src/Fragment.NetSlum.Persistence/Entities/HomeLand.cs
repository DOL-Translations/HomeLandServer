using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fragment.NetSlum.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fragment.NetSlum.Persistence.Entities;

public class HomeLand : IConfigurableEntity<HomeLand>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint HomeLandId { get; set; }

    public uint ExternalIp { get; set; }
    
    [MySqlCharSet("cp932")]
    [MySqlCollation("cp932_japanese_ci")]
    public required string HomeLandName { get; set; }

    [MySqlCharSet("cp932")]
    [MySqlCollation("cp932_japanese_ci")]
    public required string Password { get; set; }

    [MySqlCharSet("cp932")]
    [MySqlCollation("cp932_japanese_ci")]
    public required string Comment { get; set; }

    public sbyte RegisteredPlayerCount { get; set; }

    public sbyte MaxPlayerCount { get; set; }

    public uint ClearCount { get; set; }

    public byte IsMostRecent { get; set; }

    public ushort Latency { get; set; } //move somewhere else?

    void IConfigurableEntity<HomeLand>.Configure(EntityTypeBuilder<HomeLand> entityBuilder)
    {
    }
}
