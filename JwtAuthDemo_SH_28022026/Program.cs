using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// *** AUTENTIKOINTI (JWT) ***
// Määritetään, että sovellus käyttää JWT-tunnisteita käyttäjän tunnistamiseen.
// DefaultAuthenticateScheme = miten käyttäjä tunnistetaan
// DefaultChallengeScheme = mitä käytetään, kun käyttäjä EI ole tunnistautunut
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // JWT-tunnisteen tarkistussäännöt
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,   // Tarkista kuka tokenin myönsi
        ValidateAudience = true, // Tarkista kenelle token on tarkoitettu
        ValidateLifetime = true, // Tarkista ettei token ole vanhentunut
        ValidateIssuerSigningKey = true, // Tarkista allekirjoitus

        ValidIssuer = "MyTestAuthServer",       // Tokenin myöntäjä
        ValidAudience = "MyTestApiUsers",       // Tokenin vastaanottaja
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("your_secret_key_that_is_at_least_32_bytes_long")
        ) // Symmetrinen salainen avain tokenin allekirjoitukseen
    };
});

// *** AUTORIZAATIO (KÄYTTÖOIKEUDET) ***
// Lisätään oma policy, joka vaatii Admin-roolin claimin
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

// *** CONTROLLERIT JA SWAGGER ***
// Rekisteröidään kontrollerit ja Swagger dokumentointia varten
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger näkyy vain kehitysympäristössä
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// *** MIDDLEWARE-JÄRJESTYS ***
// TÄRKEÄÄ: Authentication ennen Authorization
app.UseAuthentication(); // Tarkistaa tokenin
app.UseAuthorization();  // Tarkistaa oikeudet (roolit, policy)

app.MapControllers();

app.Run();


