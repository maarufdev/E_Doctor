using E_Doctor.Application.DTOs.ManageUsers.RequestDTOs;
using E_Doctor.Application.Interfaces.Features.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Admin.Controllers;

[Authorize]
public class ManageUserController : Controller
{
    private readonly IUserManagerService _userManagerService;
    public ManageUserController(IUserManagerService userManagerService)
    {
        _userManagerService = userManagerService;
    }

    public async Task<IActionResult> GetManageUsers([FromQuery] GetManageUserRequest request)
    {
        var result = await _userManagerService.GetManageUsers(request);

        if (result.IsFailure) return BadRequest("Something went wrong");

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> SaveManageUser([FromBody] SaveManageUserRequest request)
    {
        if (request is null) return BadRequest("Something Went Wrong!");

        var result = await _userManagerService.SaveManagePatientAccount(request);

        if(result.IsFailure) return BadRequest(result.Value);

        return Ok(result.IsSuccess);
    }

    public async Task<IActionResult> GetManageUserById(int userId)
    {
        if (userId == 0) return BadRequest("Invalid User Id");

        var result = await _userManagerService.GetManageUserById(userId);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
