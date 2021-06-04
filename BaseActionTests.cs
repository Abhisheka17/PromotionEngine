using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromotionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionEngine.Tests
{
    [TestClass()]
    public class BaseActionTests
    {
        [TestMethod()]
        public void AddProductTest()
        {
            List<Product> products = new List<Product>();
            BaseAction action = new BaseAction();
            action.AddProduct(products, "A", 50, 1);
            Assert.AreEqual(1, products.Count);
            Assert.AreEqual(products.Any(x => x.SKUID == "A"), true);
        }

        [TestMethod()]
        public void AddPromotionTest()
        {
            List<Promotion> promotions = new List<Promotion>();
            BaseAction action = new BaseAction();
            action.AddPromotion(promotions, new Product("A", 50, 1), 0);
            action.AddPromotion(promotions, new Product("B", 40, 1), 1);
            action.AddPromotion(promotions, new Product("V", 30, 1), 1);

            Assert.AreEqual(3, promotions.Count);
            Assert.AreEqual(promotions.Count(x => x.GroupId == 1), 2);
            Assert.AreEqual(promotions.Count(x => x.GroupId == 0), 1);
            Assert.AreEqual(promotions.Any(x => x.GroupId == 1), true);
        }

        [TestMethod()]
        public void AddItemInCartTest()
        {
            List<Promotion> promotions = new List<Promotion>();
            List<Product> products = new List<Product>();
            Cart c1 = new Cart();
            c1.Products = new List<Product>();
            BaseAction action = new BaseAction();
            action.AddPromotion(promotions, new Product("A", 3, 50), 0);
            action.AddPromotion(promotions, new Product("B", 1, 0), 1);
            action.AddPromotion(promotions, new Product("V", 1, 30), 1);

            action.AddProduct(products, "A", 1, 20);
            action.AddProduct(products, "B", 1, 10);
            action.AddProduct(products, "V", 1, 30);

            c1.Products.Add(new Product("A", 10, 0));
            c1.Products.Add(new Product("B", 20, 0));
            c1.Products.Add(new Product("V", 30, 0));

            Assert.AreEqual(3, promotions.Count);
            Assert.AreEqual(3, products.Count);
            Assert.AreEqual(3, c1.Products.Count);
            Assert.AreEqual(30, c1.Products.FirstOrDefault(x => x.SKUID == "V").Quantity);


        }

        [TestMethod()]
        public void CalculateTotalTest()
        {
            List<Promotion> promotions = new List<Promotion>();
            List<Product> products = new List<Product>();
            Cart c1 = new Cart();
            c1.Products = new List<Product>();
            BaseAction action = new BaseAction();
            action.AddPromotion(promotions, new Product("A", 3, 130), 0);
            action.AddPromotion(promotions, new Product("B", 2, 45), 1);
            action.AddPromotion(promotions, new Product("C", 1, 0), 2);
            action.AddPromotion(promotions, new Product("D", 1, 30), 2);


            action.AddProduct(products, "A", 50, 1);
            action.AddProduct(products, "B", 30, 1);
            action.AddProduct(products, "C", 20, 1);
            action.AddProduct(products, "D", 15, 1);

            //Scenario A
            c1.Products.Add(new Product("A", 1, 0));
            c1.Products.Add(new Product("B", 1, 0));
            c1.Products.Add(new Product("C", 1, 0));

            int scenarioAAmt=action.CalculateTotal(c1, promotions, products);
            Assert.AreEqual(100, scenarioAAmt);
            //Scenario B
            c1.Products = new List<Product>();
            c1.Products.Add(new Product("A", 5, 0));
            c1.Products.Add(new Product("B", 5, 0));
            c1.Products.Add(new Product("C", 1, 0));

            int scenarioBAmt = action.CalculateTotal(c1, promotions, products);

            Assert.AreEqual(370, scenarioBAmt);

            //Scenario c
            c1.Products = new List<Product>();
            c1.Products.Add(new Product("A", 3, 0));
            c1.Products.Add(new Product("B", 5, 0));
            c1.Products.Add(new Product("C", 1, 0));
            c1.Products.Add(new Product("D", 1, 0));

            int scenarioCAmt = action.CalculateTotal(c1, promotions, products);

            Assert.AreEqual(280, scenarioCAmt);


        }
    }
}