

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static System.Console;
using TheCart;

namespace HelloWorld
{
    class Program
    {
        static void Main()
        {
            using (var db = new StoreContext())
            {
                Cart cart = new();
                IOrderedQueryable<Artist> artists = db.Artist.Include(a => a.Album).OrderBy(b => b.artist_id);
                bool buying = true;
                ConsoleAction(artists, cart);
                while (buying)
                {

                    Write("\nenter an option: add (album id), remove (album id), checkout: ");
                    try
                    {
                        string[] response = ReadLine()!.Split(" ");
                        int album_id = response.Length > 1 ? int.Parse(response[1]) : default;
                        Album selected;
                        switch (response[0].Trim().ToLower())
                        {
                            case "add":
                                selected = db.Album.Find(album_id)!;
                                if (selected.stock > 0)
                                {
                                    selected.stock--;
                                    cart.bill += selected.cost;
                                    if (CheckExisting(selected, cart))
                                    {
                                        cart.items.Where(w => w.album_id == selected.album_id).ToList().ForEach(s => s.amount++);
                                    }
                                    else
                                    {
                                        CartItem item = new() { album_id = selected.album_id, title = selected.title, Artist = selected.Artist, amount = 1, cost = selected.cost, release_date = selected.release_date };
                                        cart.items.Add(item);
                                        ConsoleAction(artists, cart);
                                    }

                                }
                                else
                                {
                                    WriteLine("\nNo stock left for that record!");
                                }

                                break;
                            case "remove":

                                selected = db.Album.Find(album_id)!;
                                if (CheckExisting(selected, cart))
                                {
                                    selected.stock++;
                                    cart.bill -= selected.cost;
                                    CartItem target = cart.items.Where(w => w.album_id == selected.album_id).First();
                                    target.amount--;
                                    if (target.amount == 0)
                                    {
                                        cart.items.Remove(target);
                                    }

                                    ConsoleAction(artists, cart);

                                }
                                else
                                {
                                    WriteLine("... that record doesn't exist in your cart...");
                                }
                                break;
                            case "checkout":
                                bool checkingout = true;
                                while (checkingout)
                                {
                                    Write("\nwhat's your email address? ");
                                    string email = ReadLine()!;
                                    EntityEntry<Invoice> something = db.Invoice.Add(new() { email = email });
                                    db.SaveChanges();
                                    foreach (CartItem item in cart.items)
                                    {
                                        db.InvoiceItems.Add(new() { album_id = item.album_id, amount = item.amount, invoice_id = something.Entity.invoice_id });
                                    }
                                    db.SaveChanges();
                                    checkingout = false;
                                }
                                cart.Init();
                                WriteLine("\n... changes saved...\n");
                                break;

                            default:
                                WriteLine("huh");
                                break;
                        }

                    }
                    catch
                    {
                        WriteLine("huh?");
                    }
                }
            }
        }

        static void ConsoleAction(IOrderedQueryable<Artist> artists, Cart cart)
        {
            Console.Clear();
            ListAlbums(artists);
            ShowCart(cart);

        }

        static bool CheckExisting(Album selected, Cart cart)
        {
            return cart.items.Any(a => a.album_id == selected.album_id);
        }

        static void ShowCart(Cart cart)
        {
            if (cart.items.Count() > 0)
            {

                WriteLine("\n...you cart, total {0} ...\n", cart.bill.ToString("€###.00"));
                WriteLine(format, "artist", "title", "released", "id", "price", "amount");
                WriteLine(line);
                foreach (CartItem album in cart.items)
                {
                    WriteLine(format, album.Artist!.name, album.title, album.release_date, album.album_id, album.cost.ToString("€00.00"), album.amount);
                }
            }
        }

        static void ListAlbums(IOrderedQueryable<Artist> artists)
        {
            if (artists.Count() > 0)
            {
                WriteLine("\n....available records....\n");
                WriteLine(format, "artist", "title", "released", "id", "price", "stock");
                WriteLine(line);
                foreach (Artist artist in artists)
                {
                    if (artist.Album.Count() > 0)
                    {
                        IOrderedEnumerable<Album> ordered = artist.Album.OrderBy(b => b.album_id);
                        foreach (Album album in ordered)
                        {
                            WriteLine(format, album.Artist!.name, album.title, album.release_date, album.album_id, album.cost.ToString("€00.00"), album.stock);
                        }
                    }
                }
            }
            else
            {
                WriteLine("no artists created yet");
            }
        }

        static string line = new string('-', 92);
        static string format = "{0,-20} {1,-30} {2,-15} {3,-7} {4,-10} {5}";


    }
}

