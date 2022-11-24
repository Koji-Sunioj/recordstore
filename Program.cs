

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
            Cart cart = new();
            using (var db = new StoreContext())
            {
                IOrderedQueryable<Artist> artists = db.Artist.Include(a => a.Album).OrderBy(b => b.artist_id);
                bool buying = true;
                WriteLine("welcome to the store here is what we have:\n");
                while (buying)
                {
                    ListAlbums(artists);
                    Write("\nenter an option: add (album id), remove (album id), checkout: ");
                    // try
                    // {
                    string[] response = ReadLine()!.Split(" ");
                    int album_id;
                    Album selected;
                    switch (response[0].Trim().ToLower())
                    {
                        case "add":
                            album_id = int.Parse(response[1]);
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
                                    CartItem item = new() { album_id = selected.album_id, title = selected.title, Artist = selected.Artist, amount = 1, cost = selected.cost };
                                    cart.items.Add(item);
                                }

                                //db.SaveChanges();
                            }
                            else
                            {
                                WriteLine("\nNo stock left for that record!");
                            }

                            break;
                        case "remove":
                            album_id = int.Parse(response[1]);
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
                    ShowCart(cart);
                }
            }
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
                WriteLine("\n" + format2, "id", "title", "cost", "amount");
                foreach (CartItem album in cart.items)
                {
                    WriteLine(format2, album.album_id, album.title, album.cost.ToString("€00.00"), album.amount);
                }
            }
        }

        static void ListAlbums(IOrderedQueryable<Artist> artists)
        {
            if (artists.Count() > 0)
            {
                WriteLine("\n....available records....\n");
                foreach (Artist artist in artists)
                {
                    WriteLine();
                    WriteLine($"{artist.name}, id: {artist.artist_id}");
                    if (artist.Album.Count() > 0)
                    {

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

        static string format = "\t{0,-2} {1,-30} {2,-10} {3,-7} {4}";
        static string format2 = "\t{0,-2} {1,-30} {2,-10} {3,-7}";


    }
}




// int album_id = int.Parse(ReadLine()!);
// Album selected = db.Album.Where(a => a.album_id == album_id).First();
// if (selected.stock > 0)
// {
//     cart.bill += selected.cost;
//     if (cart.items.Any(a => a.album_id == selected.album_id))
//     {
//         cart.items.Where(w => w.album_id == selected.album_id).ToList().ForEach(s => s.amount++);
//     }
//     else
//     {
//         CartItem item = new() { album_id = selected.album_id, title = selected.title, Artist = selected.Artist, amount = 1, cost = selected.cost };
//         cart.items.Add(item);
//     }
//     selected.stock--;

//     //db.SaveChanges();
// }
