using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using VultPay.DataBase.ShoppingCart;

namespace Tests
{
    public class Tests
    {
        IConfiguration configuration;

        [SetUp]
        public void Setup()
        {
            configuration = new ConfigurationBuilder()
                                .AddJsonFile(@"C:\temp\data\appsettings\default.json")
                                .Build();
        }

        [Test]
        public async Task CreateNewShoppingCart()
        {
            ShoppingCartDB cartDB = ShoppingCartDB.Create(configuration);
            string cartID = await cartDB.CreateShoppingCart();

            Assert.Pass();
        }

        [Test]
        public async Task AddItemToCart()
        {
            ShoppingCartDB cartDB = ShoppingCartDB.Create(configuration);
            await cartDB.AddItemToCart("5daa068dce0d256d805c7d7e", new VultPay.Domain.Models.Application.ShoppingCart.ShoppingCartItem
            {
                Id = "1538DD28",
                Name = "Bolsa Feminina",
                IsGift = false,
                Quantity = 1,
                UnitPrice = 10.0,
                Url = "https://www.bobstore.com.br/produto/bolsa-saco-bicolor/A-sku4222879.O19C5A103?gclid=CjwKCAjwxaXtBRBbEiwAPqPxcKMsbV_6E7ycE4wD73BzJzoC15ZuiDUoffMltCLy5sGzjCzDUaSMPhoCmscQAvD_BwE",
                Thumb = "https://d1xjvqax0h862y.cloudfront.net/BS/prod/O19C5A103/O19C5A103_B-54_product_1_698x970.jpg"
            });
            Assert.Pass();
        }

        [Test]
        public async Task UpdateItemQuantity()
        {
            ShoppingCartDB cartDB = ShoppingCartDB.Create(configuration);
            await cartDB.UpdateItemQuantity("5daa068dce0d256d805c7d7e", "1538DD28", 1);
        }

        [Test]
        public async Task RemoveItemFromCart()
        {
            ShoppingCartDB cartDB = ShoppingCartDB.Create(configuration);
            await cartDB.RemoveItemFromCart("5daa068dce0d256d805c7d7e", "1538DD28");
        }
    }
}