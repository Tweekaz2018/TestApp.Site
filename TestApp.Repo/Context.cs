using Microsoft.EntityFrameworkCore;
using TestApp.Domain;

namespace TestApp.Repo
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasData(new UserRole()
            {
                Id = 1,
                name = "Admin"
            });
            modelBuilder.Entity<UserRole>().HasData(new UserRole()
            {
                Id = 2,
                name = "User"
            });
            modelBuilder.Entity<User>().HasData(new User()
            {
                Id = 1,
                login = "Admin",
                password = "FxPFTPF9lzc4xOwc/uYc5Z1e3kFSNk5Usg85K9YqkeE=",
                phone = "0635170358",
                UserRoleId = 1
            });
            modelBuilder.Entity<Cart>().HasData(new Cart()
            {
                Id = 1,
                UserId = 1
            });
            modelBuilder.Entity<OrderPayMethod>().HasData(new OrderPayMethod()
            {
                Id = 1,
                name = "Cash"
            });
            modelBuilder.Entity<OrderDeliveryMethod>().HasData(new OrderDeliveryMethod()
            {
                Id = 1,
                name = "Nova Poshta"
            });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 1, name = "Chocolate Candy" });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 2, name = "Fruit Candy" });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 3, name = "Gummy Candy" });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 4, name = "Halloween Candy" });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 5, name = "Hard Candy" }); 
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 1,
                title = "Assorted Chocolate Candy",
                price = 10,
                description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Cursus risus at ultrices mi tempus imperdiet nulla malesuada pellentesque. Tortor posuere ac ut consequat. Sagittis nisl rhoncus mattis rhoncus urna neque viverra justo. Lacus sed turpis tincidunt id aliquet risus feugiat in. Viverra aliquet eget sit amet tellus cras adipiscing enim eu.",
                CategoryId = 1,
                imagePath = "\\Images\\chocolateCandy.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 2,
                title = "Another Assorted Chocolate Candy",
                price = 15,
                description = "Venenatis tellus in metus vulputate eu scelerisque felis imperdiet proin. Quisque egestas diam in arcu cursus. Sed viverra tellus in hac. Quis commodo odio aenean sed adipiscing diam donec adipiscing.",
                CategoryId = 1,
                imagePath = "\\Images\\chocolateCandy2.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 3,
                title = "Another Chocolate Candy",
                price = 12,
                description = "Turpis egestas pretium aenean pharetra magna ac placerat vestibulum. Sed faucibus turpis in eu mi bibendum neque egestas. At in tellus integer feugiat scelerisque. Elementum integer enim neque volutpat ac tincidunt.",
                CategoryId = 1,
                imagePath = "\\Images\\chocolateCandy3.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 4,
                title = "Assorted Fruit Candy",
                price = 50,
                description = "Vitae congue eu consequat ac felis donec et. Praesent semper feugiat nibh sed pulvinar proin gravida hendrerit. Vel eros donec ac odio. A lacus vestibulum sed arcu non odio euismod lacinia at. Nisl suscipit adipiscing bibendum est ultricies integer. Nec tincidunt praesent semper feugiat nibh.",
                CategoryId = 2,
                imagePath = "\\Images\\fruitCandy.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 5,
                title = "Fruit Candy",
                price = 25,
                description = "Purus sit amet luctus venenatis lectus magna fringilla. Consectetur lorem donec massa sapien faucibus et molestie ac. Sagittis nisl rhoncus mattis rhoncus urna neque viverra.",
                CategoryId = 2,
                imagePath = "\\Images\\fruitCandy2.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 6,
                title = "Another Assorted Fruit Candy",
                price = 25.25,
                description = "Ultrices vitae auctor eu augue ut. Leo vel fringilla est ullamcorper eget. A diam maecenas sed enim ut. Massa tincidunt dui ut ornare lectus. Nullam non nisi est sit amet facilisis magna. ",
                CategoryId = 2,
                imagePath = "\\Images\\fruitCandy3.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 7,
                title = "Assorted Gummy Candy",
                price = 11,
                description = "Diam sit amet nisl suscipit adipiscing bibendum est ultricies integer. Molestie at elementum eu facilisis sed odio morbi quis commodo. Odio facilisis mauris sit amet massa vitae tortor condimentum lacinia. Urna porttitor rhoncus dolor purus non enim praesent elementum facilisis.",
                CategoryId = 3,
                imagePath = "\\Images\\gummyCandy.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 8,
                title = "Another Assorted Gummy Candy",
                price = 70,
                description = "Posuere ac ut consequat semper viverra nam libero justo laoreet. Ultrices dui sapien eget mi proin sed libero enim. Etiam non quam lacus suspendisse faucibus interdum. Amet nisl suscipit adipiscing bibendum est ultricies integer quis.",
                CategoryId = 3,
                imagePath = "\\Images\\gummyCandy2.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 9,
                title = "Gummy Candy",
                price = 3,
                description = "Ut ornare lectus sit amet est placerat in egestas. Iaculis nunc sed augue lacus viverra vitae. Bibendum ut tristique et egestas quis ipsum suspendisse ultrices gravida. Accumsan tortor posuere ac ut consequat semper viverra.",
                CategoryId = 3,
                imagePath = "\\Images\\gummyCandy3.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 10,
                title = "Halloween Candy",
                price = 22,
                description = "Vitae congue eu consequat ac felis donec et odio. Tellus orci ac auctor augue mauris augue. Feugiat sed lectus vestibulum mattis ullamcorper velit sed. Sit amet consectetur adipiscing elit pellentesque habitant morbi tristique senectus. Sed pulvinar proin gravida hendrerit lectus a.",
                CategoryId = 4,
                imagePath = "\\Images\\halloweenCandy.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 11,
                title = "Assorted Halloween Candy",
                price = 17,
                description = "Hac habitasse platea dictumst quisque sagittis purus sit. Dui nunc mattis enim ut. Mauris commodo quis imperdiet massa tincidunt nunc pulvinar sapien et.",
                CategoryId = 4,
                imagePath = "\\Images\\halloweenCandy2.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 12,
                title = "Another Halloween Candy",
                price = 8,
                description = "Pulvinar neque laoreet suspendisse interdum consectetur libero id faucibus. Ultrices vitae auctor eu augue ut lectus arcu bibendum at. Vulputate eu scelerisque felis imperdiet proin fermentum.",
                CategoryId = 4,
                imagePath = "\\Images\\halloweenCandy3.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 13,
                title = "Hard Candy",
                price = 50,
                description = "Vestibulum mattis ullamcorper velit sed ullamcorper morbi tincidunt ornare massa. Arcu cursus euismod quis viverra.",
                CategoryId = 5,
                imagePath = "\\Images\\hardCandy.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 14,
                title = "Another Hard Candy",
                price = 45,
                description = "Blandit massa enim nec dui nunc mattis enim ut tellus. Duis at consectetur lorem donec massa sapien faucibus et. At auctor urna nunc id cursus metus. Ut enim blandit volutpat maecenas volutpat blandit.",
                CategoryId = 5,
                imagePath = "\\Images\\hardCandy2.jpg",

            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 15,
                title = "Best Hard Candy",
                price = 70,
                description = "Nisi lacus sed viverra tellus in. Morbi non arcu risus quis varius quam quisque id. Cras adipiscing enim eu turpis egestas. Tristique nulla aliquet enim tortor. Quisque id diam vel quam. Id faucibus nisl tincidunt eget nullam.",
                CategoryId = 5,
                imagePath = "\\Images\\hardCandy3.jpg",

            });
        }
    }
}