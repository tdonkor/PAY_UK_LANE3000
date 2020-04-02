namespace PAY_UK_IPP350.Communicator
{
    public interface ICommunicatorCallbacks
    {
        void InitRequest(object parameters);

        void TestRequest(object parameters);

        void PayRequest(object parameters);
    }
}
