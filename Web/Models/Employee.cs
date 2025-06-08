using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Employee
    {

        [Key] public int employeeId { get; set; }
        public string name { get; set; }
        public DateTime birth { get; set; }
        public string gender { set; get; }
        public string address { get; set; }
        public string phone { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public virtual string Role { set; get; }
        public string WorkPlace { get; set; }
        public int Shift { get; set; }
        public double Salary { get; set; }
        public Employee()
        {
            name = "";
            birth = new DateTime(0, 0, 0);
            gender = "";
            address = "";
            phone = "";
            userName = "";
            password = "";
            WorkPlace = " ";
            Shift = 0;
            Salary = 0;
        }
        public Employee(string workPlace, int shift, double salary, string name,
            DateTime birth, string gender,
            string address, string phone, string userName, string password)
        {
            this.name = name;
            this.birth = birth;
            this.gender = gender;
            this.address = address;
            this.phone = phone;
            this.userName = userName;
            this.password = password;
            this.WorkPlace = workPlace;
            this.Shift = shift;
            this.Salary = salary;
        }
    }
}
