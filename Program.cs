using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionEngine
{
    class Program : StaticData
    {
        static void Main(string[] args)
        {
            StaticData sd1 = new StaticData();
            sd1.AddingProductsAndPromotions();
            CartScenario sa = new CartScenario();
            sa.CartScenarioA();
            int amountA = sd1.CalculateTotal(sa.cart1, sd1.promotions, sd1.products);
            Console.WriteLine("Total:" + amountA.ToString());
            Console.WriteLine();
            sa.CartScenarioB();
            int amountB = sd1.CalculateTotal(sa.cart1, sd1.promotions, sd1.products);
            Console.WriteLine("Total:" + amountB.ToString());
            Console.WriteLine();
            sa.CartScenarioC();
            int amountC = sd1.CalculateTotal(sa.cart1, sd1.promotions, sd1.products);
            Console.WriteLine("Total:" + amountC.ToString());
            Console.WriteLine();
            Console.ReadLine();
        }
    }
    public class Product
    {
        public string SKUID { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public Product(string skuId, int quantity, int price)
        {
            SKUID = skuId;
            Price = price;
            Quantity = quantity; ;
        }

        public Product()
        {
        }
    }
    public class Cart
    {
        public List<Product> Products { get; set; }
        public int Price { get; set; }
    }
    public class Promotion
    {
        public Product PromotionProduct { get; set; }
        public int GroupId { get; set; }
    }
    public class BaseAction
    {
        public bool AddProduct(List<Product> products, string skuId, int price, int quantity)
        {
            if (products.Any(x => x.SKUID.ToLower() == skuId.ToLower()))
            {
                return false;
            }
            else
            {
                products.Add(new Product()
                {
                    SKUID = skuId,
                    Price = price,
                    Quantity = quantity
                });
                return true;
            }
        }
        public void AddPromotion(List<Promotion> promotions, Product product, int groupId)
        {
            promotions.Add(new Promotion()
            {
                PromotionProduct = product,
                GroupId = groupId,
            });
        }
        public void AddItemInCart(Cart cart1, string skuId, int quantity)
        {
            cart1.Products.Add(new Product(skuId, quantity, 0));
        }
        public int CalculateTotal(Cart cart1, List<Promotion> promotions, List<Product> products)
        {
            Dictionary<string, StringBuilder> dicmessage = new Dictionary<string, StringBuilder>();
            int totalAmount = 0;
            if (cart1 != null && cart1.Products != null && cart1.Products.Any())
            {
                var cartPromotionItems = promotions.Where(x => cart1.Products.Any(y => x.PromotionProduct.SKUID == y.SKUID));
                var groupedPromotionItems = cartPromotionItems.GroupBy(p => p.GroupId, p => p.PromotionProduct, (key, g) => new { GroupId = key, PromotionProducts = g.ToList() });
                foreach (var item in groupedPromotionItems)
                {
                    if (item != null && item.PromotionProducts.Count() == 1)
                    {
                        var promotionProduct = item.PromotionProducts.FirstOrDefault();
                        var cartProduct = cart1.Products.FirstOrDefault(x => x.SKUID == promotionProduct.SKUID);
                        int promotionQty = 0;
                        if (cartProduct.Quantity > 1)
                        {
                            promotionQty = (cartProduct.Quantity / promotionProduct.Quantity);
                            cartProduct.Price = promotionQty * promotionProduct.Price;
                            AddMessage(dicmessage, cartProduct.SKUID, string.Format("{0}*{1}  {2}", cartProduct.Quantity, cartProduct.SKUID, cartProduct.Price));
                        }
                        int normalQty = cartProduct.Quantity == 1 ? 1 : (cartProduct.Quantity % promotionProduct.Quantity);
                        if (normalQty > 0)
                        {
                            var normalProduct = products.FirstOrDefault(x => x.SKUID == cartProduct.SKUID);
                            cartProduct.Price = cartProduct.Price + normalQty * normalProduct.Price;
                            if (promotionQty > 0)
                            {
                                AddMessage(dicmessage, cartProduct.SKUID, string.Format("{0}*{1}", normalQty, normalProduct.Price));
                            }
                            else
                            {
                                AddMessage(dicmessage, cartProduct.SKUID, string.Format("{0}*{1}  {2}*{3}", cartProduct.Quantity, cartProduct.SKUID, normalQty, normalProduct.Price));
                            }
                        }
                    }
                    totalAmount = cart1.Products.Sum(x => x.Price);
                    if (item != null && item.PromotionProducts.Count() > 1)
                    {
                        Dictionary<string, int> groupCal = new Dictionary<string, int>();
                        foreach (var item1 in item.PromotionProducts)
                        {
                            var cartProduct = cart1.Products.FirstOrDefault(x => x.SKUID == item1.SKUID);
                            if (cartProduct != null)
                            {
                                groupCal.Add(item1.SKUID, cartProduct.Quantity / item1.Quantity);
                            }
                            else
                            {
                                groupCal.Add(item1.SKUID, 0);
                            }
                        }
                        int maxgroup = groupCal.OrderBy(x => x.Value).Min(x => x.Value);
                        int totalMultipleSku = 0;
                        int quantityConsumed = 0;
                        foreach (var item1 in item.PromotionProducts)
                        {
                            var cartItem = cart1.Products.FirstOrDefault(x => x.SKUID == item1.SKUID);
                            if (maxgroup > 0)
                            {
                                quantityConsumed = (item1.Quantity * maxgroup);
                                totalMultipleSku = totalMultipleSku + maxgroup * item1.Price;
                                AddMessage(dicmessage, cartItem.SKUID, maxgroup * item1.Price == 0 ? string.Format("{0}*{1} -", cartItem.Quantity, cartItem.SKUID) : string.Format("{0}*{1} {2}", cartItem.Quantity, cartItem.SKUID, maxgroup * item1.Price));
                            }
                            var prod1 = products.FirstOrDefault(x => item1.SKUID == x.SKUID);
                            totalMultipleSku = totalMultipleSku + (cartItem.Quantity - quantityConsumed) * prod1.Price;
                            if (cartItem.Quantity - quantityConsumed > 0)
                            {
                                AddMessage(dicmessage, cartItem.SKUID, string.Format("{0}*{1} {2}", cartItem.Quantity - quantityConsumed, cartItem.SKUID, (cartItem.Quantity - quantityConsumed) * prod1.Price));
                            }
                        }
                        totalAmount = totalAmount + totalMultipleSku;
                    }
                }
            }
            PrintMessage(dicmessage);
            return totalAmount;
        }
        private void PrintMessage(Dictionary<string, StringBuilder> dicmessage)
        {
            foreach (var item in dicmessage)
            {
                Console.WriteLine(item.Value);
            }
        }
        private void AddMessage(Dictionary<string, StringBuilder> dicmessage, string key, string message)
        {
            if (dicmessage.ContainsKey(key))
            {
                dicmessage[key].Append("+");
                dicmessage[key].Append(message);
            }
            else
            {
                StringBuilder sb = new StringBuilder(message);
                dicmessage.Add(key, sb);
            }
        }
    }
    class StaticData : BaseAction
    {
        public List<Product> products = new List<Product>();
        public List<Promotion> promotions = new List<Promotion>();
        // Adding Product here
        public void AddingProductsAndPromotions()
        {
            AddProduct(products, "A", 50, 1);
            AddProduct(products, "B", 30, 1);
            AddProduct(products, "C", 20, 1);
            AddProduct(products, "D", 15, 1);

            //Adding Promotion
            AddPromotion(promotions, new Product("A", 3, 130), 1);
            AddPromotion(promotions, new Product("B", 2, 45), 2);
            AddPromotion(promotions, new Product("C", 1, 0), 3);
            AddPromotion(promotions, new Product("D", 1, 30), 3);
        }

    }
    class CartScenario : BaseAction
    {
        public Cart cart1 = new Cart();
        // Adding Product in cart
        public void CartScenarioA()
        {
            cart1.Products = new List<Product>();
            AddItemInCart(cart1, "A", 1);
            AddItemInCart(cart1, "B", 1);
            AddItemInCart(cart1, "C", 1);
        }
        public void CartScenarioB()
        {
            cart1.Products = new List<Product>();
            AddItemInCart(cart1, "A", 5);
            AddItemInCart(cart1, "B", 5);
            AddItemInCart(cart1, "C", 1);
        }
        public void CartScenarioC()
        {
            cart1.Products = new List<Product>();
            AddItemInCart(cart1, "A", 3);
            AddItemInCart(cart1, "B", 5);
            AddItemInCart(cart1, "C", 1);
            AddItemInCart(cart1, "D", 1);
        }
        public void CartScenarioD()
        {
            cart1.Products = new List<Product>();
            AddItemInCart(cart1, "A", 3);
            AddItemInCart(cart1, "B", 5);
            AddItemInCart(cart1, "C", 2);
            AddItemInCart(cart1, "D", 1);
        }
    }

}
