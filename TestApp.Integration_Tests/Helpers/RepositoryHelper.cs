using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestApp.Domain;
using TestApp.Repo;

namespace TestApp.Integration_Tests.Helpers
{
    public class RepositoryHelper
    {
        public static void InitializeDbForTests(Context db)
        {
            RepositoryHelper helper = new RepositoryHelper();
            var cartSet = db.Set<Cart>();
            cartSet.AddRange(helper.GetCarts());
            var cartItemsSet = db.Set<CartItem>();
            cartItemsSet.AddRange(helper.GetCartItems());
            var categorySet = db.Set<Category>();
            categorySet.AddRange(helper.GetCategories());
            var itemSet = db.Set<Item>();
            itemSet.AddRange(helper.GetItems());
            var orderSet = db.Set<Order>();
            orderSet.AddRange(helper.GetOrders());
            
            var orderDMSet = db.Set<OrderDeliveryMethod>();
            orderDMSet.AddRange(helper.GetOrderDeliveryMethods());
            var orderItemSet = db.Set<OrderItem>();
            orderItemSet.AddRange(helper.GetOrderItems());
            var orderPMSet = db.Set<OrderPayMethod>();
            orderPMSet.AddRange(helper.GetOrderPayMethods());
            var usersSet = db.Set<User>();
            usersSet.AddRange(helper.GetUsers());
            var userRolesSet = db.Set<UserRole>();
            userRolesSet.AddRange(helper.GetUserRoles());
            
            var res = db.SaveChanges();
        }

        public static void ReinitializeDbForTests(Context db)
        {
            RepositoryHelper helper = new RepositoryHelper();
            var cartSet = db.Set<Cart>();
            cartSet.RemoveRange(cartSet.ToList());
            var cartItemsSet = db.Set<CartItem>();
            cartItemsSet.RemoveRange(cartItemsSet.ToList());
            var categorySet = db.Set<Category>();
            categorySet.RemoveRange(categorySet.ToList());
            var itemSet = db.Set<Item>();
            itemSet.RemoveRange(itemSet.ToList());
            var orderSet = db.Set<Order>();
            orderSet.RemoveRange(orderSet.ToList());

            var orderDMSet = db.Set<OrderDeliveryMethod>();
            orderDMSet.RemoveRange(orderDMSet.ToList());
            var orderItemSet = db.Set<OrderItem>();
            orderItemSet.RemoveRange(orderItemSet.ToList());
            var orderPMSet = db.Set<OrderPayMethod>();
            orderPMSet.RemoveRange(orderPMSet.ToList());
            var usersSet = db.Set<User>();
            usersSet.RemoveRange(usersSet.ToList());
            var userRolesSet = db.Set<UserRole>();
            userRolesSet.RemoveRange(userRolesSet.ToList());
            db.SaveChanges();
            InitializeDbForTests(db);
        }
        public List<User> GetUsers()
        {
            return new List<User>()
            {
                new User()
                {
                    Id = 1,
                    login = "Admin",
                    password = "X5KbOtT76NbKxf50L3oLYTgMBwO+7Q0/WNc8dZqapz4=",
                    phone = "123123123",
                    UserRoleId = 2
                },
                new User()
                {
                    Id = 2,
                    login = "test",
                    password = "test",
                    phone = "1231231111",
                    UserRoleId = 1
                }
            };
        }
        public List<UserRole> GetUserRoles()
        {
            return new List<UserRole>()
            {
                new UserRole()
                {
                    Id = 1,
                    name = "Users"
                },
                new UserRole()
                {
                    Id=2,
                    name="Admin"
                }
            };
        }
        public List<Cart> GetCarts()
        {
            return new List<Cart>()
            {
                new Cart()
                {
                    Id = 1,
                    UserId = 1
                },
                new Cart()
                {
                    Id = 2,
                    UserId = 2
                }
            };
        }
        public List<CartItem> GetCartItems()
        {
            return new List<CartItem>()
            {
                new CartItem()
                {
                    Id = 1,
                    quantity = 1,
                    CartId = 1,
                    ItemId = 1
                },
                new CartItem()
                {
                    Id = 2,
                    quantity = 2,
                    CartId = 1,
                    ItemId = 2
                }
            };
        }
        public List<Category> GetCategories()
        {
            return new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    name = "Zero cat"
                },
                new Category()
                {
                    Id = 2,
                    name = "First cat"
                }
            };
        }
        public List<Item> GetItems()
        {
            return new List<Item>()
            {
                new Item()
                {
                    description = "descr",
                    CategoryId = 1,
                    Id = 1,
                    imagePath = "//",
                    price = 10,
                    title = "Zero item"
                },
                 new Item()
                 {
                     description = "descr",
                     CategoryId = 1,
                     Id = 2,
                     imagePath = "//",
                     price = 14,
                     title = "First item"
                 },
                 new Item()
                 {
                     description = "descr",
                     CategoryId = 1,
                     Id = 3,
                     imagePath = "//",
                     price = 14,
                     title = "Second item"
                 }
            };
        }
        public List<OrderDeliveryMethod> GetOrderDeliveryMethods()
        {
            return new List<OrderDeliveryMethod>()
            {
                new OrderDeliveryMethod()
                {
                    Id = 1,
                    name = "Nova Poshta"
                },
                new OrderDeliveryMethod()
                {
                    Id = 2,
                    name = "Ne Nova Poshta"
                },
                new OrderDeliveryMethod()
                {
                    Id = 3,
                    name = "Vse taki nova poshta"
                }
            };
        }
        public List<OrderPayMethod> GetOrderPayMethods()
        {
            return new List<OrderPayMethod>()
            {
                new OrderPayMethod()
                {
                    Id = 1,
                    name = "Cash"
                },
                new OrderPayMethod()
                {
                    Id = 2,
                    name = "Cart"
                }
            };
        }
        public List<Order> GetOrders()
        {
            return new List<Order>()
            {
                new Order()
                {
                    DateCreate = new DateTime(2020, 12,31),
                    OrderDeliveryMethodId = 1,
                    OrderPayMethodId = 1,
                    Id = 1,
                    UserId = 1
                },
                 new Order()
                 {
                     DateCreate = new DateTime(2020, 12, 30),
                     OrderDeliveryMethodId = 1,
                     OrderPayMethodId = 1,
                     Id = 2,
                     UserId = 2
                 }
            };
        }
        public List<OrderItem> GetOrderItems()
        {
            return new List<OrderItem>()
            {
                new OrderItem()
                {
                    Id = 1,
                    ItemId = 1,
                    OrderId = 1,
                    quantity = 1
                },
                new OrderItem()
                {
                    Id = 2,
                    ItemId = 1,
                    OrderId = 1,
                    quantity = 2
                },
                 new OrderItem()
                 {
                     Id= 3,
                     ItemId = 1,
                     OrderId = 2,
                     quantity = 2
                 }
            };
        }
    }
}
