using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using SWAPIWebApplication.Models;

namespace SWAPIWebApplication.Pages
{
	public class SWAPIModel : PageModel
	{
		private readonly ILogger<SWAPIModel> _logger;
		private readonly HttpClient _httpClient;

		public PersonModel Person { get; set; }
		public List<FilmModel> Films { get; set; } = new List<FilmModel>();

		public SWAPIModel(ILogger<SWAPIModel> logger, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task OnGetAsync()
		{
			_logger.LogInformation("OnGet SWAPI Page");
			Person = await GetPersonAsync("https://swapi.dev/api/people/1");
			foreach ( string filmUrl in Person.Films )
			{
				FilmModel film = await GetFilmAsync(filmUrl);
				Films.Add(film);
			}
		}

		private async Task<PersonModel> GetPersonAsync(string personUrl)
		{
			using ( var response = await _httpClient.GetAsync(personUrl) )
			{
				if ( response.IsSuccessStatusCode )
				{
					JsonSerializerOptions options = new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					};
					string responseText = await response.Content.ReadAsStringAsync();
					PersonModel person = JsonSerializer.Deserialize<PersonModel>(responseText, options);
					return person;
				}
				else
				{
					throw new ApplicationException(response.ReasonPhrase);
				}
			}
		}

		private async Task<FilmModel> GetFilmAsync(string filmUrl)
		{
			using ( var response = await _httpClient.GetAsync(filmUrl) )
			{
				if ( response.IsSuccessStatusCode )
				{
					JsonSerializerOptions options = new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					};
					string responseText = await response.Content.ReadAsStringAsync();
					FilmModel film = JsonSerializer.Deserialize<FilmModel>(responseText, options);
					return film;
				}
				else
				{
					throw new ApplicationException(response.ReasonPhrase);
				}
			}
		}
	}
}
