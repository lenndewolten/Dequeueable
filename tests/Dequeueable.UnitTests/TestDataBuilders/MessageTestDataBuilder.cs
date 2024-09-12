using Dequeueable.UnitTests.Configurations;

namespace Dequeueable.UnitTests.TestDataBuilders
{
    public class MessageTestDataBuilder
    {
        private string _messageId = "some id";
        private DateTimeOffset _nextVisibileOn = DateTimeOffset.UtcNow.AddMinutes(1);
        private BinaryData _body = BinaryData.FromString("test body");

        public TestMessage Build()
        {
            return new TestMessage
            {
                MessageId = _messageId,
                NextVisibleOn = _nextVisibileOn,
                Body = _body
            };
        }

        public MessageTestDataBuilder WithmessageId(string messageId)
        {
            _messageId = messageId;
            return this;
        }

        public MessageTestDataBuilder WithNextVisibileOn(DateTimeOffset nextVisibileOn)
        {
            _nextVisibileOn = nextVisibileOn;
            return this;
        }

        public MessageTestDataBuilder WithBody(string body)
        {
            _body = BinaryData.FromString(body);
            return this;
        }

        public MessageTestDataBuilder WithBody(BinaryData body)
        {
            _body = body;
            return this;
        }
    }
}
