using Microsoft.AspNetCore.Mvc;
using Resources.Base.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos.Toko;
using WeVeed.Application.Services.Toko;
using WeVeedWebAPI.Extensions;

namespace WeVeedWebAPI.Controllers
{
    [Route("[controller]")]
    public class TokoController : Controller
    {
        private readonly ITokoRoomAppService _tokoRoomAppService;

        public TokoController(ITokoRoomAppService tokoRoomAppService)
        {
            _tokoRoomAppService = tokoRoomAppService;
        }

        #region Open Discussions Rooms 

        [HttpGet("{id}")]
        public async Task<IEnumerable<string>> Get(string id)
        {
            var a = Request.Headers;
            var x = User.Claims;
            return new string[] { "111111111", "22222" };
        }

        [HttpGet("getAllRooms/{roomType}")]
        public async Task<IActionResult> GetAllRooms(string roomType)
        {
            if (roomType == null || (roomType != "opendiscussion" && roomType != "business" && roomType != "drinking"))
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var rooms = await _tokoRoomAppService.GetAllRoomsAsync(roomType);
            var response = new BaseResponse<List<TokoRoomViewDto>>(rooms);
            return Response.Ok(response);
        }

        [HttpGet("addAttendantToRoom/{roomType}/{roomNumber}")]
        public async Task<IActionResult> AddAttendantToRoom(string roomType, int roomNumber)
        {
            if (roomType == null || (roomType != "opendiscussion" && roomType != "business" && roomType != "drinking"))
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var isSuccess = await _tokoRoomAppService.AddAttendantToRoomAsync(roomType, roomNumber);
            var response = new BaseResponse(isSuccess);
            return Response.Ok(response);
        }

        [HttpGet("removeAttendantFromRoom/{roomType}/{roomNumber}")]
        public async Task<IActionResult> RemoveAttendantFromRoom(string roomType, int roomNumber)
        {
            if (roomType == null || (roomType != "opendiscussion" && roomType != "business" && roomType != "drinking"))
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var isSuccess = await _tokoRoomAppService.RemoveAttendantFromRoomAsync(roomType, roomNumber);
            var response = new BaseResponse(isSuccess);
            return Response.Ok(response);
        }

        #endregion
    }
}
