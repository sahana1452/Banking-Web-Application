using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using AuthenticationWithJWT.Models;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationWithJWT.Controllers
{
    public class TransactionsController : Controller
    {
        private IConfiguration _config;

        public TransactionsController(IConfiguration configuration)
        {
            _config = configuration;
        }
        public class UpdateTransactionRequestModel
        {
            public string TransactionType { get; set; }
            public decimal Amount { get; set; }
            public DateTime TransactionDate { get; set; }
            // Add other properties as needed
        }
        public class InsertTransactionRequestModel
        {
            public int AccountID { get; set; }
            public string TransactionType { get; set; }
            public decimal Amount { get; set; }
            public DateTime TransactionDate { get; set; }
            // Add other properties as needed
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("InsertTransaction")]
        public IActionResult InsertTransaction([FromBody] InsertTransactionRequestModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("InsertTransaction", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@AccountID", model.AccountID);
                        command.Parameters.AddWithValue("@TransactionType", model.TransactionType);
                        command.Parameters.AddWithValue("@Amount", model.Amount);
                        command.Parameters.AddWithValue("@TransactionDate", model.TransactionDate);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Transaction inserted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("GetTransactionByID/{transactionID}")]
        [Authorize]
        public IActionResult GetTransactionByID(int transactionID)
        {
            try
            {
                TransactionModel transaction = null;

                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("GetTransactionByID", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@TransactionID", transactionID);

                        // Open the connection and execute the stored procedure
                        con.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                transaction = new TransactionModel
                                {
                                    TransactionID = Convert.ToInt32(reader["TransactionID"]),
                                    AccountID = Convert.ToInt32(reader["AccountID"]),
                                    TransactionType = reader["TransactionType"].ToString(),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    TransactionDate = Convert.ToDateTime(reader["TransactionDate"])
                                    // Add other properties as needed
                                };
                            }
                        }
                    }
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("UpdateTransaction/{transactionID}")]
        public IActionResult UpdateTransaction(int transactionID, [FromBody] UpdateTransactionRequestModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("UpdateTransaction", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@TransactionID", transactionID);
                        command.Parameters.AddWithValue("@TransactionType", model.TransactionType);
                        command.Parameters.AddWithValue("@Amount", model.Amount);
                        command.Parameters.AddWithValue("@TransactionDate", model.TransactionDate);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Transaction updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpDelete("DeleteTransaction/{transactionID}")]
        public IActionResult DeleteTransaction(int transactionID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("DeleteTransaction", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@TransactionID", transactionID);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Transaction deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}

