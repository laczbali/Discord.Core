using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Core.Utils
{
	internal class HttpClientWrapper
	{
		/// <summary>
		/// Sends a JSON content to the provided endpoint, with the configured auth params. Will attempt to convert the end result.
		/// </summary>
		/// <typeparam name="ResponseType"></typeparam>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authScheme"></param>
		/// <param name="authValue"></param>
		/// <returns></returns>
		public async Task<ResponseType> MakeRequestAsync<ResponseType>(
			string url,
			HttpMethod method,
			object content,
			string authScheme,
			string authValue)
		{
			return await MakeRequestAsync<ResponseType>(url, method, JsonContent.Create(content), authScheme, authValue);
		}

		/// <summary>
		/// Sends a request to the provided endpoint, with the configured auth params. Will attempt to convert the end result.
		/// </summary>
		/// <typeparam name="ResponseType"></typeparam>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authScheme"></param>
		/// <param name="authValue"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<ResponseType> MakeRequestAsync<ResponseType>(
			string url,
			HttpMethod method,
			HttpContent content,
			string authScheme,
			string authValue)
		{
			var response = await MakeRequestAsync(url, method, content, authScheme, authValue);
			try
			{
				return JsonConvert.DeserializeObject<ResponseType>(response);
			}
			catch(Exception)
			{
				throw new Exception($"Failed to deserialize [{response}] into [{typeof(ResponseType)}]");
			}
		}

		/// <summary>
		/// Sends a JSON content to the provided endpoint, with the configured auth params.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authScheme"></param>
		/// <param name="authValue"></param>
		/// <returns></returns>
		public async Task<string> MakeRequestAsync(
			string url,
			HttpMethod method,
			object content,
			string authScheme,
			string authValue)
		{
			return await MakeRequestAsync(url, method, JsonContent.Create(content), authScheme, authValue);
		}

		/// <summary>
		/// Sends a request to the provided endpoint, with the configured auth params.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authScheme"></param>
		/// <param name="authValue"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<string> MakeRequestAsync(
			string url,
			HttpMethod method,
			HttpContent content,
			string authScheme,
			string authValue)
		{
			var client = new HttpClient();
			client.BaseAddress = new Uri(url);
			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.ConnectionClose = true;

			var requestMessage = new HttpRequestMessage(method, string.Empty);
			requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authScheme, authValue);
			requestMessage.Content = content;

			var result = await client.SendAsync(requestMessage);
			string responseBody = result.Content.ReadAsStringAsync().Result;
			if (!result.IsSuccessStatusCode)
			{
				throw new Exception($"{result.StatusCode} - {responseBody}");
			}
			HandleRateLimit(result.Headers);

			return responseBody;
		}

		private void HandleRateLimit(HttpResponseHeaders headers)
		{
			IEnumerable<string> headerValues;
			if(headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
			{
				var remaining = int.Parse(headerValues.FirstOrDefault());
				if (remaining == 1)
				{
					var headerVal = headers.GetValues("X-RateLimit-Reset-After").FirstOrDefault().Replace(".", ",");
					var resetAfterSec = float.Parse(headerVal);
					Thread.Sleep((int)(resetAfterSec * 1000));
				}
			}
		}
	}
}
