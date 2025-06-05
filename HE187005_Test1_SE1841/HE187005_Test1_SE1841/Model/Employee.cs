using System.ComponentModel.DataAnnotations;
namespace HE187005_Test1_SE1841.Model
{
    public class Employee
    {
        [Key]
        public int EmpID { get; set; }

        [Required]
        public string EmpName { get; set; }

        [Required]
        public string IDCard { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }
    }


}
