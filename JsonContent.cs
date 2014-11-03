namespace DynamicPowerShellApi
{
	using System.IO;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Threading.Tasks;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	/// <summary>
	/// The json content.
	/// </summary>
	public class JsonContent : HttpContent
	{
		/// <summary>
		/// The _value.
		/// </summary>
		private readonly JToken _value;

		/// <summary>
		/// Initialises a new instance of the <see cref="JsonContent"/> class.
		/// </summary>
		/// <param name="value">
		/// The value.
		/// </param>
		public JsonContent(JToken value)
		{
			_value = value;
			Headers.ContentType = new MediaTypeHeaderValue("application/json");
		}

		/// <summary>
		/// Overriding the serialize to stream async method.
		/// </summary>
		/// <param name="stream">
		/// The stream.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			var jw = new JsonTextWriter(new StreamWriter(stream))
			{
				Formatting = Formatting.Indented
			};
			_value.WriteTo(jw);
			jw.Flush();
			return Task.FromResult<object>(null);
		}

		/// <summary>
		/// Try compute length.
		/// </summary>
		/// <param name="length">
		/// The length.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		protected override bool TryComputeLength(out long length)
		{
			length = -1;
			return false;
		}
	}
}
