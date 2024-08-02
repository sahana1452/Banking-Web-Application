using AuthenticationWithJWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AuthenticationWithJWT.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration configuration)
        {
            _config = configuration;
        }
        public class InsertLoginCredentialsRequestModel
        {
            public string Username { get; set; }
            public string PasswordHash { get; set; }
            public int CustomerID { get; set; }
            // Add other properties as needed
        }
        public class UpdateLoginCredentialsRequestModel
        {
            public string Username { get; set; }
            public string PasswordHash { get; set; }
            // Add other properties as needed
        }
        private Users AuthenticateUser(Users user)
        {
            Users _user = null;

            SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString());
            using (SqlCommand command = new SqlCommand("sp_getUsers", con))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Create and add parameters
                command.Parameters.Add(new SqlParameter("@UserName", SqlDbType.VarChar,200)).Value = user.UserName;
                command.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 200)).Value = user.Password;

                con.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // Read the results row by row
                        while (reader.Read())
                        {
                            // Access columns by name or index
                            string username = reader["UserName"].ToString();
                            string password = reader["Password"].ToString();
                            if (user.UserName == username && user.Password == password)
                            {
                                _user = new Users { UserName = username };
                            }
                            _user = new Users { UserName = username };
                        }
                    }

                    
                }
            }
            //if (user.Username == "admin" && user.Password == "12345")
            //{
            //    _user = new Users { Username = "Manoj Deshwal" };
            //}
            return _user;
        }
        private string GenerateToken(Users users)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(Users user)
        {
            IActionResult response = Unauthorized();
            var user_ = AuthenticateUser(user);
            if (user_ != null)
            {
                var token = GenerateToken(user_);
                response = Ok(new { token = token });
            }
            return response;
        }
        [HttpPost("InsertLoginCredentials")]
        public IActionResult InsertLoginCredentials([FromBody] InsertLoginCredentialsRequestModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("InsertLoginCredentials", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@Username", model.Username);
                        command.Parameters.AddWithValue("@PasswordHash", model.PasswordHash);
                        command.Parameters.AddWithValue("@CustomerID", model.CustomerID);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Login credentials inserted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("GetLoginCredentialsByCustomerID/{customerID}")]
        public IActionResult GetLoginCredentialsByCustomerID(int customerID)
        {
            try
            {
                List<LoginCredentialsModel> loginCredentialsList = new List<LoginCredentialsModel>();

                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("GetLoginCredentialsByCustomerID", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@CustomerID", customerID);

                        // Open the connection and execute the stored procedure
                        con.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoginCredentialsModel loginCredentials = new LoginCredentialsModel
                                {
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    Username = reader["Username"].ToString(),
                                    PasswordHash = reader["PasswordHash"].ToString()
                                    // Add other properties as needed
                                };

                                loginCredentialsList.Add(loginCredentials);
                            }
                        }
                    }
                }

                return Ok(loginCredentialsList);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("UpdateLoginCredentials/{customerID}")]
        public IActionResult UpdateLoginCredentials(int customerID, [FromBody] UpdateLoginCredentialsRequestModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("UpdateLoginCredentials", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@CustomerID", customerID);
                        command.Parameters.AddWithValue("@Username", model.Username);
                        command.Parameters.AddWithValue("@PasswordHash", model.PasswordHash);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Login credentials updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteLoginCredentials/{customerID}")]
        public IActionResult DeleteLoginCredentials(int customerID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("EcomDatabase").ToString()))
                {
                    using (SqlCommand command = new SqlCommand("DeleteLoginCredentials", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameter
                        command.Parameters.AddWithValue("@CustomerID", customerID);

                        // Open the connection and execute the stored procedure
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Login credentials deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


    }
}
