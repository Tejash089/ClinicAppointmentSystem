using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class DashboardController : Controller
{
    [Authorize(Roles = "Admin")]
    public IActionResult AdminDashboard() => View();

    [Authorize(Roles = "Doctor")]
    public IActionResult DoctorDashboard() => View();

    [Authorize(Roles = "Patient")]
    public IActionResult PatientDashboard() => View();
}
