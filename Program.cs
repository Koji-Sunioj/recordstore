

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static System.Console;

namespace HelloWorld
{
    class Program
    {

        static void Main()
        {

            using (var db = new StoreContext())
            {
                IOrderedQueryable<Artist> artists = db.Artist.Include(a => a.Album).OrderBy(b => b.artist_id);

                bool buying = true;
                while (buying)
                {

                    WriteLine("welcome to the store here is what we have:\n");
                    ListAlbums(artists);
                    Write("\nenter an album number to put in your cart");
                    int album_id = int.Parse(ReadLine()!);

                    Album selected = db.Album.Where(a => a.album_id == album_id).First();

                    WriteLine($"you have selected:\n{selected.Artist!.name}-{selected.title}\n");

                }
            }
        }

        static void ListAlbums(IOrderedQueryable<Artist> artists)
        {
            if (artists.Count() > 0)
            {

                foreach (Artist artist in artists)
                {
                    WriteLine();
                    WriteLine($"{artist.name}, id: {artist.artist_id}");
                    if (artist.Album.Count() > 0)
                    {
                        string format = "\t{0,-2} {1,-30} {2,-10} {3,-7} {4}";
                        WriteLine("\n" + format, "id", "title", "released", "price", "stock");
                        var something = artist.Album.OrderBy(b => b.album_id);
                        foreach (Album album in something)
                        {
                            WriteLine(format, album.album_id, album.title, album.release_date, album.cost.ToString("€00.00"), album.stock);
                        }
                    }

                }
            }
            else
            {
                WriteLine("no artists created yet");
            }
        }
    }
}

