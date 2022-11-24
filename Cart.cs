
namespace TheCart
{

    public class Cart
    {
        public List<CartItem> items = new();

        public decimal bill { get; set; } = 0;

        public void Init()
        {
            items = new();
            bill = 0;
        }
    }

    public class CartItem : Album
    {
        public int amount { get; set; }
    }
}