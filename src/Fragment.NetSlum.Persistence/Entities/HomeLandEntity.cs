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

    public uint LocalIp { get; set; }
    
    [MySqlCharSet("cp932")]
    [MySqlCollation("cp932_japanese_ci")]
    public required string HomeLandName { get; set; }

    public ushort Location { get; set; } //Country code

    public byte Time { get; set; } //Elapsed time?

    [MySqlCharSet("cp932")]
    [MySqlCollation("cp932_japanese_ci")]
    public required string Password { get; set; }

    [MySqlCharSet("cp932")]
    [MySqlCollation("cp932_japanese_ci")]
    public required string Comment { get; set; }

    public sbyte RegisteredPlayerCnt { get; set; }

    public sbyte MaxPlayerCnt { get; set; }

    public uint ClearCnt { get; set; }

    public byte IsMostRecent { get; set; }

    public ushort Latency { get; set; } //move somewhere else?

    void IConfigurableEntity<HomeLandEntity>.Configure(EntityTypeBuilder<HomeLandEntity> entityBuilder)
    {
    }
}
