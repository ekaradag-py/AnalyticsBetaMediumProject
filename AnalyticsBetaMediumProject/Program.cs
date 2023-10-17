using Google.Analytics.Data.V1Beta;
using Google.Api.Gax;
using Google.Api.Gax.Grpc;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;

internal class Program
{
    private static void Main(string[] args)
    {
        {
            GoogleCredential credential;
            using (var stream = new FileStream("t1.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream);
            }

            // Create a Google Analytics Data client
            var client = new BetaAnalyticsDataClientBuilder
            {
                ChannelCredentials = credential.ToChannelCredentials()
            }.Build();

            // Create a request to read data
            var request = new RunReportRequest
            {
                Property = "properties/" + "*********",
                DateRanges = { new DateRange { StartDate = "2023-09-01", EndDate = "2023-10-01" } },
                Dimensions = { new Dimension { Name = "customEvent:itemId" } },
                Metrics = { new Metric { Name = "eventCount" } },
                Limit = 1000000
            };

            DateTime dateTime = DateTime.Now;
            var ts = dateTime.AddMinutes(6) - dateTime;
            var deadline = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Utc);

            CallSettings callSettings = CallSettings.FromExpiration(Expiration.FromDeadline(deadline));
            var aa = callSettings.WithTimeout(ts);

            // Make the request and retrieve the response
            var response = client.RunReport(request, aa);

            // Process the response
            foreach (var row in response.Rows)
            {
                Console.WriteLine($"DimensionValues: {row.DimensionValues[0].Value}, MetricValues: {row.MetricValues[0].Value}");
            }
            Console.ReadLine();
        }
    }
}