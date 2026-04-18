using MoviesExam.DTO; 
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;





namespace MoviesExam
{
    public class MoviesTests

    {
        private RestClient client;
        private static string movieId;


        [OneTimeSetUp]
        public void Setup()
        {
        
         string jwtToken = GetJwtToken("Sasho123@email.com", "123123");
         RestClientOptions options = new RestClientOptions("http://144.91.123.158:5000")
             {
                 Authenticator = new JwtAuthenticator(jwtToken)
              };
              client = new RestClient(options);
       
        }
              private string GetJwtToken(string email, string password)
        {
            RestClient client = new RestClient("http://144.91.123.158:5000");
            RestRequest request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new { email, password });
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = JsonSerializer.Deserialize<JsonElement>(response.Content);
                var token = content.GetProperty("accessToken").GetString();

                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("Token not found in the response.");
                }
                return token;
            }
            else
            {
                throw new InvalidOperationException($"Failed to authenticate. Status code: {response.StatusCode}, Response: {response.Content}");
            }
        }

        [Order(1)]
        [Test]
        public void CreateNewMovie_ShouldReurnOk()
        {

            var Movie = new MovieDto 
            { 
                 Title = "Terminator",
                 Description = "Robot is coming from the future",
            };

            var request = new RestRequest("/api/Movie/Create", Method.Post);
            request.AddJsonBody(Movie);

            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            
            var createResponse = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);

          
            Assert.That(createResponse.Movie.Id, Is.Not.Null);
            Assert.That(createResponse.Movie.Id, Is.Not.Empty);
            Assert.That(createResponse.Msg, Is.EqualTo("Movie created successfully!"));

            movieId = createResponse.Movie.Id;

        }

        [Order(2)]
        [Test]
        public void EditCreatedMovie_ShouldEditSuccessfully()
        {
            var request = new RestRequest($"/api/Movie/Edit?movieId={movieId}", Method.Put);
            request.AddBody(new
            {
             title = "Batman",
             description = "Movie for Superhero ",
             posterUrl ="",
             });
          
            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
          
            var editResponse = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(editResponse.Msg, Is.EqualTo("Movie edited successfully!"));
        }

        [Order(3)]
        [Test]
        public void GetAllMovies_ShouldReturnAllMovies()
        {
            var request = new RestRequest("/api/Catalog/All", Method.Get);
            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            List <MovieDto> readyResponse = JsonSerializer.Deserialize<List<MovieDto>>(response.Content);
            Assert.That(readyResponse, Is.Not.Null);
            Assert.That(readyResponse, Is.Not.Empty);

        }

        [Order(4)]
        [Test]
        public void DeleteCreatedMovie_ShoudDeleteit()
        {
            var request = new RestRequest($"/api/Movie/Delete?movieId={movieId}", Method.Delete);
            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var readyResponse = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);
            Assert.That(readyResponse.Msg, Is.EqualTo("Movie deleted successfully!"));
        }

        [Order(5)]
        [Test]
        public void CreateMovieWithouRequireFields_ShoudReturnError()
        {
            var Movie = new MovieDto
            {
                Title = "",
                Description = "",
            };

            var request = new RestRequest("/api/Movie/Create", Method.Post);
            request.AddJsonBody(Movie);

            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        }

        [Order(6)]
        [Test]
        public void EditNonExistingMove_ShouldReturnERror()

        {

            string nonExistingId = "2331233";
            var request = new RestRequest($"/api/Movie/Edit?movieId={nonExistingId}", Method.Put);
            request.AddBody(new
            {
                title = "Batman",
                description = "Movie for Superhero ",
                posterUrl = "",
            });

            var response = this.client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var editResponse = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);

            Assert.That(editResponse.Msg,Is.EqualTo("Unable to edit the movie! Check the movieId parameter or user verification!"));
        }

        [Order(7)]
        [Test]
        public void DeleteNonExisitingMovie_ShouldNotWork()
        {
            string nonExistingId = "2331233";
            var request = new RestRequest($"/api/Movie/Delete?movieId={nonExistingId}", Method.Delete);
            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var readyResponse = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);
            Assert.That(readyResponse.Msg, Is.EqualTo("Unable to delete the movie! Check the movieId parameter or user verification!"));
        }

  
            [OneTimeTearDown] 
        public void TearDown()
        {
            this.client?.Dispose();
        }
    }
}