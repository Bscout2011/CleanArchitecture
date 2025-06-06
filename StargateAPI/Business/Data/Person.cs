﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StargateAPI.Business.Data;

public class Person
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public virtual AstronautDetail? AstronautDetail { get; set; }

    public virtual ICollection<AstronautDuty> AstronautDuties { get; set; } =
        new HashSet<AstronautDuty>();
}

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Person");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder
            .HasOne(z => z.AstronautDetail)
            .WithOne(z => z.Person)
            .HasForeignKey<AstronautDetail>(z => z.PersonId);
        builder
            .HasMany(z => z.AstronautDuties)
            .WithOne(z => z.Person)
            .HasForeignKey(z => z.PersonId);
    }
}
