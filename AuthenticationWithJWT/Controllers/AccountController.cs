using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using AuthenticationWithJWT.Models;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationWithJWT.Controllers
{
    public class AccountController : Controller
    {
        private IConfiguration _config;

        public AccountController(IConfiguration configuration)
        {
            _config = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public class AccountRequestModel
        {
            public int CustomerID { get; set; }
            public string AccountType { get; set; }
            public decimal Balance { get; set; }
            // Add other properties as needed
        }
        public class UpdateAccountRequestModel
        {
            public string AccountType { get; set; }
            public decimal Balance { get; set; }
            // Add other properties as needed
        }
        [HttpPost("InsertAccount")]
        public IActionResult InsertAccount([FromBody] AccountRequestModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("InsertAccount", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@CustomerID", model.CustomerID);
                        command.Parameters.AddWithValue("@AccountType", model.AccountType);
                        command.Parameters.AddWithValue("@Balance", model.Balance);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Account inserted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        [HttpGet("GetAccountByID/{accountID}")]
        [Authorize]
        public IActionResult GetAccountByID(int accountID)
        {
            try
            {
                AccountModel account = null;

                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("GetAccountByID", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@AccountID", accountID);

                        // Open the connection and execute the stored procedure
                        con.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                account = new AccountModel
                                {
                                    AccountID = Convert.ToInt32(reader["AccountID"]),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    AccountType = reader["AccountType"].ToString(),
                                    Balance = Convert.ToDecimal(reader["Balance"])
                                    // Add other properties as needed
                                };
                            }
                        }
                    }
                }

                if (account != null)
                    return Ok(account);
                else
                    return NotFound("Account not found.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPut("UpdateAccount/{accountID}")]
        public IActionResult UpdateAccount(int accountID, [FromBody] UpdateAccountRequestModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("UpdateAccount", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@AccountID", accountID);
                        command.Parameters.AddWithValue("@AccountType", model.AccountType);
                        command.Parameters.AddWithValue("@Balance", model.Balance);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Account updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteAccount/{accountID}")]
        public IActionResult DeleteAccount(int accountID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("DeleteAccount", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@AccountID", accountID);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Account deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


    }


}

