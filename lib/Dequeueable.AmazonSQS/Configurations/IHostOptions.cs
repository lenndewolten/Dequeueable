using Amazon.Runtime;
using Amazon.SQS;

namespace Dequeueable.AmazonSQS.Configurations
{
    /// <summary>
    /// Use the IHostOptions to configure the settings of the host
    /// </summary>
    public interface IHostOptions
    {
        /// <summary>
        /// The maximum number of messages processed in parallel. Valid values: 1 to 10.
        /// </summary>
        int BatchSize { get; set; }
        /// <summary>
        /// The timeout after the queue message is visible again for other services. Valid values: 30 to 43200 (12 hours) seconds.
        /// </summary>
        int VisibilityTimeoutInSeconds { get; set; }
        /// <summary>
        /// The URL of the Amazon SQS queue from which messages are received.
        /// </summary>
        string QueueUrl { get; set; }
        /// <summary>
        /// Optional configuration settings for the Amazon SQS client.
        /// Use this property to customize the SQS client's behavior, such as setting the service URL, region, or other configurations.
        /// </summary>
        AmazonSQSConfig? AmazonSQSConfig { get; set; }

        /// <summary>
        /// AWS credentials used to authenticate requests to Amazon SQS.
        /// There are several ways to provide AWSCredentials. Use this property to explicitly set the credentials in code if not using environment variables, IAM roles, or other automatic methods.
        /// </summary>
        AWSCredentials? AWSCredentials { get; set; }

        /// <summary>
        /// A list of attributes that need to be returned along with each message <see cref="Amazon.SQS.Model.Message.Attributes"/>.
        /// </summary>
        IEnumerable<string> AttributeNames { get; set; }
    }
}