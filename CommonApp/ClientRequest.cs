using System;

namespace CommonApp
{
    [Serializable]
    public class ClientRequest
    {
        public OperationType Operation { get; set; }
        public Client Client { get; set; }
    }
}