﻿using Amazon.Runtime;
using Amazon.SQS;
using System.ComponentModel.DataAnnotations;

namespace Dequeueable.AmazonSQS.Configurations
{
    /// <summary>
    /// HostOptions to configure the settings of the host
    /// </summary>
    public class HostOptions : IHostOptions
    {
        private IEnumerable<string> _attributeNames = Enumerable.Empty<string>();

        /// <summary>
        /// Constant string used to bind the appsettings.*.json
        /// </summary>
        public static string Dequeueable => nameof(Dequeueable);

        /// <summary>
        /// The URL of the Amazon SQS queue from which messages are received.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} cannot be empty.")]
        public string QueueUrl { get; set; } = string.Empty;

        /// <summary>
        /// The maximum number of messages processed in parallel. Valid values: 1 to 10.
        /// </summary>
        [Range(1, 10,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int BatchSize { get; set; } = 4;

        /// <summary>
        /// The timeout after the queue message is visible again for other services. Valid values: 30 to 43200 (12 hours) seconds.
        /// </summary>
        [Range(30, 43200,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int VisibilityTimeoutInSeconds { get; set; } = 300;

        /// <summary>
        /// Optional configuration settings for the Amazon SQS client.
        /// Use this property to customize the SQS client's behavior, such as setting the service URL, region, or other configurations.
        /// </summary>
        public AmazonSQSConfig? AmazonSQSConfig { get; set; }

        /// <summary>
        /// AWS credentials used to authenticate requests to Amazon SQS.
        /// There are several ways to provide AWSCredentials. Use this property to explicitly set the credentials in code if not using environment variables, IAM roles, or other automatic methods.
        /// </summary>
        public AWSCredentials? AWSCredentials { get; set; }

        /// <summary>
        /// A list of attributes that need to be returned along with each message <see cref="Amazon.SQS.Model.Message.Attributes"/>.
        /// </summary>
        public IEnumerable<string> AttributeNames
        {
            get => _attributeNames.Distinct().ToList();
            set
            {
                _attributeNames = value ?? Enumerable.Empty<string>();
            }
        }
    }
}
