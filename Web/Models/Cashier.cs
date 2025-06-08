namespace Web.Models
{
    public class Cashier : Employee
    {
        public string Role = "Cashier" ;
        public Cashier(string workPlace, int shift, double salary, string name, DateTime birth, string gender,
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
        public Cashier() { }
    }
}
