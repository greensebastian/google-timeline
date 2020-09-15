using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Model.Timeline.External;
using Domain.Interface;
using GoogleTimelineUI.Services;

namespace GoogleTimeline.Pages.Timeline
{
    [Authorize]
    public class UploadModel : PageModel
    {
        private readonly ITimelineService _timelineService;
        private readonly UserService _userService;

        public bool UploadCompleted { get; set; }

        public UploadModel(ITimelineService timelineService, UserService userService)
        {
            _timelineService = timelineService;
            _userService = userService;
        }

        public async Task OnPost()
        {
            if (Request.Form.Files.Count > 0)
            {
                var serialize = new JsonSerializer();
                var timelines = Request.Form.Files.Select(file =>
                {
                    using (var stream = file.OpenReadStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            using (var jtr = new JsonTextReader(sr))
                            {
                                return serialize.Deserialize<SemanticTimeline>(jtr);
                            }
                        }
                    }
                });

                _timelineService.AddTimelineData(await _userService.CurrentUser(), timelines);
                UploadCompleted = true;
            }
        }
    }
}
