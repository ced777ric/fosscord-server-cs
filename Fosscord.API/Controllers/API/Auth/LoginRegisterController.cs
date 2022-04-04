using Fosscord.API.Classes;
using Fosscord.API.PostData;
using Fosscord.DbModel;
using Fosscord.DbModel.Scaffold;
using Fosscord.Gateway;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fosscord.API.Controllers.API.Auth;

//[Authorize]
[Controller]
[Route("/")]
public class AuthController : Controller
{
    private readonly Db _db;
    private readonly JWTAuthenticationManager _auth;
    private static readonly Random rnd = new Random();
    
    public AuthController(Db db, JWTAuthenticationManager auth)
    {
        _db = db;
        _auth = auth;
    }

    [HttpPost("/api/auth/register")]
    public async Task<object> Register([FromBody] RegisterData data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
            {
                {
                    "email", new FieldError()
                    {
                        _errors = new List<FieldErrorInner>()
                        {
                            new FieldErrorInner()
                            {
                                code = "MISSING_DATA",
                                message = "Missing fields" //todo translations, and better error description
                            }
                        }
                    }
                }
            }));
        }

        if (!data.Consent)
        {
            return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
            {
                {
                    "consent", new FieldError()
                    {
                        _errors = new List<FieldErrorInner>()
                        {
                            new FieldErrorInner()
                            {
                                code = "CONSENT_REQUIRED",
                                message = "You must agree to the Terms of Service and Privacy Policy." //todo translations, and better error description
                            }
                        }
                    }
                }
            }));
        }
        
        Console.WriteLine(JsonConvert.SerializeObject(data));
        //todo email check for gmail tricks??
        //todo, check if email is required or not
        string discrim = rnd.Next(10000).ToString();
        if (_db.Users.Any(x => x.Email == data.Email))
        {
            return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
            {
                {
                    "email", new FieldError()
                    {
                        _errors = new List<FieldErrorInner>()
                        {
                            new FieldErrorInner()
                            {
                                code = "EMAIL_ALREADY_REGISTERED",
                                message = "Email is already registered" //todo translations
                            }
                        }
                    }
                }
            }));
        }
        //todo, check for duplicate disciminators on the same usernames
        
        //todo, configs for, proxy, disabling registration, and add password requirments, captcha, gift_code_sku_id or something

        int minumumAge = FosscordConfig.GetInt("register_minimum_age", 13);
        bool dateOfBirthRequired = FosscordConfig.GetBool("register_date_of_birth_required", true);
        if (dateOfBirthRequired)
        {
            if (!DateOnly.TryParse(data.DateOfBirth, out DateOnly birthTime))
            {
                return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
                {
                    {
                        "date_of_birth", new FieldError()
                        {
                            _errors = new List<FieldErrorInner>()
                            {
                                new FieldErrorInner()
                                {
                                    code = "BASE_TYPE_REQUIRED",
                                    message = "This field is required" //todo translations
                                }
                            }
                        }
                    }
                }));
            }
            int age = DateTime.UtcNow.Year - birthTime.Year;  
            if (DateTime.Now.DayOfYear < birthTime.DayOfYear)  
                age = age - 1;

            if (age < minumumAge)
            {
                return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
                {
                    {
                        "date_of_birth", new FieldError()
                        {
                            _errors = new List<FieldErrorInner>()
                            {
                                new FieldErrorInner()
                                {
                                    code = "DATE_OF_BIRTH_UNDERAGE",
                                    message = $"You need to be {minumumAge} years or older" //todo translations
                                }
                            }
                        }
                    }
                }));
            }
        }
        
        //todo, invite only

        var user = new User()
        {
            CreatedAt = DateTime.Now,
            Username = data.Username,
            Discriminator = discrim,
            Id = new IdGen.IdGenerator(0).CreateId() + "",
            Bot = false,
            System = false,
            Desktop = false,
            Mobile = false,
            Premium = true,
            PremiumType = 2,
            Bio = "",
            MfaEnabled = false,
            Verified = true,
            Disabled = false,
            Deleted = false,
            Email = data.Email,
            Rights = "0", // TODO = grant rights correctly, as 0 actually stands for no rights at all
            NsfwAllowed = true, // TODO = depending on age
            PublicFlags = 0,
            Flags = "0", // TODO = generate
            Data = JsonConvert.SerializeObject(new
            {
                hash = BCrypt.Net.BCrypt.HashPassword(data.Password, 12),
                valid_tokens_since = DateTime.Now,
            }),
            Settings = JsonConvert.SerializeObject(new UserSettings()),
            Fingerprints = "",
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        
        
        //todo, invites joining
        
        var token = _auth.Authenticate(data.Email, data.Password);
        if (token == null) return new StatusCodeResult(500); //todo figure out what to do with this
        return new {token = token};
    }
    [HttpPost("/api/auth/login")]
    public async Task<object> Login([FromBody] LoginData data)
    {
        Console.WriteLine(JsonConvert.SerializeObject(data));
        if (!ModelState.IsValid)
        {
            return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
            {
                {
                    "login", new FieldError()
                    {
                        _errors = new List<FieldErrorInner>()
                            {
                                new FieldErrorInner()
                                {
                                    code = "INVALID_LOGIN",
                                    message = "Missing fields" //todo translations, and better error description
                                }
                            }
                    }
                }
            }));
        }
        User user = _db.Users.FirstOrDefault(x => x.Email == data.Login);
        if (user == null)
        {
            return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
            {
                {
                    "login", new FieldError()
                    {
                        _errors = new List<FieldErrorInner>()
                        {
                            new FieldErrorInner()
                            {
                                code = "INVALID_LOGIN",
                                message = "E-Mail or Phone not found" //todo translations, and better error description
                            }
                        }
                    }
                }
            }));
        }

        if (data.Undelete)
        {
            if (user.Disabled)
                user.Disabled = false;
            
            if (user.Deleted)
                user.Deleted = false;
            
            if (user.Deleted || user.Disabled)
                await _db.SaveChangesAsync();
        }
        else
        {
            if (user.Disabled)
                return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
                {
                    {
                        "login", new FieldError()
                        {
                            _errors = new List<FieldErrorInner>()
                            {
                                new FieldErrorInner()
                                {
                                    code = 20013,
                                    message = "This account is disabled" //todo translations, and better error description
                                }
                            }
                        }
                    }
                }));
            
            if (user.Deleted)
                return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
                {
                    {
                        "login", new FieldError()
                        {
                            _errors = new List<FieldErrorInner>()
                            {
                                new FieldErrorInner()
                                {
                                    code = 20011,
                                    message = "This account is scheduled for deletion." //todo translations
                                }
                            }
                        }
                    }
                }));
        }
        
        var token = _auth.Authenticate(data.Login, data.Password);
        if (token == null)
        {
            return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
            {
                {
                    "login", new FieldError()
                    {
                        _errors = new List<FieldErrorInner>()
                        {
                            new FieldErrorInner()
                            {
                                code = "INVALID_PASSWORD",
                                message = "Invalid Password" //todo translations
                            }
                        }
                    }
                }
            }));
        }

        if (user.MfaEnabled)
        {
            //return new {mfs = true, sms = true, ticket = "todo"};
        }
        
        return new {token = token, settings = user.Settings};
    }
}