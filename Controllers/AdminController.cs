using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace WebApplication.Controllers {
    public class AdminController : Controller {

        public struct User {
            public int Id;
            public string Username;
            public string FirstName;
            public string LastName;
        }
        List<User> UserList = new List<User>() { };

        public struct Product {
            public int Id;
            public string Name;
            public int Price;
            public string CreationYear;
            public string Certified;
        }
        public List<Product> ProductList = new List<Product>() { };

        [HttpGet]
        public ActionResult Index() {
            if (Session.Count != 0) {
                Response.Redirect("~/Home/Index");
                return View();
            }

            return View();
        }

        [HttpGet]
        public ActionResult Logout() {
            if (Session.Count != 0) {
                Session.Abandon();
                Response.Redirect("~/Home/Index");
                return View();
            }

            return View("Home");
        }

        [HttpPost]
        public ActionResult Index(string username, string password) {
            if (Session.Count != 0) {
                Response.Redirect("~/Home/Index");
                return View();
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) {
                ViewBag.ErrorMessage = "Empty username or password";
            }

            if (this.UserList.Count > 0)
                this.UserList.Clear();

            using (SqlConnection connection = new SqlConnection(Database.ConnectionString)) {
                string query = "SELECT id, username, first_name, last_name FROM webapp.dbo.users WHERE username = @username AND password = @password;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@username", sqlDbType: System.Data.SqlDbType.VarChar);
                command.Parameters.Add("@password", sqlDbType: System.Data.SqlDbType.VarChar);
                command.Parameters["@username"].Value = username;
                command.Parameters["@password"].Value = password;

                int rows = 0;
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        rows++;
                        this.UserList.Add(new User() {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3)
                        });
                    }
                }

                if (rows == 1) {
                    Session["Id"] = this.UserList[0].Id;
                    Session["Username"] = this.UserList[0].Username;
                    Session["FirstName"] = this.UserList[0].FirstName;
                    Session["LastName"] = this.UserList[0].LastName;

                    Response.Redirect("~/Management", false);
                    return View();

                } else if (rows > 1) {
                    ViewBag.ErrorMessage = "Something has gone wrong. Please try again later";
                    return View();
                } else {
                    ViewBag.ErrorMessage = "Invalid credential";
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult Management() {
            if (Session.Count == 0) {
                Response.Redirect("~/Home/Index");
            }

            GetProducts();
            return View();
        }

        [HttpPost]
        public ActionResult DeleteProduct(string id) {
            if (Session.Count == 0) {
                Response.Redirect("~/Home/Index");
                return View();
            }

            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id)) {
                ViewBag.InfoMessage = null;
                ViewBag.ErrorMessage = "Invalid action provided";

                GetProducts();
                return View("Management");
            }

            bool res = false;
            using (SqlConnection connection = new SqlConnection(Database.ConnectionString)) {
                string query = "DELETE FROM webapp.dbo.products WHERE id = @ID;";

                try {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@ID", sqlDbType: System.Data.SqlDbType.Int);
                    command.Parameters["@ID"].Value = id;
                    command.ExecuteNonQuery();
                    res = true;
                } catch (SqlException e) {
                    res = false;
                } finally {
                    connection.Close();
                }
            }

            if (res) {
                ViewBag.ErrorMessage = null;
                ViewBag.InfoMessage = "Item successfully deleted";

                GetProducts();
                return View("Management");
            } else {
                ViewBag.ErrorMessage = "Something has gone wrong. Please try again later";
                ViewBag.InfoMessage = null;

                GetProducts();
                return View("Management");
            }
        }

        [HttpPost]
        public ActionResult AddProduct(string name, string price, string creationyear, string certified) {
            if (Session.Count == 0) {
                Response.Redirect("~/Home/Index");
                return View();
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name)
                || string.IsNullOrEmpty(price) || string.IsNullOrWhiteSpace(price)
                || string.IsNullOrEmpty(creationyear) || string.IsNullOrWhiteSpace(creationyear)
                || string.IsNullOrEmpty(certified) || string.IsNullOrWhiteSpace(certified)) {
                ViewBag.InfoMessage = null;
                ViewBag.ErrorMessage = "Invalid parameter supplied";

                GetProducts();
                return View("Management");
            }

            bool res = false;
            using (SqlConnection connection = new SqlConnection(Database.ConnectionString)) {
                string query =
                    "INSERT INTO webapp.dbo.products (name, price, creation_year, certified) " +
                    "VALUES('" + name + "', " + price + ", '" + creationyear + "', '" + certified + "'); ";

                try {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    res = true;
                } catch (SqlException e) {
                    res = false;
                } finally {
                    connection.Close();
                }
            }

            if (res) {
                ViewBag.ErrorMessage = null;
                ViewBag.InfoMessage = "Item successfully added";

                GetProducts();
                return View("Management");
            } else {
                ViewBag.ErrorMessage = "Something has gone wrong. Please try again later";
                ViewBag.InfoMessage = null;

                GetProducts();
                return View("Management");
            }
        }

        private void GetProducts() {
            using (SqlConnection connection = new SqlConnection(Database.ConnectionString)) {
                if (this.ProductList.Count > 0)
                    this.ProductList.Clear();

                string query = "SELECT * FROM webapp.dbo.products";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            this.ProductList.Add(new Product() {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetInt32(2),
                                CreationYear = reader.GetString(3),
                                Certified = reader.GetString(4)
                            });
                        }
                    }
                }
            }

            ViewBag.ProductList = this.ProductList;
        }
    }
}