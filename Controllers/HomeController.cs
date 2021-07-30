using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace WebApplication.Controllers {
    public class HomeController : Controller {

        public struct Product {
            public int Id;
            public string Name;
            public int Price;
            public string CreationYear;
            public string Certified;
        }
        public List<Product> ProductList = new List<Product>() { };

        public string Something = "asdsadadssad";

        [HttpGet]
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult Contact() => View();

        [HttpGet]
        public ActionResult Staff() => View();

        [HttpGet]
        public ActionResult Privacy() => View();

        [HttpGet]
        public ActionResult Products() {
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
            return View();
        }
    }
}