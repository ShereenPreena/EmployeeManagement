using System.Net;
using Serilog;

namespace EmployeeService.Api.DepartmentClient
{
    public class DepartmentClient : IDepartmentClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<DepartmentClient> _log;

        public DepartmentClient(HttpClient http, ILogger<DepartmentClient> log)
        {
            _http = http; _log = log;
        }

        public async Task<bool> DepartmentExistsAsync(int departmentId, CancellationToken ct = default)
        {
            try
            {
                var res = await _http.GetAsync($"/api/departments/{departmentId}", ct);
                return res.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "DepartmentService unreachable at {Base}", _http.BaseAddress);
                return false;
            }
        }
    }
}
