﻿using Microsoft.EntityFrameworkCore;
using Natom.AccessMonitor.Core.Biz.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz
{
    public class MasterDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Zona> Zonas { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Docket> Dockets { get; set; }
        public DbSet<DocketRange> DocketRanges { get; set; }
        public DbSet<TipoDocumento> TiposDocumento { get; set; }


        //public DbSet<spPrecioGetResult> spPrecioGetResult { get; set; }

        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder
            //    .Entity<spBookingGetForCalendarResult>(builder => builder.HasNoKey())
            //    .Entity<spPropertiesGetForAgreementsResult>(builder => builder.HasNoKey());
        }
    }
}
