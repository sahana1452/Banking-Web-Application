using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using AuthenticationWithJWT.Models;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationWithJWT.Controllers
{
    public class CustomerController : Controller
    {

        private IConfiguration _config;

        public CustomerController(IConfiguration configuration)
        {
            _config = configuration;
        }
        public class UpdateCustomerRequestModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            // Add other properties as needed
        }
        public class InsertCustomerRequestModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            // Add other properties as needed
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("InsertCustomer")]
        public IActionResult InsertCustomer([FromBody] InsertCustomerRequestModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("InsertCustomer", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@FirstName", model.FirstName);
                        command.Parameters.AddWithValue("@LastName", model.LastName);
                        command.Parameters.AddWithValue("@Address", model.Address);
                        command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                        command.Parameters.AddWithValue("@Email", model.Email);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Customer inserted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpGet("GetCustomerByID/{customerID}")]
        [Authorize]
        public IActionResult GetCustomerByID(int customerID)
        {
            try
            {
                CustomerModel customer = null;

                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("GetCustomerByID", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@CustomerID", customerID);

                        // Open the connection and execute the stored procedure
                        con.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                customer = new CustomerModel
                                {
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    Email = reader["Email"].ToString()
                                    // Add other properties as needed
                                };
                            }
                        }
                    }
                }

                if (customer != null)
                    return Ok(customer);
                else
                    return NotFound("Customer not found.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("UpdateCustomer/{customerID}")]
        public IActionResult UpdateCustomer(int customerID, [FromBody] UpdateCustomerRequestModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("UpdateCustomer", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@CustomerID", customerID);
                        command.Parameters.AddWithValue("@FirstName", model.FirstName);
                        command.Parameters.AddWithValue("@LastName", model.LastName);
                        command.Parameters.AddWithValue("@Address", model.Address);
                        command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                        command.Parameters.AddWithValue("@Email", model.Email);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Customer updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteCustomer/{customerID}")]
        public IActionResult DeleteCustomer(int customerID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("DeleteCustomer", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@CustomerID", customerID);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Customer deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("CalculateCustomerBalance/{customerID}")]
        [Authorize]
        public IActionResult CalculateCustomerBalance(int customerID)
        {
            try
            {
                decimal balance = 0;

                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("CalculateCustomerBalance", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@CustomerID", customerID);

                        // Open the connection and execute the stored procedure
                        con.Open();

                        // Use ExecuteScalar to get the result of the stored procedure
                        var result = command.ExecuteScalar();

                        // Check if the result is not null
                        if (result != null && result != DBNull.Value)
                        {
                            balance = Convert.ToDecimal(result);
                        }
                    }
                }

                return Ok(balance);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


    }
}
