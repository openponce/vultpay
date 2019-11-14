using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VultPay.DataBase.Core;
using VultPay.Domain.Models.Application.ShoppingCart;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;

namespace VultPay.DataBase.ShoppingCart
{
    public class ShoppingCartDB : BaseDB
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public ShoppingCartDB(IConfiguration configuration) : base(configuration)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public static ShoppingCartDB Create(IConfiguration configuration)
        {
            return new ShoppingCartDB(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        public async Task<Domain.Models.Application.ShoppingCart.ShoppingCart> GetShoppingCart(string cartId)
        {
            try
            {
                var collection = MongoDatabaseInstance.GetCollection<Domain.Models.Application.ShoppingCart.ShoppingCart>(CartCollection);
                return (await collection.FindAsync(c => c.Id == ObjectId.Parse(cartId))).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> CreateShoppingCart()
        {
            try
            {
                Domain.Models.Application.ShoppingCart.ShoppingCart shoppingCart = new Domain.Models.Application.ShoppingCart.ShoppingCart();
                await CreateCollectionWithTTLAsync(CartCollection);
                var collection = MongoDatabaseInstance.GetCollection<Domain.Models.Application.ShoppingCart.ShoppingCart>(CartCollection);
                await collection.InsertOneAsync(shoppingCart);
                return shoppingCart.Id.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveShoppingCart(string cartId)
        {
            try
            {
                var cart = await GetShoppingCart(cartId);
                if (cart != null)
                {
                    var collection = MongoDatabaseInstance.GetCollection<Domain.Models.Application.ShoppingCart.ShoppingCart>(CartCollection);
                    var filter = Builders<Domain.Models.Application.ShoppingCart.ShoppingCart>.Filter.Eq(s => s.Id, ObjectId.Parse(cartId));
                    return (await collection.DeleteOneAsync(filter)).IsAcknowledged;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> AddItemToCart(string cartId, ShoppingCartItem item)
        {
            try
            {
                var cart = await GetShoppingCart(cartId);
                if (cart != null)
                {
                    var itemExists = cart.ShopItems.Exists(c => c.Id == item.Id);
                    //
                    if (itemExists)
                    {
                        item = cart.ShopItems.Where(c => c.Id == item.Id).FirstOrDefault();
                        cart.ShopItems.Remove(item);
                        item.Quantity += 1;
                        cart.ShopItems.Add(item);
                    }
                    else
                    {
                        cart.ShopItems.Add(item);
                    }
                    //
                    var filter = Builders<Domain.Models.Application.ShoppingCart.ShoppingCart>.Filter.Eq(s => s.Id, ObjectId.Parse(cartId));
                    var result = await MongoDatabaseInstance
                                        .GetCollection<Domain.Models.Application.ShoppingCart.ShoppingCart>(CartCollection)
                                        .ReplaceOneAsync(filter, cart);
                    //
                    return result.IsAcknowledged;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> UpdateItemQuantity(string cartId, string itemId, int quantity)
        {
            try
            {
                var cart = await GetShoppingCart(cartId);
                if (cart != null)
                {
                    if (quantity > 0)
                    {
                        //
                        var itemCart = cart.ShopItems.Where(c => c.Id == itemId).FirstOrDefault();
                        //
                        if (itemCart != null)
                        {
                            //
                            itemCart.Quantity = quantity;
                            //
                            cart.ShopItems.RemoveAll(c => c.Id == itemId);
                            cart.ShopItems.Add(itemCart);
                            //
                            var filter = Builders<Domain.Models.Application.ShoppingCart.ShoppingCart>.Filter.Eq(s => s.Id, ObjectId.Parse(cartId));
                            var result = await MongoDatabaseInstance
                                                .GetCollection<Domain.Models.Application.ShoppingCart.ShoppingCart>(CartCollection)
                                                .ReplaceOneAsync(filter, cart);
                            //
                            return result.IsAcknowledged;
                        }
                    }
                    else if (quantity <= 0)
                    {
                        return await RemoveItemFromCart(cartId, itemId);
                    }
                    else
                    {
                        return false;
                    }
                }
                //
                return false;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> RemoveItemFromCart(string cartId, string itemId)
        {
            try
            {
                var cart = await GetShoppingCart(cartId);
                if (cart != null)
                {
                    var itemExists = cart.ShopItems.Exists(c => c.Id == itemId);
                    if (itemExists)
                    {
                        var item = cart.ShopItems.Where(c => c.Id == itemId).FirstOrDefault();
                        //
                        if (item != null)
                        {
                            cart.ShopItems.RemoveAll(c => c.Id == item.Id);
                            //
                            var filter = Builders<Domain.Models.Application.ShoppingCart.ShoppingCart>.Filter.Eq(s => s.Id, ObjectId.Parse(cartId));
                            var result = await MongoDatabaseInstance
                                                    .GetCollection<Domain.Models.Application.ShoppingCart.ShoppingCart>(CartCollection)
                                                    .ReplaceOneAsync(filter, cart);
                            //
                            return result.IsAcknowledged;
                        }
                    }
                }
                //
                return false;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        private async Task CreateCollectionWithTTLAsync(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = await MongoDatabaseInstance.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            if (!(await collections.AnyAsync()))
            {
                var indexKeys = Builders<Domain.Models.Application.ShoppingCart.ShoppingCart>.IndexKeys;
                var indexModel = new CreateIndexModel<Domain.Models.Application.ShoppingCart.ShoppingCart>(indexKeys.Ascending("CreateAt"),
                                                                new CreateIndexOptions { ExpireAfter = new TimeSpan(60, 0, 0, 0) });
                await MongoDatabaseInstance.CreateCollectionAsync(CartCollection);
                await MongoDatabaseInstance.GetCollection<Domain.Models.Application.ShoppingCart.ShoppingCart>(CartCollection).Indexes.CreateOneAsync(indexModel);
            }
        }
    }
}