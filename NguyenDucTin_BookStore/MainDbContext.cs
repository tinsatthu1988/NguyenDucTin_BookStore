using Microsoft.EntityFrameworkCore;
using NguyenDucTin_BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NguyenDucTin_BookStore
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {

        }
        public DbSet<BookModel> Books { get; set; }
    }
}
