namespace Graphene.ApiCommunication
{
    public interface INotificationService
    {
        event Http.NetworkAction OnRequestSuccess;
        event Http.NetworkAction OnRequestNotAuthorized;
        event Http.NetworkAction OnRequestFailed;
        
        void RequestSuccess();
        void RequestNotAuthorized();
        void RequestFailed();
    }
}