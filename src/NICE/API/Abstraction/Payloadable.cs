namespace NICE.API.Abstraction
{
    public abstract class Payloadable
    {
        private IProtocol _payload;

        public IProtocol Payload
        {
            get => _payload;
            set
            {
                if (Payload is Payloadable payloadable)
                {
                    payloadable.Payload = value;
                }
                else
                {
                    _payload = value;
                }
            }
        }
    }
}