﻿using BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.DAL
{
    public class MeetingRoomContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<MeetingRoom> MeetingRooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


        public MeetingRoomContext(DbContextOptions opt) : base(opt)
        {

        }

        public MeetingRoomContext()
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}