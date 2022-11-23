using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations; // [Required], [StringLength]
using System.ComponentModel.DataAnnotations.Schema;
public class StoreContext : DbContext
{
    public DbSet<Artist> Artist { get; set; }
    public DbSet<Album> Album { get; set; }

    public string DbPath { get; }

    public StoreContext()
    {

        DbPath = "/home/koji/Desktop/csharp/RecordStore.db";
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class Artist
{
    [Key]
    public int artist_id { get; set; }

    public string? name { get; set; }

    public List<Album> Album { get; } = new();
}

public class Album
{
    [Key]
    public int album_id { get; set; }
    public string? title { get; set; }
    public string? release_date { get; set; }

    public Decimal cost { get; set; }
    public int artist_id { get; set; }

    public int stock { get; set; }

    [ForeignKey("artist_id")]
    public Artist? Artist { get; set; }
}