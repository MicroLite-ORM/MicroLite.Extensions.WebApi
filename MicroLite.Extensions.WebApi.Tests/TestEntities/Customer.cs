using System;

namespace MicroLite.Extensions.WebApi.Tests.TestEntities
{
    public class Customer
    {
        public DateTime Created { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Forename { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Reference { get; set; }

        public CustomerStatus Status { get; set; }

        public string Surname { get; set; }
    }
}
