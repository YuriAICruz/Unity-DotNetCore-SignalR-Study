namespace Graphene.ApiCommunication
{
    public class DirectNotificationService : INotificationService
    {
        public event Http.NetworkAction OnRequestSuccess;
        public event Http.NetworkAction OnRequestNotAuthorized;
        public event Http.NetworkAction OnRequestFailed;
        
        
        public void RequestSuccess()
        {
            OnRequestSuccess?.Invoke();
        }

        public void RequestNotAuthorized()
        {
            OnRequestNotAuthorized?.Invoke();
        }

        public void RequestFailed()
        {
            OnRequestFailed?.Invoke();
        }
    }
}