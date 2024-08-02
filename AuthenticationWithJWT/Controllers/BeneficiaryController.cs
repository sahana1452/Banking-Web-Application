using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using AuthenticationWithJWT.Models;

namespace AuthenticationWithJWT.Controllers
{
    public class BeneficiaryController : Controller
    {
        private IConfiguration _config;

        public BeneficiaryController(IConfiguration configuration)
        {
            _config = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public class InsertBeneficiaryRequestModel
        {
            public int CustomerID { get; set; }
            public string BeneficiaryName { get; set; }
            public string AccountNumber { get; set; }
            // Add other properties as needed
        }
        public class UpdateBeneficiaryRequestModel
        {
            public int CustomerID { get; set; }
            public string BeneficiaryName { get; set; }
            public string AccountNumber { get; set; }
            // Add other properties as needed
        }
        [HttpPost("InsertBeneficiary")]
        public IActionResult InsertBeneficiary([FromBody] InsertBeneficiaryRequestModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("InsertBeneficiary", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@CustomerID", model.CustomerID);
                        command.Parameters.AddWithValue("@BeneficiaryName", model.BeneficiaryName);
                        command.Parameters.AddWithValue("@AccountNumber", model.AccountNumber);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Beneficiary inserted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet("GetBeneficiaryByID/{beneficiaryID}")]
        public IActionResult GetBeneficiaryByID(int beneficiaryID)
        {
            try
            {
                BeneficiaryModel beneficiary = null;

                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("GetBeneficiaryByID", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@BeneficiaryID", beneficiaryID);

                        // Open the connection and execute the stored procedure
                        con.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                beneficiary = new BeneficiaryModel
                                {
                                    BeneficiaryID = Convert.ToInt32(reader["BeneficiaryID"]),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    BeneficiaryName = reader["BeneficiaryName"].ToString(),
                                    AccountNumber = reader["AccountNumber"].ToString()
                                    // Add other properties as needed
                                };
                            }
                        }
                    }
                }

                if (beneficiary != null)
                    return Ok(beneficiary);
                else
                    return NotFound("Beneficiary not found.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPut("UpdateBeneficiary/{beneficiaryID}")]
        public IActionResult UpdateBeneficiary(int beneficiaryID, [FromBody] UpdateBeneficiaryRequestModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("UpdateBeneficiary", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@BeneficiaryID", beneficiaryID);
                        command.Parameters.AddWithValue("@CustomerID", model.CustomerID);
                        command.Parameters.AddWithValue("@BeneficiaryName", model.BeneficiaryName);
                        command.Parameters.AddWithValue("@AccountNumber", model.AccountNumber);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Beneficiary updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteBeneficiary/{beneficiaryID}")]
        public IActionResult DeleteBeneficiary(int beneficiaryID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("DeleteBeneficiary", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@BeneficiaryID", beneficiaryID);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Beneficiary deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
