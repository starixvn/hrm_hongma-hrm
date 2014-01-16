namespace Fanex.NexHR.Core.Services
{
    using System.Web.Mvc;
    using Fanex.NexHR.ViewModels;

    public interface IRequestService
    {
        bool CheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool LateInEarlyOut(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model);

        bool Leave(ModelStateDictionary modelState, LeaveRequestViewModel model);

        bool ChangeShift(ModelStateDictionary modelState, ChangeShiftViewModel model);

        bool UpdateCheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool UpdateLateInEarlyOut(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model);

        bool UpdateLeave(ModelStateDictionary modelState, LeaveRequestViewModel model);

        bool UpdateChangeShift(ModelStateDictionary modelState, ChangeShiftViewModel model);

        bool Process(string command, BaseProcessModel model);
    }
}
