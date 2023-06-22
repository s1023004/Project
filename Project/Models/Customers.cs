using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    [TableAttribute(name:"Customers")]
    public class Customers
    {
        [KeyAttribute]
        [ColumnAttribute(name:"CustomerID")]
        [MaxLengthAttribute(5)]
        [RequiredAttribute]
        public String customerId { set; get; }
        [ColumnAttribute(name: "CompanyName")]
        public String companyName { set; get; }
        [ColumnAttribute(name: "Address")]
        public String address { set; get; }
        [ColumnAttribute(name: "Phone")]
        public String? phone { set; get; }
        [ColumnAttribute(name: "Country")]
        public String? country { set; get; }
    }
}
